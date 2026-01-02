using System;
using TMPro;
using UnityEngine;


[ExecuteInEditMode]
public class BookVisual : MonoBehaviour
{
    public MeshRenderer insideVisual;
    public MeshRenderer leftVisual, rightVisual, backVisual;

    public float bookHeight, bookThickness, bookOuterThickness, bookMargin, bookRatio;
    public Color bookOuterColor;
    public float bookBackTitleStartPos;
    public float fontSize;
    
    public TMP_Text bookBackText, bookRightText;
    public string bookName;

    private Material m_leftMat;
    private Material m_rightMat;
    private Material m_backMat;
    
    private void Update()
    {
        float outerHeight = bookHeight;
        float outerWidth = bookHeight / bookRatio;
        
        // Scale
        leftVisual.transform.localScale = new Vector3(bookOuterThickness, outerHeight, outerWidth);
        rightVisual.transform.localScale = new Vector3(bookOuterThickness, outerHeight, outerWidth);
        insideVisual.transform.localScale = new Vector3(bookThickness - 2 * bookOuterThickness,
            outerHeight - 2 * bookMargin,
            outerWidth - bookMargin - bookOuterThickness);
        backVisual.transform.localScale = new Vector3(bookThickness - 2 * bookOuterThickness, outerHeight, bookOuterThickness);

        bookBackText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, outerHeight - bookBackTitleStartPos);
        // bookBackText.transform.localScale = new Vector3(bookThickness - 2 * bookOuterThickness, outerHeight, bookOuterThickness);
        
        // Poses
        leftVisual.transform.localPosition = new Vector3(-(bookThickness - bookOuterThickness) / 2f, outerHeight / 2f, outerWidth / 2f);
        rightVisual.transform.localPosition = new Vector3((bookThickness - bookOuterThickness) / 2f, outerHeight / 2f, outerWidth / 2f);
        insideVisual.transform.localPosition = new Vector3(0f, outerHeight / 2f, (outerWidth - bookMargin + bookOuterThickness) / 2f);
        backVisual.transform.localPosition = new Vector3(0f, outerHeight / 2f, bookOuterThickness / 2f);
        
        bookBackText.transform.localPosition = new Vector3(0f, outerHeight / 2f, -0.01f);
        
        // Color
        UpdateMaterialIfNeeded(ref m_leftMat, leftVisual, bookOuterColor);
        UpdateMaterialIfNeeded(ref m_rightMat, rightVisual, bookOuterColor);
        UpdateMaterialIfNeeded(ref m_backMat, backVisual, bookOuterColor);
        
        // Font Size
        bookBackText.fontSize = fontSize * bookThickness;
    }
    
    private void UpdateMaterialIfNeeded(ref Material cachedMat, MeshRenderer meshRenderer, Color targetColor)
    {
        if (meshRenderer == null) return;

        // 如果还没有缓存材质，或者当前 Renderer 使用的不是我们的缓存材质
        if (cachedMat == null || meshRenderer.sharedMaterial != cachedMat)
        {
            // 创建一次性的材质实例（基于当前 sharedMaterial）
            cachedMat = Instantiate(meshRenderer.sharedMaterial);
            meshRenderer.material = cachedMat; // 应用到 Renderer（只做一次）
        }

        // 更新颜色（只改缓存的实例，不影响原始资源）
        if (cachedMat.color != targetColor)
        {
            cachedMat.color = targetColor;
        }
    }

}
