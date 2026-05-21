using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace CalmQuest.AI
{
    /// <summary>
    /// Bridges the Unity game with the Python emotion detection server.
    /// Polls the AI server at regular intervals and broadcasts results via events.
    /// </summary>
    public class AIBridge : MonoBehaviour
    {
        [Header("Server Config")]
        [SerializeField] private string serverUrl = "http://localhost:5000";
        [SerializeField] private float pollInterval = 2.0f;

        // ── Events ──────────────────────────────────────────────
        public static event Action<EmotionData> OnEmotionDetected;
        public static event Action<bool>        OnConnectionStatusChanged;

        private bool _isConnected = false;

        // ── Lifecycle ───────────────────────────────────────────
        private void Start()
        {
            StartCoroutine(CheckHealth());
            StartCoroutine(PollEmotion());
        }

        // ── Health check ────────────────────────────────────────
        private IEnumerator CheckHealth()
        {
            using var req = UnityWebRequest.Get($"{serverUrl}/health");
            yield return req.SendWebRequest();

            bool ok = req.result == UnityWebRequest.Result.Success;
            if (ok != _isConnected)
            {
                _isConnected = ok;
                OnConnectionStatusChanged?.Invoke(_isConnected);
                Debug.Log($"[AIBridge] Server {(ok ? "connected ✓" : "unreachable ✗")}");
            }
        }

        // ── Polling loop ────────────────────────────────────────
        private IEnumerator PollEmotion()
        {
            while (true)
            {
                yield return new WaitForSeconds(pollInterval);
                yield return StartCoroutine(FetchEmotion());
            }
        }

        private IEnumerator FetchEmotion()
        {
            using var req = UnityWebRequest.Get($"{serverUrl}/detect/live");
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[AIBridge] Request failed: {req.error}");
                yield break;
            }

            var data = JsonUtility.FromJson<EmotionData>(req.downloadHandler.text);
            OnEmotionDetected?.Invoke(data);
        }
    }

    // ── Data model ──────────────────────────────────────────────────────
    [Serializable]
    public class EmotionData
    {
        public string dominant_emotion;
        public float  stress_level;       // 0.0 → 1.0
        public bool   face_detected;
    }
}
