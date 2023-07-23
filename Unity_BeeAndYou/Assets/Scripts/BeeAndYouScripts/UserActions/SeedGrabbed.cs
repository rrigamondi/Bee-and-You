using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGrabbed : MonoBehaviour
{

    public GameObject notification;

    public bool seedIsGrabbed = false;
    public bool firstGrab = true;

    public GameObject eventSys;

    public float notificationFirstAnimationSpeed = 1.0f;
    public Vector3 notificationEndPosition;

    bool animationActive = false;
    // Start is called before the first frame update
    void Start()
    {
      eventSys = GameObject.Find("EventSystem");
    }

    // Update is called once per frame
    void Update()
    {
      if(animationActive)
      {
        Vector3 direction = (notificationEndPosition - notification.transform.position).normalized;
        if (Vector3.Distance(notification.transform.position, notificationEndPosition) >= 0.1f)
        {
          notification.transform.position += direction * notificationFirstAnimationSpeed * Time.deltaTime;
        }
      }

    }

    public void Grabbed()
    {
      seedIsGrabbed = true;
      GetComponent<Rigidbody>().useGravity = true;

      transform.Rotate(-180,0,0);

      if (firstGrab == false)
      {
        notification.SetActive(false);
      }
      else
      {
        eventSys.GetComponent<SeedBehavior>().FirstGrab();

        animationActive = true;
        // var originalPos = transform.TransformPoint(notification.GetComponent<Transform>().position);
        // var step =  notificationFirstAnimationSpeed * Time.deltaTime;
        // transform.TransformPoint(notificationEndPosition);
        // notification.GetComponent<Transform>().position = Vector3.MoveTowards(originalPos, notificationEndPosition, step);
      }
      //Debug.Log(firstGrab + " / " + notification.GetComponent<Transform>().position);
    }

    public void Ungrabbed()
    {
      seedIsGrabbed = false;
      firstGrab = false;
    }
}
