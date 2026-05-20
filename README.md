<div align="center">

# 🌿 CalmQuest

### AI-Driven Serious Game for Emotional Regulation

*"Control your breath to control the world."*

[![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-black?logo=unity)](https://unity.com/)
[![Python](https://img.shields.io/badge/Python-3.10+-blue?logo=python)](https://python.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Status](https://img.shields.io/badge/Status-In%20Development-orange)]()

</div>

---

## 📖 Overview

**CalmQuest** is a serious game that uses AI-powered facial emotion recognition to detect a player's emotional state in real-time and adapt gameplay accordingly. Players calm a virtual world — and their virtual companion — by calming themselves through breathing and mindfulness exercises.

The core mechanic is simple but powerful: **your breath controls the world.**

---

## 🎮 Mini-Games

| Mini-Game | Concept | Core Mechanic |
|-----------|---------|---------------|
| 🌪️ **Storm Calmer** | Calm a world consumed by chaos | Breathing pace controls storm intensity |
| 🌤️ **Sky Balance** | Navigate a fragile sky world | Inhale = rise, Exhale = descend |
| 🎵 **Breath Harmony** | Restore harmony through rhythm | Synchronized breathing creates music |

---

## 🧠 AI System

```
📷 Scan Face  →  🤖 Detect Emotion  →  🎮 Select Mini-Game  →  📊 Track Progress
```

- **Emotion Detection**: Real-time facial expression analysis via front camera
- **Adaptive Engine**: Selects the most appropriate mini-game based on detected stress level
- **Progress Tracking**: Monitors player performance over time to improve recommendations

### Breathing Techniques Taught
- **Box Breathing** (4-4-4-4): inhale / hold / exhale / hold — 4 seconds each
- **4-7-8 Method**: inhale 4s / hold 7s / exhale 8s — fast relaxation

---

## 🏗️ Project Structure

```
CalmQuest/
│
├── 📁 ai/                          # Python AI emotion detection module
│   ├── models/                     # Trained emotion recognition models
│   │   └── emotion_model.pt        # PyTorch model weights
│   ├── src/
│   │   ├── detector.py             # Face & emotion detection pipeline
│   │   ├── preprocessor.py         # Image preprocessing utilities
│   │   └── server.py               # Local API server (Flask) for Unity bridge
│   ├── notebooks/                  # Research & training notebooks
│   │   ├── 01_data_exploration.ipynb
│   │   ├── 02_model_training.ipynb
│   │   └── 03_evaluation.ipynb
│   ├── tests/                      # Unit tests for AI module
│   └── requirements.txt            # Python dependencies
│
├── 📁 unity/                       # Unity game project
│   ├── Assets/
│   │   ├── Scripts/
│   │   │   ├── Core/               # Game manager, state machine
│   │   │   ├── AI/                 # AI bridge & emotion receiver
│   │   │   ├── MiniGames/
│   │   │   │   ├── StormCalmer/
│   │   │   │   ├── SkyBalance/
│   │   │   │   └── BreathHarmony/
│   │   │   ├── UI/                 # Menus, HUD, progress screens
│   │   │   └── Utils/              # Helpers & extensions
│   │   ├── Scenes/
│   │   │   ├── MainMenu.unity
│   │   │   ├── EmotionScan.unity
│   │   │   ├── StormCalmer.unity
│   │   │   ├── SkyBalance.unity
│   │   │   └── BreathHarmony.unity
│   │   ├── Prefabs/
│   │   ├── Audio/
│   │   └── Resources/
│   ├── Packages/
│   └── ProjectSettings/
│
├── 📁 docs/                        # Documentation & design assets
│   ├── design/
│   │   ├── game-design-document.md
│   │   └── ui-wireframes/
│   ├── assets/
│   │   └── CalmQuest_Presentation.pptx
│   └── architecture.md
│
├── .gitignore
├── CHANGELOG.md
├── CONTRIBUTING.md
├── LICENSE
└── README.md
```

---

## 🛠️ Tech Stack

### Game Engine
- **Unity 2022.3 LTS** — Main game engine
- **C#** — Game logic & AI bridge

### AI / Emotion Detection
- **Python 3.10+**
- **PyTorch** — Emotion classification model
- **OpenCV** — Face detection & camera feed
- **DeepFace / FER** — Pre-trained emotion recognition
- **Flask** — Local REST API bridge between Python and Unity

### Communication
- **HTTP (localhost)** — Unity ↔ Python AI module bridge

---

## 🚀 Getting Started

### Prerequisites
- Unity Hub + Unity 2022.3 LTS
- Python 3.10+
- pip / virtualenv

### 1. Clone the repository
```bash
git clone https://github.com/azanguim123/CalmQuest.git
cd CalmQuest
```

### 2. Set up the AI module
```bash
cd ai
python -m venv venv
source venv/bin/activate      # Windows: venv\Scripts\activate
pip install -r requirements.txt
python src/server.py           # Starts the emotion detection API on localhost:5000
```

### 3. Open the Unity project
```
Unity Hub → Open → Select the /unity folder
```

### 4. Play
- Make sure the Python AI server is running
- Press ▶ Play in Unity Editor

---

## 🌿 DPE Framework

CalmQuest is built around three design layers:

| Layer | Description |
|-------|-------------|
| **Didactic** | Box Breathing, 4-7-8 technique, stress regulation strategies |
| **Playful** | Adaptive mini-games, scoring, unlockables, streaks |
| **Experiential** | Player is a guardian — the world heals as they breathe |

---

## 📊 Impact Goals

- 🧠 **Emotional Awareness** — Players learn to recognize their emotional states
- 🌬️ **Breathing Techniques** — Science-backed methods in a playful context
- 🎮 **Engaging Learning** — Evidence-based psychoeducation through gameplay
- 📈 **Measurable Progress** — Visible improvement in calm scores over time

---

## 🗺️ Roadmap

- [x] Project structure & architecture
- [ ] AI emotion detection module (Python)
- [ ] Unity ↔ Python bridge
- [ ] Mini-game: Storm Calmer
- [ ] Mini-game: Sky Balance
- [ ] Mini-game: Breath Harmony
- [ ] UI / Main Menu / Progress screen
- [ ] Build & packaging

---

## 🤝 Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

---

## 📄 License

This project is licensed under the MIT License — see [LICENSE](LICENSE) for details.

---

<div align="center">
  <i>Built with 🌿 and breath.</i>
</div>
