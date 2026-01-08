using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    [Header("设置")]
    public Camera mainCamera;          // 主摄像机
    public float planeHeight = 0.5f;   // 物体保持的目标 Y 高度
    public LayerMask draggableLayer;   // 可拾取层（可选）
    public float followSpeed = 10f;    // 跟随速度（越大越快，建议 5~20）

    private bool isDragging = false;
    private Vector3 originalPosition;
    private Transform originalParent;
    private Rigidbody rb;

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

        // 确保初始状态受重力（未拾取时）
        rb.useGravity = true;
    }

    void Update()
    {
        // === 拾取：点击物体 ===
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
        // === 释放：再次点击（任意位置）===
        else if (Input.GetMouseButtonDown(0) && isDragging)
        {
            Drop();
        }
    }

    void PickUp()
    {
        isDragging = true;
        originalPosition = transform.position;
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

        // 恢复重力
        rb.useGravity = true;

        // 显示鼠标
        Cursor.visible = true;
    }

    // 👇 所有 Rigidbody 操作必须在 FixedUpdate 中进行！
    void FixedUpdate()
    {
        if (!isDragging || rb == null) return;

        // 1. 计算鼠标在世界空间的目标位置（固定 Y）
        Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, planeHeight, 0));

        if (groundPlane.Raycast(mouseRay, out float distance))
        {
            Vector3 targetPosition = mouseRay.GetPoint(distance);
            targetPosition.y = planeHeight;

            // 2. 计算当前位置到目标的偏移
            Vector3 offset = targetPosition - rb.position;

            // 3. 根据偏移计算期望速度（带阻尼的平滑跟随）
            Vector3 desiredVelocity = offset * followSpeed;

            // 4. 直接设置 velocity（最稳定的方式）
            rb.velocity = desiredVelocity;
        }
    }
}