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
            // ���respDataList���Ƿ���ֵ
            if (respDataList.Count > 0)
            {
                hasFirstValue = true;
            }
            return;
        }


        // ��ʱ���ۼ�
        timer += Time.deltaTime;

        // �����ʱ������5��
        if (timer >= 5f)
        {
            // ���㼯���еķ�ֵ����
            int peakCount = CountPeaks(respDataList);
            Debug.Log("peak:" + peakCount);

            if(respDataList.Count > 1)
            {

                // �����ֵ����3��
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
                // ���ü�ʱ�������ݼ���
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
