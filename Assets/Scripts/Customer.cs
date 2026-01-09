using UnityEngine;
using System.Collections;

public class Customer : MonoBehaviour
{
    public Animator animator;
    public float speed = 2f;

    [Tooltip("移动过程中是否平滑转向行进方向")]
    public bool smoothRotation = true;
    public float rotationSpeed = 5f;

    [Tooltip("到达后对齐路点朝向的渐变时间（秒）")]
    public float alignRotationDuration = 0.3f;

    public Transform pPoint1, pPoint2, pPoint3, pPoint4;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(MoveAlongPath());
    }

    IEnumerator MoveAlongPath()
    {
        yield return null;

        yield return MoveTo(pPoint1);
        yield return WaitAtPoint(5f);

        yield return MoveTo(pPoint2);
        yield return WaitAtPoint(5f);

        yield return MoveTo(pPoint3); // 不停留

        yield return MoveTo(pPoint4);
        yield return WaitAtPoint(5f);
    }

    IEnumerator MoveTo(Transform targetWaypoint)
    {
        if (targetWaypoint == null) yield break;

        Vector3 targetPosition = targetWaypoint.position;

        if (animator != null)
            animator.SetBool("isWalking", true);

        // 计算移动过程中的朝向（面向行进方向）
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion moveRotation = transform.rotation;
        if (direction.magnitude > 0.01f)
        {
            moveRotation = Quaternion.LookRotation(direction, Vector3.up);
            moveRotation = Quaternion.Euler(0, moveRotation.eulerAngles.y, 0); // 仅 Y 轴
        }

        // === 移动到目标位置 ===
        while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (smoothRotation)
                transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, rotationSpeed * Time.deltaTime);
            else
                transform.rotation = moveRotation;

            yield return null;
        }

        // 精确对齐位置
        transform.position = targetPosition;

        // === 平滑旋转到路点的朝向 ===
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = targetWaypoint.rotation;
        float elapsed = 0f;

        while (elapsed < alignRotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / alignRotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 确保最终完全对齐
        transform.rotation = targetRotation;

        // 停止行走
        if (animator != null)
            animator.SetBool("isWalking", false);
    }

    IEnumerator WaitAtPoint(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}