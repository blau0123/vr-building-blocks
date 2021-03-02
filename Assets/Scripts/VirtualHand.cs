using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualHand : MonoBehaviour
{
    private GameObject collidedObj;
    private GameObject hoveredMenuItem;
    // Keeps track of the most recently selected object, so can unhighlight it when select new object
    private GameObject mostRecentSelected;
    private Vector3 objVelocity;
    private Vector3 prePosition;
    private Vector3 currPosition;
    // Set to true if moving an object, so can't move 2 objects at once
    private bool movingObject = false;

    // Slider
    private Vector3 prePosSlider;
    private Vector3 currPosSlider;
    private bool movingSlider = false;

    // Scale dial
    private Vector3 preRotDial;
    private Vector3 currRotDial;
    private bool rotatingDial = false;

    public GameObject rHandPrefab;
    public GameObject gravitySlider;

    // Prefabs for the shapes the user can create
    public GameObject cubePrefab;
    public GameObject spherePrefab;
    public GameObject cylinderPrefab;
    public GameObject cuboidPrefab;

    public Sprite highlightedMaterialImg;
    public Sprite normalMaterialImg;
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
        string objTag = other.gameObject.tag;
        // When hover one of the radial menu items, highlight it by changing material of the button
        if (hoveredMenuItem == null && (objTag == "SquareButton" || objTag == "SphereButton" || objTag == "CylinderButton" || objTag == "CuboidButton"))
        {
            hoveredMenuItem = other.gameObject;
            other.gameObject.GetComponent<Image>().sprite = highlightedMaterialImg;
        }

        // When hover the slider, want to highlight it so the user knows they are touching the slider
        if (objTag == "Slider")
            other.gameObject.GetComponent<Outline>().OutlineWidth = 5;

        // When hover any of the color buttons, highlight it so the user knows they are touching the color button
        if (objTag == "ColorButton")
            other.gameObject.GetComponent<Outline>().OutlineWidth = 5;

        if (objTag == "ScaleDial")
            other.gameObject.GetComponent<Outline>().OutlineWidth = 5;
    }
    
    private void OnTriggerStay(Collider other)
    {
        string objTag = other.gameObject.tag;
        // If hovering a menu item, then changed which item the user is hovering (Without ending the collision
        // with the menu), then need to update the hover
        if (hoveredMenuItem == null && (objTag == "SquareButton" || objTag == "SphereButton" || objTag == "CylinderButton" || objTag == "CuboidButton"))
        {
            hoveredMenuItem = other.gameObject;
            other.gameObject.GetComponent<Image>().sprite = highlightedMaterialImg;
        }

        // If colliding, check if the user is trying to edit the object (pick up and move)
        if (!movingObject && other.gameObject.tag == "Selectable" && collidedObj == null && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            // If selected object, and we had already previously selected an object, unhighlight the prevous object
            if (mostRecentSelected != null)
            {
                mostRecentSelected.GetComponent<Outline>().OutlineWidth = 0;
                mostRecentSelected = null;
            }

            PickUpObject(other);

            // Highlight the object when select it
            if (other.gameObject.GetComponent<Outline>() != null)
                other.gameObject.GetComponent<Outline>().OutlineWidth = 5;
        }

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

        // If the user has grabbed an object from the menu, then instantiate the given object
        // Check if collidedObj == null, so that after we created an object, we don't keep instantiating new ones
        if (collidedObj == null && other.gameObject.tag == "SquareButton" && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
            InstantiateAndPickUpObject("cube");

        if (collidedObj == null && other.gameObject.tag == "SphereButton" && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
            InstantiateAndPickUpObject("sphere");

        if (collidedObj == null && other.gameObject.tag == "CylinderButton" && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
            InstantiateAndPickUpObject("cylinder");

        if (collidedObj == null && other.gameObject.tag == "CuboidButton" && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
            InstantiateAndPickUpObject("cuboid");

        // If click on a color button, change the color of the selected object to be the color of the button
        if (mostRecentSelected != null && other.gameObject.tag == "ColorButton" && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
            mostRecentSelected.GetComponent<Renderer>().material.color = other.gameObject.GetComponent<Renderer>().material.color;

        // If rotating the scale dial, only check the user's hands y rotation and rotate the scale by that much
        if (other.gameObject.tag == "ScaleDial" && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            // Rotate the dial
            currRotDial = transform.rotation.eulerAngles;
            /* On trigger enter, we would've just started to rotate, so previous rot = curr rot
            if (preRotDial.x == 0 && preRotDial.y == 0 && preRotDial.z == 0)
                preRotDial = currRotDial;
            */
            if (!rotatingDial)
            {
                preRotDial = currRotDial;
                rotatingDial = true;
            }

            float rotationAmount = currRotDial.y - preRotDial.y;
            ScaleDial scaleDialScript = other.gameObject.GetComponent<ScaleDial>();
            if (scaleDialScript != null)
                scaleDialScript.UpdateDialOrientation(rotationAmount);

            // Scale the selected item
            if (mostRecentSelected != null)
            {
                float scaleAmount = rotationAmount * 0.01f;
                // If currRotDial.y is greater, then we are turning clockwise (increase scale)
                if (currRotDial.y > preRotDial.y)
                {
                    mostRecentSelected.transform.localScale += new Vector3(scaleAmount, scaleAmount, scaleAmount);
                }
                else
                {
                    mostRecentSelected.transform.localScale -= new Vector3(scaleAmount, scaleAmount, scaleAmount);
                }
            }

            preRotDial = currRotDial;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string objTag = other.gameObject.tag;

        // When stop hovering one of the menu items, then unhighlight the item
        if (objTag == hoveredMenuItem.tag)
        {
            hoveredMenuItem.GetComponent<Image>().sprite = normalMaterialImg;
            hoveredMenuItem = null;
        }

        // If not colliding with the dial anymore, reset the previous position tracker
        if (objTag == "ScaleDial")
        {
            other.gameObject.GetComponent<Outline>().OutlineWidth = 0;
            preRotDial = new Vector3(0, 0, 0);
            rotatingDial = false;
        }

        // If trigger left, then reset position trackers
        if (objTag == "Slider")
        {
            // Unhighlight the slider if we aren't colliding with it anymore
            other.gameObject.GetComponent<Outline>().OutlineWidth = 0;
            prePosSlider = new Vector3(0, 0, 0);
        }

        // If not colliding with the color buttons any more, unhighlight them
        if (objTag == "ColorButton")
            other.gameObject.GetComponent<Outline>().OutlineWidth = 0;
    }

    private void PickUpObject(Collider other)
    {
        movingObject = true;
        collidedObj = other.gameObject;
        collidedObj.GetComponent<Rigidbody>().isKinematic = true;
        collidedObj.transform.SetParent(transform);

        mostRecentSelected = collidedObj;
    }

    private void InstantiateAndPickUpObject(string type)
    {
        // If something was previously selected, then unhighlight it when creating new object
        if (mostRecentSelected != null)
        {
            mostRecentSelected.GetComponent<Outline>().OutlineWidth = 0;
            mostRecentSelected = null;
        }

        // Create new object at the hand's position
        GameObject objInst;

        if (type == "sphere")
            objInst = Instantiate(spherePrefab, transform.position, Quaternion.identity);
        else if (type == "cube")
            objInst = Instantiate(cubePrefab, transform.position, Quaternion.identity);
        else if (type == "cylinder")
            objInst = Instantiate(cylinderPrefab, transform.position, Quaternion.identity);
        else if (type == "cuboid")
            objInst = Instantiate(cuboidPrefab, transform.position, Quaternion.identity);
        else
            objInst = Instantiate(spherePrefab, transform.position, Quaternion.identity);

        // pick up the newly instantiated object
        movingObject = true;
        collidedObj = objInst;
        // Right when instantiate, want it to be highlighted (because we are selecting it)
        collidedObj.GetComponent<Outline>().OutlineWidth = 5;
        collidedObj.GetComponent<Rigidbody>().isKinematic = true;
        collidedObj.transform.SetParent(transform);

        mostRecentSelected = collidedObj;
    }
}
