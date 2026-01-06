using System;
using Shapes;
using UnityEngine;


public class BookPattern : MonoBehaviour
{
    public BookType bookType;

    private RegularPolygon _regularPolygon;

    private void Awake()
    {
        _regularPolygon = GetComponent<RegularPolygon>();
    }

    public void SetBookType(BookType type)
    {
        bookType = type;
        
        UpdatePattern();
    }

    private void Update()
    {
        UpdatePattern();
    }

    private void UpdatePattern()
    {
        UpdateBookColor();
        UpdateBookShape();
        UpdateBookLineType();
    }
    
    private void UpdateBookColor()
    {
        switch (bookType.color)
        {
            case EBookColor.Red:
                _regularPolygon.UseFill = false;
                _regularPolygon.Color = Color.red;
                break;
            case EBookColor.Blue:
                _regularPolygon.UseFill = false;
                _regularPolygon.Color = Color.blue;
                break;
            case EBookColor.Whatever:
                // _regularPolygon.UseFill = true;
                // _regularPolygon.FillColorStart = Color.red;
                // _regularPolygon.FillColorEnd = Color.blue;
                _regularPolygon.Color = Color.white;

                break;
        }
    }

    public void UpdateBookShape()
    {
        switch (bookType.shape)
        {
            case EBookShape.Triangle:
                _regularPolygon.Sides = 3;
                _regularPolygon.Radius = 1f;
                _regularPolygon.Angle = -30f * Mathf.Deg2Rad;
                break;
            case EBookShape.Rectangle:
                _regularPolygon.Sides = 4;
                _regularPolygon.Radius = 1f;
                _regularPolygon.Angle = -45f * Mathf.Deg2Rad;
                break;
            case EBookShape.Whatever:
                _regularPolygon.Sides = 32;
                _regularPolygon.Radius = 0.8f;
                _regularPolygon.Angle = 0f;
                break;
        }
    }

    public void UpdateBookLineType()
    {
        switch (bookType.lineType)
        {
            case EBookLineType.Dashed:
                _regularPolygon.Border = true;
                _regularPolygon.Dashed = true;
                break;
            case EBookLineType.Solid:
                _regularPolygon.Border = true;
                _regularPolygon.Dashed = false;
                break;
            case EBookLineType.Whatever:
                _regularPolygon.Border = false;
                _regularPolygon.Dashed = false;
                break;
        }
    }
    
}
