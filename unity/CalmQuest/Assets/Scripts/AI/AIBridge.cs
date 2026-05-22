using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace CalmQuest.AI
{
    /// <summary>
    /// Bridges Unity with the Python emotion detection server.
    /// Polls /detect/live every N seconds and broadcasts EmotionData via events.
    /// </summary>
    public class AIBridge : MonoBehaviour
    {
        [Header("Server Config")]
        [SerializeField] private string serverUrl   = "http://localhost:5000";
        [SerializeField] private float  pollInterval = 2.0f;

        [Header("Fallback (when server is offline)")]
        [SerializeField] private bool  useFallback       = true;
        [SerializeField] private float fallbackStress     = 0.5f;

        // ── Events ──────────────────────────────────────────────────────
        public static event Action<EmotionData> OnEmotionDetected;
        public static event Action<bool>        OnConnectionStatusChanged;

        // ── State ────────────────────────────────────────────────────────
        public static bool IsConnected { get; private set; } = false;
        public static EmotionData LastResult { get; private set; }

        // ── Lifecycle ────────────────────────────────────────────────────
        private void Start()
        {
            StartCoroutine(HealthCheckLoop());
            StartCoroutine(PollLoop());
        }

        // ── Health check ─────────────────────────────────────────────────
        private IEnumerator HealthCheckLoop()
        {
            while (true)
            {
                yield return StartCoroutine(CheckHealth());
                yield return new WaitForSeconds(5f);
            }
        }

        private IEnumerator CheckHealth()
        {
            using var req = UnityWebRequest.Get($"{serverUrl}/health");
            req.timeout = 3;
            yield return req.SendWebRequest();

            bool ok = req.result == UnityWebRequest.Result.Success;
            if (ok != IsConnected)
            {
                IsConnected = ok;
                OnConnectionStatusChanged?.Invoke(IsConnected);
                Debug.Log($"[AIBridge] Server {(ok ? "🟢 connected" : "🔴 disconnected")}");
            }
        }

        // ── Poll loop ────────────────────────────────────────────────────
        private IEnumerator PollLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(pollInterval);

                if (IsConnected)
                    yield return StartCoroutine(FetchEmotion());
                else if (useFallback)
                    BroadcastFallback();
            }
        }

        private IEnumerator FetchEmotion()
        {
            using var req = UnityWebRequest.Get($"{serverUrl}/detect/live");
            req.timeout = 3;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[AIBridge] Fetch failed: {req.error}");
                if (useFallback) BroadcastFallback();
                yield break;
            }

            try
            {
                var data = JsonUtility.FromJson<EmotionData>(req.downloadHandler.text);
                LastResult = data;
                OnEmotionDetected?.Invoke(data);
                Debug.Log($"[AIBridge] {data.dominant_emotion} | stress: {data.stress_level:F2} | stability: {data.stability:F2}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AIBridge] JSON parse error: {e.Message}");
            }
        }

        // ── Fallback ─────────────────────────────────────────────────────
        private void BroadcastFallback()
        {
            var fallback = new EmotionData
            {
                dominant_emotion = "neutral",
                stress_level     = fallbackStress,
                face_detected    = false,
                confidence       = 0f,
                stability        = 1f
            };
            LastResult = fallback;
            OnEmotionDetected?.Invoke(fallback);
        }
    }

    // ── Data model ───────────────────────────────────────────────────────
    [Serializable]
    public class EmotionData
    {
        public string dominant_emotion;
        public float  stress_level;
        public bool   face_detected;
        public float  confidence;
        public float  stability;
    }
}