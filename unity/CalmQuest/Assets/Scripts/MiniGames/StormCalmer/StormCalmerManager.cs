using System.Collections;
using UnityEngine;
using CalmQuest.AI;

namespace CalmQuest.MiniGames.StormCalmer
{
    public class StormCalmerManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StormController stormController;
        [SerializeField] private BreathingGuide  breathingGuide;
        [SerializeField] private StormCalmerUI   ui;

        [Header("Game Settings")]
        [SerializeField] private float gameDuration      = 120f;
        [SerializeField] private float winStreakRequired = 10f;
        [SerializeField] private float calmThreshold     = 0.25f;

        private float _timeRemaining;
        private float _calmStreak;
        private float _currentStress;
        private bool  _isPlaying;
        private int   _score;

        private void OnEnable()  => AIBridge.OnEmotionDetected += HandleEmotion;
        private void OnDisable() => AIBridge.OnEmotionDetected -= HandleEmotion;

        private void Start() => StartGame();

        private void Update()
        {
            if (!_isPlaying) return;

            _timeRemaining -= Time.deltaTime;

            // Null checks before calling UI
            if (ui != null) ui.UpdateTimer(_timeRemaining);

            if (_currentStress <= calmThreshold)
            {
                _calmStreak += Time.deltaTime;
                if (ui != null) ui.UpdateCalmStreak(_calmStreak, winStreakRequired);
                if (_calmStreak >= winStreakRequired) { TriggerWin(); return; }
            }
            else
            {
                _calmStreak = 0f;
            }

            if (_timeRemaining <= 0f) TriggerTimeUp();
        }

        private void StartGame()
        {
            _timeRemaining = gameDuration;
            _calmStreak    = 0f;
            _currentStress = 1.0f;
            _score         = 0;
            _isPlaying     = true;

            if (stormController != null) stormController.SetIntensity(1.0f);
            if (breathingGuide  != null) breathingGuide.StartGuide(BreathingTechnique.BoxBreathing);
            if (ui              != null) ui.ShowGame();

            Debug.Log("[StormCalmer] Game started 🌪️");
        }

        private void TriggerWin()
        {
            _isPlaying = false;
            _score     = CalculateScore();
            if (stormController != null) stormController.TriggerClear();
            if (breathingGuide  != null) breathingGuide.StopGuide();
            if (ui              != null) ui.ShowWin(_score);
            Debug.Log($"[StormCalmer] Won! Score: {_score} 🌤️");
        }

        private void TriggerTimeUp()
        {
            _isPlaying = false;
            _score     = CalculateScore();
            if (breathingGuide != null) breathingGuide.StopGuide();
            if (ui             != null) ui.ShowTimeUp(_score);
            Debug.Log($"[StormCalmer] Time's up. Score: {_score}");
        }

        private void HandleEmotion(EmotionData data)
        {
            if (!_isPlaying || !data.face_detected) return;
            _currentStress = data.stress_level;
            if (stormController != null) stormController.SetIntensity(_currentStress);
            if (ui              != null) ui.UpdateStress(_currentStress);

            if      (_currentStress <= calmThreshold) { if (ui != null) ui.ShowFeedback("Breathe... 🌿"); }
            else if (_currentStress <= 0.5f)          { if (ui != null) ui.ShowFeedback("Keep going... 🌬️"); }
            else                                      { if (ui != null) ui.ShowFeedback("Follow the guide 🔵"); }
        }

        private int CalculateScore()
        {
            float timeBonus = (_timeRemaining / gameDuration) * 500f;
            float calmBonus = (_calmStreak / winStreakRequired) * 500f;
            return Mathf.RoundToInt(timeBonus + calmBonus);
        }
    }

    public enum BreathingTechnique { BoxBreathing, FourSevenEight }
}
