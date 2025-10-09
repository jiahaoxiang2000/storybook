using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using EmotionMaze.Core;

namespace EmotionMaze.Mazes
{
    /// <summary>
    /// Controls the Sadness Maze mechanics.
    /// Teaches emotional regulation through cognitive reframing (finding happy thoughts).
    /// Player taps 3-4 grey thought bubbles to restore their color.
    /// </summary>
    public class SadnessMazeController : MonoBehaviour
    {
        #region Settings
        [Header("UI References")]
        [SerializeField] private Text _instructionText;
        [SerializeField] private Text _completionText;
        [SerializeField] private Text _counterText;

        [Header("Thought Bubbles")]
        [SerializeField] private List<Button> _thoughtBubbles = new List<Button>();

        [Header("Particle Effects")]
        [SerializeField] private ParticleSystem _rainParticles;

        [Header("Audio")]
        [SerializeField] private AudioClip _chimeSound;
        [SerializeField] private AudioClip _completionSound;
        [SerializeField] private AudioClip _backgroundMusic;

        [Header("Gameplay Settings")]
        [SerializeField] private float _returnDelaySeconds = 2f;
        [SerializeField] private float _bubblePulseSpeed = 2f;
        [SerializeField] private float _bubblePulseIntensity = 0.05f;

        [Header("Visual Feedback")]
        [SerializeField] private Color _bubbleGreyColor = new Color(0.96f, 0.96f, 0.96f); // #F5F5F5
        [SerializeField] private Color[] _bubbleColorfulColors = new Color[]
        {
            new Color(0.4f, 0.73f, 0.42f),   // #66BB6A - Green
            new Color(1f, 0.79f, 0.16f),      // #FFCA28 - Yellow
            new Color(0.5f, 0.8f, 0.77f),     // #80CBC4 - Teal
            new Color(1f, 0.55f, 0.73f),     // #FF8CBA - Pink (4th bubble)
        };

        [Header("Happy Thoughts")]
        [SerializeField] private string[] _happyThoughts = new string[]
        {
            "Friends!",
            "Play!",
            "Sunshine!",
            "Hugs!"
        };
        #endregion

        #region State
        private int _bubblesTapped = 0;
        private bool _isCompleted = false;
        private AudioSource _audioSource;
        private Dictionary<Button, bool> _bubbleStates = new Dictionary<Button, bool>();
        private Dictionary<Button, Vector3> _bubbleNormalScales = new Dictionary<Button, Vector3>();
        private float _pulsePhase = 0f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Setup audio
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;

            // Initialize bubble states and scales
            foreach (var bubble in _thoughtBubbles)
            {
                if (bubble != null)
                {
                    _bubbleStates[bubble] = false; // false = grey, true = colorful
                    _bubbleNormalScales[bubble] = bubble.transform.localScale;
                }
            }

            InitializeUI();
        }

        private void OnEnable()
        {
            // Add listeners to all thought bubbles
            for (int i = 0; i < _thoughtBubbles.Count; i++)
            {
                if (_thoughtBubbles[i] != null)
                {
                    int index = i; // Capture index for closure
                    _thoughtBubbles[i].onClick.AddListener(() => OnThoughtBubbleTapped(index));
                }
            }
        }

        private void OnDisable()
        {
            // Remove listeners
            foreach (var bubble in _thoughtBubbles)
            {
                if (bubble != null)
                {
                    bubble.onClick.RemoveAllListeners();
                }
            }
        }

        private void Update()
        {
            if (!_isCompleted)
            {
                UpdateBubblePulse();
            }
        }
        #endregion

