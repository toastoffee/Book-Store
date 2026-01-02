using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;

public class BookVisual : MonoBehaviour
{
    public Rectangle mainRectangle;

    public float lightUpRatios;

    public TMP_Text bookHeader;

    public BookData bookData;
    
    private void Update()
    {
        mainRectangle.FillColorStart = bookData.color;
        mainRectangle.FillColorEnd = bookData.color.Lighten(lightUpRatios);

        bookHeader.text = bookData.title;
    }
}
