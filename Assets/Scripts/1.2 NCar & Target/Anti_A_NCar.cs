using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anti_A_NCar : Agent
{
    private Rigidbody rCar;
    private Transform tCar;
    public WheelCollider FRWheel, FLWheel;

    public Transform tTarget;//, tTarget0, tTarget1, tTarget2, tCar0, tCar1, tCar2;

    public float SteerAngle, MoterTorque;
    private float moterSpeed = 100f, steerSpeed = 2.5f, breakSpeed = 1000f;
    public float C2T;//, C2T0, C2T1, C2T2;
    private float oldC2T;

    private float moterSignal, steerSignal;
    public float R;

    //public bool isSUCCESS = false;

    public override void Initialize()
    {
        rCar = GetComponent<Rigidbody>();
        tCar = GetComponent<Transform>();
    }

    public override void OnEpisodeBegin()
    {
        MaxStep = 100000;
        selfStop();
        selfReset();
        targetReset();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //3*9+1 = 28
        sensor.AddObservation(tCar.localPosition); 
        //sensor.AddObservation(tCar0.localPosition);
        //sensor.AddObservation(tCar1.localPosition);
        //sensor.AddObservation(tCar2.localPosition);

        sensor.AddObservation(tTarget.localPosition);
        //sensor.AddObservation(tTarget0.localPosition);
        //sensor.AddObservation(tTarget1.localPosition);
        //sensor.AddObservation(tTarget2.localPosition);

        sensor.AddObservation(rCar.velocity);
        sensor.AddObservation(SteerAngle);

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if (vectorAction[0] == 2) { moterSignal = 1; }
        else { moterSignal = -vectorAction[0]; }

        if (vectorAction[1] == 2) { steerSignal = 1; }
        else { steerSignal = -vectorAction[1]; }



        // if (isSUCCESS == false) { Drive(); }
        Drive();
        Check();

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

        if (moterSignal == 0)
        {
            FRWheel.brakeTorque = breakSpeed;
            FLWheel.brakeTorque = breakSpeed;
        }
        else
        {
            FRWheel.brakeTorque = 0;
            FLWheel.brakeTorque = 0;
        }

    }

    void Check()
    {
        C2T = Vector3.Distance(tTarget.localPosition, this.transform.localPosition);
        R = Vector3.Angle(tCar.forward, tTarget.forward);
        //if (isSUCCESS == false)
        {
            if ((C2T <= 0.5f) & (rCar.velocity.z <= 0.5f) )//SUCCESS
            {
                selfStop();
                SetReward(100f);
                targetReset();
                //isSUCCESS = true;
            }
            if (tCar.localPosition.y < -1f) //FALL
            {
                EndEpisode();
            }
            if (Mathf.Abs(tCar.localEulerAngles.x - 180) < 178.5f) //FLIP
            {
                EndEpisode();
            }
            if (MaxStep < 0) //TIMES UP
            {
                EndEpisode();
            }

            if (oldC2T>C2T)
            {
                SetReward(0.01f);
            }
            if (Mathf.Abs(tCar.localPosition.x - tTarget.localPosition.x) < 0.5f)
            {
                SetReward(0.01f);
                if (Vector3.Angle(tCar.forward, tTarget.forward) < 0.5f)
                {
                    SetReward(0.01f);
                }
            }
        }

        oldC2T = C2T;
        MaxStep -= 1;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Car")
        {
            selfStop();
            selfReset();
            EndEpisode();
        }
    }

    void targetReset()
    {
        tTarget.localPosition = new Vector3( Random.Range(8f, -8f), 0.1f, Random.Range(8f, -8f) );
    }

    void selfReset()
    {
        tCar.localPosition = new Vector3(Random.Range(8f, -8f), 0.5f, Random.Range(8f, -8f));
        tCar.localEulerAngles = Vector3.zero;
    }
    void selfStop() 
    {
        FRWheel.motorTorque = 0f;
        FRWheel.steerAngle = 0f;
        FLWheel.motorTorque = 0f;
        FLWheel.steerAngle = 0f;

        FRWheel.brakeTorque = 1000000000000f;
        FLWheel.brakeTorque = 1000000000000f;

        rCar.velocity = Vector3.zero;
        rCar.angularVelocity = Vector3.zero;

        FRWheel.brakeTorque = 0f;
        FLWheel.brakeTorque = 0f;

    }
}
