using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelsMovement : MonoBehaviour
{
    public WheelCollider TWheel;

    private Vector3 WPosition = new Vector3();
    private Quaternion WRotation = new Quaternion();

    private void Update()
    {
        TWheel.GetWorldPose(out WPosition, out WRotation);
        transform.position = WPosition;
        transform.rotation = WRotation;
    }
}
