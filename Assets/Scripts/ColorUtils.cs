using UnityEngine;


public static class ColorUtils
{
    /// <summary>
    /// 返回一个更亮的颜色（通过提高 HSV 中的 Value）
    /// </summary>
    /// <param name="color">原始颜色</param>
    /// <param name="brightnessFactor">亮度倍数（>1 变亮，<1 变暗）</param>
    public static Color Lighten(this Color color, float brightnessFactor = 1.2f)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        v = v * brightnessFactor;
        // v = Mathf.Clamp01(v * brightnessFactor); // 提高亮度，限制在 [0,1]
        Color lighted = Color.HSVToRGB(h, s, v);
        lighted.a = color.a;

        return lighted;
    }
}
