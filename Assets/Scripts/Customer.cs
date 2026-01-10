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
    [Tooltip("Animator中控制移动/站立的Blend参数名称（Float类型，0=站立，1=行走）")]
    public string animatorBlendParameter = "Blend";
    [Tooltip("是否根据移动速度动态调整Blend参数")]
    public bool useDynamicBlend = true;

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

    [Header("避让设置")]
    [Tooltip("检测附近顾客的半径")]
    public float avoidanceRadius = 2f;
    [Tooltip("避让力度（数值越大，避让越明显）")]
    public float avoidanceStrength = 1.5f;
    [Tooltip("检测附近顾客的层级（用于优化性能）")]
    public LayerMask customerLayer = -1;

    // 私有变量
    private List<BookData> shoppingBagBooks = new List<BookData>();
    private bool isLeaving = false;
    private float stayStartTime;
    private float stayDuration;

    public Vector3 shelfOffset;
    
    // 路径点占用管理（静态，所有顾客共享）
    private static Dictionary<Transform, Customer> occupiedWaypoints = new Dictionary<Transform, Customer>();
    private Transform currentOccupiedWaypoint = null;

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

            // 选择一个未被占用的闲逛路径点
            if (wanderWaypoints != null && wanderWaypoints.Length > 0)
            {
                Transform randomWaypoint = GetAvailableWaypoint();
                if (randomWaypoint != null)
                {
                    // 占用该路径点
                    OccupyWaypoint(randomWaypoint);
                    
                    yield return StartCoroutine(MoveTo(randomWaypoint));

                    // 在路径点停留随机时间
                    float waitTime = Random.Range(minWaitTime, maxWaitTime);
                    yield return StartCoroutine(WaitAtPoint(waitTime));
                    
                    // 释放路径点
                    ReleaseWaypoint();
                }
                else
                {
                    // 如果所有路径点都被占用，等待一小段时间后重试
                    yield return StartCoroutine(WaitAtPoint(1f));
                }
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

        int moneyAcquire = 0;
        foreach (var data in shoppingBagBooks)
        {
            moneyAcquire += data.sellPrice;
        }
        MoneyManager.instance.money += moneyAcquire;

        Debug.Log($"顾客结账完成，共购买了 {shoppingBagBooks.Count} 本书");
    }

    /// <summary>
    /// 离开店铺
    /// </summary>
    IEnumerator LeaveStore()
    {
        isLeaving = true;
        
        // 释放占用的路径点
        ReleaseWaypoint();

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
    
    private void OnDestroy()
    {
        // 确保在销毁时释放占用的路径点
        ReleaseWaypoint();
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
    /// 获取一个未被占用的随机路径点
    /// </summary>
    Transform GetAvailableWaypoint()
    {
        if (wanderWaypoints == null || wanderWaypoints.Length == 0) return null;
        
        // 过滤出未被占用的路径点
        List<Transform> availableWaypoints = new List<Transform>();
        foreach (var waypoint in wanderWaypoints)
        {
            if (waypoint != null && !occupiedWaypoints.ContainsKey(waypoint))
            {
                availableWaypoints.Add(waypoint);
            }
        }
        
        // 如果所有路径点都被占用，清理无效的占用（可能顾客已经销毁但未释放）
        if (availableWaypoints.Count == 0)
        {
            CleanupInvalidOccupations();
            // 再次尝试
            foreach (var waypoint in wanderWaypoints)
            {
                if (waypoint != null && !occupiedWaypoints.ContainsKey(waypoint))
                {
                    availableWaypoints.Add(waypoint);
                }
            }
        }
        
        if (availableWaypoints.Count > 0)
        {
            return availableWaypoints[Random.Range(0, availableWaypoints.Count)];
        }
        
        return null;
    }
    
    /// <summary>
    /// 占用一个路径点
    /// </summary>
    void OccupyWaypoint(Transform waypoint)
    {
        if (waypoint == null) return;
        
        // 如果已经有占用的路径点，先释放
        if (currentOccupiedWaypoint != null)
        {
            ReleaseWaypoint();
        }
        
        occupiedWaypoints[waypoint] = this;
        currentOccupiedWaypoint = waypoint;
    }
    
    /// <summary>
    /// 释放当前占用的路径点
    /// </summary>
    void ReleaseWaypoint()
    {
        if (currentOccupiedWaypoint != null)
        {
            if (occupiedWaypoints.ContainsKey(currentOccupiedWaypoint) && 
                occupiedWaypoints[currentOccupiedWaypoint] == this)
            {
                occupiedWaypoints.Remove(currentOccupiedWaypoint);
            }
            currentOccupiedWaypoint = null;
        }
    }
    
    /// <summary>
    /// 清理无效的路径点占用（已销毁的顾客占用的路径点）
    /// </summary>
    void CleanupInvalidOccupations()
    {
        List<Transform> toRemove = new List<Transform>();
        foreach (var kvp in occupiedWaypoints)
        {
            if (kvp.Value == null || kvp.Key == null)
            {
                toRemove.Add(kvp.Key);
            }
        }
        
        foreach (var key in toRemove)
        {
            occupiedWaypoints.Remove(key);
        }
    }
    
    /// <summary>
    /// 获取避让向量（检测附近的其他顾客并计算避让方向）
    /// </summary>
    Vector3 GetAvoidanceVector()
    {
        Vector3 avoidanceVector = Vector3.zero;
        
        // 如果 customerLayer 设置为有效层级，使用物理检测（更高效）
        if (customerLayer != -1)
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, avoidanceRadius, customerLayer);
            foreach (var col in nearbyColliders)
            {
                Customer otherCustomer = col.GetComponent<Customer>();
                if (otherCustomer != null && otherCustomer != this)
                {
                    Vector3 directionAway = (transform.position - otherCustomer.transform.position);
                    float distance = directionAway.magnitude;
                    
                    if (distance > 0.01f)
                    {
                        // 距离越近，避让力度越大
                        float avoidanceForce = 1f - (distance / avoidanceRadius);
                        avoidanceVector += directionAway.normalized * avoidanceForce;
                    }
                }
            }
        }
        else
        {
            // 如果未设置层级，使用 FindObjectsOfType（性能较低，但更通用）
            Customer[] allCustomers = FindObjectsOfType<Customer>();
            foreach (var otherCustomer in allCustomers)
            {
                if (otherCustomer != null && otherCustomer != this)
                {
                    float distance = Vector3.Distance(transform.position, otherCustomer.transform.position);
                    if (distance < avoidanceRadius && distance > 0.01f)
                    {
                        Vector3 directionAway = (transform.position - otherCustomer.transform.position).normalized;
                        float avoidanceForce = 1f - (distance / avoidanceRadius);
                        avoidanceVector += directionAway * avoidanceForce;
                    }
                }
            }
        }
        
        if (avoidanceVector.magnitude > 0.01f)
        {
            return avoidanceVector.normalized;
        }
        
        return Vector3.zero;
    }

    /// <summary>
    /// 移动到目标位置
    /// </summary>
    IEnumerator MoveTo(Transform targetWaypoint, Vector3 offset)
    {
        if (targetWaypoint == null) yield break;

        Vector3 targetPosition = targetWaypoint.position + offset;

        // 设置Animator Blend参数为行走状态
        SetAnimatorBlend(1f);

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
            // 保存移动前的位置（用于动态Blend计算）
            Vector3 beforeMove = transform.position;
            
            // 计算基础移动方向（朝向目标）
            Vector3 toTarget = (targetPosition - transform.position);
            Vector3 moveDirection = toTarget.normalized;
            
            // 应用避让逻辑
            Vector3 avoidanceVector = GetAvoidanceVector();
            Vector3 finalMoveDirection = moveDirection;
            
            if (avoidanceVector.magnitude > 0.01f)
            {
                // 将避让向量与移动方向结合，但保持朝向目标的方向为主要方向
                finalMoveDirection = (moveDirection + avoidanceVector * avoidanceStrength * 0.3f).normalized;
            }
            
            // 计算移动距离（限制在到目标的距离内）
            float moveDistance = speed * Time.deltaTime;
            float distanceToTarget = toTarget.magnitude;
            moveDistance = Mathf.Min(moveDistance, distanceToTarget);
            
            // 应用移动
            transform.position += finalMoveDirection * moveDistance;

            // 计算实际移动速度，用于动态调整Blend参数
            if (useDynamicBlend)
            {
                float actualSpeed = Vector3.Distance(transform.position, beforeMove) / Time.deltaTime;
                float blendValue = Mathf.Clamp01(actualSpeed / speed);
                SetAnimatorBlend(blendValue);
            }
            else
            {
                // 如果不使用动态Blend，直接设置为最大值（行走状态）
                SetAnimatorBlend(1f);
            }
            
            // 更新朝向（面向实际移动方向）
            if (finalMoveDirection.magnitude > 0.01f)
            {
                moveRotation = Quaternion.LookRotation(finalMoveDirection, Vector3.up);
                moveRotation = Quaternion.Euler(0, moveRotation.eulerAngles.y, 0);
            }

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

        // 停止行走，设置Animator Blend参数为站立状态
        SetAnimatorBlend(0f);
    }

    /// <summary>
    /// 移动到目标位置（无偏移版本）
    /// </summary>
    IEnumerator MoveTo(Transform targetWaypoint)
    {
        if (targetWaypoint == null) yield break;

        Vector3 targetPosition = targetWaypoint.position;

        // 设置Animator Blend参数为行走状态
        SetAnimatorBlend(1f);

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
            // 保存移动前的位置（用于动态Blend计算）
            Vector3 beforeMove = transform.position;
            
            // 计算基础移动方向（朝向目标）
            Vector3 toTarget = (targetPosition - transform.position);
            Vector3 moveDirection = toTarget.normalized;
            
            // 应用避让逻辑
            Vector3 avoidanceVector = GetAvoidanceVector();
            Vector3 finalMoveDirection = moveDirection;
            
            if (avoidanceVector.magnitude > 0.01f)
            {
                // 将避让向量与移动方向结合，但保持朝向目标的方向为主要方向
                finalMoveDirection = (moveDirection + avoidanceVector * avoidanceStrength * 0.3f).normalized;
            }
            
            // 计算移动距离（限制在到目标的距离内）
            float moveDistance = speed * Time.deltaTime;
            float distanceToTarget = toTarget.magnitude;
            moveDistance = Mathf.Min(moveDistance, distanceToTarget);
            
            // 应用移动
            transform.position += finalMoveDirection * moveDistance;

            // 计算实际移动速度，用于动态调整Blend参数
            if (useDynamicBlend)
            {
                float actualSpeed = Vector3.Distance(transform.position, beforeMove) / Time.deltaTime;
                float blendValue = Mathf.Clamp01(actualSpeed / speed);
                SetAnimatorBlend(blendValue);
            }
            else
            {
                // 如果不使用动态Blend，直接设置为最大值（行走状态）
                SetAnimatorBlend(1f);
            }
            
            // 更新朝向（面向实际移动方向）
            if (finalMoveDirection.magnitude > 0.01f)
            {
                moveRotation = Quaternion.LookRotation(finalMoveDirection, Vector3.up);
                moveRotation = Quaternion.Euler(0, moveRotation.eulerAngles.y, 0);
            }

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

        // 停止行走，设置Animator Blend参数为站立状态
        SetAnimatorBlend(0f);
    }

    /// <summary>
    /// 在当前位置等待指定时间
    /// </summary>
    IEnumerator WaitAtPoint(float seconds)
    {
        // 确保Animator设置为站立状态
        SetAnimatorBlend(0f);
        
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

    /// <summary>
    /// 设置Animator的Blend参数（用于控制移动/站立状态）
    /// </summary>
    /// <param name="value">0 = 站立，1 = 行走</param>
    void SetAnimatorBlend(float value)
    {
        if (animator == null || string.IsNullOrEmpty(animatorBlendParameter))
            return;

        // 检查参数是否存在且为Float类型
        if (HasAnimatorParameter(animatorBlendParameter, AnimatorControllerParameterType.Float))
        {
            animator.SetFloat(animatorBlendParameter, Mathf.Clamp01(value));
        }
        else if (HasAnimatorParameter(animatorBlendParameter))
        {
            // 如果参数存在但不是Float类型，输出警告
            Debug.LogWarning($"Animator参数 '{animatorBlendParameter}' 不是Float类型！请检查Animator Controller。");
        }
        // 如果参数不存在，不输出警告（可能用户还没有设置）
    }

    /// <summary>
    /// 检查Animator是否有指定参数
    /// </summary>
    bool HasAnimatorParameter(string paramName)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 检查Animator是否有指定类型的参数
    /// </summary>
    bool HasAnimatorParameter(string paramName, AnimatorControllerParameterType paramType)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName && param.type == paramType)
                return true;
        }
        return false;
    }
}
