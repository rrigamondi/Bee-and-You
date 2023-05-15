using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class BitalinoTest : MonoBehaviour
{
    //variabili
    private PluxDeviceManager pluxDevManager;

    // Class constants (CAN BE EDITED BY IN ACCORDANCE TO THE DESIRED DEVICE CONFIGURATIONS)
    [System.NonSerialized]
    public List<string> domains = new List<string>() { "BTH" };
    private int BitalinoPID = 1538;
    public int samplingRate = 100;
    public int resolution = 10;
    public string bitalinoMacAddress = "84:BA:20:AE:BB:FE";

    // Start is called before the first frame update
    void Start()
    {
        // Initialise object
        pluxDevManager = new PluxDeviceManager(ScanResults, ConnectionDone, AcquisitionStarted, OnDataReceived, OnEventDetected, OnExceptionRaised);

        // Important call for debug purposes by creating a log file in the root directory of the project.
        pluxDevManager.WelcomeFunctionUnity();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BitalinoStart(){
        Debug.Log("BITALINO START");

        // Search for PLUX devices
        pluxDevManager.GetDetectableDevicesUnity(domains);
        // Connect to the device selected in the Dropdown list.
        pluxDevManager.PluxDev(bitalinoMacAddress);

        // Initializing the sources array.
        List<PluxDeviceManager.PluxSource> pluxSources = new List<PluxDeviceManager.PluxSource>();
        if (pluxDevManager.GetProductIdUnity() == BitalinoPID)
        {
            // Starting a real-time acquisition from:
            // >>> BITalino [Channels A1 and A2 active]
            pluxDevManager.StartAcquisitionUnity(samplingRate, new List<int> { 1, 2 }, resolution);
        }

    }

    public void BitalinoSop(){
        Debug.Log("BITALINO STOP");

         // Stop the real-time acquisition.
        pluxDevManager.StopAcquisitionUnity();
        Debug.Log("Fine acquisizione");
        // Disconnect from the device.
        pluxDevManager.DisconnectPluxDev();
        Debug.Log("Bitalino disconnesso");
    }

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



    // CALLBACKS

    //callback che riceve la lista di PLUX devices trovata nello scan Bluetooth
    public void ScanResults(List<string> listDevices){
        if(listDevices.Count > 0){
            Debug.Log("Bitalino trovato");
        }
        else{
            Debug.Log("Nessun Bitalino connesso via Bluetooth");
        }
    }

    //callback invocato quando la connessione con un device PLUX viene stabilita
    //connectionStatus -> flag che indica la connessione del device
    public void ConnectionDone(bool connectionStatus){
        if (connectionStatus){
            Debug.Log("Bitalino connesso");
        }
        else{
            Debug.Log("Non è stato possibile connettersi con Bitalino");
        }
    }

    //callback invocato quando il passaggio di dati tra il PLUX device e il computer è iniziato
    //acquisitionStatus -> flag che indica l'inizio del passaggio di dati
    //exceptionRaised -> flag che indica un'eccezione
    public void AcquisitionStarted(bool acquisitionStatus, bool exceptionRaised = false, string exceptionMessage =""){
        if(acquisitionStatus){
            Debug.Log("Acquisizione dati iniziata");
        }
        else{
            Debug.Log(exceptionMessage);
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
            Debug.Log(exceptionDescription);
        }
    }

    // Callback that receives the data acquired from the PLUX devices that are streaming real-time data.
    // nSeq -> Number of sequence identifying the number of the current package of data.
    // data -> Package of data containing the RAW data samples collected from each active channel ([sample_first_active_channel, sample_second_active_channel,...]).
    public void OnDataReceived(int nSeq, int[] data)
    {
        // Show samples with a 1s interval.
        if (nSeq % samplingRate == 0)
        {
            // Show the current package of data.
            string outputString = "Acquired Data:\n";
            for (int j = 0; j < data.Length; j++)
            {
                outputString += data[j] + "\t";
            }

            // Show the values in the Donsole
            Debug.Log(outputString);
        }
    }

    // Callback that receives the events raised from the PLUX devices that are streaming real-time data.
    // pluxEvent -> Event object raised by the PLUX API.
    public void OnEventDetected(PluxDeviceManager.PluxEvent pluxEvent)
    {
        if (pluxEvent is PluxDeviceManager.PluxDisconnectEvent)
        {
            // Present an error message.
           Console.WriteLine(
                "The connection between the computer and the PLUX device was interrupted due to the following event: " +
                (pluxEvent as PluxDeviceManager.PluxDisconnectEvent).reason);

            // Securely stop the real-time acquisition.
            pluxDevManager.StopAcquisitionUnity(-1);

        }
        else if (pluxEvent is PluxDeviceManager.PluxDigInUpdateEvent)
        {
            PluxDeviceManager.PluxDigInUpdateEvent digInEvent = (pluxEvent as PluxDeviceManager.PluxDigInUpdateEvent);
            Console.WriteLine("Digital Input Update Event Detected on channel " + digInEvent.channel + ". Current state: " + digInEvent.state);
        }
    }
}
