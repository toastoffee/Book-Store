using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Customer : MonoBehaviour
{
    [Header("移动设置")]
    public Animator animator;
    public float speed = 2f;
    [Tooltip("移动过程中是否平滑转向行进方向")]
    public bool smoothRotation = true;
    public float rotationSpeed = 5f;
    [Tooltip("到达后对齐路点朝向的渐变时间（秒）")]
    public float alignRotationDuration = 0.3f;

    [Header("闲逛设置")]
    [Tooltip("店铺内的闲逛路径点")]
    public Transform[] wanderWaypoints;
    [Tooltip("在每个路径点停留的最短时间（秒）")]
    public float minWaitTime = 2f;
    [Tooltip("在每个路径点停留的最长时间（秒）")]
    public float maxWaitTime = 5f;

    [Header("书架设置")]
    [Tooltip("店铺内的书架列表")]
    public BookShelf[] bookShelves;
    [Tooltip("在书架前停留的概率（0-1）")]
    [Range(0f, 1f)]
    public float stopAtShelfProbability = 0.6f;
    [Tooltip("在书架前停留的时间（秒）")]
    public float shelfBrowseTime = 3f;
    [Tooltip("购买书籍的概率（0-1）")]
    [Range(0f, 1f)]
    public float purchaseProbability = 0.4f;

    [Header("购物袋设置")]
    [Tooltip("购物袋Transform（用于显示购买的书籍）")]
    public Transform shoppingBag;

    [Header("收银台设置")]
    [Tooltip("收银台位置")]
    public Transform checkoutCounter;
    [Tooltip("在收银台结账的时间（秒）")]
    public float checkoutTime = 2f;

    [Header("离开设置")]
    [Tooltip("在店铺停留的总时间范围（秒）")]
    public Vector2 stayDurationRange = new Vector2(20f, 40f);
    [Tooltip("店铺出口位置")]
    public Transform exitPoint;

    // 私有变量
    private List<BookData> shoppingBagBooks = new List<BookData>();
    private bool isLeaving = false;
    private float stayStartTime;
    private float stayDuration;

    public Vector3 shelfOffset;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // 随机决定停留时间
        stayDuration = Random.Range(stayDurationRange.x, stayDurationRange.y);
        stayStartTime = Time.time;

        // 开始闲逛
        StartCoroutine(CustomerBehavior());
    }

    /// <summary>
    /// 顾客行为主协程
    /// </summary>
    IEnumerator CustomerBehavior()
    {
        // 在店铺内闲逛
        yield return StartCoroutine(WanderInStore());

        // 如果购物袋中有书，去收银台结账
        if (shoppingBagBooks.Count > 0)
        {
            yield return StartCoroutine(GoToCheckout());
        }

        // 离开店铺
        yield return StartCoroutine(LeaveStore());
    }

    /// <summary>
    /// 在店铺内闲逛
    /// </summary>
    IEnumerator WanderInStore()
    {
        while (!ShouldLeave())
        {
            // 有概率在书架前停留
            if (bookShelves != null && bookShelves.Length > 0 && Random.value < stopAtShelfProbability)
            {
                // 随机选择一个书架
                BookShelf randomShelf = GetRandomShelfWithBooks();
                if (randomShelf != null)
                {
                    // 移动到书架前
                    Transform shelfPosition = randomShelf.transform;
                    yield return StartCoroutine(MoveTo(shelfPosition, shelfOffset));

                    // 在书架前停留并浏览
                    yield return StartCoroutine(WaitAtPoint(shelfBrowseTime));

                    // 有概率购买书籍
                    if (Random.value < purchaseProbability)
                    {
                        yield return StartCoroutine(PurchaseBook(randomShelf));
                    }
                }
            }

            // 随机选择一个闲逛路径点
            if (wanderWaypoints != null && wanderWaypoints.Length > 0)
            {
                Transform randomWaypoint = wanderWaypoints[Random.Range(0, wanderWaypoints.Length)];
                yield return StartCoroutine(MoveTo(randomWaypoint));

                // 在路径点停留随机时间
                float waitTime = Random.Range(minWaitTime, maxWaitTime);
                yield return StartCoroutine(WaitAtPoint(waitTime));
            }
            else
            {
                // 如果没有设置路径点，等待一小段时间
                yield return StartCoroutine(WaitAtPoint(1f));
            }
        }
    }

    /// <summary>
    /// 购买书籍
    /// </summary>
    IEnumerator PurchaseBook(BookShelf shelf)
    {
        if (shelf == null || !shelf.hasBooks) yield break;

        // 从书架移除书籍
        BookData purchasedBook = shelf.RemoveBook();
        if (purchasedBook != null)
        {
            // 添加到购物袋
            shoppingBagBooks.Add(purchasedBook);
            
            Debug.Log($"顾客购买了书籍: {purchasedBook.bookName}");

            // 可以在这里添加将书籍放入购物袋的视觉效果
            // 例如：实例化一个书籍模型并移动到购物袋位置
            if (shoppingBag != null)
            {
                // 可以在这里添加书籍放入购物袋的动画效果
            }
        }

        yield return null;
    }

    /// <summary>
    /// 前往收银台结账
    /// </summary>
    IEnumerator GoToCheckout()
    {
        if (checkoutCounter == null)
        {
            Debug.LogWarning("收银台位置未设置！");
            yield break;
        }

        isLeaving = true;

        // 移动到收银台
        yield return StartCoroutine(MoveTo(checkoutCounter));

        // 在收银台结账
        yield return StartCoroutine(WaitAtPoint(checkoutTime));

        Debug.Log($"顾客结账完成，共购买了 {shoppingBagBooks.Count} 本书");
    }

    /// <summary>
    /// 离开店铺
    /// </summary>
    IEnumerator LeaveStore()
    {
        isLeaving = true;

        if (exitPoint != null)
        {
            // 移动到出口
            yield return StartCoroutine(MoveTo(exitPoint));
        }
        else
        {
            Debug.LogWarning("出口位置未设置！顾客将停留在当前位置");
        }

        // 离开后销毁对象（或者可以设置为禁用）
        Debug.Log("顾客离开了店铺");
        Destroy(gameObject);
    }

    /// <summary>
    /// 获取一个有书籍的随机书架
    /// </summary>
    BookShelf GetRandomShelfWithBooks()
    {
        if (bookShelves == null || bookShelves.Length == 0) return null;

        List<BookShelf> shelvesWithBooks = new List<BookShelf>();
        foreach (var shelf in bookShelves)
        {
            if (shelf != null && shelf.hasBooks)
            {
                shelvesWithBooks.Add(shelf);
            }
        }

        if (shelvesWithBooks.Count == 0) return null;

        return shelvesWithBooks[Random.Range(0, shelvesWithBooks.Count)];
    }

    /// <summary>
    /// 判断是否应该离开店铺
    /// </summary>
    bool ShouldLeave()
    {
        return Time.time - stayStartTime >= stayDuration || isLeaving;
    }

    /// <summary>
    /// 移动到目标位置
    /// </summary>
    IEnumerator MoveTo(Transform targetWaypoint, Vector3 offset)
    {
        if (targetWaypoint == null) yield break;

        Vector3 targetPosition = targetWaypoint.position + offset;

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

        /// <summary>
    /// 移动到目标位置
    /// </summary>
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

    /// <summary>
    /// 在当前位置等待指定时间
    /// </summary>
    IEnumerator WaitAtPoint(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    // 公共方法：获取购物袋中的书籍数量
    public int GetShoppingBagBookCount()
    {
        return shoppingBagBooks.Count;
    }

    // 公共方法：获取购物袋中的书籍列表（只读）
    public IReadOnlyList<BookData> GetShoppingBagBooks()
    {
        return shoppingBagBooks.AsReadOnly();
    }
}
