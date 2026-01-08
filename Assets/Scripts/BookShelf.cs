using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShelf : MonoBehaviour
{
    public int maxContent = 32;
    
    public List<BookVisual> allBookVisuals = new List<BookVisual>();
    public List<Transform> anchorPoses = new List<Transform>();
    
    public List<BookData> allBooks = new List<BookData>();


    private void Start()
    {
        for (int i = 0; i < maxContent; i++)
        {
            Transform anchor = new GameObject("anchor").transform;
            anchor.SetParent(transform);
            anchor.localPosition = allBookVisuals[i].transform.localPosition;
            anchor.localRotation = allBookVisuals[i].transform.localRotation;
            
            allBookVisuals[i].transform.SetParent(anchor);
            allBookVisuals[i].transform.localPosition = Vector3.zero;
            anchorPoses.Add(anchor);
            
        }
    }

    private void Update()
    {
        UpdateVisuals();
    }

    public void AddBook(BookVisual book)
    {
        allBooks.Add(book.bookData);
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < maxContent; i++)
        {
            BookData bookData = allBooks.Count > i ? allBooks[i] : null;
            allBookVisuals[i].gameObject.SetActive(bookData != null);
            
            if(bookData == null) continue;
            allBookVisuals[i].SetBookData(bookData);
            
        }
    }
}
