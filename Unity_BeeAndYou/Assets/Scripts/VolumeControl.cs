using UnityEngine;

public class VolumeControl : MonoBehaviour
{
    public GameObject natureObject;
    private AudioSource audioSource;
    private float volumeChangeAmount = 0.1f;

    private void Start()
    {
        // ��ȡNature GameObject�ϵ�AudioSource���
        audioSource = natureObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        // ʹ�� "W" ����������
        if (Input.GetKeyDown(KeyCode.W))
        {
            IncreaseVolume();
            Debug.Log("Volume: " + audioSource.volume);
        }

        // ʹ�� "S" ����С����
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
            // ��������
            audioSource.volume += volumeChangeAmount;
            // ȷ���������������ֵ��1��
            audioSource.volume = Mathf.Clamp01(audioSource.volume);
        }
    }

    private void DecreaseVolume()
    {
        if (audioSource != null)
        {
            // ��С����
            audioSource.volume -= volumeChangeAmount;
            // ȷ��������С����Сֵ��0��
            audioSource.volume = Mathf.Clamp01(audioSource.volume);
        }
    }
}
