using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using EmotionMaze.Core;

namespace EmotionMaze.Mazes
{
    /// <summary>
    /// Controls the Fear Maze mechanics.
    /// Teaches emotional regulation through facing fears in a safe environment.
    /// Player taps "Light" button 3-4 times to reveal friendly shapes from dark shadows.
    /// </summary>
    public class FearMazeController : MonoBehaviour
    {
        #region Settings
        [Header("UI References")]
        [SerializeField] private Text _instructionText;
        [SerializeField] private Button _lightButton;
        [SerializeField] private Text _lightButtonText;
        [SerializeField] private Text _completionText;
        [SerializeField] private Text _counterText;

        [Header("Shadow Shapes")]
        [SerializeField] private List<GameObject> _shadowShapes = new List<GameObject>();
        [SerializeField] private List<GameObject> _friendlyShapes = new List<GameObject>();

        [Header("Particle Effects")]
        [SerializeField] private ParticleSystem _glowParticles;
        [SerializeField] private ParticleSystem _lightBeamParticles;

        [Header("Light Effects")]
        [SerializeField] private Image _lightOverlay; // Full screen overlay for lighting effect

        [Header("Audio")]
        [SerializeField] private AudioClip _friendlySound;
        [SerializeField] private AudioClip _completionSound;
        [SerializeField] private AudioClip _backgroundMusic;

        [Header("Gameplay Settings")]
        [SerializeField] private float _returnDelaySeconds = 2f;
        [SerializeField] private float _buttonPulseSpeed = 2f;
        [SerializeField] private float _buttonPulseIntensity = 0.1f;
        [SerializeField] private float _sceneBrightenDuration = 1f;

        [Header("Visual Feedback")]
        [SerializeField] private Color _buttonDarkColor = new Color(0.16f, 0.2f, 0.58f); // #283593 Deep Indigo
        [SerializeField] private Color _buttonBrightColor = new Color(1f, 0.92f, 0.23f); // Bright Yellow
        [SerializeField] private Color _backgroundDarkColor = new Color(0.16f, 0.2f, 0.58f); // #283593
        [SerializeField] private Color _backgroundBrightColor = new Color(0.37f, 0.29f, 0.69f); // #5E35B1 Lighter Purple
        #endregion

        #region State
        private int _shadowsRevealed = 0;
        private bool _isCompleted = false;
        private AudioSource _audioSource;
        private Image _buttonImage;
        private Image _backgroundImage;
        private Vector3 _buttonNormalScale;
        private float _pulsePhase = 0f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Setup audio
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;

            // Get button image
            if (_lightButton != null)
            {
                _buttonImage = _lightButton.GetComponent<Image>();
                _buttonNormalScale = _lightButton.transform.localScale;
            }

            // Get background image from scene
            Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                Transform bgTransform = canvas.transform.Find("Background");
                if (bgTransform != null)
                {
                    _backgroundImage = bgTransform.GetComponent<Image>();
                }
            }

