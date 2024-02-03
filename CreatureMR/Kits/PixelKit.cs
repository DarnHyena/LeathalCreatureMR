using System.Linq;
using UnityEngine;

namespace CackleCrew.ThisIsMagical
{
    //PixelStuff
    public static partial class PixelKit
    {
        public static void HSV(ref Color original, float H, float S, float V)
        {
            original = Color.HSVToRGB(H, S, V);
        }
        public static void HV(ref Color original, float H, float V)
        {
            Color.RGBToHSV(original, out _, out var s, out _);
            original = Color.HSVToRGB(H, s, V);
        }
        public static void HS(ref Color original, float H, float S)
        {
            Color.RGBToHSV(original, out _, out _, out var v);
            original = Color.HSVToRGB(H, S, v);
        }
        public static void SV(ref Color original, float S, float V)
        {
            Color.RGBToHSV(original, out var h, out _, out _);
            original = Color.HSVToRGB(h, S, V);
        }
        public static void Hue(ref Color original, float amount)
        {
            Color.RGBToHSV(original, out _, out var s, out var v);
            original = Color.HSVToRGB(amount, s, v);
        }
        public static void Saturate(ref Color original, float amount)
        {
            Color.RGBToHSV(original, out var h, out _, out var v);
            original = Color.HSVToRGB(h, amount, v);
        }
        public static void Value(ref Color original, float amount)
        {
            Color.RGBToHSV(original, out var h, out var s, out _);
            original = Color.HSVToRGB(h, s, amount);
        }
        public static float SoftLight(float a,float b)
        {
            return 2 * (a * b) + (a * a) - 2 * (a * a) * b;
        }
        public static Color SoftLight(Color original,Color color)
        {
            Color newColor = original;
            newColor.r = SoftLight(newColor.r, color.r);
            newColor.b = SoftLight(newColor.b, color.b);
            newColor.g = SoftLight(newColor.g, color.g);
            return newColor;
        }
        public static void SoftLight(ref Color original, Color color)
        {
            original = SoftLight(original,color);
        }
        public static void SoftLight_Mask(ref Color original, Color mask, Color color)
        {
            original = Color.Lerp(original, SoftLight(original, color), 1 - mask.r);
        }
        public static void SoftLight_Alpha(ref Color original, Color color)
        {
            original = Color.Lerp(original, SoftLight(original, color), color.a);
        }
        public static void Multiply(ref Color original, Color color)
        {
            original = original * color;
        }
        public static void Multiply_Mask(ref Color original, Color mask, Color color)
        {
            original = Color.Lerp(original, original * color, 1 - mask.r);
        }
        public static void Invert(ref Color original)
        {
            original = original - Color.white;
        }
        public static void Invert_Mask(ref Color original, Color mask)
        {
            original = Color.Lerp(original, original - Color.white, 1 - mask.r);
        }
        public static void Lerp_Mask(ref Color original, Color mask, Color color)
        {
            original = Color.Lerp(original, color, 1 - mask.r);
        }
        public static void Lerp_Alpha(ref Color original, Color reference)
        {
            original = Color.Lerp(original, reference, reference.a);
        }
    }
    //Much more General functions to handle Coloring over the 
    public static partial class PixelKit
    {
        public static void SoftLight(ref Color[] original, Color color)
        {
            for (int i = 0; i < original.Length; i++)
            {
                SoftLight(ref original[i], color);
            }
        }
        public static void SoftLight_Mask(ref Color[] original, Color[] mask, Color color)
        {
            for (int i = 0; i < original.Length; i++)
            {
                SoftLight_Mask(ref original[i], mask[i], color);
            }
        }
        public static void SoftLight_Alpha(ref Color[] original, Color[] reference)
        {
            for (int i = 0; i < original.Length; i++)
            {
                SoftLight_Alpha(ref original[i], reference[i]);
            }
        }
        public static void Multiply(ref Color[] original, Color color)
        {
            for (int i = 0; i < original.Length; i++)
            {
                Multiply(ref original[i], color);
            }
        }
        public static void Multiply(ref Color[] original, Color[] colors)
        {
            for (int i = 0; i < original.Length; i++)
            {
                Multiply(ref original[i], colors[i]);
            }
        }
        public static void Multiply_Mask(ref Color[] original, Color[] mask, Color color)
        {
            for (int i = 0; i < original.Length; i++)
            {
                Multiply_Mask(ref original[i], mask[i], color);
            }
        }
        public static void Multiply_Mask(ref Color[] original, Color[] mask, Color[] colors)
        {
            for (int i = 0; i < original.Length; i++)
            {
                Multiply_Mask(ref original[i], mask[i], colors[i]);
            }
        }
        public static void Invert(ref Color[] original)
        {
            for (int i = 0; i < original.Length; i++)
            {
                Invert(ref original[i]);
            }
        }
        public static void Invert_Mask(ref Color[] original, Color[] mask)
        {
            for (int i = 0; i < original.Length; i++)
            {
                Invert_Mask(ref original[i], mask[i]);
            }
        }
        public static void Lerp_Mask(ref Color[] original, Color[] mask, Color color)
        {
            for (int i = 0; i < original.Length; i++)
            {
                Lerp_Mask(ref original[i], mask[i], color);
            }
        }
        public static void Lerp_Mask(ref Color[] original, Color[] mask, Color[] colors)
        {
            for (int i = 0; i < original.Length; i++)
            {
                Lerp_Mask(ref original[i], mask[i], colors[i]);
            }
        }
        public static void Lerp_Alpha(ref Color[] original, Color[] reference)
        {
            for (int i = 0; i < original.Length; i++)
            {
                Lerp_Alpha(ref original[i], reference[i]);
            }
        }
    }

    //PixelStuff
    public static partial class PixelKit
    {
        public static Color[] GenerateColorBlock(Color color, Vector2Int size)
        {
            int pixelcount = size.x * size.y;
            Color[] colors = new Color[pixelcount];
            for (int i = 0; i < pixelcount; i++)
            {
                colors[i] = color;
            }
            return colors;
        }
    }
}