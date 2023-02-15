using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineLift : EngineBase, IEngine
{
    public float EvenPowerMP;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyForceVector();
    }

    new public void ApplyForceVector()
    {
        Vector3 ThrustDirection = transform.up * ThrusterPower * EvenPowerMP * CurrentPowerMP;
        rb.AddForceAtPosition(ThrustDirection, transform.position, ForceMode.Force);
    }
}
