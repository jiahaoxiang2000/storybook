using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using EmotionMaze.Core;

namespace EmotionMaze.Mazes
{
    /// <summary>
    /// Controls the Joy Maze mechanics.
    /// Teaches emotional regulation through managing overstimulation.
    /// Player taps "Calm Down" button 5-6 times to slow down bouncing colorful orbs.
    /// </summary>
    public class JoyMazeController : MonoBehaviour
    {
        #region Settings
        [Header("UI References")]
        [SerializeField] private Text _instructionText;
        [SerializeField] private Button _calmButton;
        [SerializeField] private Text _calmButtonText;
        [SerializeField] private Text _completionText;
        [SerializeField] private Text _counterText;

        [Header("Orbs")]
        [SerializeField] private List<GameObject> _orbs = new List<GameObject>();

        [Header("Particle Effects")]
        [SerializeField] private ParticleSystem _swirlParticles;
        [SerializeField] private ParticleSystem _calmingParticles;

        [Header("Audio")]
        [SerializeField] private AudioClip _dingSound;
        [SerializeField] private AudioClip _completionSound;

        [Header("Gameplay Settings")]
        [SerializeField] private int _requiredTaps = 6;
        [SerializeField] private float _returnDelaySeconds = 2f;
        [SerializeField] private float _buttonPulseSpeed = 2f;
        [SerializeField] private float _buttonPulseIntensity = 0.1f;
        [SerializeField] private float _initialOrbSpeed = 300f;
        [SerializeField] private float _minOrbSpeed = 50f;

        [Header("Visual Feedback")]
        [SerializeField] private Color _buttonExcitedColor = new Color(1f, 0.92f, 0.23f); // #FFEB3B Yellow
        [SerializeField] private Color _buttonCalmColor = new Color(0.5f, 0.8f, 0.77f); // #80CBC4 Teal
        [SerializeField] private Color[] _orbColors = new Color[]
        {
            new Color(1f, 0.92f, 0.23f),   // #FFEB3B - Yellow
            new Color(0.93f, 0.25f, 0.48f), // #EC407A - Pink
            new Color(0.4f, 0.73f, 0.42f),  // #66BB6A - Green
            new Color(1f, 0.55f, 0.0f),     // #FF8C00 - Orange
            new Color(0.58f, 0.4f, 0.74f),  // #9466BC - Purple
            new Color(0.13f, 0.59f, 0.95f)  // #2196F3 - Blue
        };
        #endregion

        #region State
        private int _tapCount = 0;
        private bool _isCompleted = false;
        private AudioSource _audioSource;
        private Image _buttonImage;
        private Vector3 _buttonNormalScale;
        private float _pulsePhase = 0f;

        // Orb bouncing state
        private Dictionary<GameObject, Vector2> _orbVelocities = new Dictionary<GameObject, Vector2>();
        private Dictionary<GameObject, float> _orbSpeeds = new Dictionary<GameObject, float>();
        private RectTransform _canvasRect;
        private Vector2 _boundsMin;
        private Vector2 _boundsMax;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Setup audio
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;

            // Get button image
            if (_calmButton != null)
            {
                _buttonImage = _calmButton.GetComponent<Image>();
                _buttonNormalScale = _calmButton.transform.localScale;
            }

            // Get canvas bounds for orb bouncing
            Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                _canvasRect = canvas.GetComponent<RectTransform>();

                // Calculate bounds based on canvas size
                Vector2 canvasSize = _canvasRect.sizeDelta;
                _boundsMin = new Vector2(-canvasSize.x / 2f + 50f, -canvasSize.y / 2f + 50f);
                _boundsMax = new Vector2(canvasSize.x / 2f - 50f, canvasSize.y / 2f - 50f);
            }

