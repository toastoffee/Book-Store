using System;
using UnityEngine;

public enum BookGenre
{
    Classic,
    ScienceFiction,
    Sceptical,
    LoveStory,
    Psychological,
    Historical
}

[Serializable]
public class BookData
{
    public string bookName;

    public BookGenre genre;

    public int sellPrice;

    public int costPrice;
}
