using UnityEngine;
using UnityEngine.UI;
using EmotionMaze.Core;
using System.Collections.Generic;

namespace EmotionMaze.UI
{
    /// <summary>
    /// Displays 4 dots at the bottom of the screen representing progress through the 4 emotion mazes.
    /// Active emotion's dot is highlighted with the corresponding emotion color.
    /// </summary>
    public class ProgressIndicator : MonoBehaviour
    {
        #region Settings
        [Header("Dot References")]
        [SerializeField] private Image _angerDot;
        [SerializeField] private Image _sadnessDot;
        [SerializeField] private Image _fearDot;
        [SerializeField] private Image _joyDot;

        [Header("Visual Settings")]
        [SerializeField] private float _dotPulseSpeed = 2f;
        [SerializeField] private float _dotPulseIntensity = 0.15f;
        [SerializeField] private float _completedDotScale = 1.2f;

        [Header("Colors")]
        [SerializeField] private Color _inactiveColor = ColorPalette.UI.Inactive;
        [SerializeField] private bool _showCompletedState = true;
        #endregion

        #region State
        private Dictionary<EmotionType, Image> _dotMap;
        private EmotionType _currentEmotion = EmotionType.Calm;
        private float _pulsePhase = 0f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeDotMap();
            ResetAllDots();
        }

        private void OnEnable()
        {
            // Subscribe to emotion and maze completion events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEmotionChanged += OnEmotionChanged;
                GameManager.Instance.OnMazeCompleted += OnMazeCompleted;

                // Initialize with current state
                if (_dotMap != null && _dotMap.Count > 0)
                {
                    UpdateAllDots();
                }
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEmotionChanged -= OnEmotionChanged;
                GameManager.Instance.OnMazeCompleted -= OnMazeCompleted;
            }
        }

        private void Update()
        {
            if (_dotMap != null && _dotMap.Count > 0)
            {
                UpdatePulseEffect();
            }
        }
        #endregion

        #region Initialization
        private void InitializeDotMap()
        {
            _dotMap = new Dictionary<EmotionType, Image>();

            if (_angerDot != null) _dotMap.Add(EmotionType.Anger, _angerDot);
            if (_sadnessDot != null) _dotMap.Add(EmotionType.Sadness, _sadnessDot);
            if (_fearDot != null) _dotMap.Add(EmotionType.Fear, _fearDot);
            if (_joyDot != null) _dotMap.Add(EmotionType.Joy, _joyDot);
        }

        private void ResetAllDots()
        {
            foreach (var dot in _dotMap.Values)
            {
                if (dot != null)
                {
                    dot.color = _inactiveColor;
                    dot.transform.localScale = Vector3.one;
                }
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when the current emotion changes.
        /// </summary>
        private void OnEmotionChanged(EmotionType newEmotion)
        {
            _currentEmotion = newEmotion;
            UpdateAllDots();
        }

        /// <summary>
        /// Called when a maze is completed.
        /// </summary>
        private void OnMazeCompleted(EmotionType emotion)
        {
            UpdateDotForEmotion(emotion);
        }
        #endregion

        #region Dot Updates
        /// <summary>
        /// Updates all dots based on current game state.
        /// </summary>
        private void UpdateAllDots()
        {
            foreach (var emotion in _dotMap.Keys)
            {
                UpdateDotForEmotion(emotion);
            }
        }

        /// <summary>
        /// Updates a specific dot based on its emotion state.
        /// </summary>
        private void UpdateDotForEmotion(EmotionType emotion)
        {
            if (!_dotMap.TryGetValue(emotion, out Image dot) || dot == null)
                return;

            bool isCurrentEmotion = (_currentEmotion == emotion);
            bool isCompleted = GameManager.Instance != null && GameManager.Instance.IsMazeCompleted(emotion);

            // Determine color
            Color targetColor;
            if (isCurrentEmotion)
            {
                // Active emotion: use emotion color
                targetColor = ColorPalette.GetEmotionColor(emotion);
            }
            else if (isCompleted && _showCompletedState)
            {
                // Completed: use dimmed emotion color
                targetColor = ColorPalette.GetEmotionColor(emotion) * 0.6f;
            }
            else
            {
                // Inactive: use inactive color
                targetColor = _inactiveColor;
            }

            dot.color = targetColor;

            // Determine scale
            float targetScale = (isCompleted && _showCompletedState) ? _completedDotScale : 1f;
            dot.transform.localScale = Vector3.one * targetScale;
        }
        #endregion

        #region Visual Effects
        /// <summary>
        /// Applies subtle pulsing effect to the currently active emotion dot.
        /// </summary>
        private void UpdatePulseEffect()
        {
            if (_currentEmotion == EmotionType.Calm)
                return;

            if (!_dotMap.TryGetValue(_currentEmotion, out Image activeDot) || activeDot == null)
                return;

            _pulsePhase += Time.deltaTime * _dotPulseSpeed;
            float pulseValue = 1f + (Mathf.Sin(_pulsePhase) * _dotPulseIntensity);

            bool isCompleted = GameManager.Instance != null && GameManager.Instance.IsMazeCompleted(_currentEmotion);
            float baseScale = (isCompleted && _showCompletedState) ? _completedDotScale : 1f;

            activeDot.transform.localScale = Vector3.one * baseScale * pulseValue;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Manually refreshes all dot states (useful after loading a saved game).
        /// </summary>
        public void RefreshDots()
        {
            UpdateAllDots();
        }

        /// <summary>
        /// Shows or hides the entire progress indicator.
        /// </summary>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
        #endregion
    }
}
