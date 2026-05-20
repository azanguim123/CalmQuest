"""
CalmQuest – Emotion Detector
Captures webcam feed, detects face, classifies emotion in real-time.
"""

import cv2
import numpy as np
from fer import FER
from dataclasses import dataclass
from typing import Optional


STRESS_MAP = {
    "angry":   1.0,
    "fear":    0.9,
    "sad":     0.7,
    "disgust": 0.6,
    "surprise":0.4,
    "neutral": 0.2,
    "happy":   0.0,
}


@dataclass
class EmotionResult:
    dominant_emotion: str
    stress_level: float          # 0.0 (calm) → 1.0 (stressed)
    emotions: dict               # full probability map
    face_detected: bool


class EmotionDetector:
    def __init__(self, use_mtcnn: bool = False):
        self.detector = FER(mtcnn=use_mtcnn)
        self.cap: Optional[cv2.VideoCapture] = None

    # ------------------------------------------------------------------
    # Camera
    # ------------------------------------------------------------------
    def start_camera(self, camera_index: int = 0) -> bool:
        self.cap = cv2.VideoCapture(camera_index)
        return self.cap.isOpened()

    def stop_camera(self):
        if self.cap:
            self.cap.release()

    # ------------------------------------------------------------------
    # Detection
    # ------------------------------------------------------------------
    def detect_from_frame(self, frame: np.ndarray) -> EmotionResult:
        """Run emotion detection on a single BGR frame."""
        results = self.detector.detect_emotions(frame)

        if not results:
            return EmotionResult(
                dominant_emotion="neutral",
                stress_level=0.2,
                emotions={},
                face_detected=False,
            )

        # Take the face with the highest detection confidence
        face = max(results, key=lambda r: sum(r["emotions"].values()))
        emotions = face["emotions"]
        dominant = max(emotions, key=emotions.get)
        stress = self._compute_stress(emotions)

        return EmotionResult(
            dominant_emotion=dominant,
            stress_level=round(stress, 3),
            emotions=emotions,
            face_detected=True,
        )

    def detect_from_camera(self) -> Optional[EmotionResult]:
        """Capture one frame from the camera and run detection."""
        if not self.cap or not self.cap.isOpened():
            return None
        ret, frame = self.cap.read()
        if not ret:
            return None
        return self.detect_from_frame(frame)

    # ------------------------------------------------------------------
    # Stress scoring
    # ------------------------------------------------------------------
    @staticmethod
    def _compute_stress(emotions: dict) -> float:
        """
        Weighted average of stress scores per emotion.
        Returns a value in [0.0, 1.0].
        """
        total, weight_sum = 0.0, 0.0
        for emotion, prob in emotions.items():
            stress_weight = STRESS_MAP.get(emotion, 0.3)
            total += stress_weight * prob
            weight_sum += prob
        return total / weight_sum if weight_sum > 0 else 0.2
