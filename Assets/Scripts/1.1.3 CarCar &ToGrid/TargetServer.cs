using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetServer : MonoBehaviour
{
    public Transform[] tTargets = new Transform[4];
    public Transform[] tCar = new Transform[4];
    public Transform[] tGrids = new Transform[10];

    private bool[] Info = new bool[10];
    private bool isFull = false;
    private float C2T;
    private int R;
    void Start()
    {
        for (int i=0; i<10; i++)
        {
            Info[i] = false;
        }
        for (int i = 0; i < 4; i++)
        {
            ResetMe(i);
        }

    }

    void TARGETRESET()
    {
        isFull = Gridcheck();
        if (isFull)
        {
            Debug.Log("Grid is Full!");
            for (int i=0; i<10; i++)
            {
                Info[i] = false;
            }
        }

        for (int i=0; i<4; i++)
        {
            C2T = Vector3.Distance(tCar[i].localPosition, tTargets[i].localPosition);
            if (C2T < 0.5f)
            {
                Debug.Log("Reset " + i);
                ResetMe(i);
            }
        }
    }

    void ResetMe(int n)
    {
        do
        {
            R = Random.Range(0, tGrids.Length);
        }while (Info[R] == true);

        Info[R] = true;
        Debug.Log("Target" + n + "tp to Grid" + R);
        tTargets[n].localPosition = tGrids[R].localPosition;
    }
    bool Gridcheck()
    {
        bool r = true;
        for (int i = 0; i < 10; i++)
        {
            if (Info[i] == false)
            {
                r = false;
            }
        }
        return (r);
    }
}
