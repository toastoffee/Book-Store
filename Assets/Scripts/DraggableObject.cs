using System;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    [Header("设置")]
    public Camera mainCamera;   
    public float planeHeight = 1.0f;
    public LayerMask draggableLayer;
    public float followSpeed = 8f;

    private bool isDragging = false;
    private Transform originalParent;
    private Rigidbody rb;

    public Action mouseUpHandler;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("DraggableObject requires a Rigidbody!");
            enabled = false;
            return;
        }

        rb.useGravity = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layerMask = draggableLayer != default(LayerMask) ? draggableLayer.value : Physics.AllLayers;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.root == transform.root)
                {
                    PickUp();
                }
            }
        }
        
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            mouseUpHandler?.Invoke();
            
            Drop();
        }
    }

    void PickUp()
    {
        isDragging = true;
        originalParent = transform.parent;
        transform.SetParent(null);

        // 关闭重力，但保持物理响应（不设 isKinematic）
        rb.useGravity = false;

        // 隐藏鼠标（可选）
        Cursor.visible = false;
    }

    void Drop()
    {
        isDragging = false;
        transform.SetParent(originalParent);

        rb.useGravity = true;

        Cursor.visible = true;
    }

    void FixedUpdate()
    {
        if (!isDragging || rb == null) return;

        Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, planeHeight, 0));

        if (groundPlane.Raycast(mouseRay, out float distance))
        {
            Vector3 targetPosition = mouseRay.GetPoint(distance);
            targetPosition.y = planeHeight;

            Vector3 offset = targetPosition - rb.position;

            Vector3 desiredVelocity = offset * followSpeed;

            float magnitude = Mathf.Clamp(desiredVelocity.magnitude, 0f, 15f);

            rb.velocity = desiredVelocity.normalized * magnitude;
        }
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }
}