            InitializeUI();
        }

        private void OnEnable()
        {
            if (_lightButton != null)
            {
                _lightButton.onClick.AddListener(OnLightButtonClicked);
            }
        }

        private void OnDisable()
        {
            if (_lightButton != null)
            {
                _lightButton.onClick.RemoveListener(OnLightButtonClicked);
            }
        }

        private void Update()
        {
            if (!_isCompleted)
            {
                UpdateButtonPulse();
            }
        }
        #endregion

        #region Initialization
        private void InitializeUI()
        {
            // Set instruction text
            if (_instructionText != null)
            {
                _instructionText.text = "It's dark and scary. Let's turn on the lights!";
                _instructionText.gameObject.SetActive(true);
            }

            // Set button text with light bulb emoji
            if (_lightButtonText != null)
            {
                _lightButtonText.text = "💡 Turn On Light";
                _lightButtonText.fontSize = 28;
            }

            // Set button color
            if (_buttonImage != null)
            {
                _buttonImage.color = _buttonDarkColor;
            }

            // Set background to dark
            if (_backgroundImage != null)
            {
                _backgroundImage.color = _backgroundDarkColor;
            }

            // Initialize light overlay to full darkness (will fade as lights turn on)
            if (_lightOverlay != null)
            {
                _lightOverlay.color = new Color(0f, 0f, 0f, 0.7f); // Dark overlay
                _lightOverlay.raycastTarget = false; // Don't block button clicks
            }

            // Hide completion text initially
            if (_completionText != null)
            {
                _completionText.gameObject.SetActive(false);
            }

            // Initialize counter text
            UpdateCounterText();

            // Show all shadow shapes, hide all friendly shapes
            for (int i = 0; i < _shadowShapes.Count; i++)
            {
                if (_shadowShapes[i] != null)
                {
                    _shadowShapes[i].SetActive(true);
                }
            }

            for (int i = 0; i < _friendlyShapes.Count; i++)
            {
                if (_friendlyShapes[i] != null)
                {
                    _friendlyShapes[i].SetActive(false);
                }
            }

            // Make sure particles are stopped initially
            if (_glowParticles != null)
            {
                _glowParticles.Stop();
            }

            if (_lightBeamParticles != null)
            {
                _lightBeamParticles.Stop();
            }

            // Start background music
            PlayBackgroundMusic();

            Debug.Log("[FearMazeController] Maze initialized");
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when the light button is clicked.
        /// </summary>
        private void OnLightButtonClicked()
        {
            if (_isCompleted)
                return;

            if (_shadowsRevealed >= _shadowShapes.Count)
                return;

            Debug.Log($"[FearMazeController] Light {_shadowsRevealed + 1}/{_shadowShapes.Count}");

            // Play friendly sound
            PlayFriendlySound();

            // Play light beam effect from button to shadow
            PlayLightBeamEffect(_shadowsRevealed);

            // Show glow particles at the shadow position
            PlayGlowParticles(_shadowsRevealed);

            // Transform shadow to friendly shape
            RevealShadow(_shadowsRevealed);

            _shadowsRevealed++;

            // Gradually lighten the scene
            UpdateLightOverlay();

            // Update counter
            UpdateCounterText();

            // Transition button color towards bright
            UpdateButtonColor();

            // Check for completion
            if (_shadowsRevealed >= _shadowShapes.Count)
            {
                CompleteMaze();
            }
        }
        #endregion

        #region Visual Updates
        /// <summary>
        /// Updates the button pulsing animation.
        /// </summary>
        private void UpdateButtonPulse()
        {
            if (_lightButton == null)
                return;

            _pulsePhase += Time.deltaTime * _buttonPulseSpeed;
            float pulseValue = 1f + (Mathf.Sin(_pulsePhase) * _buttonPulseIntensity);

            _lightButton.transform.localScale = _buttonNormalScale * pulseValue;
        }

        /// <summary>
        /// Updates the button color based on progress.
        /// </summary>
        private void UpdateButtonColor()
        {
            if (_buttonImage == null)
                return;

            float progress = (float)_shadowsRevealed / _shadowShapes.Count;
            _buttonImage.color = Color.Lerp(_buttonDarkColor, _buttonBrightColor, progress);
        }

        /// <summary>
        /// Updates the counter text to show progress.
        /// </summary>
        private void UpdateCounterText()
        {
            if (_counterText != null)
            {
                _counterText.text = $"{_shadowsRevealed}/{_shadowShapes.Count} lights on";
            }
        }

        /// <summary>
        /// Transforms a shadow shape into a friendly shape.
        /// </summary>
        private void RevealShadow(int index)
        {
            if (index >= _shadowShapes.Count || index >= _friendlyShapes.Count)
                return;

            // Hide shadow
            if (_shadowShapes[index] != null)
            {
                StartCoroutine(FadeOutShadow(_shadowShapes[index]));
            }

            // Show friendly shape with fade-in effect
            if (_friendlyShapes[index] != null)
            {
                _friendlyShapes[index].SetActive(true);
                StartCoroutine(FadeInFriendlyShape(_friendlyShapes[index]));
            }
        }

        /// <summary>
        /// Fades out a shadow shape.
        /// </summary>
        private IEnumerator FadeOutShadow(GameObject shadow)
        {
            Image shadowImage = shadow.GetComponent<Image>();
            if (shadowImage == null)
            {
                shadow.SetActive(false);
                yield break;
            }

            Color startColor = shadowImage.color;
            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                shadowImage.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
                yield return null;
            }

            shadow.SetActive(false);
        }

        /// <summary>
        /// Fades in a friendly shape with glow effect.
        /// </summary>
        private IEnumerator FadeInFriendlyShape(GameObject friendlyShape)
        {
            Image friendlyImage = friendlyShape.GetComponent<Image>();
            if (friendlyImage == null)
                yield break;

            Color targetColor = friendlyImage.color;
            friendlyImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);

            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                friendlyImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, t);

                // Add slight scale animation
                float scale = 1f + (Mathf.Sin(t * Mathf.PI) * 0.2f);
                friendlyShape.transform.localScale = Vector3.one * scale;

                yield return null;
            }

            friendlyImage.color = targetColor;
            friendlyShape.transform.localScale = Vector3.one;
        }
        #endregion

        #region Effects
        /// <summary>
        /// Plays a light beam effect from the button to the shadow.
        /// </summary>
        private void PlayLightBeamEffect(int shadowIndex)
        {
            if (_lightBeamParticles == null || shadowIndex >= _shadowShapes.Count)
                return;

            if (_shadowShapes[shadowIndex] != null && _lightButton != null)
            {
                // Position light beam particles between button and shadow
                Vector3 buttonPos = _lightButton.transform.position;
                Vector3 shadowPos = _shadowShapes[shadowIndex].transform.position;
                Vector3 midPoint = (buttonPos + shadowPos) / 2f;

                _lightBeamParticles.transform.position = midPoint;

                // Rotate beam towards shadow
                Vector3 direction = shadowPos - buttonPos;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                _lightBeamParticles.transform.rotation = Quaternion.Euler(0, 0, angle);

                _lightBeamParticles.Play();
            }
        }

        /// <summary>
        /// Plays the glow particle effect at a shadow's position.
        /// </summary>
        private void PlayGlowParticles(int shadowIndex)
        {
            if (_glowParticles == null || shadowIndex >= _shadowShapes.Count)
                return;

            if (_shadowShapes[shadowIndex] != null)
            {
                // Position particles at the shadow
                _glowParticles.transform.position = _shadowShapes[shadowIndex].transform.position;
                _glowParticles.Play();
            }
        }

        /// <summary>
        /// Updates the light overlay to gradually lighten the scene.
        /// </summary>
        private void UpdateLightOverlay()
        {
            if (_lightOverlay == null || _shadowShapes.Count == 0)
                return;

            // Calculate how much the scene should be lit (0 = all dark, 1 = all light)
            float lightProgress = (float)_shadowsRevealed / _shadowShapes.Count;

            // Fade the dark overlay based on progress
            float overlayAlpha = Mathf.Lerp(0.7f, 0f, lightProgress);
            Color overlayColor = new Color(0f, 0f, 0f, overlayAlpha);

            _lightOverlay.color = overlayColor;
        }

        /// <summary>
        /// Brightens the background scene.
        /// </summary>
        private IEnumerator BrightenScene()
        {
            if (_backgroundImage == null)
                yield break;

            Color startColor = _backgroundImage.color;
            float elapsed = 0f;

            while (elapsed < _sceneBrightenDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _sceneBrightenDuration;
                _backgroundImage.color = Color.Lerp(startColor, _backgroundBrightColor, t);
                yield return null;
            }

            _backgroundImage.color = _backgroundBrightColor;
        }
        #endregion

        #region Audio
        private void PlayFriendlySound()
        {
            if (_friendlySound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_friendlySound);
            }
        }

        private void PlayCompletionSound()
        {
            if (_completionSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_completionSound);
            }
        }

        private void PlayBackgroundMusic()
        {
            if (_backgroundMusic != null && _audioSource != null)
            {
                _audioSource.clip = _backgroundMusic;
                _audioSource.loop = true;
                _audioSource.Play();
            }
        }

        private void StopBackgroundMusic()
        {
            if (_audioSource != null && _audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
        }
        #endregion

        #region Completion
        /// <summary>
        /// Completes the maze and initiates return to home.
        /// </summary>
        private void CompleteMaze()
        {
            if (_isCompleted)
                return;

            _isCompleted = true;
            Debug.Log("[FearMazeController] Maze completed!");

            // Disable button
            if (_lightButton != null)
            {
                _lightButton.interactable = false;
            }

            // Show completion message
            if (_completionText != null)
            {
                _completionText.text = "Not so scary after all!";
                _completionText.gameObject.SetActive(true);
            }

            // Hide instruction text
            if (_instructionText != null)
            {
                _instructionText.gameObject.SetActive(false);
            }

            // Brighten the scene
            StartCoroutine(BrightenScene());

            // Stop background music
            StopBackgroundMusic();

            // Play completion sound
            PlayCompletionSound();

            // Mark maze as completed in GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteMaze(EmotionType.Fear);
            }

            // Auto-navigate back to Home after delay
            StartCoroutine(ReturnToHomeAfterDelay());
        }

        /// <summary>
        /// Waits for a delay then navigates to Credits if all mazes completed, otherwise returns to Home.
        /// </summary>
        private IEnumerator ReturnToHomeAfterDelay()
        {
            yield return new WaitForSeconds(_returnDelaySeconds);

            if (NavigationManager.Instance != null)
            {
                // Check if all mazes are completed - GameManager will handle the Credits transition
                if (GameManager.Instance != null && GameManager.Instance.AllMazesCompleted)
                {
                    NavigationManager.Instance.NavigateToCredits();
                }
                else
                {
                    NavigationManager.Instance.NavigateToHome();
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Resets the maze state (useful for testing).
        /// </summary>
        public void ResetMaze()
        {
            _shadowsRevealed = 0;
            _isCompleted = false;

            if (_lightButton != null)
            {
                _lightButton.interactable = true;
            }

            InitializeUI();
        }
        #endregion
    }
}
