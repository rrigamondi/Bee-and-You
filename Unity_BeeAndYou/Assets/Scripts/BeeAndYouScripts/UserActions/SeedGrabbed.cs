using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGrabbed : MonoBehaviour
{

    public GameObject notification;

    public bool seedIsGrabbed = false;
    bool firstGrab = true;

    public GameObject eventSys;
    // Start is called before the first frame update
    void Start()
    {
      eventSys = GameObject.Find("EventSystem");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Grabbed()
    {
      seedIsGrabbed = true;
      GetComponent<Rigidbody>().useGravity = true;
      notification.SetActive(false);

      transform.Rotate(-180,0,0);

      if (firstGrab)
      {
        eventSys.GetComponent<SeedBehavior>().FirstGrab();
        firstGrab = false;
      }
    }

    public void Ungrabbed()
    {
      seedIsGrabbed = false;
    }
}
