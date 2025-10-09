using UnityEngine;
using System;

namespace EmotionMaze.Core
{
    /// <summary>
    /// Central game state manager implementing a singleton pattern.
    /// Manages overall game state, emotion tracking, and maze completion progress.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _instance = go.AddComponent<GameManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event fired when the game state changes.
        /// </summary>
        public event Action<GameState> OnGameStateChanged;

        /// <summary>
        /// Event fired when Moro's emotional state changes.
        /// </summary>
        public event Action<EmotionType> OnEmotionChanged;

        /// <summary>
        /// Event fired when a maze is completed.
        /// </summary>
        public event Action<EmotionType> OnMazeCompleted;
        #endregion

        #region State
        [Header("Game State")]
        [SerializeField] private GameState _currentState = GameState.Home;
        [SerializeField] private EmotionType _currentEmotion = EmotionType.Calm;

        [Header("Progress Tracking")]
        [SerializeField] private bool _angerMazeCompleted = false;
        [SerializeField] private bool _sadnessMazeCompleted = false;
        [SerializeField] private bool _fearMazeCompleted = false;
        [SerializeField] private bool _joyMazeCompleted = false;

        public GameState CurrentState => _currentState;
        public EmotionType CurrentEmotion => _currentEmotion;

        public bool AngerMazeCompleted => _angerMazeCompleted;
        public bool SadnessMazeCompleted => _sadnessMazeCompleted;
        public bool FearMazeCompleted => _fearMazeCompleted;
        public bool JoyMazeCompleted => _joyMazeCompleted;

        /// <summary>
        /// Gets the total number of completed mazes.
        /// </summary>
        public int CompletedMazeCount
        {
            get
            {
                int count = 0;
                if (_angerMazeCompleted) count++;
                if (_sadnessMazeCompleted) count++;
                if (_fearMazeCompleted) count++;
                if (_joyMazeCompleted) count++;
                return count;
            }
        }

        /// <summary>
        /// Checks if all mazes have been completed.
        /// </summary>
        public bool AllMazesCompleted => CompletedMazeCount == 4;
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

            InitializeGame();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        #endregion

        #region Initialization
        private void InitializeGame()
        {
            Debug.Log("[GameManager] Game initialized");
            SetGameState(GameState.Home);
            SetEmotion(EmotionType.Calm);
        }
        #endregion

        #region State Management
        /// <summary>
        /// Sets the current game state and notifies listeners.
        /// </summary>
        /// <param name="newState">The new game state</param>
        public void SetGameState(GameState newState)
        {
            if (_currentState == newState)
                return;

            GameState previousState = _currentState;
            _currentState = newState;

            Debug.Log($"[GameManager] State changed: {previousState} -> {newState}");
            OnGameStateChanged?.Invoke(newState);
        }

        /// <summary>
        /// Sets Moro's current emotional state and notifies listeners.
        /// </summary>
        /// <param name="newEmotion">The new emotion</param>
        public void SetEmotion(EmotionType newEmotion)
        {
            if (_currentEmotion == newEmotion)
                return;

            EmotionType previousEmotion = _currentEmotion;
            _currentEmotion = newEmotion;

            Debug.Log($"[GameManager] Emotion changed: {previousEmotion} -> {newEmotion}");
            OnEmotionChanged?.Invoke(newEmotion);
        }

        /// <summary>
        /// Enters a specific maze, updating both game state and emotion.
        /// </summary>
        /// <param name="emotion">The emotion maze to enter</param>
        public void EnterMaze(EmotionType emotion)
        {
            SetEmotion(emotion);

            GameState mazeState = emotion switch
            {
                EmotionType.Anger => GameState.MazeAnger,
                EmotionType.Sadness => GameState.MazeSadness,
                EmotionType.Fear => GameState.MazeFear,
                EmotionType.Joy => GameState.MazeJoy,
                _ => GameState.Home
            };

            SetGameState(mazeState);
        }

        /// <summary>
        /// Marks a maze as completed and transitions to the credits scene.
        /// </summary>
        /// <param name="emotion">The completed maze's emotion</param>
        public void CompleteMaze(EmotionType emotion)
        {
            bool wasAlreadyCompleted = false;

            switch (emotion)
            {
                case EmotionType.Anger:
                    wasAlreadyCompleted = _angerMazeCompleted;
                    _angerMazeCompleted = true;
                    break;
                case EmotionType.Sadness:
                    wasAlreadyCompleted = _sadnessMazeCompleted;
                    _sadnessMazeCompleted = true;
                    break;
                case EmotionType.Fear:
                    wasAlreadyCompleted = _fearMazeCompleted;
                    _fearMazeCompleted = true;
                    break;
                case EmotionType.Joy:
                    wasAlreadyCompleted = _joyMazeCompleted;
                    _joyMazeCompleted = true;
                    break;
            }

            if (!wasAlreadyCompleted)
            {
                Debug.Log($"[GameManager] {emotion} maze completed! Total: {CompletedMazeCount}/4");
                OnMazeCompleted?.Invoke(emotion);
            }

            // Transition to credits
            SetGameState(GameState.Credits);
        }

        /// <summary>
        /// Returns to the home state and resets emotion to calm.
        /// </summary>
        public void ReturnToHome()
        {
            SetEmotion(EmotionType.Calm);
            SetGameState(GameState.Home);
        }

        /// <summary>
        /// Resets all progress (useful for testing or replay).
        /// </summary>
        public void ResetProgress()
        {
            _angerMazeCompleted = false;
            _sadnessMazeCompleted = false;
            _fearMazeCompleted = false;
            _joyMazeCompleted = false;

            Debug.Log("[GameManager] Progress reset");
            ReturnToHome();
        }
        #endregion

        #region Utility
        /// <summary>
        /// Checks if a specific maze has been completed.
        /// </summary>
        /// <param name="emotion">The emotion maze to check</param>
        /// <returns>True if completed, false otherwise</returns>
        public bool IsMazeCompleted(EmotionType emotion)
        {
            return emotion switch
            {
                EmotionType.Anger => _angerMazeCompleted,
                EmotionType.Sadness => _sadnessMazeCompleted,
                EmotionType.Fear => _fearMazeCompleted,
                EmotionType.Joy => _joyMazeCompleted,
                _ => false
            };
        }
        #endregion
    }

    /// <summary>
    /// Enumeration of possible game states.
    /// </summary>
    public enum GameState
    {
        Home,
        MazeAnger,
        MazeSadness,
        MazeFear,
        MazeJoy,
        Credits
    }
}
