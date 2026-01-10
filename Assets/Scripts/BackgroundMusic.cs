// 可选：淡入淡出脚本（避免突兀开始）
using System.Collections;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public float fadeInTime = 2f;
    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
        audioSource.Play();
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeInTime)
        {
            audioSource.volume = Mathf.Lerp(0f, 0.5f, t / fadeInTime);
            t += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 0.5f;
    }
}
