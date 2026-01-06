using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HandCards : MonoBehaviour
{
    private List<BookType> _bookTypes = new List<BookType>();

    public int cardCount;

    public BookPattern bookPatternPrefab;

    public List<BookPattern> _handCards = new List<BookPattern>();

    public float interval;


    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _bookTypes.Clear();
        for (int i = 0; i < cardCount; ++i)
        {
            _bookTypes.Add(GetRandomBook());
        }

        while (_handCards.Count < cardCount)
        {
            BookPattern bookPattern = Instantiate(bookPatternPrefab);
            _handCards.Add(bookPattern);
        }

        for (int i = 0; i < _bookTypes.Count; ++i)
        {
            _handCards[i].transform.position = transform.position + i * Vector3.right * interval;
            
            _handCards[i].SetBookType(_bookTypes[i]);
        }
    }
    
    private BookType GetRandomBook()
    {
        BookType bookType = new BookType();

        bookType.color = Random.Range(0, 1f) > 0.5f ? EBookColor.Blue : EBookColor.Red;
        bookType.shape = Random.Range(0, 1f) > 0.5f ? EBookShape.Rectangle : EBookShape.Triangle;
        bookType.lineType = Random.Range(0, 1f) > 0.5f ? EBookLineType.Solid : EBookLineType.Dashed;

        return bookType;
    }
}
