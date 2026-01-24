//using UnityEngine;
//using System.Collections.Generic;
//using System;

//namespace LCDModule
//{
//    /// <summary>
//    /// Manages a two-line (20x2) LCD-style display using sprite-based characters.
//    /// Supports initialization, color/alpha adjustments, and text rendering.
//    /// </summary>
//    public class LCDModule : MonoBehaviour
//    {
//        [Header("Atlas Settings")]
//        [Tooltip("Texture atlas that contains all character sprites (0–9, A–Z, symbols, etc.)")]
//        public Texture2D atlasTexture;       // Texture containing all character glyphs

//        private int columns = 128;           // Number of characters (horizontal count)
//        private Sprite[] sprite;             // Array of sprites generated from the atlas
//        private int cellWidth = 5;           // Width of each cell (in pixels)
//        private int cellHeight = 8;          // Height of each cell (in pixels)

//        [Header("Display Settings")]
//        [Tooltip("List of SpriteRenderers for the first row (20 digits)")]
//        public List<SpriteRenderer> digitsA; // References for row 1 renderers

//        [Tooltip("List of SpriteRenderers for the second row (20 digits)")]
//        public List<SpriteRenderer> digitsB; // References for row 2 renderers

//        private List<Color> initialColorsA;  // Stores initial colors of row 1
//        private List<Color> initialColorsB;  // Stores initial colors of row 2

//        /// <summary>
//        /// Initializes the LCD by generating sprites and saving initial color states.
//        /// </summary>
//        void Start()
//        {
//            // Create sprite instances for each cell in the atlas
//            sprite = new Sprite[columns];
//            for (int i = 0; i < columns; i++)
//            {
//                int x = i % columns * cellWidth;
//                var rect = new Rect(x, 0, cellWidth, cellHeight);
//                sprite[i] = Sprite.Create(atlasTexture, rect, new Vector2(0.5f, 0.5f), 850f);
//            }

//            // Store initial color states for both digit rows
//            initialColorsA = new List<Color>();
//            initialColorsB = new List<Color>();

//            if (digitsA != null)
//            {
//                foreach (var renderer in digitsA)
//                {
//                    if (renderer != null)
//                        initialColorsA.Add(renderer.color);
//                }
//            }

//            if (digitsB != null)
//            {
//                foreach (var renderer in digitsB)
//                {
//                    if (renderer != null)
//                        initialColorsB.Add(renderer.color);
//                }
//            }
//        }

//        /// <summary>
//        /// Restores the original color state of all SpriteRenderers.
//        /// </summary>
//        public void RestoreInitialColors()
//        {
//            if (digitsA != null && initialColorsA != null)
//            {
//                for (int i = 0; i < Mathf.Min(digitsA.Count, initialColorsA.Count); i++)
//                {
//                    if (digitsA[i] != null)
//                        digitsA[i].color = initialColorsA[i];
//                }
//            }

//            if (digitsB != null && initialColorsB != null)
//            {
//                for (int i = 0; i < Mathf.Min(digitsB.Count, initialColorsB.Count); i++)
//                {
//                    if (digitsB[i] != null)
//                        digitsB[i].color = initialColorsB[i];
//                }
//            }
//        }

//        void Update()
//        {
//            // No update behavior (values are externally controlled)
//        }

//        /// <summary>
//        /// Retrieves a sprite from the atlas based on the given index.
//        /// </summary>
//        /// <param name="index">Character index within the atlas</param>
//        /// <returns>Corresponding sprite</returns>
//        private Sprite GetSprite(int index)
//        {
//            int spriteIndex = index % columns;
//            return sprite[spriteIndex];
//        }

//        /// <summary>
//        /// Displays a string on both LCD rows (up to 20 characters each).
//        /// </summary>
//        /// <param name="row1str">String for the first row</param>
//        /// <param name="row2str">String for the second row</param>
//        public void setSTR(string row1str, string row2str)
//        {
//            // ---- Row 1 ----
//            if (!String.IsNullOrEmpty(row1str))
//            {
//                for (int i = 0; i < digitsA.Count; i++)
//                {
//                    if (i < row1str.Length)
//                        digitsA[i].sprite = GetSprite(row1str[i]);
//                    else
//                        digitsA[i].sprite = GetSprite(' ');
//                }
//            }
//            else if ("".Equals(row1str))
//            {
//                foreach (var v in digitsA) { v.sprite = GetSprite(' '); }
//            }

