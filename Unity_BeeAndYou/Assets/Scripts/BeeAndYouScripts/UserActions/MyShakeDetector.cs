using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;
using System.Collections;
using System.Collections.Generic;

public class MyShakeDetector : MonoBehaviour
{
    public float shakeThreshold = 2.0f;
    public GameObject interactiveObject;

    private InputDevice controller;
    private bool isShaking = false;

    void Start()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, new List<InputDevice>());
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, devices);

        foreach (InputDevice device in devices)
        {
            if (device.name.Contains("YourControllerName"))
            {
                controller = device;
                break;
            }
        }

        StartCoroutine(ReadVelocity());
    }

    IEnumerator ReadVelocity()
    {
        while (true)
        {
            Vector3 angularVelocity;
            if (controller.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out angularVelocity))
            {
                Debug.Log("Angular Velocity: " + angularVelocity.magnitude);

                if (angularVelocity.magnitude > shakeThreshold && !isShaking)
                {
                    isShaking = true;
                   // interactiveObject.GetComponent<InteractiveScript>().TriggerAction();
                }
                else if (angularVelocity.magnitude < shakeThreshold && isShaking)
                {
                    isShaking = false;
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}
