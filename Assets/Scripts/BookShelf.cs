using System;
using System.Collections.Generic;
using UnityEngine;

public class BookShelf : MonoBehaviour
{
    private List<Book> bookVisuals = new List<Book>();
    public IReadOnlyList<BookData> bookDatas => bookLibrary.Books;

    public BookLibrary bookLibrary;

    public Book bookPrefab;

    public float bookInterval;

    public Transform bookAlignPos;
     
    private void Start()
    {
        for (int i = 0; i < bookDatas.Count; i++)
        {
            Vector2 pos = (Vector2)bookAlignPos.position + i * Vector2.right * bookInterval;
            Book book = Instantiate(bookPrefab, pos, Quaternion.identity);
            book.transform.SetParent(bookAlignPos);
            
            book.bookData = bookDatas[i];
            bookVisuals.Add(book);
        }
    }

}
