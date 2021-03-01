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
    private Vector3 prePosSlider;
    private Vector3 currPosSlider;
    private bool movingSlider = false;
    // Set to true if moving an object, so can't move 2 objects at once
    private bool movingObject = false;

    public GameObject rHandPrefab;
    public GameObject gravitySlider;
    // public GameObject worldRotation;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // If selecting object, then keep track of velocity of object
        if (movingObject && collidedObj != null && collidedObj.tag == "Selectable" && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            currPosition = collidedObj.transform.position;
            if (prePosition.x == 0 && prePosition.y == 0 && prePosition.z == 0)
                prePosition = currPosition;
            objVelocity = (currPosition - prePosition) / Time.deltaTime;
            prePosition = currPosition;
        }

        // We were colliding with object, but we aren't selecting the object anymore, so stop selecting
        if (movingObject && collidedObj != null && collidedObj.tag == "Selectable" && !OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            // When release the object, have it keep the velocity from when we were moving it
            collidedObj.GetComponent<Rigidbody>().velocity = objVelocity;
            prePosition = new Vector3(0, 0, 0);

            collidedObj.GetComponent<Rigidbody>().isKinematic = false;
            collidedObj.transform.SetParent(null);
            collidedObj = null;

            movingObject = false;
        }

        if (movingSlider && !OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            // Done moving slider, so reset the position tracking
            movingSlider = false;
            prePosSlider = new Vector3(0, 0, 0);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Outline>() != null)
            other.gameObject.GetComponent<Outline>().OutlineWidth = 5;

        // Starting collision with object, so highlight the object if selectable
        if (!movingObject && other.gameObject.tag == "Selectable")
        {
            // If colliding, check if the user is trying to edit the object (pick up and move)
            if (collidedObj == null && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
                PickUpObject(other);
        }

        // If start collision with the gravity slider, keep move slider by same amount as hand
        if (other.gameObject.tag == "Slider" && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            movingSlider = true;

            // On trigger enter, we would've just started to slide
            prePosSlider = transform.position;
            currPosSlider = transform.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // If colliding, check if the user is trying to edit the object (pick up and move)
        if (!movingObject && other.gameObject.tag == "Selectable" && collidedObj == null && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
            PickUpObject(other);

        // If start collision with the gravity slider, keep move slider by same amount as hand
        if (other.gameObject.tag == "Slider" && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            movingSlider = true;

            currPosSlider = transform.position;
            // On trigger enter, we would've just started to slide, so previous pos = curr pos
            if (prePosSlider.x == 0 && prePosSlider.y == 0 && prePosSlider.z == 0)
                prePosSlider = currPosSlider;

            GravitySlider sliderScript = gravitySlider.GetComponent<GravitySlider>();
            sliderScript.UpdateGravitySlider(currPosSlider.x - prePosSlider.x);

            prePosSlider = currPosSlider;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If we stop colliding with the selected object, should stop highlighting
        if (other.gameObject.GetComponent<Outline>() != null)
            other.gameObject.GetComponent<Outline>().OutlineWidth = 0;

        // If trigger left, then reset position trackers
        if (other.gameObject.tag == "Slider")
            prePosSlider = new Vector3(0, 0, 0);
    }

    private void PickUpObject(Collider other)
    {
        movingObject = true;
        collidedObj = other.gameObject;
        collidedObj.GetComponent<Rigidbody>().isKinematic = true;
        collidedObj.transform.SetParent(transform);
    }
}
