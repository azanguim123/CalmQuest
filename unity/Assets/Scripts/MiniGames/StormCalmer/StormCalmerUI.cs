using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CalmQuest.MiniGames.StormCalmer
{
    /// <summary>
    /// Manages all UI elements for Storm Calmer:
    /// timer, stress bar, calm streak, feedback text, win/lose screens.
    /// </summary>
    public class StormCalmerUI : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private TMP_Text   timerText;
        [SerializeField] private Slider     stressBar;
        [SerializeField] private Slider     calmStreakBar;
        [SerializeField] private TMP_Text   feedbackText;
        [SerializeField] private Image      stressBarFill;

        [Header("Screens")]
        [SerializeField] private GameObject gameScreen;
        [SerializeField] private GameObject winScreen;
        [SerializeField] private GameObject timeUpScreen;

        [Header("Win Screen")]
        [SerializeField] private TMP_Text winScoreText;

        [Header("Time Up Screen")]
        [SerializeField] private TMP_Text timeUpScoreText;

        [Header("Colors")]
        [SerializeField] private Color stressColorHigh = new Color(0.90f, 0.30f, 0.30f);
        [SerializeField] private Color stressColorLow  = new Color(0.49f, 0.85f, 0.77f);

        private float _feedbackTimer;
        private const float FeedbackDuration = 2.0f;

        // ── Lifecycle ────────────────────────────────────────────────────
        private void Update()
        {
            // Auto-hide feedback
            if (_feedbackTimer > 0f)
            {
                _feedbackTimer -= Time.deltaTime;
                if (_feedbackTimer <= 0f)
                    feedbackText.text = "";
            }
        }

        // ── Public API ───────────────────────────────────────────────────
        public void ShowGame()
        {
            gameScreen.SetActive(true);
            winScreen.SetActive(false);
            timeUpScreen.SetActive(false);
        }

        public void UpdateTimer(float timeRemaining)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";

            // Flash red when under 15s
            timerText.color = timeRemaining < 15f
                ? Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * 2f, 1f))
                : Color.white;
        }

        public void UpdateStress(float stress)
        {
            if (stressBar != null)
                stressBar.value = stress;

            if (stressBarFill != null)
                stressBarFill.color = Color.Lerp(stressColorLow, stressColorHigh, stress);
        }

        public void UpdateCalmStreak(float streak, float required)
        {
            if (calmStreakBar != null)
                calmStreakBar.value = streak / required;
        }

        public void ShowFeedback(string message)
        {
            feedbackText.text = message;
            _feedbackTimer    = FeedbackDuration;
        }

        public void ShowWin(int score)
        {
            gameScreen.SetActive(false);
            winScreen.SetActive(true);
            winScoreText.text = $"Score: {score}";
        }

        public void ShowTimeUp(int score)
        {
            gameScreen.SetActive(false);
            timeUpScreen.SetActive(true);
            timeUpScoreText.text = $"Score: {score}";
        }
    }
}