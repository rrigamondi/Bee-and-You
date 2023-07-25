using UnityEngine;

public class VolumeControl : MonoBehaviour
{
    public GameObject natureObject;
    private AudioSource audioSource;
    private float volumeChangeAmount = 0.1f;

    private void Start()
    {
        // 获取Nature GameObject上的AudioSource组件
        audioSource = natureObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        // 使用 "W" 键增加音量
        if (Input.GetKeyDown(KeyCode.W))
        {
            IncreaseVolume();
            Debug.Log("Volume: " + audioSource.volume);
        }

        // 使用 "S" 键减小音量
        if (Input.GetKeyDown(KeyCode.S))
        {
            DecreaseVolume();
            Debug.Log ("Volume: " + audioSource.volume);
        }
    }

    private void IncreaseVolume()
    {
        if (audioSource != null)
        {
            // 增加音量
            audioSource.volume += volumeChangeAmount;
            // 确保音量不超过最大值（1）
            audioSource.volume = Mathf.Clamp01(audioSource.volume);
        }
    }

    private void DecreaseVolume()
    {
        if (audioSource != null)
        {
            // 减小音量
            audioSource.volume -= volumeChangeAmount;
            // 确保音量不小于最小值（0）
            audioSource.volume = Mathf.Clamp01(audioSource.volume);
        }
    }
}
