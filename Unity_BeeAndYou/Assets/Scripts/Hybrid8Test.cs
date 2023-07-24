using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Hybrid8Test : MonoBehaviour
{
    // Class Variables
    private PluxDeviceManager pluxDevManager;

    public GameObject natureObject;
    private AudioSource audioSource;
    private float volumeChangeAmount = 0.1f;

    // GUI Objects.
    public Button ScanButton;

    public Button ConnectButton;
    public Button DisconnectButton;
    public Button StartAcqButton;
    public Button StopAcqButton;
    public Dropdown DeviceDropdown;
    public Dropdown SamplingRateDropdown;
    public Dropdown ResolutionDropdown;
    public Dropdown RedIntensityDropdown;
    public Dropdown InfraredIntensityDropdown;
    public Text OutputMsgText;

    // Class constants (CAN BE EDITED BY IN ACCORDANCE TO THE DESIRED DEVICE CONFIGURATIONS)
    [System.NonSerialized]
    public List<string> domains = new List<string>() { "BTH" };

    public int samplingRate = 100;

    private int Hybrid8PID = 517;
    private int BiosignalspluxPID = 513;
    private int BitalinoPID = 1538;
    private int MuscleBanPID = 1282;
    private int MuscleBanNewPID = 2049;
    private int CardioBanPID = 2050;
    private int BiosignalspluxSoloPID = 532;
    private int MaxLedIntensity = 255;

    public int outputResp;
    public int outputHeart;

    // 定义用于存储呼吸信号的列表
List<float> breathSignalData = new List<float>();
// 定义10秒的时间间隔
float detectionInterval = 10f;
float timer = 0f;
private int peakCount = 0;
float volumeIncreaseAmount = 0.5f; // 假设为0.1
float volumeDecreaseAmount = 0.5f; // 假设为0.1
private FlockController flockController;

    // Start is called before the first frame update
    private void Start()
    {
        // Initialise object
        pluxDevManager = new PluxDeviceManager(ScanResults, ConnectionDone, AcquisitionStarted, OnDataReceived, OnEventDetected, OnExceptionRaised);

         flockController = GameObject.Find("NameOfFlockControllerGameObject").GetComponent<FlockController>();

        // Important call for debug purposes by creating a log file in the root directory of the project.
        pluxDevManager.WelcomeFunctionUnity();

         flockController = GameObject.Find("NameOfFlockControllerGameObject").GetComponent<FlockController>();
    }

    // Update function, being constantly invoked by Unity.
    private void Update()
    { }

    // Method invoked when the application was closed.
    private void OnApplicationQuit()
    {
        try
        {
            // Disconnect from device.
            if (pluxDevManager != null)
            {
                pluxDevManager.DisconnectPluxDev();
                Console.WriteLine("Application ending after " + Time.time + " seconds");
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine("Device already disconnected when the Application Quit.");
        }
    }

    /**
     * =================================================================================
     * ============================= GUI Events ========================================
     * =================================================================================
     */

    // Method called when the "Scan for Devices" button is pressed.
    public void ScanButtonFunction()
    {
        // Search for PLUX devices
        pluxDevManager.GetDetectableDevicesUnity(domains);

        // Disable the "Scan for Devices" button.
        ScanButton.interactable = false;
    }

    // Method called when the "Connect to Device" button is pressed.
    public void ConnectButtonFunction()
    {
        // Disable Connect button.
        ConnectButton.interactable = false;

        // Connect to the device selected in the Dropdown list.
        pluxDevManager.PluxDev(DeviceDropdown.options[DeviceDropdown.value].text);
    }

    // Method called when the "Disconnect Device" button is pressed.
    public void DisconnectButtonFunction()
    {
        // Disconnect from the device.
        pluxDevManager.DisconnectPluxDev();

        // Reboot GUI elements state.
        RebootGUI();
    }

    // Method called when the "Start Acquisition" button is pressed.
    public void StartButtonFunction()
    {
        // Get the Sampling Rate and Resolution values.
        samplingRate = int.Parse(SamplingRateDropdown.options[SamplingRateDropdown.value].text);
        int resolution = int.Parse(ResolutionDropdown.options[ResolutionDropdown.value].text);

        // Initializing the sources array.
        List<PluxDeviceManager.PluxSource> pluxSources = new List<PluxDeviceManager.PluxSource>();

        // biosignalsplux Hybrid-8 device (3 sensors >>> 1 Analog + 2 Digital SpO2/fNIRS)
        if (pluxDevManager.GetProductIdUnity() == Hybrid8PID)
        {
            // Add the sources of the digital channels (CH1 and CH2).
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x03));
            pluxSources.Add(new PluxDeviceManager.PluxSource(2, 1, resolution, 0x03));

            // Define the LED Intensities of both sensors (CH1 and CH2) as: {RED, INFRARED}
            int redLedIntensity = (int)(int.Parse(RedIntensityDropdown.options[RedIntensityDropdown.value].text) * (MaxLedIntensity / 100f)); // A 8-bit value (0-255)
            int infraredLedIntensity = (int)(int.Parse(InfraredIntensityDropdown.options[InfraredIntensityDropdown.value].text) * (MaxLedIntensity / 100f)); // A 8-bit value (0-255)
            int[] ledIntensities = new int[2] { redLedIntensity, infraredLedIntensity };
            pluxDevManager.SetParameter(1, 0x03, ledIntensities);
            pluxDevManager.SetParameter(2, 0x03, ledIntensities);

            // Add the source of the analog channel (CH8).
            pluxSources.Add(new PluxDeviceManager.PluxSource(8, 1, resolution, 0x01));

            // Add the sources of the internal IMU channels (CH11 with 9 derivations [3xACC | 3xGYRO | 3xMAG] defined by the 0x01FF chMask).
            int imuPort = 11;
            pluxSources.Add(new PluxDeviceManager.PluxSource(imuPort, 1, resolution, 0x01FF));

            // Alternatively only some of the derivations can be activated.
            // >>> 3xACC (channel mask 0x0007)
            // pluxSources.Add(new PluxDeviceManager.PluxSource(imuPort, 1, resolution, 0x0007));
            // >>> 3xGYR (channel mask 0x0038)
            // pluxSources.Add(new PluxDeviceManager.PluxSource(imuPort, 1, resolution, 0x0038));
            // >>> 3xMAG (channel mask 0x01C0)
            // pluxSources.Add(new PluxDeviceManager.PluxSource(imuPort, 1, resolution, 0x01C0));
        }
        // biosignalsplux (2 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == BiosignalspluxPID)
        {
            // Starting a real-time acquisition from:
            // >>> biosignalsplux [CH1 and CH2 active]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01)); // CH1 | EDA
            pluxSources.Add(new PluxDeviceManager.PluxSource(2, 1, resolution, 0x01)); // CH2 | ECG

            // Add the sources of the digital channel (CH9 | fNIRS/SpO2).
            // pluxSources.Add(new PluxDeviceManager.PluxSource(9, 1, resolution, 0x03));

            // Define the LED Intensities of the CH9 sensor as: {RED, INFRARED}
            // int redLedIntensity = (int)(int.Parse(RedIntensityDropdown.options[RedIntensityDropdown.value].text) * (MaxLedIntensity / 100f)); // A 8-bit value (0-255)
            // int infraredLedIntensity = (int)(int.Parse(InfraredIntensityDropdown.options[InfraredIntensityDropdown.value].text) * (MaxLedIntensity / 100f)); // A 8-bit value (0-255)
            // int[] ledIntensities = new int[2] { redLedIntensity, infraredLedIntensity };
            // pluxDevManager.SetParameter(9, 0x03, ledIntensities);
        }
        // muscleBAN (7 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == MuscleBanPID)
        {
            // Starting a real-time acquisition from:
            // >>> muscleBAN [CH1 > EMG]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01));
            // >>> muscleBAN [CH2-CH4 > ACC | CH5-CH7 > MAG active]
            pluxSources.Add(new PluxDeviceManager.PluxSource(2, 1, resolution, 0x3F));
        }
        // muscleBAN v2 (7 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == MuscleBanNewPID)
        {
            // Starting a real-time acquisition from:
            // >>> muscleBAN [CH1 > EMG]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01));
            // >>> muscleBAN Virtual Port [CH2-CH4 > ACC | CH5-CH7 > MAG active]
            pluxSources.Add(new PluxDeviceManager.PluxSource(11, 1, resolution, 0x3F));
        }
        // cardioBAN (7 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == CardioBanPID)
        {
            // Starting a real-time acquisition from:
            // >>> cardioBAN [CH1 > ECG]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01));
            // >>> cardioBAN Virtual Port [CH2-CH4 > ACC | CH5-CH7 > MAG active]
            pluxSources.Add(new PluxDeviceManager.PluxSource(11, 1, resolution, 0x3F));
        }
        // biosignalsplux Solo (8 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == BiosignalspluxSoloPID)
        {
            // Starting a real-time acquisition from:
            // >>> biosignalsplux Solo [CH1 > MICRO]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01));
            // >>> biosignalsplux Solo [CH2 > CUSTOM]
            pluxSources.Add(new PluxDeviceManager.PluxSource(2, 1, resolution, 0x01));
            // >>> biosignalsplux Solo Virtual Port [CH3-CH5 > ACC | CH6-CH8 > MAG]
            pluxSources.Add(new PluxDeviceManager.PluxSource(11, 1, resolution, 0x3F));
        }

        // BITalino (2 Analog sensors)
        if (pluxDevManager.GetProductIdUnity() == BitalinoPID)
        {
            // Starting a real-time acquisition from:
            // >>> BITalino [Channels A2 and A5 active]
            pluxDevManager.StartAcquisitionUnity(samplingRate, new List<int> { 2, 5 }, 10);
        }
        else
        {
            // Start a real-time acquisition with the created sources.
            pluxDevManager.StartAcquisitionBySourcesUnity(samplingRate, pluxSources.ToArray());
        }
    }

    public void ProcessBreathSignalData(string outputString)
    {
        // 将呼吸信号值（outputString）添加到列表中
        breathSignalData.Add(float.Parse(outputString));
        // 每过 detectionInterval 秒进行一次峰值数量检测
        timer += Time.deltaTime;
        if (timer >= detectionInterval)
        {
            timer = 0f;
            // 计算峰值数量
            peakCount = CountPeaks(breathSignalData);
            //flockController.breathSignalPeakCount = peakCount;
            // 根据峰值数量调整音乐音量
            AdjustMusicVolume(peakCount);
            // 清空呼吸信号列表，准备下一次检测
            breathSignalData.Clear();
        }
    }
    int CountPeaks(List<float> signalData)
    {
        int peakCount = 0;
        for (int i = 1; i < signalData.Count - 1; i++)
        {
            if (signalData[i] > signalData[i - 1] && signalData[i] > signalData[i + 1])
            {
                peakCount++;
            }
        }
        return peakCount;
    }
    void AdjustMusicVolume(int peakCount)
    {
        // 根据游戏对象名字 "Nature" 获取 AudioSource 组件
       // audioSource = natureObject.GetComponent<AudioSource>();
        // 假设音乐的音量范围在 0~1 之间
       // float currentVolume = natureAudioSource.volume;
        if (peakCount < 3)
        {
            // 如果峰值数量小于 3 个，则将音量提高
            IncreaseVolume();
            Debug.Log("Volume:" + audioSource.volume);
        }
        else
        {
            // 如果峰值数量大于等于 3 个，则将音量降低
            DecreaseVolume();
            Debug.Log("Volume:" + audioSource.volume);
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

    // Method called when the "Stop Acquisition" button is pressed.
    public void StopButtonFunction()
    {
        // Stop the real-time acquisition.
        pluxDevManager.StopAcquisitionUnity();

        // Enable the "Start Acquisition" button and disable the "Stop Acquisition" button.
        StartAcqButton.interactable = true;
        StopAcqButton.interactable = false;
    }

    /**
     * =================================================================================
     * ============================= Callbacks =========================================
     * =================================================================================
     */

    // Callback that receives the list of PLUX devices found during the Bluetooth scan.
    public void ScanResults(List<string> listDevices)
    {
        // Enable the "Scan for Devices" button.
        ScanButton.interactable = true;

        if (listDevices.Count > 0)
        {
            // Update list of devices.
            DeviceDropdown.ClearOptions();
            DeviceDropdown.AddOptions(listDevices);

            // Enable the Dropdown and the Connect button.
            DeviceDropdown.interactable = true;
            ConnectButton.interactable = true;

            // Show an informative message about the number of detected devices.
            OutputMsgText.text = "Scan completed.\nNumber of devices found: " + listDevices.Count;
        }
        else
        {
            // Show an informative message stating the none devices were found.
            OutputMsgText.text = "Bluetooth device scan didn't found any valid devices.";
        }
    }

    // Callback invoked once the connection with a PLUX device was established.
    // connectionStatus -> A boolean flag stating if the connection was established with success (true) or not (false).
    public void ConnectionDone(bool connectionStatus)
    {
        if (connectionStatus)
        {
            // Disable some GUI elements.
            ScanButton.interactable = false;
            DeviceDropdown.interactable = false;
            ConnectButton.interactable = false;

            // Enable some generic GUI elements.
            if (pluxDevManager.GetProductIdUnity() != BitalinoPID)
            {
                ResolutionDropdown.interactable = true;
            }

            SamplingRateDropdown.interactable = true;
            StartAcqButton.interactable = true;
            DisconnectButton.interactable = true;

            // Enable some biosignalsplux Hybrid-8 specific GUI elements.
            if (pluxDevManager.GetProductIdUnity() == Hybrid8PID || pluxDevManager.GetProductIdUnity() == BiosignalspluxPID)
            {
                RedIntensityDropdown.interactable = true;
                InfraredIntensityDropdown.interactable = true;
            }
        }
        else
        {
            // Enable Connect button.
            ConnectButton.interactable = true;

            // Show an informative message stating the connection with the device was not established with success.
            OutputMsgText.text = "It was not possible to establish a connection with the device. Please, try to repeat the connection procedure.";
        }
    }

    // Callback invoked once the data streaming between the PLUX device and the computer is started.
    // acquisitionStatus -> A boolean flag stating if the acquisition was started with success (true) or not (false).
    // exceptionRaised -> A boolean flag that identifies if an exception was raised and should be presented in the GUI (true) or not (false).
    public void AcquisitionStarted(bool acquisitionStatus, bool exceptionRaised = false, string exceptionMessage = "")
    {
        if (acquisitionStatus)
        {
            // Enable the "Stop Acquisition" button and disable the "Start Acquisition" button.
            StartAcqButton.interactable = false;
            StopAcqButton.interactable = true;
        }
        else
        {
            // Present an informative message about the error.
            OutputMsgText.text = !exceptionRaised ? "It was not possible to start a real-time data acquisition. Please, try to repeat the scan/connect/start workflow." : exceptionMessage;

            // Reboot GUI.
            RebootGUI();
        }
    }

    // Callback invoked every time an exception is raised in the PLUX API Plugin.
    // exceptionCode -> ID number of the exception to be raised.
    // exceptionDescription -> Descriptive message about the exception.
    public void OnExceptionRaised(int exceptionCode, string exceptionDescription)
    {
        if (pluxDevManager.IsAcquisitionInProgress())
        {
            // Present an informative message about the error.
            OutputMsgText.text = exceptionDescription;

            // Reboot GUI.
            RebootGUI();
        }
    }

    // Callback that receives the data acquired from the PLUX devices that are streaming real-time data.
    // nSeq -> Number of sequence identifying the number of the current package of data.
    // data -> Package of data containing the RAW data samples collected from each active channel ([sample_first_active_channel, sample_second_active_channel,...]).
    public void OnDataReceived(int nSeq, int[] data)
     {
         // Show samples with a 1s interval.
         int subsamplingFactor = (int)(0.01 * samplingRate);
        if (nSeq % subsamplingFactor == 0)
         {
             // Show the current package of data.
             string outputString = ">";
             for (int j = 0; j < data.Length; j++)
             {
               // if (j==0)
               // {
               //   outputHeart = data[j];
               // }
               // else if (j==1)
               // {
               //   outputResp = data[j];
               // }
               outputHeart = data[0];
               outputResp = data[1];
               outputString += data[j] + "-";
             }
             // Show the values in the GUI.
             OutputMsgText.text = outputString;

             ProcessBreathSignalData(outputString);

             //File.AppendAllText("BITalinoLOG.txt", outputString); //Environment.NewLine
         }
          //Invoke("StopButtonFunction", 5);



     }
    // Callback that receives the events raised from the PLUX devices that are streaming real-time data.
    // pluxEvent -> Event object raised by the PLUX API.
    public void OnEventDetected(PluxDeviceManager.PluxEvent pluxEvent)
    {
        if (pluxEvent is PluxDeviceManager.PluxDisconnectEvent)
        {
            // Present an error message.
            OutputMsgText.text =
                "The connection between the computer and the PLUX device was interrupted due to the following event: " +
                (pluxEvent as PluxDeviceManager.PluxDisconnectEvent).reason;

            // Securely stop the real-time acquisition.
            pluxDevManager.StopAcquisitionUnity(-1);

            // Reboot GUI.
            RebootGUI();
        }
        else if (pluxEvent is PluxDeviceManager.PluxDigInUpdateEvent)
        {
            PluxDeviceManager.PluxDigInUpdateEvent digInEvent = (pluxEvent as PluxDeviceManager.PluxDigInUpdateEvent);
            Console.WriteLine("Digital Input Update Event Detected on channel " + digInEvent.channel + ". Current state: " + digInEvent.state);
        }
    }

    /**
     * =================================================================================
     * ========================== Auxiliary Methods ====================================
     * =================================================================================
     */

    // Auxiliary method used to reboot the GUI elements.
    public void RebootGUI()
    {
        ScanButton.interactable = true;
        ConnectButton.interactable = false;
        DisconnectButton.interactable = false;
        StartAcqButton.interactable = false;
        StopAcqButton.interactable = false;
        DeviceDropdown.interactable = false;
        SamplingRateDropdown.interactable = false;
        ResolutionDropdown.interactable = false;
        RedIntensityDropdown.interactable = false;
        InfraredIntensityDropdown.interactable = false;
    }
}
