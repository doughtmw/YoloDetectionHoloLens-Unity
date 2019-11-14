using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DrawingUtils namespace from personal git repo.
/// https://github.com/doughtmw/BoundingBoxUtils-Unity
/// </summary>
namespace DrawingUtils
{
    public abstract class Texture2DExtension
    {
        // Draw a box given two vec2d points.
        // Top left corner, bottom right corner
        public static Texture2D Box(
            Texture2D tex,
            Vector2 tl,
            Vector2 br,
            Color color)
        {
            // Draw line connecting top left and top right
            var tr = new Vector2(br.x, tl.y);
            tex = Line(tex, tl, tr, color);

            // Draw line connecting top left and bottom left
            var bl = new Vector2(tl.x, br.y);
            tex = Line(tex, tl, bl, color);

            // Draw line connecting bottom left and bottom right
            tex = Line(tex, bl, br, color);

            // Draw line connecting bottom right and top right
            tex = Line(tex, br, tr, color);

            return tex;
        }

        // Draw line between two vec2d points.
        public static Texture2D Line(
            Texture2D tex,
            Vector2 p1,
            Vector2 p2,
            Color color)
        {
            Vector2 t = p1;
            float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
            float ctr = 0;

            while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
            {
                t = Vector2.Lerp(p1, p2, ctr);
                ctr += frac;
                tex.SetPixel((int)t.x - 1, (int)t.y - 1, color);
                tex.SetPixel((int)t.x, (int)t.y, color);
                tex.SetPixel((int)t.x + 1, (int)t.y + 1, color);
            }
            return tex;
        }

        // Set texture as transparent to initialize canvas for drawing.
        public static Texture2D TransparentTexture(
            Texture2D tex)
        {
            Color fillColor = Color.clear;
            Color[] fillPixels = new Color[tex.width * tex.height];

            for (int i = 0; i < fillPixels.Length; i++)
            {
                fillPixels[i] = fillColor;
            }

            tex.SetPixels(fillPixels);
            return tex;
        }
    }

}