"""
CalmQuest – Emotion Detector
Captures webcam feed, detects face, classifies emotion in real-time.
Uses DeepFace for emotion recognition.
"""
 
import cv2
import numpy as np
from deepface import DeepFace
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
    def __init__(self):
        # Warm up DeepFace on init to avoid cold start delay
        print("🧠 Loading emotion model...")
        try:
            dummy = np.zeros((48, 48, 3), dtype=np.uint8)
            DeepFace.analyze(dummy, actions=["emotion"],
                             enforce_detection=False, silent=True)
            print("✅ Model loaded")
        except Exception:
            print("⚠️  Warm-up skipped")
 
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
        try:
            results = DeepFace.analyze(
                frame,
                actions=["emotion"],
                enforce_detection=False,
                silent=True
            )
 
            # DeepFace returns a list
            if isinstance(results, list):
                result = results[0]
            else:
                result = results
 
            emotions = result["emotion"]
            # Normalize to 0-1 range (DeepFace returns percentages)
            total = sum(emotions.values())
            if total > 0:
                emotions = {k: v / total for k, v in emotions.items()}
 
            dominant = result["dominant_emotion"]
            stress = self._compute_stress(emotions)
 
            return EmotionResult(
                dominant_emotion=dominant,
                stress_level=round(stress, 3),
                emotions={k: round(v, 4) for k, v in emotions.items()},
                face_detected=True,
            )
 
        except Exception as e:
            print(f"⚠️  Detection error: {e}")
            return EmotionResult(
                dominant_emotion="neutral",
                stress_level=0.2,
                emotions={},
                face_detected=False,
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