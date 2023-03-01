using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineLift : EngineBase, IEngine
{
    [HideInInspector] public float EvenPowerMP;
    public float MaxCorrectionDistance;
    [Range(0f,1f)]public float CorrectionPowerMP;
    public float CutOffAngle = 25f;
    public float targetHeight;

    float COMDistanceMP;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        //targetHeight = rb.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CalculateDeltaToTarget();
        //ApplyForceVector();
    }

    new public void ApplyForceVector()
    {
        if (Vector3.Angle(transform.up, Vector3.up) < CutOffAngle)
        {
            DesiredVectorThrust = Vector3.up * ThrusterPower * Mathf.Clamp01( CurrentPowerMP  * EvenPowerMP - COMDistanceMP);

            rb.AddForceAtPosition(DesiredVectorThrust, transform.position, ForceMode.Force);
            //rb.AddForce(DesiredVectorThrust, ForceMode.Force);

            Debug.DrawRay(transform.position, DesiredVectorThrust / ThrusterPower * 100f, Color.red, Time.deltaTime);
            //Debug.Log("Lifting!");

        }
    }

    void CalculateDeltaToTarget()
    {   
        float COMDistanceDelta;
        float COMDistanceLocal;
        float COMDistanceTo;

        COMDistanceLocal = transform.localPosition.y - rb.centerOfMass.y;

        COMDistanceTo = transform.position.y - targetHeight /*- rb.worldCenterOfMass.y*/ - COMDistanceLocal;

        //calculates the distance to the center of mass.
        COMDistanceDelta = Mathf.Clamp(COMDistanceTo ,-MaxCorrectionDistance,MaxCorrectionDistance);

        COMDistanceMP = COMDistanceDelta * CorrectionPowerMP / MaxCorrectionDistance;
        //Debug.Log(COMDistanceMP);
    }

    new public void GetNewVector(Vector3 inputVector)
    {
        
    }
}
