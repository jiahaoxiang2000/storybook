using UnityEngine;
using EmotionMaze.Core;

namespace EmotionMaze.Character
{
    /// <summary>
    /// Controls Moro the little monster's appearance, color, and animations.
    /// Moro's color and scale change dynamically based on emotional state.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class MoroController : MonoBehaviour
    {
        #region Components
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        #endregion

        #region Settings
        [Header("Visual Settings")]
        [SerializeField] private float _colorTransitionSpeed = 2f;
        [SerializeField] private float _breathingSpeed = 1f;
        [SerializeField] private float _breathingIntensity = 0.05f;

        [Header("Emotion-Based Scale Modifiers")]
        [SerializeField] private float _angerScaleMultiplier = 1.2f;   // Inflated when angry
        [SerializeField] private float _sadnessScaleMultiplier = 0.8f; // Shrunk when sad
        [SerializeField] private float _fearScaleMultiplier = 0.9f;    // Slightly shrunk when afraid
        [SerializeField] private float _joyScaleMultiplier = 1.1f;     // Slightly larger when joyful
        [SerializeField] private float _scaleTransitionSpeed = 1.5f;

        [Header("Shadow Effect for Fear")]
        [SerializeField] private bool _enableFearShadow = true;
        [SerializeField] private Color _fearShadowColor = new Color(0, 0, 0, 0.5f);
        #endregion

        #region State
        private EmotionType _currentEmotion = EmotionType.Calm;
        private Color _targetColor;
        private Color _currentColor;
        private Vector3 _baseScale = Vector3.one;
        private Vector3 _targetScale = Vector3.one;
        private float _breathingPhase = 0f;

        // Rainbow effect for Joy emotion
        private bool _isRainbowActive = false;
        private float _rainbowPhase = 0f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();

            _baseScale = transform.localScale;
            _targetScale = _baseScale;

            // Initialize with calm color
            _currentColor = ColorPalette.Moro.CalmTeal;
            _targetColor = _currentColor;
            _spriteRenderer.color = _currentColor;
        }

