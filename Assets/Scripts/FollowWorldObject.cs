using UnityEngine;

public class FollowWorldTransformOnScreen : MonoBehaviour
{
    [Tooltip("要跟随的目标 Transform")]
    public Transform target;

    [Tooltip("用于渲染 UI 的相机（Screen Space - Camera 时必填）")]
    public Camera uiCamera; // 如果是 Overlay，可设为 null，会自动用主相机

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("FollowWorldTransformOnScreen 需要挂载在带有 RectTransform 的 GameObject 上！");
            enabled = false;
            return;
        }

        if (target == null)
        {
            Debug.LogWarning("FollowWorldTransformOnScreen 的 target 未指定！");
        }

        if (uiCamera == null && GetComponentInParent<Canvas>()?.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Debug.LogWarning("ScreenSpace-Camera 模式下建议指定 uiCamera！");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Camera cam = uiCamera != null ? uiCamera : Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("未找到用于屏幕坐标的相机！");
            return;
        }

        Vector2 screenPos;
        // if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //     rectTransform.parent as RectTransform,
        //     cam.WorldToScreenPoint(target.position),
        //     cam,
        //     out screenPos))
        // {
        //     Debug.Log(screenPos);
        //     
        //     rectTransform.anchoredPosition = screenPos;
        // }

        var pos = cam.WorldToScreenPoint(target.position);

        rectTransform.position = pos;
    }
}
