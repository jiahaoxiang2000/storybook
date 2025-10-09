using UnityEngine;

namespace EmotionMaze.Core
{
    /// <summary>
    /// Controls the Home scene with Moro and emotion portals.
    /// Manages background music and scene initialization.
    /// </summary>
    public class HomeController : MonoBehaviour
    {
        #region Settings
        [Header("Audio")]
        [SerializeField] private AudioClip _backgroundMusic;
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
            InitializeHome();
        }

        private void OnDestroy()
        {
            // Stop music when scene is destroyed
            StopBackgroundMusic();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes the home scene with background music.
        /// </summary>
        private void InitializeHome()
        {
            if (_hasInitialized)
                return;

            _hasInitialized = true;

            Debug.Log("[HomeController] Home scene initialized");

            // Start background music
            PlayBackgroundMusic();
        }
        #endregion

        #region Audio
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
    }
}
