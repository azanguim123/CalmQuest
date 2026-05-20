# 🏗️ CalmQuest – Technical Architecture

## Overview

CalmQuest uses a **two-process architecture**: a Unity game client and a Python AI server communicating via HTTP on localhost.

```
┌──────────────────────────────────────────────┐
│                 UNITY (C#)                   │
│                                              │
│  WebCamTexture → AIBridge → GameManager      │
│                                ↓             │
│              MiniGame Selector (StressLevel) │
│                                ↓             │
│     StormCalmer | SkyBalance | BreathHarmony │
└──────────────┬───────────────────────────────┘
               │  HTTP (localhost:5000)
               │  GET /detect/live
               │  POST /detect  (base64 image)
┌──────────────┴───────────────────────────────┐
│               PYTHON (Flask)                 │
│                                              │
│  server.py → EmotionDetector → FER model     │
│                                ↓             │
│         { dominant_emotion, stress_level }   │
└──────────────────────────────────────────────┘
```

## Stress Level → Mini-Game Mapping

| Stress Level | Range     | Mini-Game      | Technique Used      |
|--------------|-----------|----------------|---------------------|
| High         | 0.7 – 1.0 | Storm Calmer   | Box Breathing 4-4-4-4 |
| Medium       | 0.4 – 0.7 | Sky Balance    | Slow breathing      |
| Low          | 0.0 – 0.4 | Breath Harmony | 4-7-8 rhythm        |

## Communication Protocol

### GET /detect/live
```json
{
  "dominant_emotion": "angry",
  "stress_level": 0.85,
  "emotions": { "angry": 0.7, "fear": 0.2, "neutral": 0.1 },
  "face_detected": true
}
```

### POST /detect (base64 image from Unity)
```json
// Request
{ "image": "<base64_encoded_jpeg>" }

// Response (same as above)
```