        private void OnEnable()
        {
            // Subscribe to emotion changes
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEmotionChanged += OnEmotionChanged;
                // Set to current emotion if different from default
                SetEmotion(GameManager.Instance.CurrentEmotion);
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEmotionChanged -= OnEmotionChanged;
            }
        }

        private void Update()
        {
            UpdateColor();
            UpdateScale();
            UpdateBreathing();

            if (_isRainbowActive)
            {
                UpdateRainbowEffect();
            }
        }
        #endregion

        #region Emotion Handling
        /// <summary>
        /// Called when GameManager's emotion state changes.
        /// </summary>
        private void OnEmotionChanged(EmotionType newEmotion)
        {
            SetEmotion(newEmotion);
        }

        /// <summary>
        /// Sets Moro's emotional state and updates visuals accordingly.
        /// </summary>
        /// <param name="emotion">The new emotion</param>
        public void SetEmotion(EmotionType emotion)
        {
            if (_currentEmotion == emotion)
                return;

            _currentEmotion = emotion;

            // Update target color based on emotion
            _targetColor = GetEmotionColor(emotion);

            // Update target scale based on emotion
            _targetScale = _baseScale * GetEmotionScaleMultiplier(emotion);

            // Handle special effects
            _isRainbowActive = (emotion == EmotionType.Joy);

            Debug.Log($"[MoroController] Emotion changed to: {emotion}");

            // Trigger animation if available
            TriggerEmotionAnimation(emotion);
        }

        /// <summary>
        /// Gets the primary color for a given emotion.
        /// </summary>
        private Color GetEmotionColor(EmotionType emotion)
        {
            return emotion switch
            {
                EmotionType.Calm => ColorPalette.Moro.CalmTeal,
                EmotionType.Anger => ColorPalette.Moro.AngerBrightRed,
                EmotionType.Sadness => ColorPalette.Moro.SadnessDustyBlue,
                EmotionType.Fear => ColorPalette.Moro.FearIndigo,
                EmotionType.Joy => ColorPalette.Moro.JoyYellow,
                _ => ColorPalette.Moro.CalmTeal
            };
        }

        /// <summary>
        /// Gets the scale multiplier for a given emotion.
        /// </summary>
        private float GetEmotionScaleMultiplier(EmotionType emotion)
        {
            return emotion switch
            {
                EmotionType.Anger => _angerScaleMultiplier,
                EmotionType.Sadness => _sadnessScaleMultiplier,
                EmotionType.Fear => _fearScaleMultiplier,
                EmotionType.Joy => _joyScaleMultiplier,
                _ => 1f
            };
        }
        #endregion

        #region Visual Updates
        /// <summary>
        /// Smoothly transitions Moro's color to the target color.
        /// </summary>
        private void UpdateColor()
        {
            if (_isRainbowActive)
                return; // Rainbow effect handles color directly

            _currentColor = Color.Lerp(_currentColor, _targetColor, Time.deltaTime * _colorTransitionSpeed);
            _spriteRenderer.color = _currentColor;
        }

        /// <summary>
        /// Smoothly transitions Moro's scale to the target scale.
        /// </summary>
        private void UpdateScale()
        {
            Vector3 currentScale = transform.localScale;
            Vector3 newScale = Vector3.Lerp(currentScale, _targetScale, Time.deltaTime * _scaleTransitionSpeed);
            transform.localScale = newScale;
        }

        /// <summary>
        /// Applies subtle breathing animation to Moro's scale.
        /// </summary>
        private void UpdateBreathing()
        {
            _breathingPhase += Time.deltaTime * _breathingSpeed;
            float breathingOffset = Mathf.Sin(_breathingPhase) * _breathingIntensity;

            Vector3 breathingScale = _targetScale * (1f + breathingOffset);
            transform.localScale = Vector3.Lerp(transform.localScale, breathingScale, Time.deltaTime * _scaleTransitionSpeed);
        }

        /// <summary>
        /// Applies rainbow color cycling effect for Joy emotion.
        /// </summary>
        private void UpdateRainbowEffect()
        {
            _rainbowPhase += Time.deltaTime * 2f;

            // Cycle through rainbow colors
            int colorCount = ColorPalette.Joy.RainbowColors.Length;
            float normalizedPhase = (_rainbowPhase % colorCount) / colorCount;
            int colorIndex = Mathf.FloorToInt(_rainbowPhase) % colorCount;
            int nextColorIndex = (colorIndex + 1) % colorCount;

            Color currentRainbow = ColorPalette.Joy.RainbowColors[colorIndex];
            Color nextRainbow = ColorPalette.Joy.RainbowColors[nextColorIndex];

            float t = normalizedPhase * colorCount - colorIndex;
            _spriteRenderer.color = Color.Lerp(currentRainbow, nextRainbow, t);
        }
        #endregion

        #region Animation
        /// <summary>
        /// Triggers animator state based on emotion (if animator is present).
        /// </summary>
        private void TriggerEmotionAnimation(EmotionType emotion)
        {
            if (_animator == null)
                return;

            // Set animator parameters based on emotion
            // These trigger names should match animator controller setup
            switch (emotion)
            {
                case EmotionType.Calm:
                    _animator.SetTrigger("Calm");
                    break;
                case EmotionType.Anger:
                    _animator.SetTrigger("Angry");
                    break;
                case EmotionType.Sadness:
                    _animator.SetTrigger("Sad");
                    break;
                case EmotionType.Fear:
                    _animator.SetTrigger("Afraid");
                    break;
                case EmotionType.Joy:
                    _animator.SetTrigger("Joyful");
                    break;
            }
        }

        /// <summary>
        /// Plays a happy dance animation (for credits scene).
        /// </summary>
        public void PlayHappyDance()
        {
            if (_animator != null)
            {
                _animator.SetTrigger("Dance");
            }
        }

        /// <summary>
        /// Returns to idle breathing animation.
        /// </summary>
        public void PlayIdleBreathing()
        {
            if (_animator != null)
            {
                _animator.SetTrigger("Idle");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Manually sets Moro's color (for special effects).
        /// </summary>
        public void SetColor(Color color)
        {
            _targetColor = color;
            _currentColor = color;
            _spriteRenderer.color = color;
        }

        /// <summary>
        /// Gets the current emotion state.
        /// </summary>
        public EmotionType CurrentEmotion => _currentEmotion;

        /// <summary>
        /// Resets Moro to calm state immediately.
        /// </summary>
        public void ResetToCalm()
        {
            SetEmotion(EmotionType.Calm);
            _currentColor = _targetColor;
            _spriteRenderer.color = _currentColor;
            transform.localScale = _baseScale;
        }
        #endregion
    }
}
