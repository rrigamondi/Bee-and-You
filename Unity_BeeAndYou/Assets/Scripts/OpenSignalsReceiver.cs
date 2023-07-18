using System;
using UnityEngine;

public class OpenSignalsReceiver : MonoBehaviour
{
    private bool isConnected = false;
    private BitalinoReceiver bitalinoReceiver;

    private void Start()
    {
        // ��ʼ��BitalinoReceiver
        bitalinoReceiver = new BitalinoReceiver("2.2.1");
        bitalinoReceiver.OnConnected += OnConnected;
        bitalinoReceiver.OnDataReceived += OnDataReceived;

        // ���ӵ�OpenSignals
        bitalinoReceiver.Connect();
    }

    private void OnConnected(object sender, EventArgs e)
    {
        isConnected = true;
        Debug.Log("Connected to OpenSignals");
    }

    private void OnDataReceived(object sender, BitalinoDataEventArgs e)
    {
        // ��ӡ���յ���ÿ����ֵ
        foreach (float value in e.Data)
        {
            Debug.Log("Received data from OpenSignals: " + value);
        }
    }

    private void Update()
    {
        // ģ���������
        if (isConnected)
        {
            // ��ȡģ�����ݣ�ʵ������¸���Bitalino (r)evolution��ʵ�ʽӿ�����ȡ����
            BitalinoData data = bitalinoReceiver.GetSimulatedData();
            if (data != null)
            {
                // �������ݽ����¼�
                bitalinoReceiver.InvokeDataReceived(data);
            }
        }
    }

    private void OnDestroy()
    {
        // �Ͽ����Ӳ��ͷ���Դ
        bitalinoReceiver.Disconnect();
        bitalinoReceiver.OnConnected -= OnConnected;
        bitalinoReceiver.OnDataReceived -= OnDataReceived;
        bitalinoReceiver.Dispose();
    }
}

public class BitalinoDataEventArgs : EventArgs
{
    public float[] Data { get; private set; }

    public BitalinoDataEventArgs(float[] data)
    {
        Data = data;
    }
}

public class BitalinoReceiver
{
    public event EventHandler OnConnected;
    public event EventHandler<BitalinoDataEventArgs> OnDataReceived;

    private string version;

    public BitalinoReceiver(string version)
    {
        this.version = version;
    }

    public void Connect()
    {
        // ���ӵ�OpenSignals���߼�
        // ʹ��ָ���İ汾�ţ�version
        // ����OnConnected�¼�
        OnConnected?.Invoke(this, EventArgs.Empty);
    }

    public BitalinoData GetSimulatedData()
    {
        // ģ���ȡBitalino (r)evolution���ݵ��߼�
        // ����ģ�����ݣ�ʵ������¸���Bitalino (r)evolution��ʵ�ʽӿ�����ȡ����
        return new BitalinoData(new float[] { 1.2f, 3.4f, 5.6f });
    }

    public void InvokeDataReceived(BitalinoData data)
    {
        // ����OnDataReceived�¼�������������
        OnDataReceived?.Invoke(this, new BitalinoDataEventArgs(data.Values));
    }

    public void Disconnect()
    {
        // �Ͽ����ӵ��߼�
    }

    public void Dispose()
    {
        // �ͷ���Դ���߼�
    }
}

public class BitalinoData
{
    public float[] Values { get; private set; }

    public BitalinoData(float[] values)
    {
        Values = values;
    }
}