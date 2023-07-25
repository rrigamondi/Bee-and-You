using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBioControl : MonoBehaviour
{
    public GameObject dataSource;
    public int peakCount;
    private List<float> respDataList = new List<float>();
    private float timer = 0f;
    public GameObject natureObject;
    private AudioSource audioSource;
    private float volumeChangeAmount = 0.2f;
    private bool hasFirstValue = false;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = natureObject.GetComponent<AudioSource>();
        Debug.Log("orginalNatureVolume:" + audioSource.volume);
    }

    // Update is called once per frame
    void Update()
    {
        var data = dataSource.GetComponent<Hybrid8Test>().outputResp;
        respDataList.Add(data);

        if (!hasFirstValue)
        {
            // 检查respDataList中是否有值
            if (respDataList.Count > 0)
            {
                hasFirstValue = true;
            }
            return;
        }


        // 计时器累加
        timer += Time.deltaTime;

        // 如果计时器超过5秒
        if (timer >= 5f)
        {
            // 计算集合中的峰值数量
            int peakCount = CountPeaks(respDataList);
            Debug.Log("peak:" + peakCount);

            if(respDataList.Count > 1)
            {

                // 如果峰值多于3个
                if (peakCount < 6)
                {
                    IncreaseVolume();
                    Debug.Log("NatureVolume:" + audioSource.volume);
                }
                else
                {
                    DecreaseVolume();
                    Debug.Log("NatureVolume:" + audioSource.volume);
                }
                // 重置计时器和数据集合
                timer = 0f;
                respDataList.Clear();

            }
           
        }
    }

    int CountPeaks(List<float> dataList)
    {
        int peakCount = 0;
        for (int i = 1; i < dataList.Count - 1; i++)
        {
            for(int j=1; j < 30; j++)
            {

                if (dataList[i] > dataList[i - j] && dataList[i] < dataList[i + j])
                {
                    peakCount++;
                }

            }
        }
        return peakCount;
    }

    void IncreaseVolume()
    {
        if (audioSource.volume + volumeChangeAmount <= 1f)
        {
            audioSource.volume += volumeChangeAmount;
        }
        else
        {
            audioSource.volume = 1f;
        }
    }

    void DecreaseVolume()
    {
        if (audioSource.volume - volumeChangeAmount >= 0f)
        {
            audioSource.volume -= volumeChangeAmount;
        }
        else
        {
            audioSource.volume = 0f;
        }
    }
}