        #region Initialization
        private void InitializeUI()
        {
            // Set instruction text
            if (_instructionText != null)
            {
                _instructionText.text = "Moro feels sad and grey. Let's find happy thoughts!";
                _instructionText.gameObject.SetActive(true);
            }

            // Initialize all bubbles to grey with "?" text
            for (int i = 0; i < _thoughtBubbles.Count; i++)
            {
                if (_thoughtBubbles[i] != null)
                {
                    Image bubbleImage = _thoughtBubbles[i].GetComponent<Image>();
                    if (bubbleImage != null)
                    {
                        bubbleImage.color = _bubbleGreyColor;
                    }

                    // Reset text to "?"
                    Text bubbleText = _thoughtBubbles[i].GetComponentInChildren<Text>();
                    if (bubbleText != null)
                    {
                        bubbleText.text = "?";
                        bubbleText.color = new Color(0.7f, 0.7f, 0.7f);
                        bubbleText.fontSize = 48;
                    }
                }
            }

            // Hide completion text initially
            if (_completionText != null)
            {
                _completionText.gameObject.SetActive(false);
            }

            // Initialize counter text
            UpdateCounterText();

            // Start rain particles
            if (_rainParticles != null)
            {
                _rainParticles.Play();
            }

            // Start background music
            PlayBackgroundMusic();

            Debug.Log("[SadnessMazeController] Maze initialized");
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when a thought bubble is tapped.
        /// </summary>
        private void OnThoughtBubbleTapped(int bubbleIndex)
        {
            if (_isCompleted)
                return;

            if (bubbleIndex >= _thoughtBubbles.Count)
                return;

            Button bubble = _thoughtBubbles[bubbleIndex];
            if (bubble == null)
                return;

            // Check if already tapped
            if (_bubbleStates[bubble])
                return;

            // Mark as tapped
            _bubbleStates[bubble] = true;
            _bubblesTapped++;
            Debug.Log($"[SadnessMazeController] Bubble {bubbleIndex + 1} tapped. Total: {_bubblesTapped}/{_thoughtBubbles.Count}");

            // Play chime sound
            PlayChimeSound();

            // Change bubble color with bloom effect
            ColorBubble(bubble, bubbleIndex);

            // Update counter
            UpdateCounterText();

            // Check for completion
            if (_bubblesTapped >= _thoughtBubbles.Count)
            {
                CompleteMaze();
            }
        }
        #endregion

        #region Visual Updates
        /// <summary>
        /// Updates the bubble pulsing animation for untapped bubbles.
        /// </summary>
        private void UpdateBubblePulse()
        {
            _pulsePhase += Time.deltaTime * _bubblePulseSpeed;
            float pulseValue = 1f + (Mathf.Sin(_pulsePhase) * _bubblePulseIntensity);

            foreach (var kvp in _bubbleStates)
            {
                Button bubble = kvp.Key;
                bool isTapped = kvp.Value;

                if (bubble != null && !isTapped && _bubbleNormalScales.ContainsKey(bubble))
                {
                    bubble.transform.localScale = _bubbleNormalScales[bubble] * pulseValue;
                }
            }
        }

        /// <summary>
        /// Changes a bubble from grey to colorful with a bloom effect.
        /// </summary>
        private void ColorBubble(Button bubble, int bubbleIndex)
        {
            Image bubbleImage = bubble.GetComponent<Image>();
            if (bubbleImage == null)
                return;

            // Select color from the array
            Color targetColor = _bubbleColorfulColors[bubbleIndex % _bubbleColorfulColors.Length];

            // Update bubble text to show happy thought
            Text bubbleText = bubble.GetComponentInChildren<Text>();
            if (bubbleText != null && bubbleIndex < _happyThoughts.Length)
            {
                bubbleText.text = _happyThoughts[bubbleIndex];
                bubbleText.color = Color.white;
                bubbleText.fontSize = 32;
            }

            // Start color transition coroutine
            StartCoroutine(TransitionBubbleColor(bubbleImage, targetColor));
        }

        /// <summary>
        /// Smoothly transitions a bubble from grey to colorful.
        /// </summary>
        private IEnumerator TransitionBubbleColor(Image bubbleImage, Color targetColor)
        {
            Color startColor = bubbleImage.color;
            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                bubbleImage.color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }

            bubbleImage.color = targetColor;
        }

        /// <summary>
        /// Updates the counter text to show progress.
        /// </summary>
        private void UpdateCounterText()
        {
            if (_counterText != null)
            {
                _counterText.text = $"{_bubblesTapped}/{_thoughtBubbles.Count} happy thoughts";
            }
        }
        #endregion

        #region Audio
        private void PlayChimeSound()
        {
            if (_chimeSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_chimeSound);
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
            Debug.Log("[SadnessMazeController] Maze completed!");

            // Disable all bubbles
            foreach (var bubble in _thoughtBubbles)
            {
                if (bubble != null)
                {
                    bubble.interactable = false;
                }
            }

            // Stop rain
            if (_rainParticles != null)
            {
                _rainParticles.Stop();
            }

            // Show completion message
            if (_completionText != null)
            {
                _completionText.text = "Remembering good things helps!";
                _completionText.gameObject.SetActive(true);
            }

            // Hide instruction text
            if (_instructionText != null)
            {
                _instructionText.gameObject.SetActive(false);
            }

            // Stop background music
            StopBackgroundMusic();

            // Play completion sound
            PlayCompletionSound();

            // Mark maze as completed in GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteMaze(EmotionType.Sadness);
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
            _bubblesTapped = 0;
            _isCompleted = false;

            // Reset bubble states
            foreach (var bubble in _thoughtBubbles)
            {
                if (bubble != null)
                {
                    bubble.interactable = true;
                    _bubbleStates[bubble] = false;
                }
            }

            InitializeUI();
        }
        #endregion
    }
}