//            // ---- Row 2 ----
//            if (!String.IsNullOrEmpty(row2str))
//            {
//                for (int i = 0; i < digitsB.Count; i++)
//                {
//                    if (i < row2str.Length)
//                        digitsB[i].sprite = GetSprite(row2str[i]);
//                    else
//                        digitsB[i].sprite = GetSprite(' ');
//                }
//            }
//            else if ("".Equals(row2str))
//            {
//                foreach (var v in digitsB) { v.sprite = GetSprite(' '); }
//            }
//        }

//        /// <summary>
//        /// Displays a string on LCD row1 (up to 20 characters).
//        /// </summary>
//        /// <param name="row1str">String for the first row</param>
//        public void setRow1(string row1str)
//        {
//            // ---- Row 1 ----
//            if (!String.IsNullOrEmpty(row1str))
//            {
//                for (int i = 0; i < digitsA.Count; i++)
//                {
//                    if (i < row1str.Length)
//                        digitsA[i].sprite = GetSprite(row1str[i]);
//                    else
//                        digitsA[i].sprite = GetSprite(' ');
//                }
//            }
//            else if ("".Equals(row1str))
//            {
//                foreach (var v in digitsA) { v.sprite = GetSprite(' '); }
//            }
//        }

//        /// <summary>
//        /// Displays a string on LCD row2 (up to 20 characters).
//        /// </summary>
//        /// <param name="row2str">String for the first row</param>
//        public void setRow2(string row2str)
//        {
//            // ---- Row 2 ----
//            if (!String.IsNullOrEmpty(row2str))
//            {
//                for (int i = 0; i < digitsB.Count; i++)
//                {
//                    if (i < row2str.Length)
//                        digitsB[i].sprite = GetSprite(row2str[i]);
//                    else
//                        digitsB[i].sprite = GetSprite(' ');
//                }
//            }
//            else if ("".Equals(row2str))
//            {
//                foreach (var v in digitsB) { v.sprite = GetSprite(' '); }
//            }
//        }

//        /// <summary>
//        /// Sets the color of all SpriteRenderers (keeps the original alpha value).
//        /// </summary>
//        /// <param name="color">New color (RGB only; alpha is preserved)</param>
//        public void SetColor(Color color)
//        {
//            // Apply to row A (preserve alpha)
//            if (digitsA != null)
//            {
//                foreach (var renderer in digitsA)
//                {
//                    if (renderer != null)
//                    {
//                        Color current = renderer.color;
//                        color.a = current.a;      // Preserve alpha
//                        renderer.color = color;   // Apply RGB
//                    }
//                }
//            }

//            // Apply to row B (preserve alpha)
//            if (digitsB != null)
//            {
//                foreach (var renderer in digitsB)
//                {
//                    if (renderer != null)
//                    {
//                        Color current = renderer.color;
//                        color.a = current.a;
//                        renderer.color = color;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Adjusts the alpha (transparency) of all digits (0–100%).
//        /// </summary>
//        /// <param name="alpha">Transparency value in percent (0–100)</param>
//        public void SetAlpha(float alpha)
//        {
//            float alphaPer = Mathf.Clamp01(alpha / 100f);

//            if (digitsA != null)
//            {
//                foreach (var renderer in digitsA)
//                {
//                    if (renderer != null)
//                    {
//                        Color c = renderer.color;
//                        c.a = alphaPer;
//                        renderer.color = c;
//                    }
//                }
//            }

//            if (digitsB != null)
//            {
//                foreach (var renderer in digitsB)
//                {
//                    if (renderer != null)
//                    {
//                        Color c = renderer.color;
//                        c.a = alphaPer;
//                        renderer.color = c;
//                    }
//                }
//            }
//        }
//    }
//}

using UnityEngine;
using System.Collections.Generic;
using System;

namespace LCDModule
{
    /// <summary>
    /// Manages a two-line (20x2) LCD-style display using sprite-based characters.
    /// Supports initialization, color/alpha adjustments, and text rendering.
    /// </summary>
    public class LCDModule : MonoBehaviour
    {
        [Header("Atlas Settings")]
        [Tooltip("Texture atlas that contains all character sprites (0–9, A–Z, symbols, etc.)")]
        public Texture2D atlasTexture;       // Texture containing all character glyphs

        private int columns = 128;           // Number of characters (horizontal count)
        private Sprite[] sprite;             // Array of sprites generated from the atlas
        private int cellWidth = 5;           // Width of each cell (in pixels)
        private int cellHeight = 8;          // Height of each cell (in pixels)

        [Header("Display Settings")]
        [Tooltip("List of SpriteRenderers for the first row (20 digits)")]
        public List<SpriteRenderer> digitsA; // References for row 1 renderers

