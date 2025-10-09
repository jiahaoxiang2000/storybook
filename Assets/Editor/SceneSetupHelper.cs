using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using EmotionMaze.Core;
using EmotionMaze.UI;
using EmotionMaze.Character;

namespace EmotionMaze.Editor
{
    /// <summary>
    /// Editor utility to help set up basic scene structure quickly.
    /// Access via: Tools > Emotion Maze > Setup Scene
    /// </summary>
    public class SceneSetupHelper : EditorWindow
    {
        [MenuItem("Tools/Emotion Maze/Setup Home Scene")]
        public static void SetupHomeScene()
        {
            if (!EditorUtility.DisplayDialog("Setup Home Scene",
                "This will create basic GameObjects for the Home scene. Continue?",
                "Yes", "Cancel"))
            {
                return;
            }

            // Create Game Managers
            CreateGameManagers();

            // Create UI Canvas
            Canvas canvas = CreateUICanvas();

            // Create Background
            CreateBackground(canvas, ColorPalette.Cream);

            // Create Moro
            CreateMoro();

            // Create Progress Indicator
            CreateProgressIndicator(canvas);

            // Create Home Button
            CreateHomeButton(canvas);

            // Create Emotion Portals
            CreateEmotionPortals(canvas);

            // Mark scene as dirty and save
            MarkSceneDirty();
            SaveCurrentScene();

            Debug.Log("[SceneSetupHelper] Home scene setup complete!");
        }

        [MenuItem("Tools/Emotion Maze/Setup Maze Scene")]
        public static void SetupMazeScene()
        {
            if (!EditorUtility.DisplayDialog("Setup Maze Scene",
                "This will create basic UI elements for a maze scene. Continue?",
                "Yes", "Cancel"))
            {
                return;
            }

            // Create UI Canvas
            Canvas canvas = CreateUICanvas();

            // Create Background (default color, user will change)
            CreateBackground(canvas, new Color(0.5f, 0.5f, 0.5f));

            // Create Progress Indicator
            CreateProgressIndicator(canvas);

            // Create Home Button
            CreateHomeButton(canvas);

            // Add placeholder text
            CreatePlaceholderText(canvas);

            // Mark scene as dirty and save
            MarkSceneDirty();
            SaveCurrentScene();

            Debug.Log("[SceneSetupHelper] Maze scene setup complete!");
        }

        [MenuItem("Tools/Emotion Maze/Setup Credits Scene")]
        public static void SetupCreditsScene()
        {
            if (!EditorUtility.DisplayDialog("Setup Credits Scene",
                "This will create basic elements for the Credits scene. Continue?",
                "Yes", "Cancel"))
            {
                return;
            }

            // Create UI Canvas
            Canvas canvas = CreateUICanvas();

            // Create Background
            CreateBackground(canvas, ColorPalette.Cream);

            // Create Moro
            GameObject moro = CreateMoro();
            moro.transform.position = new Vector3(0, 1, 0);

            // Create title text
            CreateCreditsText(canvas);

            // Create Home Button
            CreateHomeButton(canvas);

            // Mark scene as dirty and save
            MarkSceneDirty();
            SaveCurrentScene();

            Debug.Log("[SceneSetupHelper] Credits scene setup complete!");
        }

        [MenuItem("Tools/Emotion Maze/Add All Scenes to Build Settings")]
        public static void AddScenesToBuildSettings()
        {
            string[] scenePaths = new string[]
            {
                "Assets/Scenes/Home.unity",
                "Assets/Scenes/Maze_Anger.unity",
                "Assets/Scenes/Maze_Sadness.unity",
                "Assets/Scenes/Maze_Fear.unity",
                "Assets/Scenes/Maze_Joy.unity",
                "Assets/Scenes/Credits.unity"
            };

            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[scenePaths.Length];
            for (int i = 0; i < scenePaths.Length; i++)
            {
                buildScenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
            }

            EditorBuildSettings.scenes = buildScenes;

            Debug.Log("[SceneSetupHelper] Added 6 scenes to Build Settings");
        }

        // Helper Methods

        private static void CreateGameManagers()
        {
            GameObject managers = new GameObject("_GameManagers");

            GameObject gmObj = new GameObject("GameManager");
            gmObj.transform.SetParent(managers.transform);
            gmObj.AddComponent<GameManager>();

            GameObject nmObj = new GameObject("NavigationManager");
            nmObj.transform.SetParent(managers.transform);
            nmObj.AddComponent<NavigationManager>();

            Debug.Log("Created Game Managers");
        }

        private static Canvas CreateUICanvas()
        {
            // Check if canvas already exists
            Canvas existingCanvas = GameObject.FindFirstObjectByType<Canvas>();
            if (existingCanvas != null)
            {
                Debug.Log("Canvas already exists, using existing one");
                return existingCanvas;
            }

            // Create new canvas
            GameObject canvasObj = new GameObject("UI Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // Create EventSystem if needed
            if (GameObject.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                Debug.Log("Created EventSystem");
            }

            Debug.Log("Created UI Canvas");
            return canvas;
        }

        private static void CreateBackground(Canvas canvas, Color color)
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvas.transform, false);

            Image image = bgObj.AddComponent<Image>();
            image.color = color;

            RectTransform rect = bgObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            bgObj.transform.SetAsFirstSibling();

            Debug.Log("Created Background");
        }

        private static GameObject CreateMoro()
        {
            GameObject moro = new GameObject("Moro");
            SpriteRenderer sr = moro.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            sr.color = ColorPalette.Moro.CalmTeal;

            moro.transform.position = Vector3.zero;
            moro.transform.localScale = new Vector3(2, 2, 1);

            moro.AddComponent<MoroController>();

            Debug.Log("Created Moro");
            return moro;
        }

