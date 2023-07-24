using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputMessageMove : MonoBehaviour
{
    public GameObject gameobject;
    public GameObject positionIn;
    public GameObject positionOut;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveIn()
    {
      gameobject.GetComponent<RectTransform>().localPosition = positionIn.GetComponent<RectTransform>().localPosition;
    }

    public void MoveOut()
    {
      gameobject.GetComponent<RectTransform>().localPosition = positionOut.GetComponent<RectTransform>().localPosition;
    }
}
