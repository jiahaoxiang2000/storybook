using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EmotionMaze.Core;

namespace EmotionMaze.Mazes
{
    /// <summary>
    /// Controls the Anger Maze mechanics.
    /// Teaches emotional regulation through deep breathing exercises.
    /// Player taps "Take Deep Breaths" button 3-5 times to help Moro cool down.
    /// </summary>
    public class AngerMazeController : MonoBehaviour
    {
        #region Settings
        [Header("UI References")]
        [SerializeField] private Text _instructionText;
        [SerializeField] private Button _breatheButton;
        [SerializeField] private Text _breatheButtonText;
        [SerializeField] private Text _completionText;
        [SerializeField] private Text _counterText;

        [Header("Particle Effects")]
        [SerializeField] private ParticleSystem _coolingParticles;

        [Header("Audio")]
        [SerializeField] private AudioClip _breathSound;
        [SerializeField] private AudioClip _completionSound;

        [Header("Gameplay Settings")]
        [SerializeField] private int _requiredBreaths = 5;
        [SerializeField] private float _returnDelaySeconds = 2f;
        [SerializeField] private float _buttonPulseSpeed = 2f;
        [SerializeField] private float _buttonPulseIntensity = 0.1f;

        [Header("Visual Feedback")]
        [SerializeField] private Color _buttonNormalColor = new Color(1f, 0.32f, 0.32f); // #FF5252
        [SerializeField] private Color _buttonCoolingColor = new Color(0.5f, 0.8f, 0.96f); // Blue
        #endregion

        #region State
        private int _breathCount = 0;
        private bool _isCompleted = false;
        private AudioSource _audioSource;
        private Image _buttonImage;
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
            if (_breatheButton != null)
            {
                _buttonImage = _breatheButton.GetComponent<Image>();
                _buttonNormalScale = _breatheButton.transform.localScale;
            }

            InitializeUI();
        }

        private void OnEnable()
        {
            if (_breatheButton != null)
            {
                _breatheButton.onClick.AddListener(OnBreatheButtonClicked);
            }
        }

        private void OnDisable()
        {
            if (_breatheButton != null)
            {
                _breatheButton.onClick.RemoveListener(OnBreatheButtonClicked);
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
                _instructionText.text = "Moro is very hot and angry. Help Moro cool down!";
                _instructionText.gameObject.SetActive(true);
            }

            // Set button text
            if (_breatheButtonText != null)
            {
                _breatheButtonText.text = "Take Deep Breaths";
            }

            // Set button color
            if (_buttonImage != null)
            {
                _buttonImage.color = _buttonNormalColor;
            }

            // Hide completion text initially
            if (_completionText != null)
            {
                _completionText.gameObject.SetActive(false);
            }

            // Initialize counter text
            UpdateCounterText();

            // Make sure particles are stopped initially
            if (_coolingParticles != null)
            {
                _coolingParticles.Stop();
            }

            Debug.Log("[AngerMazeController] Maze initialized");
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when the breathe button is clicked.
        /// </summary>
        private void OnBreatheButtonClicked()
        {
            if (_isCompleted)
                return;

            _breathCount++;
            Debug.Log($"[AngerMazeController] Breath {_breathCount}/{_requiredBreaths}");

            // Play breath sound
            PlayBreathSound();

            // Show cooling particles
            PlayCoolingParticles();

            // Update counter
            UpdateCounterText();

            // Transition button color towards cooling
            UpdateButtonColor();

            // Check for completion
            if (_breathCount >= _requiredBreaths)
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
            if (_breatheButton == null)
                return;

            _pulsePhase += Time.deltaTime * _buttonPulseSpeed;
            float pulseValue = 1f + (Mathf.Sin(_pulsePhase) * _buttonPulseIntensity);

            _breatheButton.transform.localScale = _buttonNormalScale * pulseValue;
        }

        /// <summary>
        /// Updates the button color based on progress.
        /// </summary>
        private void UpdateButtonColor()
        {
            if (_buttonImage == null)
                return;

            float progress = (float)_breathCount / _requiredBreaths;
            _buttonImage.color = Color.Lerp(_buttonNormalColor, _buttonCoolingColor, progress);
        }

        /// <summary>
        /// Updates the counter text to show progress.
        /// </summary>
        private void UpdateCounterText()
        {
            if (_counterText != null)
            {
                _counterText.text = $"{_breathCount}/{_requiredBreaths} breaths";
            }
        }
        #endregion

        #region Effects
        /// <summary>
        /// Plays the cooling particle effect.
        /// </summary>
        private void PlayCoolingParticles()
        {
            if (_coolingParticles != null)
            {
                _coolingParticles.Play();
            }
        }
        #endregion

        #region Audio
        private void PlayBreathSound()
        {
            if (_breathSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_breathSound);
            }
        }

        private void PlayCompletionSound()
        {
            if (_completionSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_completionSound);
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
            Debug.Log("[AngerMazeController] Maze completed!");

            // Disable button
            if (_breatheButton != null)
            {
                _breatheButton.interactable = false;
            }

            // Show completion message
            if (_completionText != null)
            {
                _completionText.text = "Moro feels calmer now!";
                _completionText.gameObject.SetActive(true);
            }

            // Hide instruction text
            if (_instructionText != null)
            {
                _instructionText.gameObject.SetActive(false);
            }

            // Play completion sound
            PlayCompletionSound();

            // Mark maze as completed in GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteMaze(EmotionType.Anger);
            }

            // Auto-navigate back to Home after delay
            StartCoroutine(ReturnToHomeAfterDelay());
        }

        /// <summary>
        /// Waits for a delay then returns to the home scene.
        /// </summary>
        private IEnumerator ReturnToHomeAfterDelay()
        {
            yield return new WaitForSeconds(_returnDelaySeconds);

            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateToHome();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Resets the maze state (useful for testing).
        /// </summary>
        public void ResetMaze()
        {
            _breathCount = 0;
            _isCompleted = false;

            if (_breatheButton != null)
            {
                _breatheButton.interactable = true;
            }

            InitializeUI();
        }
        #endregion
    }
}
