using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BuyBooksPanel : MonoBehaviour
{
    private List<Vector3> originPoses = new List<Vector3>();

    private List<Transform> childrenTransforms = new List<Transform>();

    private bool isFolded = true;

    public BookBox bookBoxPrefab;

    public Transform SpawnPos;
    
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            childrenTransforms.Add(transform.GetChild(i));
        }

        foreach (var child in childrenTransforms)
        {
            originPoses.Add(child.transform.localPosition);
        }

        foreach (var child in childrenTransforms)
        {
            child.localPosition = Vector3.zero;
            child.localScale = Vector3.zero;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            FoldOrUnFold();
        }
    }

    public void FoldOrUnFold()
    {
        if (isFolded)
        {
            UnFold();
        }
        else
        {
            Fold();
        }

        isFolded = !isFolded;
    }
    
    public void UnFold()
    {
        for (int i = 0; i < childrenTransforms.Count; i+=2)
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(i * 0.05f);
            seq.Append(childrenTransforms[i].DOLocalMove(originPoses[i], 0.2f));
            seq.Join(childrenTransforms[i].DOScale(0.7f, 0.2f));
            seq.Join(childrenTransforms[i+1].DOLocalMove(originPoses[i+1], 0.2f));
            seq.Join(childrenTransforms[i+1].DOScale(0.7f, 0.2f));
        }
    }

    public void Fold()
    {
        for (int i = 0; i < childrenTransforms.Count; i+=2)
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(i * 0.05f);
            seq.Append(childrenTransforms[i].DOLocalMove(Vector3.zero, 0.2f));
            seq.Join(childrenTransforms[i].DOScale(0.0f, 0.2f));
            seq.Join(childrenTransforms[i+1].DOLocalMove(Vector3.zero, 0.2f));
            seq.Join(childrenTransforms[i+1].DOScale(0.0f, 0.2f));
        }
    }

    public void OrderClassic()
    {
        TrySpawnABookBox(BookGenre.Classic);
    }
    public void OrderScienceFiction()
    {
        TrySpawnABookBox(BookGenre.ScienceFiction);
    }
    public void OrderSceptical()
    {
        TrySpawnABookBox(BookGenre.Sceptical);
    }
    public void OrderLoveStory()
    {
        TrySpawnABookBox(BookGenre.LoveStory);
    }
    public void OrderPsychological()
    {
        TrySpawnABookBox(BookGenre.Psychological);
    }
    public void OrderHistorical()
    {
        TrySpawnABookBox(BookGenre.Historical);
    }
    
    public void TrySpawnABookBox(BookGenre genre)
    {
        if (MoneyManager.instance.money < 50) return;
        MoneyManager.instance.money -= 50;
        
        BookBox box = Instantiate(bookBoxPrefab, SpawnPos.position, Quaternion.identity);

        List<BookData> books = new List<BookData>();

        for (int i = 0; i < 5; i++)
        {
            var book = new BookData();
            book.bookName = "";
            book.sellPrice = 20;
            book.costPrice = 10;
            book.genre = genre;
            books.Add(book);
        }

        box.books = books;
    }
}