        private static void CreateProgressIndicator(Canvas canvas)
        {
            GameObject piObj = new GameObject("ProgressIndicator");
            piObj.transform.SetParent(canvas.transform, false);

            RectTransform piRect = piObj.AddComponent<RectTransform>();
            piRect.anchorMin = new Vector2(0.5f, 0);
            piRect.anchorMax = new Vector2(0.5f, 0);
            piRect.pivot = new Vector2(0.5f, 0);
            piRect.anchoredPosition = new Vector2(0, 30);
            piRect.sizeDelta = new Vector2(400, 30);

            ProgressIndicator pi = piObj.AddComponent<ProgressIndicator>();

            // Create 4 dots
            float[] xPositions = { -90, -30, 30, 90 };
            string[] dotNames = { "AngerDot", "SadnessDot", "FearDot", "JoyDot" };
            string[] fieldNames = { "_angerDot", "_sadnessDot", "_fearDot", "_joyDot" };

            SerializedObject so = new SerializedObject(pi);

            for (int i = 0; i < 4; i++)
            {
                GameObject dotObj = new GameObject(dotNames[i]);
                dotObj.transform.SetParent(piObj.transform, false);

                Image dotImage = dotObj.AddComponent<Image>();
                dotImage.color = ColorPalette.UI.Inactive;
                dotImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

                RectTransform dotRect = dotObj.GetComponent<RectTransform>();
                dotRect.anchorMin = new Vector2(0.5f, 0.5f);
                dotRect.anchorMax = new Vector2(0.5f, 0.5f);
                dotRect.anchoredPosition = new Vector2(xPositions[i], 0);
                dotRect.sizeDelta = new Vector2(20, 20);

                // Assign to ProgressIndicator using SerializedObject
                SerializedProperty dotProperty = so.FindProperty(fieldNames[i]);
                if (dotProperty != null)
                {
                    dotProperty.objectReferenceValue = dotImage;
                }
                else
                {
                    Debug.LogWarning($"[SceneSetupHelper] Could not find property: {fieldNames[i]}");
                }
            }

            so.ApplyModifiedProperties();

            Debug.Log("Created Progress Indicator");
        }

        private static void CreateHomeButton(Canvas canvas)
        {
            GameObject btnObj = new GameObject("HomeButton");
            btnObj.transform.SetParent(canvas.transform, false);

            RectTransform btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0, 1);
            btnRect.anchorMax = new Vector2(0, 1);
            btnRect.pivot = new Vector2(0, 1);
            btnRect.anchoredPosition = new Vector2(20, -20);
            btnRect.sizeDelta = new Vector2(80, 60);

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = ColorPalette.Cream;

            Button button = btnObj.AddComponent<Button>();
            btnObj.AddComponent<HomeButtonUI>();

            // Add text child
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);

            Text text = textObj.AddComponent<Text>();
            text.text = "🏠";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 32;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            Debug.Log("Created Home Button");
        }

        private static void CreateEmotionPortals(Canvas canvas)
        {
            EmotionType[] emotions = { EmotionType.Anger, EmotionType.Sadness, EmotionType.Fear, EmotionType.Joy };
            Vector2[] positions = {
                new Vector2(-300, 100),
                new Vector2(300, 100),
                new Vector2(-300, -100),
                new Vector2(300, -100)
            };

            for (int i = 0; i < 4; i++)
            {
                GameObject portalObj = new GameObject($"{emotions[i]}Portal");
                portalObj.transform.SetParent(canvas.transform, false);

                RectTransform portalRect = portalObj.AddComponent<RectTransform>();
                portalRect.anchorMin = new Vector2(0.5f, 0.5f);
                portalRect.anchorMax = new Vector2(0.5f, 0.5f);
                portalRect.anchoredPosition = positions[i];
                portalRect.sizeDelta = new Vector2(150, 150);

                Image portalImage = portalObj.AddComponent<Image>();
                portalImage.color = ColorPalette.GetEmotionColor(emotions[i]);
                portalImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

                Button button = portalObj.AddComponent<Button>();
                EmotionPortalUI portalUI = portalObj.AddComponent<EmotionPortalUI>();

                // Set emotion type via SerializedObject
                SerializedObject so = new SerializedObject(portalUI);
                so.FindProperty("_emotionType").enumValueIndex = (int)emotions[i];
                so.FindProperty("_portalImage").objectReferenceValue = portalImage;
                so.ApplyModifiedProperties();
            }

            Debug.Log("Created 4 Emotion Portals");
        }

        private static void CreatePlaceholderText(Canvas canvas)
        {
            GameObject textObj = new GameObject("PlaceholderText");
            textObj.transform.SetParent(canvas.transform, false);

            Text text = textObj.AddComponent<Text>();
            text.text = "Maze Scene - Add maze mechanics here";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 48;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = new Vector2(800, 100);
        }

        private static void CreateCreditsText(Canvas canvas)
        {
            GameObject textObj = new GameObject("TitleText");
            textObj.transform.SetParent(canvas.transform, false);

            Text text = textObj.AddComponent<Text>();
            text.text = "You're an Emotion Hero!\nThank you!";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 72;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = ColorPalette.DarkBrown;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.7f);
            textRect.anchorMax = new Vector2(0.5f, 0.7f);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = new Vector2(1000, 200);
        }

        // Scene Save Utilities

        private static void MarkSceneDirty()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(currentScene);
        }

        private static void SaveCurrentScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            EditorSceneManager.SaveScene(currentScene);
            Debug.Log($"[SceneSetupHelper] Saved scene: {currentScene.name}");
        }
    }
}
