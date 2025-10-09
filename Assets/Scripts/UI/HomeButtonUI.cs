using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EmotionMaze.Core;

namespace EmotionMaze.UI
{
    /// <summary>
    /// Controls the cloud-shaped Home button that appears in the top-left of all scenes.
    /// Provides navigation back to the Home scene from anywhere in the game.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class HomeButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Components
        private Button _button;
        private Image _buttonImage;
        #endregion

        #region Settings
        [Header("Visual Settings")]
        [SerializeField] private float _hoverScaleMultiplier = 1.1f;
        [SerializeField] private float _scaleAnimationSpeed = 8f;
        [SerializeField] private Color _normalColor = ColorPalette.Cream;
        [SerializeField] private Color _hoverColor = ColorPalette.SoftClay;

        [Header("Audio")]
        [SerializeField] private AudioClip _clickSound;
        [SerializeField] private AudioClip _hoverSound;

        [Header("Visibility")]
        [SerializeField] private bool _hideInHomeScene = true;
        #endregion

        #region State
        private Vector3 _normalScale;
        private Vector3 _targetScale;
        private bool _isHovering = false;
        private AudioSource _audioSource;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _button = GetComponent<Button>();
            _buttonImage = GetComponent<Image>();

            _normalScale = transform.localScale;
            _targetScale = _normalScale;

            // Setup audio
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;

            // Set initial color
            if (_buttonImage != null)
            {
                _buttonImage.color = _normalColor;
            }
        }

        private void OnEnable()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(OnHomeButtonClicked);
            }

            // Subscribe to scene changes to manage visibility
            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.OnSceneTransitionComplete += OnSceneChanged;
                UpdateVisibility();
            }
        }

        private void OnDisable()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnHomeButtonClicked);
            }

            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.OnSceneTransitionComplete -= OnSceneChanged;
            }
        }

        private void Update()
        {
            // Smooth scale animation
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                _targetScale,
                Time.deltaTime * _scaleAnimationSpeed
            );
        }
        #endregion

        #region Button Interaction
        /// <summary>
        /// Called when the home button is clicked.
        /// </summary>
        private void OnHomeButtonClicked()
        {
            PlayClickSound();

            // Navigate to home
            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateToHome();
            }
        }

        /// <summary>
        /// Called when pointer enters the button area.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
            _targetScale = _normalScale * _hoverScaleMultiplier;

            if (_buttonImage != null)
            {
                _buttonImage.color = _hoverColor;
            }

            PlayHoverSound();
        }

        /// <summary>
        /// Called when pointer exits the button area.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
            _targetScale = _normalScale;

            if (_buttonImage != null)
            {
                _buttonImage.color = _normalColor;
            }
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

        #region Visibility Management
        /// <summary>
        /// Called when scene changes to update button visibility.
        /// </summary>
        private void OnSceneChanged(string sceneName)
        {
            UpdateVisibility();
        }

        /// <summary>
        /// Updates button visibility based on current scene.
        /// </summary>
        private void UpdateVisibility()
        {
            if (!_hideInHomeScene)
            {
                gameObject.SetActive(true);
                return;
            }

            // Hide button when in Home scene
            bool isAtHome = NavigationManager.Instance != null && NavigationManager.Instance.IsAtHome();
            gameObject.SetActive(!isAtHome);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Forces the button to show regardless of scene.
        /// </summary>
        public void ForceShow()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Forces the button to hide regardless of scene.
        /// </summary>
        public void ForceHide()
        {
            gameObject.SetActive(false);
        }
        #endregion
    }
}
