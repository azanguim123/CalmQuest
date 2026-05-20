using UnityEngine;
using CalmQuest.AI;

namespace CalmQuest.Core
{
    /// <summary>
    /// Central game manager. Listens to emotion events and routes
    /// the player to the appropriate mini-game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Stress Thresholds")]
        [Range(0f, 1f)] public float highStressThreshold   = 0.7f;
        [Range(0f, 1f)] public float mediumStressThreshold = 0.4f;

        public GameState CurrentState { get; private set; } = GameState.Scanning;

        // ── Lifecycle ───────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()  => AIBridge.OnEmotionDetected += HandleEmotion;
        private void OnDisable() => AIBridge.OnEmotionDetected -= HandleEmotion;

        // ── Emotion routing ─────────────────────────────────────
        private void HandleEmotion(EmotionData data)
        {
            if (!data.face_detected) return;

            MiniGame recommended = SelectMiniGame(data.stress_level);
            Debug.Log($"[GameManager] Stress: {data.stress_level:F2} → {recommended}");

            // TODO: trigger scene transition to recommended mini-game
        }

        private MiniGame SelectMiniGame(float stressLevel)
        {
            if (stressLevel >= highStressThreshold)   return MiniGame.StormCalmer;
            if (stressLevel >= mediumStressThreshold)  return MiniGame.SkyBalance;
            return MiniGame.BreathHarmony;
        }
    }

    public enum GameState  { Scanning, Playing, Results }
    public enum MiniGame   { StormCalmer, SkyBalance, BreathHarmony }
}
