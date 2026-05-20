"""
CalmQuest – Image Preprocessor
Utilities to normalize and prepare frames for emotion detection.
"""

import cv2
import numpy as np


def resize_frame(frame: np.ndarray, width: int = 640) -> np.ndarray:
    h, w = frame.shape[:2]
    ratio = width / w
    return cv2.resize(frame, (width, int(h * ratio)))


def normalize(frame: np.ndarray) -> np.ndarray:
    return frame.astype(np.float32) / 255.0


def to_grayscale(frame: np.ndarray) -> np.ndarray:
    return cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)


def equalize_histogram(frame: np.ndarray) -> np.ndarray:
    gray = to_grayscale(frame)
    return cv2.equalizeHist(gray)
