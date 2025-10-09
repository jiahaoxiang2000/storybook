using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

namespace EmotionMaze.Core
{
    /// <summary>
    /// Manages scene transitions and navigation throughout the game.
    /// Handles smooth transitions between Home, Mazes, and Credits scenes.
    /// </summary>
    public class NavigationManager : MonoBehaviour
    {
        #region Singleton
        private static NavigationManager _instance;
        public static NavigationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<NavigationManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("NavigationManager");
                        _instance = go.AddComponent<NavigationManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Scene Names
        private const string SCENE_HOME = "Home";
        private const string SCENE_MAZE_ANGER = "Maze_Anger";
        private const string SCENE_MAZE_SADNESS = "Maze_Sadness";
        private const string SCENE_MAZE_FEAR = "Maze_Fear";
        private const string SCENE_MAZE_JOY = "Maze_Joy";
        private const string SCENE_CREDITS = "Credits";
        #endregion

        #region Events
        /// <summary>
        /// Event fired when a scene transition begins.
        /// </summary>
        public event Action<string> OnSceneTransitionStart;

        /// <summary>
        /// Event fired when a scene transition completes.
        /// </summary>
        public event Action<string> OnSceneTransitionComplete;
        #endregion

        #region State
        [Header("Transition Settings")]
        [SerializeField] private float _transitionDuration = 0.5f;
        [SerializeField] private bool _isTransitioning = false;

        public bool IsTransitioning => _isTransitioning;
        public string CurrentSceneName => SceneManager.GetActiveScene().name;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Implement singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log("[NavigationManager] Initialized");
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        #endregion

        #region Navigation Methods
        /// <summary>
        /// Navigates to the Home scene.
        /// </summary>
        public void NavigateToHome()
        {
            // Reset to Calm emotion BEFORE transitioning home
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetEmotion(EmotionType.Calm);
            }

            LoadScene(SCENE_HOME, () =>
            {
                // Just set game state, emotion already set above
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetGameState(GameState.Home);
                }
            });
        }

        /// <summary>
        /// Navigates to a specific emotion maze.
        /// </summary>
        /// <param name="emotion">The emotion maze to navigate to</param>
        public void NavigateToMaze(EmotionType emotion)
        {
            string sceneName = GetMazeSceneName(emotion);
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"[NavigationManager] Invalid emotion type: {emotion}");
                return;
            }

            // Set emotion BEFORE scene transition so Moro updates immediately
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetEmotion(emotion);
            }

            LoadScene(sceneName, () =>
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.EnterMaze(emotion);
                }
            });
        }

        /// <summary>
        /// Navigates to the Credits scene.
        /// </summary>
        public void NavigateToCredits()
        {
            LoadScene(SCENE_CREDITS, () =>
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetGameState(GameState.Credits);
                }
            });
        }

        /// <summary>
        /// Replays the current maze (reloads the current maze scene).
        /// </summary>
        public void ReplayCurrentMaze()
        {
            string currentScene = CurrentSceneName;

            // Verify we're in a maze scene
            if (!currentScene.StartsWith("Maze_"))
            {
                Debug.LogWarning("[NavigationManager] Replay called but not in a maze scene");
                return;
            }

            LoadScene(currentScene, () =>
            {
                Debug.Log($"[NavigationManager] Replaying maze: {currentScene}");
            });
        }
        #endregion

        #region Scene Loading
        /// <summary>
        /// Loads a scene with optional callback after loading completes.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <param name="onComplete">Optional callback to execute after scene loads</param>
        private void LoadScene(string sceneName, Action onComplete = null)
        {
            if (_isTransitioning)
            {
                Debug.LogWarning($"[NavigationManager] Already transitioning, ignoring request to load {sceneName}");
                return;
            }

            StartCoroutine(LoadSceneCoroutine(sceneName, onComplete));
        }

        /// <summary>
        /// Coroutine that handles the actual scene loading with transition effects.
        /// </summary>
        private IEnumerator LoadSceneCoroutine(string sceneName, Action onComplete)
        {
            _isTransitioning = true;

            Debug.Log($"[NavigationManager] Starting transition to: {sceneName}");
            OnSceneTransitionStart?.Invoke(sceneName);

            // TODO: Add fade-out effect here (Phase 10)
            // For now, just a simple delay
            yield return new WaitForSeconds(_transitionDuration * 0.5f);

            // Load the scene
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            // Wait until the scene is loaded
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            // Activate the scene
            asyncLoad.allowSceneActivation = true;

            // Wait for scene activation
            yield return new WaitUntil(() => asyncLoad.isDone);

            // Execute callback
            onComplete?.Invoke();

            // TODO: Add fade-in effect here (Phase 10)
            yield return new WaitForSeconds(_transitionDuration * 0.5f);

            Debug.Log($"[NavigationManager] Transition to {sceneName} complete");
            OnSceneTransitionComplete?.Invoke(sceneName);

            _isTransitioning = false;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Gets the scene name for a specific emotion maze.
        /// </summary>
        /// <param name="emotion">The emotion type</param>
        /// <returns>The scene name, or empty string if invalid</returns>
        private string GetMazeSceneName(EmotionType emotion)
        {
            return emotion switch
            {
                EmotionType.Anger => SCENE_MAZE_ANGER,
                EmotionType.Sadness => SCENE_MAZE_SADNESS,
                EmotionType.Fear => SCENE_MAZE_FEAR,
                EmotionType.Joy => SCENE_MAZE_JOY,
                _ => string.Empty
            };
        }

        /// <summary>
        /// Gets the emotion type from a maze scene name.
        /// </summary>
        /// <param name="sceneName">The scene name</param>
        /// <returns>The emotion type, or Calm if not a maze scene</returns>
        public EmotionType GetEmotionFromSceneName(string sceneName)
        {
            return sceneName switch
            {
                SCENE_MAZE_ANGER => EmotionType.Anger,
                SCENE_MAZE_SADNESS => EmotionType.Sadness,
                SCENE_MAZE_FEAR => EmotionType.Fear,
                SCENE_MAZE_JOY => EmotionType.Joy,
                _ => EmotionType.Calm
            };
        }

        /// <summary>
        /// Checks if the current scene is a maze scene.
        /// </summary>
        /// <returns>True if in a maze, false otherwise</returns>
        public bool IsInMaze()
        {
            return CurrentSceneName.StartsWith("Maze_");
        }

        /// <summary>
        /// Checks if the current scene is the home scene.
        /// </summary>
        /// <returns>True if at home, false otherwise</returns>
        public bool IsAtHome()
        {
            return CurrentSceneName == SCENE_HOME;
        }

        /// <summary>
        /// Checks if the current scene is the credits scene.
        /// </summary>
        /// <returns>True if at credits, false otherwise</returns>
        public bool IsAtCredits()
        {
            return CurrentSceneName == SCENE_CREDITS;
        }
        #endregion
    }
}
