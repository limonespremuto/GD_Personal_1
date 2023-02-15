using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineBase : MonoBehaviour, IEngine
{
    [Header ("Stats")]
    public float ThrusterPower = 10f;
    [Tooltip("The power of the engine at this moment")]
    public float CurrentPowerMP = 1f;

    [Header("setup")]
    protected Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetNewVector(Vector3 inputVector)
    {
        throw new System.NotImplementedException();
    }
    public void ApplyForceVector()
    {
        throw new System.NotImplementedException();
    }
}

interface IEngine
{
    public void GetNewVector(Vector3 inputVector);
    public void ApplyForceVector();
}