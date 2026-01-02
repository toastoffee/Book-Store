using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BookTag
{
    Classic,
    ScienceFiction,
    Sceptical,
    Historical,
    Philosophy,
    LoveStory,
    Mental,
    Feminism,
    Education,
}

[System.Serializable]
public class BookData
{
    public string title = "Untitled";
    public string author = "Unknown";
    public List<BookTag> tags;
    
    [Header("Visual Parameters")]
    public Color color = Color.gray;
}
