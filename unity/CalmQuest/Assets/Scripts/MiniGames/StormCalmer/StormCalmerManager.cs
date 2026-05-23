using System.Collections;
using UnityEngine;
using CalmQuest.AI;

namespace CalmQuest.MiniGames.StormCalmer
{
    /// <summary>
    /// Main controller for the Storm Calmer mini-game.
    /// Listens to AI emotion data and updates the storm intensity accordingly.
    /// Win condition: maintain stress below 0.2 for 10 seconds.
    /// </summary>
    public class StormCalmerManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StormController    stormController;
        [SerializeField] private BreathingGuide     breathingGuide;
        [SerializeField] private StormCalmerUI      ui;

        [Header("Game Settings")]
        [SerializeField] private float gameDuration        = 120f; // 2 minutes
        [SerializeField] private float winStreakRequired   = 10f;  // seconds calm to win
        [SerializeField] private float calmThreshold       = 0.25f;

        // ── State ────────────────────────────────────────────────────────
        private float _timeRemaining;
        private float _calmStreak;
        private float _currentStress;
        private bool  _isPlaying;
        private int   _score;

        // ── Lifecycle ────────────────────────────────────────────────────
        private void OnEnable()  => AIBridge.OnEmotionDetected += HandleEmotion;
        private void OnDisable() => AIBridge.OnEmotionDetected -= HandleEmotion;

        private void Start()
        {
            StartGame();
        }

        private void Update()
        {
            if (!_isPlaying) return;

            // Countdown timer
            _timeRemaining -= Time.deltaTime;
            ui.UpdateTimer(_timeRemaining);

            // Track calm streak
            if (_currentStress <= calmThreshold)
            {
                _calmStreak += Time.deltaTime;
                ui.UpdateCalmStreak(_calmStreak, winStreakRequired);

                if (_calmStreak >= winStreakRequired)
                {
                    TriggerWin();
                    return;
                }
            }
            else
            {
                _calmStreak = 0f;
            }

            // Time's up
            if (_timeRemaining <= 0f)
            {
                TriggerTimeUp();
            }
        }

        // ── Game flow ────────────────────────────────────────────────────
        private void StartGame()
        {
            _timeRemaining = gameDuration;
            _calmStreak    = 0f;
            _currentStress = 1.0f;
            _score         = 0;
            _isPlaying     = true;

            stormController.SetIntensity(1.0f);
            breathingGuide.StartGuide(BreathingTechnique.BoxBreathing);
            ui.ShowGame();

            Debug.Log("[StormCalmer] Game started");
        }

        private void TriggerWin()
        {
            _isPlaying = false;
            _score     = CalculateScore();

            stormController.TriggerClear();
            breathingGuide.StopGuide();
            ui.ShowWin(_score);

            Debug.Log($"[StormCalmer] Player won! Score: {_score}");
        }

        private void TriggerTimeUp()
        {
            _isPlaying = false;
            _score     = CalculateScore();

            breathingGuide.StopGuide();
            ui.ShowTimeUp(_score);

            Debug.Log($"[StormCalmer] Time's up. Score: {_score}");
        }

        // ── AI input ─────────────────────────────────────────────────────
        private void HandleEmotion(EmotionData data)
        {
            if (!_isPlaying || !data.face_detected) return;

            _currentStress = data.stress_level;

            // Smoothly update storm intensity
            stormController.SetIntensity(_currentStress);
            ui.UpdateStress(_currentStress);

            // Give feedback based on trend
            if (_currentStress <= calmThreshold)
                ui.ShowFeedback("Breathe... 🌿");
            else if (_currentStress <= 0.5f)
                ui.ShowFeedback("Keep going... 🌬️");
            else
                ui.ShowFeedback("Follow the guide 🔵");
        }

        // ── Scoring ───────────────────────────────────────────────────────
        private int CalculateScore()
        {
            // Base score on time remaining + calm streak achieved
            float timeBonus  = (_timeRemaining / gameDuration) * 500f;
            float calmBonus  = (_calmStreak / winStreakRequired) * 500f;
            return Mathf.RoundToInt(timeBonus + calmBonus);
        }
    }

    public enum BreathingTechnique { BoxBreathing, FourSevenEight }
}