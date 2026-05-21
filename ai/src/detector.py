"""
CalmQuest – Emotion Detector v2
Improved precision with:
- Rolling average over N frames (smoothing)
- Minimum confidence threshold
- Dominant emotion stability check
"""

import cv2
import numpy as np
from deepface import DeepFace
from collections import deque
from dataclasses import dataclass, field
from typing import Optional


STRESS_MAP = {
    "angry":    1.0,
    "fear":     0.9,
    "sad":      0.7,
    "disgust":  0.6,
    "surprise": 0.4,
    "neutral":  0.2,
    "happy":    0.0,
}

EMOTIONS = ["angry", "disgust", "fear", "happy", "neutral", "sad", "surprise"]


@dataclass
class EmotionResult:
    dominant_emotion: str
    stress_level: float           # 0.0 (calm) → 1.0 (stressed)
    emotions: dict                # smoothed probability map
    face_detected: bool
    confidence: float             # how confident we are (0.0 → 1.0)
    stability: float              # how stable the dominant emotion is (0.0 → 1.0)


class EmotionDetector:
    def __init__(
        self,
        smoothing_window: int = 5,      # number of frames to average
        min_confidence: float = 0.40,   # ignore detections below this
    ):
        self.smoothing_window = smoothing_window
        self.min_confidence = min_confidence

        # Rolling buffer: stores the last N emotion dicts
        self._buffer: deque = deque(maxlen=smoothing_window)

        self.cap: Optional[cv2.VideoCapture] = None

        print("🧠 Loading emotion model...")
        try:
            dummy = np.zeros((48, 48, 3), dtype=np.uint8)
            DeepFace.analyze(dummy, actions=["emotion"],
                             enforce_detection=False, silent=True)
            print("✅ Model loaded")
        except Exception:
            print("⚠️  Warm-up skipped")

    # ── Camera ──────────────────────────────────────────────────────────
    def start_camera(self, camera_index: int = 0) -> bool:
        self.cap = cv2.VideoCapture(camera_index)
        return self.cap.isOpened()

    def stop_camera(self):
        if self.cap:
            self.cap.release()

    # ── Core detection ───────────────────────────────────────────────────
    def detect_from_frame(self, frame: np.ndarray) -> EmotionResult:
        """Detect emotion from a single frame, apply smoothing."""
        raw = self._raw_detect(frame)

        if raw is None:
            # No face — return last smoothed result or default
            return self._build_result(face_detected=False)

        confidence = max(raw.values())

        # Only add to buffer if confidence is high enough
        if confidence >= self.min_confidence:
            self._buffer.append(raw)

        return self._build_result(face_detected=True, confidence=confidence)

    def detect_from_camera(self) -> Optional[EmotionResult]:
        """Capture one frame from camera and detect."""
        if not self.cap or not self.cap.isOpened():
            return None
        ret, frame = self.cap.read()
        if not ret:
            return None
        return self.detect_from_frame(frame)

    # ── Internal helpers ─────────────────────────────────────────────────
    def _raw_detect(self, frame: np.ndarray) -> Optional[dict]:
        """Run DeepFace on a frame, return normalized emotion dict or None."""
        try:
            results = DeepFace.analyze(
                frame,
                actions=["emotion"],
                enforce_detection=False,
                silent=True
            )
            result = results[0] if isinstance(results, list) else results
            emotions = result["emotion"]

            # Normalize to 0–1
            total = sum(emotions.values())
            if total <= 0:
                return None
            return {k: v / total for k, v in emotions.items()}

        except Exception as e:
            print(f"⚠️  Detection error: {e}")
            return None

    def _build_result(
        self,
        face_detected: bool,
        confidence: float = 0.0
    ) -> EmotionResult:
        """Build a smoothed EmotionResult from the rolling buffer."""
        if not self._buffer:
            return EmotionResult(
                dominant_emotion="neutral",
                stress_level=0.2,
                emotions={e: 0.0 for e in EMOTIONS},
                face_detected=face_detected,
                confidence=0.0,
                stability=0.0,
            )

        # Average all frames in buffer
        smoothed = {}
        for emotion in EMOTIONS:
            smoothed[emotion] = round(
                sum(frame[emotion] for frame in self._buffer) / len(self._buffer), 4
            )

        dominant = max(smoothed, key=smoothed.get)
        stress = self._compute_stress(smoothed)

        # Stability = % of frames in buffer where dominant was the same
        dominant_count = sum(
            1 for frame in self._buffer
            if max(frame, key=frame.get) == dominant
        )
        stability = round(dominant_count / len(self._buffer), 2)

        return EmotionResult(
            dominant_emotion=dominant,
            stress_level=round(stress, 3),
            emotions=smoothed,
            face_detected=face_detected,
            confidence=round(confidence, 3),
            stability=stability,
        )

    @staticmethod
    def _compute_stress(emotions: dict) -> float:
        total, weight_sum = 0.0, 0.0
        for emotion, prob in emotions.items():
            stress_weight = STRESS_MAP.get(emotion, 0.3)
            total += stress_weight * prob
            weight_sum += prob
        return total / weight_sum if weight_sum > 0 else 0.2
