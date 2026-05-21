using System.Collections;
using UnityEngine;

namespace CalmQuest.MiniGames.StormCalmer
{
    /// <summary>
    /// Controls all visual/audio elements of the storm.
    /// Intensity 1.0 = full chaos, 0.0 = clear sky.
    /// </summary>
    public class StormController : MonoBehaviour
    {
        [Header("Visual Elements")]
        [SerializeField] private ParticleSystem rainParticles;
        [SerializeField] private ParticleSystem windParticles;
        [SerializeField] private Light          directionalLight;
        [SerializeField] private Material       skyboxMaterial;

        [Header("Audio")]
        [SerializeField] private AudioSource stormAudio;
        [SerializeField] private AudioSource calmAudio;

        [Header("Transition Speed")]
        [SerializeField] private float lerpSpeed = 2.0f;

        // Sky colors
        private static readonly Color SkyStormy = new Color(0.15f, 0.15f, 0.20f);
        private static readonly Color SkyClear  = new Color(0.40f, 0.70f, 1.00f);

        // Light intensities
        private const float LightStormy = 0.3f;
        private const float LightClear  = 1.2f;

        private float _targetIntensity;
        private float _currentIntensity;

        // ── Public API ───────────────────────────────────────────────────
        public void SetIntensity(float intensity)
        {
            _targetIntensity = Mathf.Clamp01(intensity);
        }

        public void TriggerClear()
        {
            StartCoroutine(ClearRoutine());
        }

        // ── Update ───────────────────────────────────────────────────────
        private void Update()
        {
            _currentIntensity = Mathf.Lerp(
                _currentIntensity, _targetIntensity,
                Time.deltaTime * lerpSpeed
            );
            ApplyIntensity(_currentIntensity);
        }

        // ── Apply ────────────────────────────────────────────────────────
        private void ApplyIntensity(float t)
        {
            // Particles
            if (rainParticles != null)
            {
                var emission = rainParticles.emission;
                emission.rateOverTime = Mathf.Lerp(0f, 300f, t);
            }
            if (windParticles != null)
            {
                var emission = windParticles.emission;
                emission.rateOverTime = Mathf.Lerp(0f, 150f, t);
            }

            // Lighting
            if (directionalLight != null)
                directionalLight.intensity = Mathf.Lerp(LightClear, LightStormy, t);

            // Sky color
            if (skyboxMaterial != null)
                skyboxMaterial.SetColor("_Tint", Color.Lerp(SkyClear, SkyStormy, t));

            // Audio crossfade
            if (stormAudio != null) stormAudio.volume = Mathf.Lerp(0f, 1f, t);
            if (calmAudio  != null) calmAudio.volume  = Mathf.Lerp(1f, 0f, t);
        }

        // ── Win sequence ─────────────────────────────────────────────────
        private IEnumerator ClearRoutine()
        {
            float elapsed = 0f;
            float duration = 3.0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _targetIntensity = Mathf.Lerp(1f, 0f, elapsed / duration);
                yield return null;
            }
            _targetIntensity = 0f;
        }
    }
}