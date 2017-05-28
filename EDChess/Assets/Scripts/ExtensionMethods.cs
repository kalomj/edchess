using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethods {

	public static void SetAlpha(this Image image, float value)
    {
        Color color = image.color;
        color.a = value;
        image.color = color;
    }

    public static void Desaturate(this Image image)
    {
        Color color = image.color;
        float h;
        float s;
        float v;

        Color.RGBToHSV(color, out h, out s, out v);

        Color.HSVToRGB(h, .5f, 1);

        image.color = Color.HSVToRGB(h, .5f, 1);
    }
}
