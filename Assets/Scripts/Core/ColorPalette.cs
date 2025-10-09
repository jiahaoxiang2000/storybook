using UnityEngine;

namespace EmotionMaze.Core
{
    /// <summary>
    /// Centralized color palette for the Emotion Maze game.
    /// Contains all color definitions used throughout the game based on emotional states.
    /// </summary>
    public static class ColorPalette
    {
        // Base/Neutral Colors
        public static readonly Color SoftClay = HexToColor("E6D7C1");
        public static readonly Color Cream = HexToColor("FFF9EC");
        public static readonly Color DarkBrown = HexToColor("5D4037");

        // Moro's Emotional States
        public static class Moro
        {
            // Calm/Default State
            public static readonly Color CalmTeal = HexToColor("80CBC4");

            // Anger States
            public static readonly Color AngerBrightRed = HexToColor("FF5252");
            public static readonly Color AngerDeepRed = HexToColor("D32F2F");

            // Sadness States
            public static readonly Color SadnessDustyBlue = HexToColor("90A4AE");
            public static readonly Color SadnessBlueGray = HexToColor("546E7A");

            // Fear States
            public static readonly Color FearIndigo = HexToColor("3949AB");
            public static readonly Color FearDeepPurple = HexToColor("5E35B1");

            // Joy States
            public static readonly Color JoyYellow = HexToColor("FFEB3B");
            public static readonly Color JoyPink = HexToColor("EC407A");
        }

        // Maze-Specific Colors
        public static class Anger
        {
            public static readonly Color DarkRed = HexToColor("B71C1C");
            public static readonly Color CoolBlue = new Color(0.4f, 0.7f, 1f, 0.6f); // Semi-transparent blue for air currents
        }

        public static class Sadness
        {
            public static readonly Color GreyBlue = HexToColor("546E7A");
            public static readonly Color BrightGreen = HexToColor("66BB6A");
            public static readonly Color BrightYellow = HexToColor("FFCA28");
        }

        public static class Fear
        {
            public static readonly Color DeepIndigo = HexToColor("283593");
            public static readonly Color LanternYellow = HexToColor("FFEB3B");
            public static readonly Color LanternOrange = new Color(1f, 0.6f, 0.2f, 0.8f);
        }

        public static class Joy
        {
            // Rainbow colors for the chaotic spire
            public static readonly Color[] RainbowColors = new Color[]
            {
                HexToColor("FF0000"), // Red
                HexToColor("FF7F00"), // Orange
                HexToColor("FFFF00"), // Yellow
                HexToColor("00FF00"), // Green
                HexToColor("0000FF"), // Blue
                HexToColor("4B0082"), // Indigo
                HexToColor("9400D3")  // Violet
            };
        }

        // UI Element Colors
        public static class UI
        {
            public static readonly Color GreyWhite = HexToColor("F5F5F5");
            public static readonly Color LightGray = HexToColor("E0E0E0");
            public static readonly Color Inactive = HexToColor("E0E0E0");
        }

        /// <summary>
        /// Converts a hex color string to Unity Color.
        /// Supports both RGB (6 characters) and RGBA (8 characters) formats.
        /// </summary>
        /// <param name="hex">Hex color string without '#' prefix</param>
        /// <returns>Unity Color object</returns>
        private static Color HexToColor(string hex)
        {
            // Remove '#' if present
            hex = hex.TrimStart('#');

            // Parse RGB values
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            // Parse alpha if present (default to 255 if not)
            byte a = hex.Length >= 8
                ? byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
                : (byte)255;

            return new Color32(r, g, b, a);
        }

        /// <summary>
        /// Gets a gradient between two colors for smooth emotional transitions.
        /// </summary>
        /// <param name="startColor">Starting color</param>
        /// <param name="endColor">Ending color</param>
        /// <param name="t">Interpolation value (0-1)</param>
        /// <returns>Interpolated color</returns>
        public static Color GetGradient(Color startColor, Color endColor, float t)
        {
            return Color.Lerp(startColor, endColor, Mathf.Clamp01(t));
        }

        /// <summary>
        /// Gets the primary color for a given emotion type.
        /// </summary>
        /// <param name="emotion">Emotion type</param>
        /// <returns>Primary emotion color</returns>
        public static Color GetEmotionColor(EmotionType emotion)
        {
            return emotion switch
            {
                EmotionType.Calm => Moro.CalmTeal,
                EmotionType.Anger => Moro.AngerBrightRed,
                EmotionType.Sadness => Moro.SadnessDustyBlue,
                EmotionType.Fear => Moro.FearIndigo,
                EmotionType.Joy => Moro.JoyYellow,
                _ => Moro.CalmTeal
            };
        }
    }

    /// <summary>
    /// Enumeration of emotion types in the game.
    /// </summary>
    public enum EmotionType
    {
        Calm,
        Anger,
        Sadness,
        Fear,
        Joy
    }
}
