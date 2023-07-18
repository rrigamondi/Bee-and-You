using System;
using UnityEngine;

public class OpenSignalsReceiver : MonoBehaviour
{
    private bool isConnected = false;
    private BitalinoReceiver bitalinoReceiver;

    private void Start()
    {
        // 初始化BitalinoReceiver
        bitalinoReceiver = new BitalinoReceiver("2.2.1");
        bitalinoReceiver.OnConnected += OnConnected;
        bitalinoReceiver.OnDataReceived += OnDataReceived;

        // 连接到OpenSignals
        bitalinoReceiver.Connect();
    }

    private void OnConnected(object sender, EventArgs e)
    {
        isConnected = true;
        Debug.Log("Connected to OpenSignals");
    }

    private void OnDataReceived(object sender, BitalinoDataEventArgs e)
    {
        // 打印接收到的每个数值
        foreach (float value in e.Data)
        {
            Debug.Log("Received data from OpenSignals: " + value);
        }
    }

    private void Update()
    {
        // 模拟接收数据
        if (isConnected)
        {
            // 获取模拟数据，实际情况下根据Bitalino (r)evolution的实际接口来获取数据
            BitalinoData data = bitalinoReceiver.GetSimulatedData();
            if (data != null)
            {
                // 触发数据接收事件
                bitalinoReceiver.InvokeDataReceived(data);
            }
        }
    }

    private void OnDestroy()
    {
        // 断开连接并释放资源
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
        // 连接到OpenSignals的逻辑
        // 使用指定的版本号：version
        // 触发OnConnected事件
        OnConnected?.Invoke(this, EventArgs.Empty);
    }

    public BitalinoData GetSimulatedData()
    {
        // 模拟获取Bitalino (r)evolution数据的逻辑
        // 返回模拟数据，实际情况下根据Bitalino (r)evolution的实际接口来获取数据
        return new BitalinoData(new float[] { 1.2f, 3.4f, 5.6f });
    }

    public void InvokeDataReceived(BitalinoData data)
    {
        // 触发OnDataReceived事件，并传递数据
        OnDataReceived?.Invoke(this, new BitalinoDataEventArgs(data.Values));
    }

    public void Disconnect()
    {
        // 断开连接的逻辑
    }

    public void Dispose()
    {
        // 释放资源的逻辑
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