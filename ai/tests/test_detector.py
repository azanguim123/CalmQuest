"""Unit tests for the EmotionDetector module."""

import numpy as np
import pytest
from unittest.mock import patch, MagicMock
from src.detector import EmotionDetector, EmotionResult, STRESS_MAP


class TestStressComputation:
    def test_happy_gives_low_stress(self):
        emotions = {"happy": 0.9, "neutral": 0.1}
        stress = EmotionDetector._compute_stress(emotions)
        assert stress < 0.2

    def test_angry_gives_high_stress(self):
        emotions = {"angry": 0.9, "neutral": 0.1}
        stress = EmotionDetector._compute_stress(emotions)
        assert stress > 0.8

    def test_stress_bounds(self):
        emotions = {"fear": 1.0}
        stress = EmotionDetector._compute_stress(emotions)
        assert 0.0 <= stress <= 1.0

    def test_empty_emotions_returns_default(self):
        stress = EmotionDetector._compute_stress({})
        assert stress == 0.2


class TestDetector:
    def test_no_face_returns_neutral(self):
        detector = EmotionDetector()
        with patch.object(detector.detector, "detect_emotions", return_value=[]):
            frame = np.zeros((480, 640, 3), dtype=np.uint8)
            result = detector.detect_from_frame(frame)
            assert result.face_detected is False
            assert result.dominant_emotion == "neutral"
