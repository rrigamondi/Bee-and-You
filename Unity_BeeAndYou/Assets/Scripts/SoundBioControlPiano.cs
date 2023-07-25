using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBioControlPiano : MonoBehaviour
{
    public GameObject dataSource;
    private float timer = 0f;
    public GameObject natureObject;
    private AudioSource PianoaudioSource;
    private float volumeChangeAmount = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        PianoaudioSource = natureObject.GetComponent<AudioSource>();
        Debug.Log("orginalPianoVolume:" + PianoaudioSource.volume);
    }

    // Update is called once per frame
    void Update()
    {
        // 计时器累加
        timer += Time.deltaTime;

        // 如果计时器超过5秒
        if (timer >= 5f)
        {
            // 计算集合中的峰值数量
            var peakPiano = dataSource.GetComponent<SoundBioControl>().peakCount;


            if (peakPiano != -1)
            {

                // 如果峰值大于4个
                if (peakPiano > 6)
                {
                    IncreaseVolume();
                    Debug.Log("PianoVolume:" + PianoaudioSource.volume);
                }
                else
                {
                    DecreaseVolume();
                    Debug.Log("PianoVolume:" + PianoaudioSource.volume);
                }
                // 重置计时器和数据集合
                timer = 0f;

            }

        }
    }


    void IncreaseVolume()
    {
        if (PianoaudioSource.volume + volumeChangeAmount <= 1f)
        {
            PianoaudioSource.volume += volumeChangeAmount;
        }
        else
        {
            PianoaudioSource.volume = 1f;
        }
    }

    void DecreaseVolume()
    {
        if (PianoaudioSource.volume - volumeChangeAmount >= 0f)
        {
            PianoaudioSource.volume -= volumeChangeAmount;
        }
        else
        {
            PianoaudioSource.volume = 0f;
        }
    }
}
