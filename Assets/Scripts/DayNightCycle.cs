using UnityEngine;

[RequireComponent(typeof(Light))]
public class DayNightCycle : MonoBehaviour
{
    public float dayLengthInSeconds = 120f;
    public Gradient sunColorGradient;
    public AnimationCurve sunIntensityCurve;
    public Gradient ambientColorGradient;

    // 只读，用于调试
    [SerializeField] private float timeOfDay;

    private Light sunLight;
    private float startTime;
    private Vector3 initialLocalEulerAngles; // 记录初始局部欧拉角

    public float TotalRange = 180f;

    void Start()
    {
        sunLight = GetComponent<Light>();
        startTime = Time.time;

        // 保存初始局部旋转（关键！）
        initialLocalEulerAngles = transform.localEulerAngles;

        // 初始化默认 Gradient（避免空引用）
        if (sunColorGradient == null)
            sunColorGradient = CreateDefaultSunGradient();
        if (ambientColorGradient == null)
            ambientColorGradient = CreateDefaultAmbientGradient();
    }

    void Update()
    {
        // 计算归一化时间 (0~1)
        timeOfDay = Mathf.Repeat((Time.time - startTime) / dayLengthInSeconds, 1f);

        // 计算太阳绕世界 Y 轴的角度（0~360）
        float sunAngleY = (timeOfDay - 0.5f) * TotalRange;

        // 构造新的局部旋转：
        // - X: 保持初始值（控制太阳高度，如 45° 表示斜射）
        // - Y: 动态更新（控制方位：东→南→西→北）
        // - Z: 保持初始值（通常为 0）
        Vector3 newLocalEulerAngles = new Vector3(
            initialLocalEulerAngles.x,  // 保留原始俯仰角（高度）
            sunAngleY + initialLocalEulerAngles.y,                  // 动态更新方位角
            initialLocalEulerAngles.z   // 保留原始滚转（通常 0）
        );

        // 应用旋转
        transform.localEulerAngles = newLocalEulerAngles;

        // 更新光照和环境
        sunLight.color = sunColorGradient.Evaluate(timeOfDay);
        sunLight.intensity = sunIntensityCurve.Evaluate(timeOfDay);

        Color ambient = ambientColorGradient.Evaluate(timeOfDay);
        RenderSettings.ambientSkyColor = ambient;
        RenderSettings.ambientEquatorColor = ambient;
        RenderSettings.ambientGroundColor = ambient;
    }

    // 默认梯度（同前）
    Gradient CreateDefaultSunGradient()
    {
        var g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new(Color.black, 0.0f),
                new(Color.red, 0.25f),
                new(Color.white, 0.5f),
                new(Color.red, 0.75f),
                new(Color.black, 1.0f)
            },
            new GradientAlphaKey[] { new(1f, 0f) }
        );
        return g;
    }

    Gradient CreateDefaultAmbientGradient()
    {
        var g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new(Color.black, 0.0f),
                new(Color.gray * 0.2f, 0.25f),
                new(Color.gray, 0.5f),
                new(Color.gray * 0.2f, 0.75f),
                new(Color.black, 1.0f)
            },
            new GradientAlphaKey[] { new(1f, 0f) }
        );
        return g;
    }
}