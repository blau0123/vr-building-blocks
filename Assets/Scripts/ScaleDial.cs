using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleDial : MonoBehaviour
{
    private Vector3 initOrientation;
    private Vector3 currOrientation;

    // Start is called before the first frame update
    void Start()
    {
        // The initial orientation of the scale dial is the orientation at the start
        initOrientation = transform.rotation.eulerAngles;
        currOrientation = initOrientation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDialOrientation(float yAxisRotationAmt)
    {
        // If the difference between init and curr orientation is 360 degrees, then don't let move scale
        // if (currOrientation.y - initOrientation.y >= 360)
        //     return;

        // Rotate the dial
        transform.RotateAround(transform.position, transform.up, yAxisRotationAmt);
        currOrientation = transform.rotation.eulerAngles;
    }
}
