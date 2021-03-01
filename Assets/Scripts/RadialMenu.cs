using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    public GameObject radialMenu;
    public GameObject cubeSection, sphereSection, cuboidSection, cylinderSection;
    public GameObject rHand;

    private Vector2 moveInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Only do radial menu functions if it's active (if user is showing menu)
        if (radialMenu.activeInHierarchy)
        {
            // Find in what segment of the menu the user's hand is in (relative to the center of the menu)
            // can try rhand.x - radialMenu.x to get relative to center of menu?
            moveInput.x = rHand.transform.position.x - (Screen.width / 2f);
            moveInput.y = rHand.transform.position.y - (Screen.height / 2f);
            // Make sure moveInput is between 0 and 1 
            moveInput.Normalize();
        }
    }
}