        [Tooltip("List of SpriteRenderers for the second row (20 digits)")]
        public List<SpriteRenderer> digitsB; // References for row 2 renderers

        private List<Color> initialColorsA;  // Stores initial colors of row 1
        private List<Color> initialColorsB;  // Stores initial colors of row 2

        void Awake()
        {
            // Create sprite instances for each cell in the atlas
            sprite = new Sprite[columns];
            for (int i = 0; i < columns; i++)
            {
                int x = i % columns * cellWidth;
                var rect = new Rect(x, 0, cellWidth, cellHeight);
                sprite[i] = Sprite.Create(atlasTexture, rect, new Vector2(0.5f, 0.5f), 850f);
            }

            // Store initial color states for both digit rows
            initialColorsA = new List<Color>();
            initialColorsB = new List<Color>();

            if (digitsA != null)
            {
                foreach (var renderer in digitsA)
                    if (renderer != null) initialColorsA.Add(renderer.color);
            }

            if (digitsB != null)
            {
                foreach (var renderer in digitsB)
                    if (renderer != null) initialColorsB.Add(renderer.color);
            }
        }

        public void RestoreInitialColors()
        {
            if (digitsA != null && initialColorsA != null)
            {
                for (int i = 0; i < Mathf.Min(digitsA.Count, initialColorsA.Count); i++)
                    if (digitsA[i] != null) digitsA[i].color = initialColorsA[i];
            }

            if (digitsB != null && initialColorsB != null)
            {
                for (int i = 0; i < Mathf.Min(digitsB.Count, initialColorsB.Count); i++)
                    if (digitsB[i] != null) digitsB[i].color = initialColorsB[i];
            }
        }

        void Update() { /* No update behavior */ }

        private Sprite GetSprite(int index)
        {
            int spriteIndex = index % columns;
            return sprite[spriteIndex];
        }

        /// <summary>
        /// Displays a string on LCD row1 (up to 20 characters).
        /// Pass null to ignore changes, pass "" to clear.
        /// </summary>
        public void setRow1(string row1str)
        {
            if (!String.IsNullOrEmpty(row1str))
            {
                // Escreve o texto e preenche o resto com espaço vazio
                for (int i = 0; i < digitsA.Count; i++)
                {
                    if (i < row1str.Length)
                        digitsA[i].sprite = GetSprite(row1str[i]);
                    else
                        digitsA[i].sprite = GetSprite(' ');
                }
            }
            else if ("".Equals(row1str))
            {
                foreach (var v in digitsA) { v.sprite = GetSprite(' '); }
            }
        }

        /// <summary>
        /// Displays a string on LCD row2 (up to 20 characters).
        /// Pass null to ignore changes, pass "" to clear.
        /// </summary>
        public void setRow2(string row2str)
        {
            if (!String.IsNullOrEmpty(row2str))
            {
                for (int i = 0; i < digitsB.Count; i++)
                {
                    if (i < row2str.Length)
                        digitsB[i].sprite = GetSprite(row2str[i]);
                    else
                        digitsB[i].sprite = GetSprite(' ');
                }
            }
            else if ("".Equals(row2str))
            {
                foreach (var v in digitsB) { v.sprite = GetSprite(' '); }
            }
        }

        /// <summary>
        /// Displays a string on both LCD rows. 
        /// Agora utiliza os métodos individuais acima.
        /// </summary>
        public void setSTR(string row1str, string row2str)
        {
            setRow1(row1str);
            setRow2(row2str);
        }

        public void SetColor(Color color)
        {
            if (digitsA != null)
            {
                foreach (var renderer in digitsA)
                {
                    if (renderer != null)
                    {
                        Color current = renderer.color;
                        color.a = current.a;
                        renderer.color = color;
                    }
                }
            }
            if (digitsB != null)
            {
                foreach (var renderer in digitsB)
                {
                    if (renderer != null)
                    {
                        Color current = renderer.color;
                        color.a = current.a;
                        renderer.color = color;
                    }
                }
            }
        }

        public void SetAlpha(float alpha)
        {
            float alphaPer = Mathf.Clamp01(alpha / 100f);
            if (digitsA != null)
            {
                foreach (var renderer in digitsA)
                {
                    if (renderer != null)
                    {
                        Color c = renderer.color;
                        c.a = alphaPer;
                        renderer.color = c;
                    }
                }
            }
            if (digitsB != null)
            {
                foreach (var renderer in digitsB)
                {
                    if (renderer != null)
                    {
                        Color c = renderer.color;
                        c.a = alphaPer;
                        renderer.color = c;
                    }
                }
            }
        }
    }
}