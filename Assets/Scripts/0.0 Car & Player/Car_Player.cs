using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Player : MonoBehaviour
{
    public WheelCollider FRWheel, FLWheel;
    private float newAngle = 0;

    public Transform TTarget;
    public Renderer RTarget;

    public float C2T;

    void FixedUpdate()
    {
        Drive();
        Check();
    }

    void Drive()
    {
        float steerAngle = Input.GetAxisRaw("Horizontal") * 1f;

        newAngle += steerAngle;
        newAngle = Mathf.Clamp(newAngle, -45f, 45f);
        FRWheel.steerAngle = newAngle;
        FLWheel.steerAngle = newAngle;

        float MoterTorque = Input.GetAxisRaw("Vertical") * 20f;

        FRWheel.motorTorque = MoterTorque;
        FLWheel.motorTorque = MoterTorque;

    }

    void Check()
    {
        C2T = Vector3.Distance(TTarget.localPosition, this.transform.localPosition);
        
        if (C2T <= 0.5f)
        {
            Targetreset();
        }
        if (this.transform.localPosition.y < -1f)
        {
            Selfreset();
        }
    }
    void Targetreset()
    {
        TTarget.position = new Vector3( Random.Range(8f, -8f), 0.5f, Random.Range(8f, -8f) );
    }

    void Selfreset()
    {
        this.transform.localPosition = new Vector3(Random.Range(8f, -8f), 1f, Random.Range(8f, -8f));
    }
}
