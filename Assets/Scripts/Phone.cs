
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Phone : MonoBehaviour
{
    public LayerMask clickLayer;

    public BuyBooksPanel panel;

    public Transform phoneTransform;

    public float phoneJumpHeight;
    public float phoneJumpDuration;
    public float phoneShakeStrength;
    public float phoneShakeDuration;
    
    private Vector3 phoneOriginPos;

    private void Start()
    {
        phoneOriginPos = phoneTransform.position;
    }

    void Update()
    {
        
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layerMask = clickLayer.value;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.root == transform.root)
                {
                    panel.FoldOrUnFold();

                    Sequence seq = DOTween.Sequence();
                    seq.Append(phoneTransform.DOLocalMove(phoneOriginPos + Vector3.up * phoneJumpHeight, phoneJumpDuration));
                    seq.Append(phoneTransform.DOShakeRotation(phoneShakeDuration, 90.0f * phoneShakeStrength));
                    seq.Append(phoneTransform.DOLocalMove(phoneOriginPos, phoneJumpDuration));
                }
            }
        }
    }

}
