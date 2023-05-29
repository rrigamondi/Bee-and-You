using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCollisionCheck : MonoBehaviour
{
  public bool groundCollision;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider collision)
    {
        Debug.Log("Hey");
        // Check if the other collider is a terrain collider
        if (collision.tag == "Terrain")
        {
            groundCollision = true;
            Debug.Log(groundCollision);
        }
    }
}
