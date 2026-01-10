using DG.Tweening;
using TMPro;
using UnityEngine;

public class StartBtn : MonoBehaviour
{
    public LayerMask clickLayer;

    public CustomerSpawner customerSpawner;

    public TMP_Text headerText;
    public TMP_Text descText;
    public TMP_Text startText;
    
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

                    Color headerColor = headerText.color;
                    headerColor.a = 0.0f;
                    headerText.DOColor(headerColor, 1.0f);
                    
                    Color descColor = descText.color;
                    descColor.a = 0.0f;
                    descText.DOColor(descColor, 1.0f);

                    Color startColor = startText.color;
                    startColor.a = 0.0f;
                    startText.DOColor(startColor, 1.0f);

                    Sequence seq = DOTween.Sequence();
                    seq.AppendInterval(2.0f);
                    seq.OnComplete(() => {
                        customerSpawner.gameObject.SetActive(true);
                        gameObject.SetActive(false);
                    });
                    
                    Debug.Log("Start");
                }
            }
        }
    }

}
