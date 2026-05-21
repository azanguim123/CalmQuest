"""Tests for EmotionDetector v2 — smoothing & confidence."""

import numpy as np
import pytest
from unittest.mock import patch
from detector import EmotionDetector, EmotionResult, EMOTIONS


def make_emotions(**overrides):
    """Helper: build a normalized emotion dict."""
    base = {e: 0.0 for e in EMOTIONS}
    base.update(overrides)
    total = sum(base.values()) or 1
    return {k: v / total for k, v in base.items()}


class TestSmoothing:
    def test_single_frame_returns_that_emotion(self):
        det = EmotionDetector(smoothing_window=3, min_confidence=0.0)
        happy = make_emotions(happy=1.0)
        with patch.object(det, "_raw_detect", return_value=happy):
            frame = np.zeros((48, 48, 3), dtype=np.uint8)
            result = det.detect_from_frame(frame)
        assert result.dominant_emotion == "happy"
        assert result.stress_level < 0.1

    def test_smoothing_averages_multiple_frames(self):
        det = EmotionDetector(smoothing_window=4, min_confidence=0.0)
        frame = np.zeros((48, 48, 3), dtype=np.uint8)

        # 3 frames of fear, then 1 frame of happy
        emotions_sequence = [
            make_emotions(fear=1.0),
            make_emotions(fear=1.0),
            make_emotions(fear=1.0),
            make_emotions(happy=1.0),
        ]
        for emotions in emotions_sequence:
            with patch.object(det, "_raw_detect", return_value=emotions):
                result = det.detect_from_frame(frame)

        # fear should still dominate (3 vs 1)
        assert result.dominant_emotion == "fear"
        assert result.stability == 0.75  # 3/4 frames were fear

    def test_low_confidence_frame_ignored(self):
        det = EmotionDetector(smoothing_window=3, min_confidence=0.50)
        frame = np.zeros((48, 48, 3), dtype=np.uint8)

        # Low confidence detection (max = 0.3) → should be ignored
        low_conf = make_emotions(angry=0.3, neutral=0.3, sad=0.4)
        with patch.object(det, "_raw_detect", return_value=low_conf):
            result = det.detect_from_frame(frame)

        # Buffer is still empty → returns default neutral
        assert result.dominant_emotion == "neutral"

    def test_stability_is_one_when_all_frames_agree(self):
        det = EmotionDetector(smoothing_window=3, min_confidence=0.0)
        frame = np.zeros((48, 48, 3), dtype=np.uint8)
        happy = make_emotions(happy=1.0)

        for _ in range(3):
            with patch.object(det, "_raw_detect", return_value=happy):
                result = det.detect_from_frame(frame)

        assert result.stability == 1.0
        assert result.dominant_emotion == "happy"


class TestStressMap:
    def test_happy_stress_near_zero(self):
        det = EmotionDetector()
        stress = det._compute_stress(make_emotions(happy=1.0))
        assert stress < 0.05

    def test_angry_stress_near_one(self):
        det = EmotionDetector()
        stress = det._compute_stress(make_emotions(angry=1.0))
        assert stress > 0.95

    def test_stress_always_in_bounds(self):
        det = EmotionDetector()
        for emotion in EMOTIONS:
            stress = det._compute_stress(make_emotions(**{emotion: 1.0}))
            assert 0.0 <= stress <= 1.0, f"Out of bounds for {emotion}: {stress}"
