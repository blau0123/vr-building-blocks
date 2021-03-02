using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Detect if the non-dominant hand is facing upwards. If it is, show the radial menu.
 */
public class DetectPalmUp : MonoBehaviour
{
    public GameObject radialMenu;
    public GameObject userHead;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check the orientation of the hand to determine if the palm is facing upwards
        Vector3 orientation = transform.rotation.eulerAngles;
        // If the y orientation is within 80 and 100 (upwards is 90), then show the menu
        if (orientation.z >= 50 && orientation.z <= 130)
        {
            radialMenu.SetActive(true);

            /* Make the radial menu always look at the user
            radialMenu.transform.LookAt(userHead.transform);
            // Rotate the arrow because lookat will make the arrow point up and rotate
            // Along the z axis (make the forward vector, not up, point towards target)
            radialMenu.transform.Rotate(Vector3.right * 90);
            */
        }
        else
            radialMenu.SetActive(false);
    }
}
