using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using EmotionMaze.Mazes;
using EmotionMaze.Core;

namespace EmotionMaze.Editor
{
    /// <summary>
    /// Editor utility to set up the Joy Maze scene with all required UI elements.
    /// Access via: Tools > Emotion Maze > Setup Joy Maze
    /// </summary>
    public class JoyMazeSetup : EditorWindow
    {
        [MenuItem("Tools/Emotion Maze/Setup Joy Maze")]
        public static void SetupJoyMaze()
        {
            if (!EditorUtility.DisplayDialog("Setup Joy Maze",
                "This will add UI elements for the Joy Maze. Make sure the Maze_Joy scene is open. Continue?",
                "Yes", "Cancel"))
            {
                return;
            }

            // Find the existing canvas
            Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[JoyMazeSetup] No canvas found in scene! Please make sure Maze_Joy scene is open.");
                return;
            }

            // Create the maze controller
            GameObject controllerObj = new GameObject("JoyMazeController");
            JoyMazeController controller = controllerObj.AddComponent<JoyMazeController>();

            // Create instructional text
            GameObject instructionObj = CreateText(canvas, "InstructionText",
                "Too much excitement! Let's find calm.",
                36, new Vector2(0, 300), new Vector2(800, 100));
            Text instructionText = instructionObj.GetComponent<Text>();

            // Create calm down button
            GameObject buttonObj = CreateButton(canvas, "CalmButton",
                "Calm Down",
                new Vector2(0, -200), new Vector2(250, 100));
            Button calmButton = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            // Style the button
            Image buttonImage = buttonObj.GetComponent<Image>();
            buttonImage.color = new Color(1f, 0.92f, 0.23f); // #FFEB3B Yellow

            // Create counter text
            GameObject counterObj = CreateText(canvas, "CounterText",
                "0/6 calm taps",
                24, new Vector2(0, -300), new Vector2(200, 50));
            Text counterText = counterObj.GetComponent<Text>();

            // Create completion text (initially hidden)
            GameObject completionObj = CreateText(canvas, "CompletionText",
                "Finding calm in the fun!",
                48, new Vector2(0, 200), new Vector2(800, 100));
            Text completionText = completionObj.GetComponent<Text>();
            completionText.color = ColorPalette.GetEmotionColor(EmotionType.Calm);
            completionObj.SetActive(false);

            // Create 6 bouncing orbs
            System.Collections.Generic.List<GameObject> orbs = new System.Collections.Generic.List<GameObject>();
            Vector2[] orbPositions = new Vector2[]
            {
                new Vector2(-300, 100),
                new Vector2(300, 100),
                new Vector2(-200, -50),
                new Vector2(200, -50),
                new Vector2(-100, 150),
                new Vector2(100, 150)
            };

            for (int i = 0; i < 6; i++)
            {
                GameObject orbObj = CreateOrb(canvas, $"Orb{i + 1}", orbPositions[i]);
                orbs.Add(orbObj);
            }

            // Create swirl particle system
            GameObject swirlObj = new GameObject("SwirlParticles");
            swirlObj.transform.SetParent(canvas.transform, false);
            ParticleSystem swirlParticles = swirlObj.AddComponent<ParticleSystem>();
            ConfigureSwirlParticles(swirlParticles);

            RectTransform swirlRect = swirlObj.AddComponent<RectTransform>();
            swirlRect.anchoredPosition = Vector2.zero;

            // Create calming particle system
            GameObject calmingObj = new GameObject("CalmingParticles");
            calmingObj.transform.SetParent(canvas.transform, false);
            ParticleSystem calmingParticles = calmingObj.AddComponent<ParticleSystem>();
            ConfigureCalmingParticles(calmingParticles);

            RectTransform calmingRect = calmingObj.AddComponent<RectTransform>();
            calmingRect.anchoredPosition = buttonObj.GetComponent<RectTransform>().anchoredPosition;

