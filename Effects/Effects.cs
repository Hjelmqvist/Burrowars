using Cinemachine;
using System.Collections;
using UnityEngine;

/// <summary>
/// Methods for all special effects
/// </summary>
public class Effects : MonoBehaviour
{
    public static Effects Instance { get; protected set; }
    [SerializeField] CinemachineVirtualCamera vcam = null;
    CinemachineBasicMultiChannelPerlin noise = null;

    [SerializeField] float normalAmplitudeGain = 0.3f, normalFrequencyGain = 0.4f;
    Coroutine shakeCoroutine = null;

    bool isShaking = false;
    float currentFrequencyGain = 0;

    void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    /// <summary>
    /// Shakes the camera
    /// </summary>
    /// <param name="amplitudeGain">"Gain to apply to the amplitudes defined in the noise profile"</param>
    /// <param name="frequencyGain">"Factor to apply to the frequencies defined in the noise profile"</param>
    /// <param name="duration">How long it will take for it to back to normal</param>
    public void Shake(float amplitudeGain, float frequencyGain, float duration)
    {
        if (!isShaking || frequencyGain > noise.m_FrequencyGain)
        {
            currentFrequencyGain = frequencyGain;

            //Control so that only shake coroutine can be active
            if (shakeCoroutine != null)
                StopCoroutine(shakeCoroutine);
            shakeCoroutine = StartCoroutine(ShakeCoroutine(amplitudeGain, frequencyGain, duration));
        }
    }

    /// <summary>
    /// Starts shaking at full gain and then falls down to the default settings
    /// </summary>
    /// <param name="amplitudeGain">"Gain to apply to the amplitudes defined in the noise profile"</param>
    /// <param name="frequencyGain">"Factor to apply to the frequencies defined in the noise profile"</param>
    /// <param name="duration">How long it will take for it to back to normal</param>
    IEnumerator ShakeCoroutine(float amplitudeGain, float frequencyGain, float duration)
    {
        isShaking = true;
        float startTime = Time.time;
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            float f = 1 / (endTime - startTime);
            f = f * (Time.time - startTime);
            noise.m_AmplitudeGain = Mathf.Lerp(amplitudeGain, normalAmplitudeGain, f);
            noise.m_FrequencyGain = Mathf.Lerp(frequencyGain, normalFrequencyGain, f);
            yield return null;
        }

        noise.m_AmplitudeGain = normalAmplitudeGain;
        noise.m_FrequencyGain = normalFrequencyGain;
        isShaking = false;
    }

    /// <summary>
    /// Stops the time for pausedTime seconds
    /// </summary>
    /// <param name="pausedTime"></param>
    public void Pause(float pausedTime)
    {
        StartCoroutine(PauseCoroutine(pausedTime));
    }

    /// <summary>
    /// Sets timescale to 0 and then back to 1 after pausedTime seconds
    /// </summary>
    /// <param name="pausedTime">How many seconds to keep it paused</param>
    IEnumerator PauseCoroutine(float pausedTime)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(pausedTime);
        Time.timeScale = 1;
    }
}
