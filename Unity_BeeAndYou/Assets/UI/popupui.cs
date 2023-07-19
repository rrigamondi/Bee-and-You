using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ModelInteraction : MonoBehaviour
{
    public GameObject popupWindow; // 

    private void Update()
    {
        // 
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            // 
            if (Physics.Raycast(ray, out hit))
            {
                // 
                if (hit.collider.gameObject == gameObject)
                {
                    ShowPopupWindow();
                }
            }
        }
    }

    void ShowPopupWindow()
    {
        popupWindow.SetActive(true);
    }
}








