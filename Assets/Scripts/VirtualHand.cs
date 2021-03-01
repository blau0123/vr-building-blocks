using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualHand : MonoBehaviour
{
    // private bool useVirtualHand = false;
    private GameObject collidedObj;
    private Vector3 objVelocity;
    private Vector3 prePosition;
    private Vector3 currPosition;

    public GameObject rHandPrefab;
    // public GameObject worldRotation;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // If selecting object, then keep track of velocity of object
        if (collidedObj != null && collidedObj.tag == "Selectable" && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            currPosition = collidedObj.transform.position;
            if (prePosition.x == 0 && prePosition.y == 0 && prePosition.z == 0)
                prePosition = currPosition;
            objVelocity = (currPosition - prePosition) / Time.deltaTime;
            prePosition = currPosition;
        }

        // We were colliding with object, but we aren't selecting the object anymore, so stop selecting
        if (collidedObj != null && collidedObj.tag == "Selectable" && !OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            // When release the object, have it keep the velocity from when we were moving it
            collidedObj.GetComponent<Rigidbody>().velocity = objVelocity;
            collidedObj.GetComponent<Rigidbody>().isKinematic = false;
            collidedObj.transform.SetParent(null);
            collidedObj = null;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // Starting collision with object, so highlight the object if selectable
        if (other.gameObject.tag == "Selectable")
        {
            other.gameObject.GetComponent<Outline>().OutlineWidth = 5;

            // If colliding, check if the user is trying to edit the object (pick up and move)
            if (collidedObj == null && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
                PickUpObject(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // If colliding, check if the user is trying to edit the object (pick up and move)
        if (other.gameObject.tag == "Selectable" && collidedObj == null && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
            PickUpObject(other);
    }

    private void OnTriggerExit(Collider other)
    {
        // If we stop colliding with the selected object, should stop highlighting
        if (other.gameObject.tag == "Selectable")
            other.gameObject.GetComponent<Outline>().OutlineWidth = 0;
    }

    private void PickUpObject(Collider other)
    {
        collidedObj = other.gameObject;
        collidedObj.GetComponent<Rigidbody>().isKinematic = true;
        collidedObj.transform.SetParent(transform);
    }
}
