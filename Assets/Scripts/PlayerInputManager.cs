using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField, Tooltip("Internal")]
    private float targetHeight;
    [SerializeField] private float MaxHeight;

    [Header("Stabilizer")]
    public float stability = 0.3f;
    public float speed = 2.0f;


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

    private void Awake()
    {
        rb.centerOfMass = CenterOfMass.position;
        targetHeight = CenterOfMass.position.y;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        InputUpdate();
        ThrustEngine();
        EvenPowerMP = CalculateLiftPowerMP();
        ApplyEvenPower();
        StabilizeShip();
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

        EngineEvenMP = Mathf.Clamp01(RequiredPower / TotalPower);
        return EngineEvenMP;
    }

    private void ApplyEvenPower()
    {
        foreach (EngineLift engine in LiftEngines)
        {
            engine.EvenPowerMP = EvenPowerMP;
        }
    }

    private void InputUpdate()
    {

        if (Input.GetAxis("Vertical") != 0f)
        {
            ApplyEngineVector(Vector3.forward * Input.GetAxis("Vertical"), FrontEngines);
            ApplyEngineVector(Vector3.forward * Input.GetAxis("Vertical"), BackEgnines);
        }

        if (Input.GetAxis("Horizontal") != 0f)
        {
            ApplyEngineVector(Vector3.right * Input.GetAxis("Horizontal"), FrontEngines);
            ApplyEngineVector(Vector3.right * Input.GetAxis("Horizontal"), BackEgnines);
        }

        if (Input.GetAxis("Steer") != 0f)
        {
            // frontal engine get input towards right back to left
            ApplyEngineVector(Vector3.right * Input.GetAxis("Steer"), FrontEngines);
            ApplyEngineVector(Vector3.left * Input.GetAxis("Steer"), BackEgnines);

            // same whit left and right engines but whit forward and back
            ApplyEngineVector(Vector3.forward * Input.GetAxis("Steer"), LeftEgnines);
            ApplyEngineVector(Vector3.back * Input.GetAxis("Steer"), RightEngines);
        }

        targetHeight += Input.GetAxis("Ascend");
        targetHeight = Mathf.Clamp(targetHeight, 0, MaxHeight);

        foreach (EngineLift engine in LiftEngines)
        {
            engine.targetHeight = targetHeight;
        }
    }
    private void ApplyEngineVector(Vector3 vector3, EngineBase[] Engines)
    {
        foreach (IEngine Engine in Engines)
        {
            Engine.GetNewVector(vector3);
            //Debug.Log(vector3);
        }


    }

    private void ThrustEngine()
    {
        foreach (IEngine engine in AllEngines)
        {
            engine.ApplyForceVector();
        }
    }

    private void StabilizeShip()
    {

        Vector3 predictedUp = Quaternion.AngleAxis(rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / speed,rb.angularVelocity) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        rb.AddTorque(torqueVector* speed * speed);
    }
}
