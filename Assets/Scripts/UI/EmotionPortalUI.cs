using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EmotionMaze.Core;

namespace EmotionMaze.UI
{
    /// <summary>
    /// Controls a single emotion portal button on the Home screen.
    /// Handles visual feedback, pulsing animations, and navigation to the corresponding maze.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class EmotionPortalUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Settings
        [Header("Emotion Configuration")]
        [SerializeField] private EmotionType _emotionType = EmotionType.Anger;

        [Header("Visual Settings")]
        [SerializeField] private Image _portalImage;
        [SerializeField] private Image _glowImage;
        [SerializeField] private ParticleSystem _particleEffect;

        [Header("Animation Settings")]
        [SerializeField] private float _basePulseSpeed = 1.5f;
        [SerializeField] private float _activePulseSpeed = 2.5f;
        [SerializeField] private float _pulseIntensity = 0.1f;
        [SerializeField] private float _hoverScaleMultiplier = 1.15f;
        [SerializeField] private float _scaleAnimationSpeed = 8f;

        [Header("Audio")]
        [SerializeField] private AudioClip _clickSound;
        [SerializeField] private AudioClip _hoverSound;
        #endregion

        #region Components
        private Button _button;
        private AudioSource _audioSource;
        #endregion

        #region State
        private Vector3 _normalScale;
        private Vector3 _targetScale;
        private float _pulsePhase = 0f;
        private bool _isHovering = false;
        private bool _isActiveEmotion = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _button = GetComponent<Button>();
            _normalScale = transform.localScale;
            _targetScale = _normalScale;

            // Setup audio
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;

            // Initialize visual state
            InitializeVisuals();
        }

        private void OnEnable()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(OnPortalClicked);
            }

            // Subscribe to emotion changes
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEmotionChanged += OnEmotionChanged;
                CheckIfActiveEmotion();
            }
        }

        private void OnDisable()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnPortalClicked);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEmotionChanged -= OnEmotionChanged;
            }
        }

        private void Update()
        {
            UpdatePulseAnimation();
            UpdateScaleAnimation();
        }
        #endregion

        #region Initialization
        private void InitializeVisuals()
        {
            // Set portal color based on emotion type
            Color emotionColor = ColorPalette.GetEmotionColor(_emotionType);

            if (_portalImage != null)
            {
                _portalImage.color = emotionColor;
            }

            if (_glowImage != null)
            {
                _glowImage.color = new Color(emotionColor.r, emotionColor.g, emotionColor.b, 0.5f);
            }

            // Configure particle system color
            if (_particleEffect != null)
            {
                var main = _particleEffect.main;
                main.startColor = emotionColor;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when the portal button is clicked.
        /// </summary>
        private void OnPortalClicked()
        {
            PlayClickSound();

            // Navigate to the corresponding maze
            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateToMaze(_emotionType);
            }
        }

        /// <summary>
        /// Called when GameManager's emotion changes.
        /// </summary>
        private void OnEmotionChanged(EmotionType newEmotion)
        {
            CheckIfActiveEmotion();
        }

        /// <summary>
        /// Called when pointer enters the portal area.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
            _targetScale = _normalScale * _hoverScaleMultiplier;

            PlayHoverSound();

            // Start particle effect
            if (_particleEffect != null && !_particleEffect.isPlaying)
            {
                _particleEffect.Play();
            }
        }

        /// <summary>
        /// Called when pointer exits the portal area.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
            _targetScale = _normalScale;

            // Stop particle effect if not active emotion
            if (_particleEffect != null && !_isActiveEmotion)
            {
                _particleEffect.Stop();
            }
        }
        #endregion

        #region Visual Updates
        /// <summary>
        /// Checks if this portal's emotion matches the current game emotion.
        /// </summary>
        private void CheckIfActiveEmotion()
        {
            if (GameManager.Instance == null)
                return;

            _isActiveEmotion = (GameManager.Instance.CurrentEmotion == _emotionType);

            // Play particle effect if active
            if (_particleEffect != null)
            {
                if (_isActiveEmotion && !_particleEffect.isPlaying)
                {
                    _particleEffect.Play();
                }
                else if (!_isActiveEmotion && !_isHovering && _particleEffect.isPlaying)
                {
                    _particleEffect.Stop();
                }
            }
        }

        /// <summary>
        /// Updates the pulsing animation of the portal.
        /// </summary>
        private void UpdatePulseAnimation()
        {
            // Use faster pulse if this is the active emotion
            float pulseSpeed = _isActiveEmotion ? _activePulseSpeed : _basePulseSpeed;
            _pulsePhase += Time.deltaTime * pulseSpeed;

            float pulseValue = 1f + (Mathf.Sin(_pulsePhase) * _pulseIntensity);

            // Apply pulse to glow image
            if (_glowImage != null)
            {
                Color glowColor = _glowImage.color;
                glowColor.a = 0.3f + (Mathf.Sin(_pulsePhase) * 0.2f);
                _glowImage.color = glowColor;

                _glowImage.transform.localScale = Vector3.one * pulseValue * 1.2f;
            }
        }

        /// <summary>
        /// Updates the scale animation (for hover effects).
        /// </summary>
        private void UpdateScaleAnimation()
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                _targetScale,
                Time.deltaTime * _scaleAnimationSpeed
            );
        }
        #endregion

        #region Audio
        private void PlayClickSound()
        {
            if (_clickSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_clickSound);
            }
        }

        private void PlayHoverSound()
        {
            if (_hoverSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_hoverSound);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the emotion type this portal represents.
        /// </summary>
        public EmotionType EmotionType => _emotionType;

        /// <summary>
        /// Sets the emotion type for this portal (useful for dynamic setup).
        /// </summary>
        public void SetEmotionType(EmotionType emotion)
        {
            _emotionType = emotion;
            InitializeVisuals();
            CheckIfActiveEmotion();
        }

        /// <summary>
        /// Enables or disables the portal button.
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            if (_button != null)
            {
                _button.interactable = interactable;
            }
        }
        #endregion
    }
}
