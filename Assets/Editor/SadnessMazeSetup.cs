using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using EmotionMaze.Mazes;
using EmotionMaze.Core;
using System.Collections.Generic;

namespace EmotionMaze.Editor
{
    /// <summary>
    /// Editor utility to set up the Sadness Maze scene with all required UI elements.
    /// Access via: Tools > Emotion Maze > Setup Sadness Maze
    /// </summary>
    public class SadnessMazeSetup : EditorWindow
    {
        [MenuItem("Tools/Emotion Maze/Setup Sadness Maze")]
        public static void SetupSadnessMaze()
        {
            if (!EditorUtility.DisplayDialog("Setup Sadness Maze",
                "This will add UI elements for the Sadness Maze. Make sure the Maze_Sadness scene is open. Continue?",
                "Yes", "Cancel"))
            {
                return;
            }

            // Find the existing canvas
            Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[SadnessMazeSetup] No canvas found in scene! Please make sure Maze_Sadness scene is open.");
                return;
            }

            // Create the maze controller
            GameObject controllerObj = new GameObject("SadnessMazeController");
            SadnessMazeController controller = controllerObj.AddComponent<SadnessMazeController>();

            // Create instructional text
            GameObject instructionObj = CreateText(canvas, "InstructionText",
                "Moro feels sad and grey. Let's find happy thoughts!",
                36, new Vector2(0, 300), new Vector2(900, 100));
            Text instructionText = instructionObj.GetComponent<Text>();

            // Create thought bubbles
            List<Button> thoughtBubbles = new List<Button>();
            Vector2[] bubblePositions = new Vector2[]
            {
                new Vector2(-300, 100),  // Top left
                new Vector2(300, 100),   // Top right
                new Vector2(-300, -150), // Bottom left
                new Vector2(300, -150)   // Bottom right
            };

            for (int i = 0; i < bubblePositions.Length; i++)
            {
                GameObject bubbleObj = CreateThoughtBubble(canvas, $"ThoughtBubble{i + 1}",
                    bubblePositions[i], new Vector2(150, 150));
                Button bubble = bubbleObj.GetComponent<Button>();
                thoughtBubbles.Add(bubble);
            }

            // Create counter text
            GameObject counterObj = CreateText(canvas, "CounterText",
                "0/4 happy thoughts",
                24, new Vector2(0, -300), new Vector2(300, 50));
            Text counterText = counterObj.GetComponent<Text>();

            // Create completion text (initially hidden)
            GameObject completionObj = CreateText(canvas, "CompletionText",
                "Remembering good things helps!",
                48, new Vector2(0, 200), new Vector2(900, 100));
            Text completionText = completionObj.GetComponent<Text>();
            completionText.color = ColorPalette.GetEmotionColor(EmotionType.Calm);
            completionObj.SetActive(false);

            // Create particle system for rain effect
            GameObject particlesObj = new GameObject("RainParticles");
            particlesObj.transform.SetParent(canvas.transform, false);
            ParticleSystem particles = particlesObj.AddComponent<ParticleSystem>();
            ConfigureRainParticles(particles);

            // Position rain at top of screen
            RectTransform particlesRect = particlesObj.AddComponent<RectTransform>();
            particlesRect.anchoredPosition = new Vector2(0, 400);

            // Wire up the controller
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("_instructionText").objectReferenceValue = instructionText;
            so.FindProperty("_completionText").objectReferenceValue = completionText;
            so.FindProperty("_counterText").objectReferenceValue = counterText;
            so.FindProperty("_rainParticles").objectReferenceValue = particles;

            // Set thought bubbles array
            SerializedProperty bubblesProperty = so.FindProperty("_thoughtBubbles");
            bubblesProperty.ClearArray();
            for (int i = 0; i < thoughtBubbles.Count; i++)
            {
                bubblesProperty.InsertArrayElementAtIndex(i);
                bubblesProperty.GetArrayElementAtIndex(i).objectReferenceValue = thoughtBubbles[i];
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

            Debug.Log("[SadnessMazeSetup] Sadness Maze setup complete!");
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

        private static GameObject CreateThoughtBubble(Canvas canvas, string name, Vector2 position, Vector2 size)
        {
            GameObject bubbleObj = new GameObject(name);
            bubbleObj.transform.SetParent(canvas.transform, false);

            RectTransform bubbleRect = bubbleObj.AddComponent<RectTransform>();
            bubbleRect.anchorMin = new Vector2(0.5f, 0.5f);
            bubbleRect.anchorMax = new Vector2(0.5f, 0.5f);
            bubbleRect.anchoredPosition = position;
            bubbleRect.sizeDelta = size;

            Image bubbleImage = bubbleObj.AddComponent<Image>();
            bubbleImage.color = new Color(0.96f, 0.96f, 0.96f); // Grey #F5F5F5

            Button button = bubbleObj.AddComponent<Button>();

            // Add a circular mask to make it look like a bubble
            Mask mask = bubbleObj.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            // Add text inside the bubble (optional - could be empty or have a simple icon)
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(bubbleObj.transform, false);

            Text text = textObj.AddComponent<Text>();
            text.text = "?";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 48;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.7f, 0.7f, 0.7f);

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            return bubbleObj;
        }

        private static void ConfigureRainParticles(ParticleSystem ps)
        {
            var main = ps.main;
            main.startLifetime = 2f;
            main.startSpeed = 5f;
            main.startSize = 0.1f;
            main.startColor = new Color(0.56f, 0.68f, 0.82f, 0.5f); // Blue-grey with transparency
            main.maxParticles = 100;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = 0.5f;

            var emission = ps.emission;
            emission.rateOverTime = 30;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Rectangle;
            shape.scale = new Vector3(20f, 0.1f, 1f); // Wide horizontal line

            // Make particles fall downward
            var velocity = ps.velocityOverLifetime;
            velocity.enabled = true;
            velocity.y = new ParticleSystem.MinMaxCurve(-3f);

            ps.Play();
        }
    }
}
