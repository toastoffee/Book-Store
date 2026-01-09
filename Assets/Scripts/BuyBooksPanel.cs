using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BuyBooksPanel : MonoBehaviour
{
    private List<Vector3> originPoses = new List<Vector3>();

    private List<Transform> childrenTransforms = new List<Transform>();

    private bool isFolded = true;
    
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
}
