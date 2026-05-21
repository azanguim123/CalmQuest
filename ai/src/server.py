"""
CalmQuest – AI Bridge Server v2
Now returns confidence + stability fields.
"""

from flask import Flask, jsonify, request
from flask_cors import CORS
import base64
import numpy as np
import cv2

from detector import EmotionDetector

app = Flask(__name__)
CORS(app)

detector = EmotionDetector(smoothing_window=5, min_confidence=0.40)
detector.start_camera()


@app.route("/health", methods=["GET"])
def health():
    return jsonify({"status": "ok", "service": "CalmQuest AI Bridge v2"})


@app.route("/detect/live", methods=["GET"])
def detect_live():
    result = detector.detect_from_camera()
    if result is None:
        return jsonify({"error": "Camera unavailable"}), 503

    return jsonify({
        "dominant_emotion": result.dominant_emotion,
        "stress_level":     result.stress_level,
        "emotions":         result.emotions,
        "face_detected":    result.face_detected,
        "confidence":       result.confidence,
        "stability":        result.stability,
    })


@app.route("/detect", methods=["POST"])
def detect_image():
    data = request.get_json()
    if not data or "image" not in data:
        return jsonify({"error": "Missing 'image' field"}), 400

    try:
        img_bytes = base64.b64decode(data["image"])
        img_array = np.frombuffer(img_bytes, dtype=np.uint8)
        frame = cv2.imdecode(img_array, cv2.IMREAD_COLOR)
    except Exception as e:
        return jsonify({"error": f"Invalid image data: {str(e)}"}), 400

    result = detector.detect_from_frame(frame)
    return jsonify({
        "dominant_emotion": result.dominant_emotion,
        "stress_level":     result.stress_level,
        "emotions":         result.emotions,
        "face_detected":    result.face_detected,
        "confidence":       result.confidence,
        "stability":        result.stability,
    })


if __name__ == "__main__":
    print("🌿 CalmQuest AI Bridge v2 running on http://localhost:5000")
    app.run(host="0.0.0.0", port=5000, debug=False)
