using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BookShelf : MonoBehaviour
{
    public int maxContent = 32;
    
    public List<BookVisual> allBookVisuals = new List<BookVisual>();
    public List<Transform> anchorPoses = new List<Transform>();
    
    public List<BookData> allBooks = new List<BookData>();

    public bool isBookFull => allBooks.Count >= maxContent;
    public bool hasBooks => allBooks.Count > 0;

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
        int newBookIdx = allBooks.Count;
        allBooks.Add(book.bookData);

        allBookVisuals[newBookIdx].transform.position = book.transform.position;
        allBookVisuals[newBookIdx].transform.rotation = book.transform.rotation;

        Sequence seq = DOTween.Sequence();
        seq.Append(allBookVisuals[newBookIdx].transform.DOLocalMoveX(0f, 0.8f));
        seq.Join(allBookVisuals[newBookIdx].transform.DOLocalMoveY(0f, 0.8f));
        seq.Join(allBookVisuals[newBookIdx].transform.DOLocalRotateQuaternion(Quaternion.identity, 0.8f));
        seq.Append(allBookVisuals[newBookIdx].transform.DOLocalMoveZ(0f, 0.6f));
    }

    /// <summary>
    /// 从书架移除一本书并返回BookData，如果没有书则返回null
    /// </summary>
    public BookData RemoveBook()
    {
        if (allBooks.Count == 0) return null;
        
        BookData removedBook = allBooks[allBooks.Count - 1];
        allBooks.RemoveAt(allBooks.Count - 1);
        
        return removedBook;
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


    private void OnTriggerEnter(Collider other)
    {
        if(isBookFull) return;
        
        BookVisual bookVisual = other.GetComponent<BookVisual>();
        DraggableObject draggableObject = other.GetComponent<DraggableObject>();

        if (bookVisual != null && draggableObject != null)
        {
            AddBook(bookVisual);
            
            Destroy(other.gameObject);
        }
    }
}
