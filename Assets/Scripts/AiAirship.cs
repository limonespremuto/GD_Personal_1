using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAirship : MonoBehaviour
{
    [Header("EnemyAI")]
    public Transform currentTarget;
    public float agroRange;
    public float stopRange;

    [SerializeField]
    private float targetHeight;
    [SerializeField]
    private float MaxHeight = 300f;

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

    #region Turrets
    [Header("Turrets")]
    [SerializeField]
    private Transform[] turretSpots;
    [SerializeField]
    private List<TurretScript> turrets;

    [SerializeField]
    LayerMask WorldWhitoutSelf;

    [SerializeField]
    LayerMask TargetLayer;
    #endregion
    //Internal variables

    private float EvenPowerMP;
    private void Awake()
    {
        rb.centerOfMass = CenterOfMass.localPosition;
        targetHeight = CenterOfMass.position.y;
        GetTurrets();
    }

    private void Update()
    {
        GiveTurretInput();
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
        
        if (Vector3.Distance(transform.position, currentTarget.position) <= agroRange)
        {
            Vector3 thrustDirection = (transform.position - currentTarget.position).normalized;
            if (Vector3.Distance(transform.position, currentTarget.position) <= stopRange)
            {
                thrustDirection = Vector3.zero;
            }
            ApplyEngineVector(thrustDirection, FrontEngines);

            float horizontallAngleDifference;
            bool IsRightOfMe = false;
            Vector3 planedTargetPosition = currentTarget.position;
            planedTargetPosition.y = 0f;
            planedTargetPosition = transform.InverseTransformPoint(planedTargetPosition);

            if (planedTargetPosition.x > 0f)
                IsRightOfMe = true;

            horizontallAngleDifference = Vector3.Angle(transform.forward, currentTarget.position);
            if (!IsRightOfMe)
            {
                horizontallAngleDifference = -horizontallAngleDifference;
            }

            if (horizontallAngleDifference != 0f)
            {
                // frontal engine get input towards right back to left
                ApplyEngineVector(Vector3.right * horizontallAngleDifference, FrontEngines);
                ApplyEngineVector(Vector3.left * -horizontallAngleDifference, BackEgnines);

                // same whit left and right engines but whit forward and back
                ApplyEngineVector(Vector3.forward * horizontallAngleDifference, LeftEgnines);
                ApplyEngineVector(Vector3.back * -horizontallAngleDifference, RightEngines);
            }

            targetHeight = currentTarget.position.y;
            targetHeight = Mathf.Clamp(targetHeight, 0, MaxHeight);

        }
        else
        {

        }


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

        Vector3 predictedUp = Quaternion.AngleAxis(rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / speed, rb.angularVelocity) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        rb.AddTorque(torqueVector * speed * speed);
    }

    public void GetTurrets()
    {
        turrets.Clear();
        foreach (Transform Turret in turretSpots)
        {
            TurretScript turretToAdd = Turret.GetComponentInChildren<TurretScript>();
            if (turretToAdd != null)
            {
                turrets.Add(turretToAdd);
            }
        }
    }

    void GiveTurretInput()
    {
        if (Vector3.Distance(transform.position,currentTarget.position) <= agroRange)
        {
            foreach (TurretScript turret in turrets)
            {
                if (turret != null)
                {
                    turret.TargetPostion = currentTarget.position;
                    turret.TryShoot();
                }
            }
        }
        else
        {
            foreach (TurretScript turret in turrets)
            {
                if (turret != null)
                {
                    turret.TargetPostion = transform.forward * 100f;
                }
            }
        }
    }
}
