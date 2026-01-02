using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class BookVisual : MonoBehaviour
{
    public Color BookColor;

    public Rectangle mainRectangle;

    public float lightUpRatios;
    
    private void Update()
    {
        mainRectangle.FillColorStart = BookColor;
        mainRectangle.FillColorEnd = BookColor.Lighten(lightUpRatios);

    }
}
