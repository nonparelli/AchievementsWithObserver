using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMe : MonoBehaviour {
    public float RotationSpeedX = 90.0f;
    public float RotationSpeedY = 0.0f;
    public float RotationSpeedZ = 0.0f;
    
    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(Time.deltaTime*RotationSpeedX, 
            Time.deltaTime*RotationSpeedY, Time.deltaTime*RotationSpeedZ);
    }
}