            // Wire up the controller
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("_instructionText").objectReferenceValue = instructionText;
            so.FindProperty("_calmButton").objectReferenceValue = calmButton;
            so.FindProperty("_calmButtonText").objectReferenceValue = buttonText;
            so.FindProperty("_completionText").objectReferenceValue = completionText;
            so.FindProperty("_counterText").objectReferenceValue = counterText;
            so.FindProperty("_swirlParticles").objectReferenceValue = swirlParticles;
            so.FindProperty("_calmingParticles").objectReferenceValue = calmingParticles;

            // Add orbs to the list
            SerializedProperty orbsProp = so.FindProperty("_orbs");
            orbsProp.ClearArray();
            for (int i = 0; i < orbs.Count; i++)
            {
                orbsProp.InsertArrayElementAtIndex(i);
                orbsProp.GetArrayElementAtIndex(i).objectReferenceValue = orbs[i];
            }

            so.ApplyModifiedProperties();

            // Update the PlaceholderText to be empty or remove it
            Text placeholder = GameObject.Find("PlaceholderText")?.GetComponent<Text>();
            if (placeholder != null)
            {
                placeholder.text = "";
            }

            // Mark scene as dirty
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Debug.Log("[JoyMazeSetup] Joy Maze setup complete!");
        }

        private static GameObject CreateText(Canvas canvas, string name, string text, int fontSize, Vector2 position, Vector2 size)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(canvas.transform, false);

            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComponent.fontSize = fontSize;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.color = ColorPalette.DarkBrown;

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            return textObj;
        }

        private static GameObject CreateButton(Canvas canvas, string name, string buttonText, Vector2 position, Vector2 size)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(canvas.transform, false);

            RectTransform btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0.5f);
            btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.anchoredPosition = position;
            btnRect.sizeDelta = size;

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = Color.white;

            Button button = btnObj.AddComponent<Button>();

            // Add text child
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);

            Text text = textObj.AddComponent<Text>();
            text.text = buttonText;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 28;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            return btnObj;
        }

        private static GameObject CreateOrb(Canvas canvas, string name, Vector2 position)
        {
            GameObject orbObj = new GameObject(name);
            orbObj.transform.SetParent(canvas.transform, false);

            RectTransform orbRect = orbObj.AddComponent<RectTransform>();
            orbRect.anchorMin = new Vector2(0.5f, 0.5f);
            orbRect.anchorMax = new Vector2(0.5f, 0.5f);
            orbRect.anchoredPosition = position;
            orbRect.sizeDelta = new Vector2(80, 80);

            Image orbImage = orbObj.AddComponent<Image>();
            orbImage.color = Color.white;

            // Make orb circular
            orbImage.sprite = CreateCircleSprite();

            return orbObj;
        }

        private static Sprite CreateCircleSprite()
        {
            // Create a simple white circle sprite
            Texture2D texture = new Texture2D(128, 128);
            Color[] pixels = new Color[128 * 128];
            Vector2 center = new Vector2(64, 64);
            float radius = 60f;

            for (int y = 0; y < 128; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    pixels[y * 128 + x] = distance <= radius ? Color.white : Color.clear;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
        }

        private static void ConfigureSwirlParticles(ParticleSystem ps)
        {
            var main = ps.main;
            main.startLifetime = 3f;
            main.startSpeed = 50f;
            main.startSize = 0.3f;
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(1f, 0.92f, 0.23f),  // #FFEB3B Yellow
                new Color(0.93f, 0.25f, 0.48f) // #EC407A Pink
            );
            main.maxParticles = 50;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 10;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 5f;

            var velocityOverLifetime = ps.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.radial = 2f;

            ps.Play();
        }

        private static void ConfigureCalmingParticles(ParticleSystem ps)
        {
            var main = ps.main;
            main.startLifetime = 1f;
            main.startSpeed = 3f;
            main.startSize = 0.2f;
            main.startColor = new Color(0.5f, 0.8f, 0.77f); // #80CBC4 Teal
            main.maxParticles = 15;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0, 8)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 1f;

            ps.Stop();
        }
    }
}
