using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("顾客预制体设置")]
    [Tooltip("单个顾客预制体（如果设置了多个预制体列表，则优先使用列表）")]
    public Customer customerPrefab;
    [Tooltip("多个顾客预制体列表（将从中随机选择）")]
    public Customer[] customerPrefabs;
    
    [Header("生成位置设置")]
    [Tooltip("生成位置（如果设置了多个生成点，将从中随机选择）")]
    public Transform spawnPoint;
    [Tooltip("多个生成点列表（将从中随机选择）")]
    public Transform[] spawnPoints;
    
    [Header("生成时间设置")]
    [Tooltip("两次生成之间的最短间隔时间（秒）")]
    public float minSpawnInterval = 5f;
    [Tooltip("两次生成之间的最长间隔时间（秒）")]
    public float maxSpawnInterval = 15f;
    
    [Header("其他设置")]
    [Tooltip("是否在游戏开始时立即生成第一个顾客")]
    public bool spawnOnStart = true;
    [Tooltip("是否启用自动生成")]
    public bool autoSpawn = true;
    [Tooltip("最大同时存在的顾客数量（0表示无限制）")]
    public int maxCustomers = 0;
    
    // 私有变量
    private List<Customer> activeCustomers = new List<Customer>();
    private Coroutine spawnCoroutine;
    
    
    public Transform[] wanderWaypoints;
    public BookShelf[] bookShelves;
    public Transform checkoutCounter;
    public Transform exitPoint;
    
    private void Start()
    {
        if (autoSpawn)
        {
            if (spawnOnStart)
            {
                // 立即生成第一个顾客
                SpawnCustomer();
            }
            
            // 开始生成循环
            spawnCoroutine = StartCoroutine(SpawnLoop());
        }
    }
    
    /// <summary>
    /// 生成循环协程
    /// </summary>
    IEnumerator SpawnLoop()
    {
        while (autoSpawn)
        {
            // 等待随机间隔时间
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
            
            // 检查是否达到最大顾客数量限制
            if (maxCustomers > 0 && activeCustomers.Count >= maxCustomers)
            {
                // 清理已销毁的顾客引用
                activeCustomers.RemoveAll(c => c == null);
                continue;
            }
            
            // 生成顾客
            SpawnCustomer();
        }
    }
    
    /// <summary>
    /// 生成一个随机顾客
    /// </summary>
    public Customer SpawnCustomer()
    {
        // 选择要使用的预制体
        Customer prefabToSpawn = GetRandomPrefab();
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("CustomerSpawner: 没有可用的顾客预制体！");
            return null;
        }
        
        // 选择生成位置
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Quaternion spawnRotation = GetRandomSpawnRotation();
        
        // 实例化顾客
        Customer newCustomer = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);

        newCustomer.wanderWaypoints = wanderWaypoints;
        newCustomer.bookShelves = bookShelves;
        newCustomer.checkoutCounter = checkoutCounter;
        newCustomer.exitPoint = exitPoint;
        
        // 添加到活动顾客列表
        if (maxCustomers > 0)
        {
            activeCustomers.Add(newCustomer);
        }
        
        Debug.Log($"生成了新的顾客，当前活跃顾客数量: {activeCustomers.Count}");
        
        return newCustomer;
    }
    
    /// <summary>
    /// 获取随机顾客预制体
    /// </summary>
    Customer GetRandomPrefab()
    {
        // 优先使用预制体列表
        if (customerPrefabs != null && customerPrefabs.Length > 0)
        {
            // 过滤掉空的引用
            List<Customer> validPrefabs = new List<Customer>();
            foreach (var prefab in customerPrefabs)
            {
                if (prefab != null)
                {
                    validPrefabs.Add(prefab);
                }
            }
            
            if (validPrefabs.Count > 0)
            {
                return validPrefabs[Random.Range(0, validPrefabs.Count)];
            }
        }
        
        // 如果没有列表，使用单个预制体
        return customerPrefab;
    }
    
    /// <summary>
    /// 获取随机生成位置
    /// </summary>
    Vector3 GetRandomSpawnPosition()
    {
        // 优先使用生成点列表
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            // 过滤掉空的引用
            List<Transform> validPoints = new List<Transform>();
            foreach (var point in spawnPoints)
            {
                if (point != null)
                {
                    validPoints.Add(point);
                }
            }
            
            if (validPoints.Count > 0)
            {
                Transform selectedPoint = validPoints[Random.Range(0, validPoints.Count)];
                return selectedPoint.position;
            }
        }
        
        // 如果没有列表，使用单个生成点
        if (spawnPoint != null)
        {
            return spawnPoint.position;
        }
        
        // 如果都没有，使用当前对象位置
        Debug.LogWarning("CustomerSpawner: 没有设置生成位置，使用当前对象位置");
        return transform.position;
    }
    
    /// <summary>
    /// 获取随机生成旋转
    /// </summary>
    Quaternion GetRandomSpawnRotation()
    {
        // 优先使用生成点列表的旋转
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            List<Transform> validPoints = new List<Transform>();
            foreach (var point in spawnPoints)
            {
                if (point != null)
                {
                    validPoints.Add(point);
                }
            }
            
            if (validPoints.Count > 0)
            {
                Transform selectedPoint = validPoints[Random.Range(0, validPoints.Count)];
                return selectedPoint.rotation;
            }
        }
        
        // 如果没有列表，使用单个生成点的旋转
        if (spawnPoint != null)
        {
            return spawnPoint.rotation;
        }
        
        // 如果都没有，使用默认旋转
        return Quaternion.identity;
    }
    
    /// <summary>
    /// 启用自动生成
    /// </summary>
    public void EnableSpawn()
    {
        if (!autoSpawn)
        {
            autoSpawn = true;
            spawnCoroutine = StartCoroutine(SpawnLoop());
        }
    }
    
    /// <summary>
    /// 禁用自动生成
    /// </summary>
    public void DisableSpawn()
    {
        autoSpawn = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
    
    /// <summary>
    /// 获取当前活跃的顾客数量
    /// </summary>
    public int GetActiveCustomerCount()
    {
        // 清理已销毁的顾客引用
        activeCustomers.RemoveAll(c => c == null);
        return activeCustomers.Count;
    }
    
    /// <summary>
    /// 清理已销毁的顾客引用（可在外部调用）
    /// </summary>
    public void CleanupDestroyedCustomers()
    {
        activeCustomers.RemoveAll(c => c == null);
    }
}
