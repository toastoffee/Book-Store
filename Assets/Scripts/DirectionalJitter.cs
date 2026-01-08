using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Light))]
public class DirectionalLightJitter : MonoBehaviour
{
    [Header("晃动参数")]
    public float maxAngleOffset = 5f;      // 最大偏移角度（度）
    public float duration = 3f;            // 每次晃动持续时间（秒）
    public Ease easeType = Ease.InOutSine; // 缓动类型（推荐正弦缓动）

    private Transform _transform;
    private Vector3 _baseEulerAngles;
    private bool _isAnimating = false;

    void Start()
    {
        _transform = transform;
        _baseEulerAngles = _transform.eulerAngles;

        // 启动第一次晃动
        StartNextJitter();
    }

    void StartNextJitter()
    {
        if (_isAnimating) return;

        // 生成小幅随机偏移（仅 Y 和 X，避免 Z 翻滚）
        float randomX = Random.Range(-maxAngleOffset, maxAngleOffset);
        float randomY = Random.Range(-maxAngleOffset, maxAngleOffset);
        Vector3 targetEulerAngles = new Vector3(
            _baseEulerAngles.x + randomX,
            _baseEulerAngles.y + randomY,
            _baseEulerAngles.z // 通常保持 Z=0
        );

        _isAnimating = true;

        // 执行平滑旋转
        _transform.DORotate(targetEulerAngles, duration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                _isAnimating = false;
                StartNextJitter(); // 循环下一次
            });
    }

    // 可选：在 OnDisable 时清理动画（避免报错）
    void OnDisable()
    {
        DOTween.Kill(_transform);
    }
}