            InitializeUI();
            InitializeOrbs();
        }

        private void OnEnable()
        {
            if (_calmButton != null)
            {
                _calmButton.onClick.AddListener(OnCalmButtonClicked);
            }
        }

        private void OnDisable()
        {
            if (_calmButton != null)
            {
                _calmButton.onClick.RemoveListener(OnCalmButtonClicked);
            }
        }

        private void Update()
        {
            if (!_isCompleted)
            {
                UpdateButtonPulse();
                UpdateOrbMovement();
            }
        }
        #endregion

        #region Initialization
        private void InitializeUI()
        {
            // Set instruction text
            if (_instructionText != null)
            {
                _instructionText.text = "Too much excitement! Let's find calm.";
                _instructionText.gameObject.SetActive(true);
            }

            // Set button text
            if (_calmButtonText != null)
            {
                _calmButtonText.text = "Calm Down";
            }

            // Set button color
            if (_buttonImage != null)
            {
                _buttonImage.color = _buttonExcitedColor;
            }

            // Hide completion text initially
            if (_completionText != null)
            {
                _completionText.gameObject.SetActive(false);
            }

            // Initialize counter text
            UpdateCounterText();

            // Start swirl particles
            if (_swirlParticles != null)
            {
                _swirlParticles.Play();
            }

            // Make sure calming particles are stopped initially
            if (_calmingParticles != null)
            {
                _calmingParticles.Stop();
            }

            Debug.Log("[JoyMazeController] Maze initialized");
        }

        private void InitializeOrbs()
        {
            // Initialize each orb with random velocity and speed
            for (int i = 0; i < _orbs.Count; i++)
            {
                if (_orbs[i] != null)
                {
                    // Set orb color
                    Image orbImage = _orbs[i].GetComponent<Image>();
                    if (orbImage != null)
                    {
                        orbImage.color = _orbColors[i % _orbColors.Length];
                    }

                    // Set random initial velocity
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;
                    _orbVelocities[_orbs[i]] = randomDirection;
                    _orbSpeeds[_orbs[i]] = _initialOrbSpeed;

                    // Position orb randomly within bounds
                    RectTransform orbRect = _orbs[i].GetComponent<RectTransform>();
                    if (orbRect != null)
                    {
                        float randomX = Random.Range(_boundsMin.x + 100f, _boundsMax.x - 100f);
                        float randomY = Random.Range(_boundsMin.y + 100f, _boundsMax.y - 100f);
                        orbRect.anchoredPosition = new Vector2(randomX, randomY);
                    }
                }
            }

            Debug.Log($"[JoyMazeController] Initialized {_orbs.Count} orbs");
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when the calm button is clicked.
        /// </summary>
        private void OnCalmButtonClicked()
        {
            if (_isCompleted)
                return;

            _tapCount++;
            Debug.Log($"[JoyMazeController] Calm tap {_tapCount}/{_requiredTaps}");

            // Play ding sound
            PlayDingSound();

            // Show calming particles
            PlayCalmingParticles();

            // Slow down all orbs
            SlowDownOrbs();

            // Update counter
            UpdateCounterText();

            // Transition button color towards calm
            UpdateButtonColor();

            // Check for completion
            if (_tapCount >= _requiredTaps)
            {
                CompleteMaze();
            }
        }
        #endregion

        #region Orb Movement
        /// <summary>
        /// Updates the movement of all orbs with bouncing physics.
        /// </summary>
        private void UpdateOrbMovement()
        {
            foreach (var orb in _orbs)
            {
                if (orb == null || !_orbVelocities.ContainsKey(orb))
                    continue;

                RectTransform orbRect = orb.GetComponent<RectTransform>();
                if (orbRect == null)
                    continue;

                // Get current velocity and speed
                Vector2 velocity = _orbVelocities[orb];
                float speed = _orbSpeeds[orb];

                // Move orb
                Vector2 newPosition = orbRect.anchoredPosition + velocity * speed * Time.deltaTime;

                // Bounce off walls
                if (newPosition.x <= _boundsMin.x || newPosition.x >= _boundsMax.x)
                {
                    velocity.x *= -1f;
                    newPosition.x = Mathf.Clamp(newPosition.x, _boundsMin.x, _boundsMax.x);
                }

                if (newPosition.y <= _boundsMin.y || newPosition.y >= _boundsMax.y)
                {
                    velocity.y *= -1f;
                    newPosition.y = Mathf.Clamp(newPosition.y, _boundsMin.y, _boundsMax.y);
                }

                // Update orb position and velocity
                orbRect.anchoredPosition = newPosition;
                _orbVelocities[orb] = velocity;
            }
        }

        /// <summary>
        /// Slows down all orbs gradually.
        /// </summary>
        private void SlowDownOrbs()
        {
            float slowdownFactor = 1f - ((float)_tapCount / _requiredTaps);
            float targetSpeed = Mathf.Lerp(_minOrbSpeed, _initialOrbSpeed, slowdownFactor);

            foreach (var orb in _orbs)
            {
                if (orb != null && _orbSpeeds.ContainsKey(orb))
                {
                    _orbSpeeds[orb] = targetSpeed;
                }
            }
        }
        #endregion

        #region Visual Updates
        /// <summary>
        /// Updates the button pulsing animation.
        /// </summary>
        private void UpdateButtonPulse()
        {
            if (_calmButton == null)
                return;

            _pulsePhase += Time.deltaTime * _buttonPulseSpeed;
            float pulseValue = 1f + (Mathf.Sin(_pulsePhase) * _buttonPulseIntensity);

            _calmButton.transform.localScale = _buttonNormalScale * pulseValue;
        }

        /// <summary>
        /// Updates the button color based on progress.
        /// </summary>
        private void UpdateButtonColor()
        {
            if (_buttonImage == null)
                return;

            float progress = (float)_tapCount / _requiredTaps;
            _buttonImage.color = Color.Lerp(_buttonExcitedColor, _buttonCalmColor, progress);
        }

        /// <summary>
        /// Updates the counter text to show progress.
        /// </summary>
        private void UpdateCounterText()
        {
            if (_counterText != null)
            {
                _counterText.text = $"{_tapCount}/{_requiredTaps} calm taps";
            }
        }
        #endregion

        #region Effects
        /// <summary>
        /// Plays the calming particle effect.
        /// </summary>
        private void PlayCalmingParticles()
        {
            if (_calmingParticles != null)
            {
                _calmingParticles.Play();
            }
        }

        /// <summary>
        /// Settles all orbs to a stop and fades swirl particles.
        /// </summary>
        private IEnumerator SettleScene()
        {
            float duration = 1f;
            float elapsed = 0f;

            // Store initial speeds
            Dictionary<GameObject, float> initialSpeeds = new Dictionary<GameObject, float>();
            foreach (var orb in _orbs)
            {
                if (orb != null && _orbSpeeds.ContainsKey(orb))
                {
                    initialSpeeds[orb] = _orbSpeeds[orb];
                }
            }

            // Gradually stop all orbs
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                foreach (var orb in _orbs)
                {
                    if (orb != null && _orbSpeeds.ContainsKey(orb) && initialSpeeds.ContainsKey(orb))
                    {
                        _orbSpeeds[orb] = Mathf.Lerp(initialSpeeds[orb], 0f, t);
                    }
                }

                yield return null;
            }

            // Stop swirl particles
            if (_swirlParticles != null)
            {
                _swirlParticles.Stop();
            }
        }
        #endregion

        #region Audio
        private void PlayDingSound()
        {
            if (_dingSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_dingSound);
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
            Debug.Log("[JoyMazeController] Maze completed!");

            // Disable button
            if (_calmButton != null)
            {
                _calmButton.interactable = false;
            }

            // Show completion message
            if (_completionText != null)
            {
                _completionText.text = "Finding calm in the fun!";
                _completionText.gameObject.SetActive(true);
            }

            // Hide instruction text
            if (_instructionText != null)
            {
                _instructionText.gameObject.SetActive(false);
            }

            // Settle the scene (stop orbs and fade swirls)
            StartCoroutine(SettleScene());

            // Play completion sound
            PlayCompletionSound();

            // Mark maze as completed in GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteMaze(EmotionType.Joy);
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
            _tapCount = 0;
            _isCompleted = false;

            if (_calmButton != null)
            {
                _calmButton.interactable = true;
            }

            InitializeUI();
            InitializeOrbs();
        }
        #endregion
    }
}
