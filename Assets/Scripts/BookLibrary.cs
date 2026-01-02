using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BookLibrary", menuName = "Books/Book Library", order = 1)]
public class BookLibrary : ScriptableObject
{
    [SerializeField]
    private List<BookData> books = new List<BookData>();

    public IReadOnlyList<BookData> Books => books;

    public BookData GetBookByTitle(string title)
    {
        return books.Find(b => b.title == title);
    }

    public BookData GetBookAt(int index)
    {
        if (index >= 0 && index < books.Count)
            return books[index];
        return null;
    }
}
