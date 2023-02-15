using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public EngineBase[] AllEngines;
    public EngineBase[] LiftEngines;

    [Header("Auxiliary engines only")]
    public EngineBase[] LeftEgnines;
    public EngineBase[] RightEngines;
    public EngineBase[] FrontEngines;
    public EngineBase[] BackEgnines;

    [Header("Setup")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform CenterOfMass;

    //Internal variables
    private float EvenPowerMP;

    // Start is called before the first frame update
    void Start()
    {
        rb.centerOfMass = CenterOfMass.position;
    }

    // Update is called once per frame
    void Update()
    {
        EvenPowerMP = CalculateLiftPowerMP();
        ApplyEvenPower();
    }

    float CalculateLiftPowerMP()
    {
        float TotalPower = 0;
        float EngineEvenMP;
        float RequiredPower;

        foreach (EngineBase engine in LiftEngines)
        {
            TotalPower += engine.ThrusterPower * engine.CurrentPowerMP;
        }

        RequiredPower = rb.mass * -Physics.gravity.y;
        //Debug.Log(RequiredPower);

        EngineEvenMP = Mathf.Clamp01 (RequiredPower / TotalPower);
        return EngineEvenMP;
    }

    private void ApplyEvenPower()
    {
        foreach (EngineLift engine in LiftEngines)
        {
            engine.EvenPowerMP = EvenPowerMP;
        }
    }
}
