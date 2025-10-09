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
    /// Editor utility to set up the Fear Maze scene with all required UI elements.
    /// Access via: Tools > Emotion Maze > Setup Fear Maze
    /// </summary>
    public class FearMazeSetup : EditorWindow
    {
        [MenuItem("Tools/Emotion Maze/Setup Fear Maze")]
        public static void SetupFearMaze()
        {
            if (!EditorUtility.DisplayDialog("Setup Fear Maze",
                "This will add UI elements for the Fear Maze. Make sure the Maze_Fear scene is open. Continue?",
                "Yes", "Cancel"))
            {
                return;
            }

            // Find the existing canvas
            Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[FearMazeSetup] No canvas found in scene! Please make sure Maze_Fear scene is open.");
                return;
            }

            // Create the maze controller
            GameObject controllerObj = new GameObject("FearMazeController");
            FearMazeController controller = controllerObj.AddComponent<FearMazeController>();

            // Create instructional text
            GameObject instructionObj = CreateText(canvas, "InstructionText",
                "It's dark and scary. Let's turn on the lights!",
                36, new Vector2(0, 300), new Vector2(900, 100));
            Text instructionText = instructionObj.GetComponent<Text>();

            // Create "Light" button
            GameObject lightButtonObj = CreateButton(canvas, "LightButton",
                "💡 Turn On Light", new Vector2(0, -250), new Vector2(250, 80));
            Button lightButton = lightButtonObj.GetComponent<Button>();
            Text lightButtonText = lightButtonObj.GetComponentInChildren<Text>();
            lightButtonText.fontSize = 28;

            // Set button color to deep indigo
            Image lightButtonImage = lightButtonObj.GetComponent<Image>();
            lightButtonImage.color = new Color(0.16f, 0.2f, 0.58f); // #283593

            // Create shadow shapes and friendly shapes
            List<GameObject> shadowShapes = new List<GameObject>();
            List<GameObject> friendlyShapes = new List<GameObject>();

            Vector2[] shapePositions = new Vector2[]
            {
                new Vector2(-250, 50),   // Left
                new Vector2(0, 100),     // Center-top
                new Vector2(250, 50),    // Right
                new Vector2(-100, -80)   // Bottom-left (optional 4th)
            };

            // Shadow types for variety
            string[] shadowTypes = new string[] { "blob", "blob", "blob", "blob" };

            for (int i = 0; i < shapePositions.Length; i++)
            {
                // Create shadow shape
                GameObject shadowObj = CreateShadowShape(canvas, $"Shadow{i + 1}",
                    shapePositions[i], new Vector2(120, 120));
                shadowShapes.Add(shadowObj);

                // Create corresponding friendly shape
                GameObject friendlyObj = CreateFriendlyShape(canvas, $"FriendlyShape{i + 1}",
                    shapePositions[i], new Vector2(120, 120), i);
                friendlyShapes.Add(friendlyObj);
            }

            // Create counter text
            GameObject counterObj = CreateText(canvas, "CounterText",
                "0/4 lights on",
                24, new Vector2(0, -350), new Vector2(300, 50));
            Text counterText = counterObj.GetComponent<Text>();

            // Create completion text (initially hidden)
            GameObject completionObj = CreateText(canvas, "CompletionText",
                "Not so scary after all!",
                48, new Vector2(0, 200), new Vector2(900, 100));
            Text completionText = completionObj.GetComponent<Text>();
            completionText.color = ColorPalette.GetEmotionColor(EmotionType.Calm);
            completionObj.SetActive(false);

            // Create light overlay (full screen dark overlay that fades as lights turn on)
            GameObject lightOverlayObj = new GameObject("LightOverlay");
            lightOverlayObj.transform.SetParent(canvas.transform, false);
            lightOverlayObj.transform.SetAsFirstSibling(); // Place behind other UI elements

            RectTransform overlayRect = lightOverlayObj.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.sizeDelta = Vector2.zero;
            overlayRect.anchoredPosition = Vector2.zero;

            Image lightOverlay = lightOverlayObj.AddComponent<Image>();
            lightOverlay.color = new Color(0f, 0f, 0f, 0.7f); // Dark overlay
            lightOverlay.raycastTarget = false;

            // Create particle system for glow effect
            GameObject glowParticlesObj = new GameObject("GlowParticles");
            glowParticlesObj.transform.SetParent(canvas.transform, false);
            ParticleSystem glowParticles = glowParticlesObj.AddComponent<ParticleSystem>();
            ConfigureGlowParticles(glowParticles);

            // Create particle system for light beam effect
            GameObject lightBeamParticlesObj = new GameObject("LightBeamParticles");
            lightBeamParticlesObj.transform.SetParent(canvas.transform, false);
            ParticleSystem lightBeamParticles = lightBeamParticlesObj.AddComponent<ParticleSystem>();
            ConfigureLightBeamParticles(lightBeamParticles);

            // Wire up the controller
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("_instructionText").objectReferenceValue = instructionText;
            so.FindProperty("_lightButton").objectReferenceValue = lightButton;
            so.FindProperty("_lightButtonText").objectReferenceValue = lightButtonText;
            so.FindProperty("_completionText").objectReferenceValue = completionText;
            so.FindProperty("_counterText").objectReferenceValue = counterText;
            so.FindProperty("_glowParticles").objectReferenceValue = glowParticles;
            so.FindProperty("_lightBeamParticles").objectReferenceValue = lightBeamParticles;
            so.FindProperty("_lightOverlay").objectReferenceValue = lightOverlay;

            // Set shadow shapes array
            SerializedProperty shadowsProperty = so.FindProperty("_shadowShapes");
            shadowsProperty.ClearArray();
            for (int i = 0; i < shadowShapes.Count; i++)
            {
                shadowsProperty.InsertArrayElementAtIndex(i);
                shadowsProperty.GetArrayElementAtIndex(i).objectReferenceValue = shadowShapes[i];
            }

            // Set friendly shapes array
            SerializedProperty friendlyProperty = so.FindProperty("_friendlyShapes");
            friendlyProperty.ClearArray();
            for (int i = 0; i < friendlyShapes.Count; i++)
            {
                friendlyProperty.InsertArrayElementAtIndex(i);
                friendlyProperty.GetArrayElementAtIndex(i).objectReferenceValue = friendlyShapes[i];
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

            Debug.Log("[FearMazeSetup] Fear Maze setup complete!");
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
            textComponent.color = ColorPalette.Cream; // Light color for dark background

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            return textObj;
        }

        private static GameObject CreateButton(Canvas canvas, string name, string buttonText, Vector2 position, Vector2 size)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(canvas.transform, false);

            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.anchoredPosition = position;
            buttonRect.sizeDelta = size;

            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = Color.white;

            Button button = buttonObj.AddComponent<Button>();

            // Add text to button
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            Text text = textObj.AddComponent<Text>();
            text.text = buttonText;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 32;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            return buttonObj;
        }

        private static GameObject CreateShadowShape(Canvas canvas, string name, Vector2 position, Vector2 size)
        {
            GameObject shapeObj = new GameObject(name);
            shapeObj.transform.SetParent(canvas.transform, false);

            RectTransform shapeRect = shapeObj.AddComponent<RectTransform>();
            shapeRect.anchorMin = new Vector2(0.5f, 0.5f);
            shapeRect.anchorMax = new Vector2(0.5f, 0.5f);
            shapeRect.anchoredPosition = position;
            shapeRect.sizeDelta = size;

            Image shapeImage = shapeObj.AddComponent<Image>();
            shapeImage.color = new Color(0.05f, 0.05f, 0.1f, 0.9f); // Very dark, almost black

            return shapeObj;
        }

        private static GameObject CreateFriendlyShape(Canvas canvas, string name, Vector2 position, Vector2 size, int shapeType)
        {
            GameObject shapeObj = new GameObject(name);
            shapeObj.transform.SetParent(canvas.transform, false);

            RectTransform shapeRect = shapeObj.AddComponent<RectTransform>();
            shapeRect.anchorMin = new Vector2(0.5f, 0.5f);
            shapeRect.anchorMax = new Vector2(0.5f, 0.5f);
            shapeRect.anchoredPosition = position;
            shapeRect.sizeDelta = size;

            Image shapeImage = shapeObj.AddComponent<Image>();

            // Different colors for different friendly shapes
            Color[] friendlyColors = new Color[]
            {
                new Color(1f, 0.92f, 0.23f),      // #FFEB3B Yellow (star)
                new Color(1f, 0.41f, 0.71f),      // #FF69B4 Pink (heart)
                new Color(0.4f, 0.73f, 0.42f),    // #66BB6A Green (smiley)
                new Color(0.5f, 0.8f, 0.77f)      // #80CBC4 Teal (4th shape)
            };

            shapeImage.color = friendlyColors[shapeType % friendlyColors.Length];

            // Add text to represent the shape (since we're using simple UI)
            GameObject textObj = new GameObject("ShapeIcon");
            textObj.transform.SetParent(shapeObj.transform, false);

            Text text = textObj.AddComponent<Text>();
            string[] shapeIcons = new string[] { "★", "♥", "☺", "✿" };
            text.text = shapeIcons[shapeType % shapeIcons.Length];
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 72;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            // Initially hidden
            shapeObj.SetActive(false);

            return shapeObj;
        }

        private static void ConfigureGlowParticles(ParticleSystem ps)
        {
            var main = ps.main;
            main.startLifetime = 1f;
            main.startSpeed = 2f;
            main.startSize = 0.3f;
            main.startColor = new Color(1f, 0.92f, 0.23f, 0.8f); // Bright yellow glow
            main.maxParticles = 50;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 20, 30, 1, 0.1f)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.5f;

            // Make particles spread outward
            var velocity = ps.velocityOverLifetime;
            velocity.enabled = true;
            velocity.radial = new ParticleSystem.MinMaxCurve(2f);

            // Fade out over lifetime
            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

            ps.Stop();
        }

        private static void ConfigureLightBeamParticles(ParticleSystem ps)
        {
            var main = ps.main;
            main.startLifetime = 0.5f;
            main.startSpeed = 5f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
            main.startColor = new Color(1f, 0.95f, 0.7f, 0.6f); // Soft yellow-white light beam
            main.maxParticles = 100;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 15, 25, 1, 0.1f)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 15f; // Narrow beam
            shape.radius = 0.1f;
            shape.length = 3f;

            // Make particles move along the beam direction
            var velocity = ps.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.Local;

            // Fade out over lifetime
            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(new Color(1f, 0.92f, 0.23f), 1f) // Yellow at end
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.8f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

            // Size over lifetime (beam gets wider)
            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 0.5f);
            sizeCurve.AddKey(1f, 1.5f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            ps.Stop();
        }
    }
}
