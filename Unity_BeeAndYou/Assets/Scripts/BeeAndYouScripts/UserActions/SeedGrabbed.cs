using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGrabbed : MonoBehaviour
{
    public bool seedIsGrabbed = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Grabbed()
    {
      seedIsGrabbed = true;
      GetComponent<Rigidbody>().useGravity = true;
    }
}
