using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EmotionMaze.Core
{
    /// <summary>
    /// Controls the Credits celebration page that appears when all 4 mazes are completed.
    /// Features a celebration background with calm Moro, confetti particles, and auto-return to Home.
    /// </summary>
    public class CreditsController : MonoBehaviour
    {
        #region Settings
        [Header("UI References")]
        [SerializeField] private Text _celebrationText;
        [SerializeField] private Image _backgroundImage;

        [Header("Particle Effects")]
        [SerializeField] private ParticleSystem _confettiParticles;

        [Header("Audio")]
        [SerializeField] private AudioClip _celebrationSound;

        [Header("Timing Settings")]
        [SerializeField] private float _returnDelaySeconds = 3f;
        [SerializeField] private float _confettiDuration = 3f;

        [Header("Visual Settings")]
        [SerializeField] private Color _backgroundColor = new Color(0.5f, 0.8f, 0.77f); // #80CBC4 Calm Teal
        #endregion

        #region State
        private AudioSource _audioSource;
        private bool _hasInitialized = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Setup audio
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        private void Start()
        {
            InitializeCredits();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes the credits page with celebration visuals and starts the sequence.
        /// </summary>
        private void InitializeCredits()
        {
            if (_hasInitialized)
                return;

            _hasInitialized = true;

            Debug.Log("[CreditsController] Credits page initialized");

            // Set celebration text
            if (_celebrationText != null)
            {
                _celebrationText.text = "Great job helping Moro!";
                _celebrationText.gameObject.SetActive(true);
            }

            // Set background color
            if (_backgroundImage != null)
            {
                _backgroundImage.color = _backgroundColor;
            }
            else
            {
                // Fallback: set camera background color
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    mainCamera.backgroundColor = _backgroundColor;
                }
            }

            // Play celebration sound
            PlayCelebrationSound();

            // Start confetti
            StartConfetti();

            // Start auto-return countdown
            StartCoroutine(ReturnToHomeAfterDelay());
        }
        #endregion

        #region Confetti System
        /// <summary>
        /// Starts the confetti particle system.
        /// </summary>
        private void StartConfetti()
        {
            if (_confettiParticles != null)
            {
                Debug.Log("[CreditsController] Starting confetti celebration");
                _confettiParticles.Play();

                // Stop confetti after duration
                StartCoroutine(StopConfettiAfterDelay());
            }
            else
            {
                Debug.LogWarning("[CreditsController] Confetti particle system not assigned");
            }
        }

        /// <summary>
        /// Stops confetti after the specified duration.
        /// </summary>
        private IEnumerator StopConfettiAfterDelay()
        {
            yield return new WaitForSeconds(_confettiDuration);

            if (_confettiParticles != null && _confettiParticles.isPlaying)
            {
                _confettiParticles.Stop();
            }
        }
        #endregion

        #region Audio
        /// <summary>
        /// Plays the celebration sound effect.
        /// </summary>
        private void PlayCelebrationSound()
        {
            if (_celebrationSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_celebrationSound);
            }
        }
        #endregion

        #region Navigation
        /// <summary>
        /// Returns to the home scene after the specified delay.
        /// </summary>
        private IEnumerator ReturnToHomeAfterDelay()
        {
            Debug.Log($"[CreditsController] Returning to Home in {_returnDelaySeconds} seconds");
            yield return new WaitForSeconds(_returnDelaySeconds);

            if (NavigationManager.Instance != null)
            {
                Debug.Log("[CreditsController] Navigating back to Home");
                NavigationManager.Instance.NavigateToHome();
            }
            else
            {
                Debug.LogError("[CreditsController] NavigationManager not found!");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Allows manual trigger of return to home (useful for testing or skip button).
        /// </summary>
        public void ReturnToHomeNow()
        {
            StopAllCoroutines();

            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateToHome();
            }
        }
        #endregion
    }
}
