using UnityEngine;


[ExecuteAlways]
public class DayNightCycle : MonoBehaviour
{
    public float dayLengthInSeconds = 120f;
    public Gradient sunColorGradient;
    public AnimationCurve sunIntensityCurve;
    public Gradient ambientColorGradient;

    [SerializeField, Range(0, 1f)] private float timeOfDay;

    private Light sunLight;
    private float startTime;
    private Vector3 initialLocalEulerAngles; // 记录初始局部欧拉角

    public float TotalRange = 360f;

    public float sunIntensityMax = 10f;

    public bool autoElapsed = false;
    
    void Start()
    {
        sunLight = GetComponentInChildren<Light>();
        startTime = Time.time;
        
        initialLocalEulerAngles = transform.localEulerAngles;
    }

    void Update()
    {
        if (autoElapsed)
        {
            timeOfDay = Mathf.Repeat((Time.time - startTime) / dayLengthInSeconds, 1f);   
        }
        
        UpdateLight();
    }
    
    public void UpdateLight()
    {
        float sunAngleZ = -timeOfDay * TotalRange;

        Vector3 newLocalEulerAngles = new Vector3(
            initialLocalEulerAngles.x,  
            initialLocalEulerAngles.y,
            sunAngleZ + initialLocalEulerAngles.z 
        );
        transform.localEulerAngles = newLocalEulerAngles;

        sunLight.color = sunColorGradient.Evaluate(timeOfDay);
        sunLight.intensity = Mathf.Pow(10f, sunIntensityCurve.Evaluate(timeOfDay)) * sunIntensityMax;

        Color ambient = ambientColorGradient.Evaluate(timeOfDay);
        RenderSettings.ambientSkyColor = ambient;
        RenderSettings.ambientEquatorColor = ambient;
        RenderSettings.ambientGroundColor = ambient;
    }
    
}