using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BookTag
{
    Classic,
    ScienceFiction,
    Crime,
    LoveStory,
    psychological,
    Politics
}

[System.Serializable]
public struct TagEntity
{
    public BookTag tag;
    public int score;
}

[System.Serializable]
public class BookData
{
    public string title = "Untitled";
    public string author = "Unknown";
    public List<TagEntity> tagScores;
    
    [Header("Visual Parameters")]
    public Color color = Color.gray;
}
