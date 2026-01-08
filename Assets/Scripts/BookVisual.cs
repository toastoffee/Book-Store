using System;
using UnityEngine;


public class BookVisual : MonoBehaviour
{
    private MeshRenderer outRenderer;

    public BookData bookData;
    
    private void Start()
    {
        outRenderer = GetComponent<MeshRenderer>();
        
        ApplyBookColor();
    }

    public void SetBookData(BookData data)
    {
        bookData = data;
        ApplyBookColor();
    }

    private void ApplyBookColor()
    {
        if(bookData == null) return;
        
        Color color = GetColor(bookData.genre);
        
        if (outRenderer != null && outRenderer.material != null)
        {
            outRenderer.material.color = color;
        }
    }

    private Color GetColor(BookGenre genre)
    {
        switch (genre)
        {
            case BookGenre.Classic:
                return new Color(1.0f, 0.4f, 0.0f);
            case BookGenre.ScienceFiction:
                return new Color(0f, 0.4f, 1f);
            case BookGenre.LoveStory:
                return new Color(1.0f, 0.4f, 0.67f);
            case BookGenre.Sceptical:
                return new Color(0.57f, 0f, 0.9f);
            case BookGenre.Psychological:
                return new Color(0f, 0.74f, 0.40f);
            case BookGenre.Historical:
                return new Color(1.0f, 0.9f, 0.0f);
        }

        return Color.white;
    }

}
