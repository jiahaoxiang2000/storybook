using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using EmotionMaze.Mazes;
using EmotionMaze.Core;

namespace EmotionMaze.Editor
{
    /// <summary>
    /// Editor utility to set up the Anger Maze scene with all required UI elements.
    /// Access via: Tools > Emotion Maze > Setup Anger Maze
    /// </summary>
    public class AngerMazeSetup : EditorWindow
    {
        [MenuItem("Tools/Emotion Maze/Setup Anger Maze")]
        public static void SetupAngerMaze()
        {
            if (!EditorUtility.DisplayDialog("Setup Anger Maze",
                "This will add UI elements for the Anger Maze. Make sure the Maze_Anger scene is open. Continue?",
                "Yes", "Cancel"))
            {
                return;
            }

            // Find the existing canvas
            Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[AngerMazeSetup] No canvas found in scene! Please make sure Maze_Anger scene is open.");
                return;
            }

            // Create the maze controller
            GameObject controllerObj = new GameObject("AngerMazeController");
            AngerMazeController controller = controllerObj.AddComponent<AngerMazeController>();

            // Create instructional text
            GameObject instructionObj = CreateText(canvas, "InstructionText",
                "Moro is very hot and angry. Help Moro cool down!",
                36, new Vector2(0, 300), new Vector2(800, 100));
            Text instructionText = instructionObj.GetComponent<Text>();

            // Create breathe button
            GameObject buttonObj = CreateButton(canvas, "BreatheButton",
                "Take Deep Breaths",
                new Vector2(0, 0), new Vector2(300, 100));
            Button breatheButton = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            // Style the button
            Image buttonImage = buttonObj.GetComponent<Image>();
            buttonImage.color = new Color(1f, 0.32f, 0.32f); // #FF5252

            // Create counter text
            GameObject counterObj = CreateText(canvas, "CounterText",
                "0/5 breaths",
                24, new Vector2(0, -100), new Vector2(200, 50));
            Text counterText = counterObj.GetComponent<Text>();

            // Create completion text (initially hidden)
            GameObject completionObj = CreateText(canvas, "CompletionText",
                "Moro feels calmer now!",
                48, new Vector2(0, 200), new Vector2(800, 100));
            Text completionText = completionObj.GetComponent<Text>();
            completionText.color = ColorPalette.GetEmotionColor(EmotionType.Calm);
            completionObj.SetActive(false);

            // Create particle system for cooling effect
            GameObject particlesObj = new GameObject("CoolingParticles");
            particlesObj.transform.SetParent(canvas.transform, false);
            ParticleSystem particles = particlesObj.AddComponent<ParticleSystem>();
            ConfigureCoolingParticles(particles);

            // Position particles at button location
            RectTransform particlesRect = particlesObj.AddComponent<RectTransform>();
            particlesRect.anchoredPosition = Vector2.zero;

            // Wire up the controller
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("_instructionText").objectReferenceValue = instructionText;
            so.FindProperty("_breatheButton").objectReferenceValue = breatheButton;
            so.FindProperty("_breatheButtonText").objectReferenceValue = buttonText;
            so.FindProperty("_completionText").objectReferenceValue = completionText;
            so.FindProperty("_counterText").objectReferenceValue = counterText;
            so.FindProperty("_coolingParticles").objectReferenceValue = particles;
            so.ApplyModifiedProperties();

            // Update the PlaceholderText to be empty or remove it
            Text placeholder = GameObject.Find("PlaceholderText")?.GetComponent<Text>();
            if (placeholder != null)
            {
                placeholder.text = "";
            }

            // Mark scene as dirty
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Debug.Log("[AngerMazeSetup] Anger Maze setup complete!");
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

        private static void ConfigureCoolingParticles(ParticleSystem ps)
        {
            var main = ps.main;
            main.startLifetime = 1f;
            main.startSpeed = 2f;
            main.startSize = 0.2f;
            main.startColor = new Color(0.5f, 0.8f, 0.96f); // Blue
            main.maxParticles = 20;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0, 10)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 1f;

            ps.Stop();
        }
    }
}
