using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookBox : MonoBehaviour
{
    public List<BookData> books = new List<BookData>();

    private DraggableObject _draggableObject;

    public List<Transform> poses = new List<Transform>();
    public float bookHeightInterval;
    
    public BookVisual bookPrefab;

    
    void Start()
    {
        _draggableObject = GetComponent<DraggableObject>();

        _draggableObject.mouseRightClickHandler = TryUnpack;
    }


    void TryUnpack()
    {
        for (int i = 0; i < books.Count; i++)
        {
            int horizontalIndex = i % poses.Count;
            float height = (int)(i / poses.Count) * bookHeightInterval;

            Vector3 pos = poses[horizontalIndex].transform.position + height * Vector3.up;

            BookVisual book = Instantiate(bookPrefab, pos, poses[horizontalIndex].transform.rotation);
            book.SetBookData(books[i]);
        }
        
        // Destroy self
        Destroy(gameObject);
    }
}
