using UnityEngine;
using System;

public class Collider2DDraggable : MonoBehaviour
{
    [Header("Settings")]
    public bool allowDragMove = true;
    public Camera eventCamera; // 通常为主相机

    [Header("Events")]
    public Action OnBeginDrag;
    public Action OnDrag;
    public Action OnEndDrag;

    private Vector3 dragOffset;
    private bool isDragging = false;

    private void Awake()
    {
        if (eventCamera == null)
        {
            eventCamera = Camera.main;
        }
    }

    private void OnMouseDown()
    {
        if (!allowDragMove) return;

        isDragging = true;
        OnBeginDrag?.Invoke();

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        dragOffset = transform.position - mouseWorldPos;
    }

    private void OnMouseDrag()
    {
        if (!isDragging || !allowDragMove) return;

        OnDrag?.Invoke();

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 desiredPosition = mouseWorldPos + dragOffset;

        transform.position = desiredPosition;
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            OnEndDrag?.Invoke();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        if (eventCamera == null)
        {
            Debug.LogError("No camera assigned for dragging!");
            return Vector3.zero;
        }

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = eventCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = transform.position.z; // 保持 Z 不变（2D 平面）
        return worldPos;
    }
}
