using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySlider : MonoBehaviour
{
    private Vector3 centerPos;
    private Vector3 currPos;
    private float maxX;
    private float minX;

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0, -5.0f, 0);

        // The center pos of the gravity slider is it's pos at the very beginning
        centerPos = transform.position;
        currPos = centerPos;

        maxX = centerPos.x + 0.15f;
        minX = centerPos.x - 0.15f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Only care about how much the user moved while holding the slider in the X direction
    public void UpdateGravitySlider(float amountMovedX)
    {
        // Only allow slider to move if within the slider range
        float newX = currPos.x + amountMovedX;
        if (newX <= maxX && newX >= minX)
        {
            // Update the position of the slider
            currPos.x = newX;
            transform.position = currPos;

            // Update the gravity of all objects based on its distance from the centerPos
            float distFromCenter = Vector3.Distance(centerPos, currPos);
            if (centerPos.x < currPos.x)
                Physics.gravity = new Vector3(0, -5.0f + (distFromCenter * 30.0f), 0);
            else
                Physics.gravity = new Vector3(0, -5.0f - (distFromCenter * 30.0f), 0);
        }
    }
}
