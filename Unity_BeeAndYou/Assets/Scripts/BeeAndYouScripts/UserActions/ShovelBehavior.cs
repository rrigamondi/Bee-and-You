using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShovelBehavior : MonoBehaviour
{
    private Transform groundCollider;
    private Collider groundColliderComponent;
    private Vector3 groundColliderPosition;
    bool shovelStraight;

    private groundCollisionCheck groundCollisionChecker;
    private bool groundCollisionBool;

    private Rigidbody shovelRB;
    public float velocityThreshold;
    private float shovelSpeed;

    public GameObject holePrefab;

    private XRGrabInteractable grabInteractable;
    private XRBaseInteractable baseInteractable;
    private bool grabbed = false;

    void Start()
    {
        // Create an instance of the groundCollisionCheck script and store it in the groundCollisionChecker variable
        groundCollisionChecker = new groundCollisionCheck();
        // Get the first child of this GameObject
        groundCollider = transform.GetChild(0);
        groundColliderComponent = groundCollider.GetComponent<Collider>();

        shovelRB = GetComponent<Rigidbody>();

        // Get the XRGrabInteractable component of the GameObject
        grabInteractable = GetComponent<XRGrabInteractable>();
        baseInteractable = GetComponent<XRBaseInteractable>();

        baseInteractable.onHoverEntered.AddListener(OnHoverStarted);
        baseInteractable.onHoverExited.AddListener(OnHoverEnded);

        // Subscribe to the grab events
        grabInteractable.onSelectEntered.AddListener(OnGrab);
        grabInteractable.onSelectExited.AddListener(OnUngrab);
    }

    void Update()
    {
        // Get the position of the child
        //groundColliderPosition = groundCollider.transform.position;
        //Debug.Log(groundColliderPosition);
        Vector3 shovelRotation = transform.rotation.eulerAngles;

        if (shovelRotation.x > -20.0f && shovelRotation.x < 20.0f)
        {
            shovelStraight = true;
        }
        else
        {
            shovelStraight = false;
        }

        // Get the value of the groundCollision variable from the groundCollisionCheck script
        groundCollisionBool = groundCollisionChecker.groundCollision;

        if (grabbed)
        {
            //Debug.Log(grabbed);
            shovelSpeed = shovelRB.velocity.y;
            //Debug.Log("shovelSpeed " + shovelRB.velocity + " velocityThreshold " + velocityThreshold);

            if (shovelStraight == true && shovelSpeed < velocityThreshold)
            {
                //Debug.Log("heyy im diggin here" + " | shovelSpeed " + shovelSpeed + " velocityThreshold " + velocityThreshold + " | groundCollPosition " + groundColliderPosition);
                Instantiate(holePrefab, groundColliderPosition, Quaternion.identity);
            }
        }
    }

    void OnGrab(XRBaseInteractor interactor)
    {
        grabbed = true;

        baseInteractable.onHoverEntered.RemoveListener(OnHoverStarted);
        baseInteractable.onHoverExited.RemoveListener(OnHoverEnded);
        baseInteractable.onHoverEntered.AddListener(OnHoverUpdated);
    }

    private void OnHoverStarted(XRBaseInteractor interactor)
{
    baseInteractable.onHoverEntered.RemoveListener(OnHoverStarted);
    baseInteractable.onHoverExited.RemoveListener(OnHoverEnded);
    baseInteractable.onHoverEntered.AddListener(OnHoverUpdated);
}

private void OnHoverEnded(XRBaseInteractor interactor)
{
    baseInteractable.onHoverEntered.RemoveListener(OnHoverUpdated);
    baseInteractable.onHoverEntered.AddListener(OnHoverStarted);
}


    private void OnHoverUpdated(XRBaseInteractor interactor)
    {
       // Get the current position of the child object in the local space of the parent object
       Vector3 childLocalPosition = groundCollider.localPosition;

       // Get the current position of the parent object in world space
       Vector3 parentWorldPosition = transform.position;

       // Calculate the current position of the child object in world space
       Vector3 groundColliderPosition = parentWorldPosition + childLocalPosition;

       // Use the childWorldPosition vector for further processing, such as updating the position of another object
       Debug.Log("Child position: " + groundColliderPosition);
       Instantiate(holePrefab, groundColliderPosition, Quaternion.identity);

       shovelSpeed = shovelRB.velocity.y;
       if (shovelStraight == true && shovelSpeed < velocityThreshold)
       {
           Debug.Log("heyy im diggin here" + " | shovelSpeed " + shovelSpeed + " velocityThreshold " + velocityThreshold + " | groundCollPosition " + groundColliderPosition);
           Instantiate(holePrefab, groundColliderPosition, Quaternion.identity);
       }
     }

    void OnUngrab(XRBaseInteractor interactor)
    {
        grabbed = false;
    }
}


// Holes position doesn't match groundCollider's actual position
// Collider itself doesn't call trigger enter/exit/stay
// Not sure which axis should be used for speed 
