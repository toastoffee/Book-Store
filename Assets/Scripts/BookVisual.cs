using System;
using UnityEngine;


[ExecuteInEditMode]
public class BookVisual : MonoBehaviour
{
    public MeshRenderer insideVisual;
    public MeshRenderer leftVisual, rightVisual, backVisual;

    public float bookHeight, bookThickness, bookOuterThickness, bookMargin, bookRatio;
    public Color bookOuterColor;

    private void Update()
    {
        float outerHeight = bookHeight;
        float outerWidth = bookHeight / bookRatio;
        
        // Scale
        leftVisual.transform.localScale = new Vector3(bookOuterThickness, outerHeight, outerWidth);
        rightVisual.transform.localScale = new Vector3(bookOuterThickness, outerHeight, outerWidth);
        insideVisual.transform.localScale = new Vector3(bookThickness - 2 * bookOuterThickness,
            outerHeight - 2 * bookMargin,
            outerWidth - bookMargin - bookOuterThickness);
        backVisual.transform.localScale = new Vector3(bookThickness - 2 * bookOuterThickness, outerHeight, bookOuterThickness);
        
        // Poses
        leftVisual.transform.localPosition = new Vector3(-(bookThickness - bookOuterThickness) / 2f, outerHeight / 2f, outerWidth / 2f);
        rightVisual.transform.localPosition = new Vector3((bookThickness - bookOuterThickness) / 2f, outerHeight / 2f, outerWidth / 2f);
        insideVisual.transform.localPosition = new Vector3(0f, outerHeight / 2f, (outerWidth - bookMargin + bookOuterThickness) / 2f);
        backVisual.transform.localPosition = new Vector3(0f, outerHeight / 2f, bookOuterThickness / 2f);
    }

}
