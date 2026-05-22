using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CalmQuest.MiniGames.StormCalmer
{
    public class BreathingGuide : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform circleTransform;
        [SerializeField] private Image         circleImage;
        [SerializeField] private TMP_Text      instructionText;
        [SerializeField] private TMP_Text      countdownText;

        [Header("Sizes")]
        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float maxScale = 1.0f;

        [Header("Colors")]
        [SerializeField] private Color inhaleColor  = new Color(0.49f, 0.85f, 0.77f);
        [SerializeField] private Color holdColor    = new Color(0.91f, 0.66f, 0.34f);
        [SerializeField] private Color exhaleColor  = new Color(0.30f, 0.60f, 0.85f);

        private Coroutine _guideCoroutine;

        public void StartGuide(BreathingTechnique technique)
        {
            StopGuide();
            _guideCoroutine = StartCoroutine(
                technique == BreathingTechnique.BoxBreathing
                    ? BoxBreathingLoop()
                    : FourSevenEightLoop()
            );
        }

        public void StopGuide()
        {
            if (_guideCoroutine != null)
                StopCoroutine(_guideCoroutine);

            // Null checks to avoid NullReferenceException
            if (instructionText != null) instructionText.text = "";
            if (countdownText   != null) countdownText.text   = "";
        }

        private IEnumerator BoxBreathingLoop()
        {
            while (true)
            {
                yield return StartCoroutine(Phase("Inhale", 4f, minScale, maxScale, inhaleColor));
                yield return StartCoroutine(Phase("Hold",   4f, maxScale, maxScale, holdColor));
                yield return StartCoroutine(Phase("Exhale", 4f, maxScale, minScale, exhaleColor));
                yield return StartCoroutine(Phase("Hold",   4f, minScale, minScale, holdColor));
            }
        }

        private IEnumerator FourSevenEightLoop()
        {
            while (true)
            {
                yield return StartCoroutine(Phase("Inhale", 4f, minScale, maxScale, inhaleColor));
                yield return StartCoroutine(Phase("Hold",   7f, maxScale, maxScale, holdColor));
                yield return StartCoroutine(Phase("Exhale", 8f, maxScale, minScale, exhaleColor));
            }
        }

        private IEnumerator Phase(string label, float duration, float startScale, float endScale, Color color)
        {
            if (instructionText != null) instructionText.text = label;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                if (circleTransform != null)
                    circleTransform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);

                if (circleImage != null)
                    circleImage.color = Color.Lerp(circleImage.color, color, Time.deltaTime * 3f);

                if (countdownText != null)
                    countdownText.text = Mathf.CeilToInt(duration - elapsed).ToString();

                yield return null;
            }
        }
    }
}