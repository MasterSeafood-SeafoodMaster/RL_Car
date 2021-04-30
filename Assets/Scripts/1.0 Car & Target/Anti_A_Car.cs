using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anti_A_Car : Agent
{
    private Rigidbody rCar;
    private Transform tCar;
    public WheelCollider FRWheel, FLWheel;
    
    public Transform tTarget;

    public float SteerAngle, MoterTorque;
    private float moterSpeed = 100f, steerSpeed = 5f;
    public float C2T;

    private float moterSignal, steerSignal;

    private float PX, RY, oldPX, oldRy;

    public float R;

    public override void Initialize()
    {
        rCar = GetComponent<Rigidbody>();
        tCar = GetComponent<Transform>();
    }

    public override void OnEpisodeBegin()
    {
        MaxStep = 100000;
        selfReset();
        targetReset();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(tCar.localPosition);
        sensor.AddObservation(tTarget.localPosition);
        sensor.AddObservation(rCar.velocity);

        sensor.AddObservation(SteerAngle);

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if (vectorAction[0] == 2) { moterSignal = 1; }
        else { moterSignal = -vectorAction[0]; }

        if (vectorAction[1] == 2) { steerSignal = 1; }
        else { steerSignal = -vectorAction[1]; }

        PX = Mathf.Abs( tTarget.localPosition.x - tCar.localPosition.x );
        RY = Vector3.Angle(tTarget.localEulerAngles, tCar.localEulerAngles);

        Drive();
        Check();

        oldPX = PX;
        oldRy = RY;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxisRaw("Vertical");
        actionsOut[1] = -Input.GetAxisRaw("Horizontal");
    }

    void Drive()
    {
        SteerAngle += steerSignal * steerSpeed;

        SteerAngle = Mathf.Clamp(SteerAngle, -45f, 45f);
        FRWheel.steerAngle = SteerAngle;
        FLWheel.steerAngle = SteerAngle;

        MoterTorque = moterSignal * moterSpeed;

        FRWheel.motorTorque = MoterTorque;
        FLWheel.motorTorque = MoterTorque;

    }

    void Check()
    {
        C2T = Vector3.Distance(tTarget.localPosition, this.transform.localPosition);
        
        if (C2T <= 0.5f)
        {
            SetReward(1000f);
            targetReset();
        }
        if (tCar.localPosition.y < -1f)
        {
            EndEpisode();
        }
        if (Mathf.Abs(tCar.localEulerAngles.x-180) < 178.5f)
        {
           EndEpisode();
        }
        if (MaxStep < 0)
        {
            EndEpisode();
        }

        //SetReward((PX + RY - oldPX - oldRy)*0.01f);

        if (Mathf.Abs(tCar.localPosition.x - tTarget.localPosition.x) < 0.05f)
        {
            SetReward(0.01f);
            if (Mathf.Abs(tCar.localEulerAngles.y - tTarget.localEulerAngles.y) < 0.05f)
            {
                SetReward(0.01f);
            }
        }


        MaxStep -= 1;
    }
    void targetReset()
    {
        tTarget.localPosition = new Vector3( Random.Range(8f, -8f), 0.1f, Random.Range(8f, -8f) );
    }

    void selfReset()
    {
        rCar.velocity = Vector3.zero;
        rCar.angularVelocity = Vector3.zero;
       
        tCar.localPosition = new Vector3(Random.Range(8f, -8f), 0.5f, Random.Range(8f, -8f));
        tCar.localEulerAngles = Vector3.zero;
    }
}
