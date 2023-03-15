using UnityEngine;

public class ShipController : MonoBehaviour, IHealth
{
    public ControllerType controllerType = ControllerType.None;
    public enum ControllerType
    {
        Player,
        Enemy,
        None,
        Destroyed
    }

    [Header("Health and damage")]
    
    [SerializeField]
    private float maxHealth = 300f, health;

    [SerializeField]
    private bool isInvincible;

    [SerializeField]
    private float collisionMinSpeed;

    [SerializeField]
    private float collisionDamageMP;

    [SerializeField, Range(1f, 4f)]
    private float boostMP = 1.5f;
    [SerializeField, Range(1f, 4f)]
    private float steerBostMP = 2.5f;
    [SerializeField, Range(0f, 1f)]
    private float overheatedMP = 0.1f;
    [SerializeField]
    private float boostCapacity = 3f, currentBoost = 3f, boostRegenRate = 3f;
    [SerializeField]
    private float boostCooldown = 1f, boostPenalty = 2f;
    [SerializeField]
    private bool isOverheated = false;

    [SerializeField]
    private float targetHeight;
    [SerializeField] private float MaxHeight = 300f;

    [Header("Stabilizer")]
    public float stability = 0.3f;
    public float stabilityPowerMP = 2.0f;


    public Engine[] AllEngines;

    float liftPower = 0f;
    float forwardPower = 0f;
    float backPower = 0f;
    float rightPower = 0f;
    float leftPower = 0f;
    float torquePower = 0f;
    float StabilityPower = 0f;
    float RequiredPower;

    [Header("Setup")]
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    Transform CenterOfMass;
    [SerializeField]
    TurretInputController turretController;
    [SerializeField]
    UIManager uiManager;
    bool useUImanager = false;
    
    [SerializeField]
    public BaseCondition[] deathConditions;

    [Header("Enemy only")]
    public Transform currentTarget;
    public float agroRange = 300f;
    public float stopRange = 50f;
    [SerializeField]
    LayerMask worldLayer;
    private void Awake()
    {
        health = maxHealth;
        rb.centerOfMass = CenterOfMass.localPosition;
        targetHeight = CenterOfMass.position.y;
        RecalculateEnginePower();
        //A failsafe for the UI manager its helps
        if (controllerType == ControllerType.Player)
        {
            //Debug.Log("UIManager not set but supposed to be used");
            useUImanager = true;
        }

        if (controllerType == ControllerType.Player)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Start()
    {
        if (useUImanager)
            uiManager.UpdateHealth(Mathf.Round(health), maxHealth);
    }
    private void Update()
    {
        if (controllerType == ControllerType.Player)
        {
            turretController.GiveTurretInput(Input.GetKey(KeyCode.Mouse0), turretController.CameraRayCast());
        }
    }

    private void FixedUpdate()
    {
        InputUpdate();
        if (useUImanager)
            uiManager.UpdateSpeed(Mathf.Round(10 * rb.velocity.magnitude) / 10);
    }

    private void OnCollisionEnter(Collision collider)
    {
        //Debug.Log("there has been a collision whit " + collision.transform.root.name);
        Rigidbody rb = transform.GetComponentInParent<Rigidbody>();
        //Debug.Log("the Speed of the colliding ship is " + rb.velocity.magnitude);
        if (rb != null)
        {
            Rigidbody otherRB = collider.collider.GetComponentInParent<Rigidbody>();

            float damageTaken = 0f;

            if (otherRB != null)
            {
                if ((rb.velocity - otherRB.velocity).magnitude >= collisionMinSpeed)
                {
                    damageTaken = (rb.velocity - otherRB.velocity).magnitude - collisionMinSpeed;
                    damageTaken = damageTaken * collisionDamageMP;
                    TakeDamage(damageTaken / 2f, 0);

                    IHealth otherIhealt = collider.transform.GetComponentInParent<IHealth>();
                    if (otherIhealt != null)
                    {
                        otherIhealt.TakeDamage(damageTaken / 2f, 0);
                    }
                }
            }
            else
            {
                if (rb.velocity.magnitude >= collisionMinSpeed)
                {
                    damageTaken = rb.velocity.magnitude - collisionMinSpeed;
                    damageTaken = damageTaken * collisionDamageMP;
                    TakeDamage(damageTaken, 0);
                }
            }
        }
    }

    private void InputUpdate()
    {
        Vector3 thrustVector = Vector3.zero;

        switch (controllerType)
        {
            case ControllerType.Player:
                {
                    if (Input.GetAxis("Vertical") != 0f)
                    {
                        if (Input.GetAxis("Vertical") >= 0f)
                        {
                            thrustVector.z += Input.GetAxis("Vertical") * forwardPower;
                        }
                        else
                        {
                            thrustVector.z += Input.GetAxis("Vertical") * backPower;
                        }
                    }

                    if (Input.GetAxis("Horizontal") != 0f)
                    {
                        if (Input.GetAxis("Horizontal") >= 0f)
                        {
                            thrustVector.x += Input.GetAxis("Horizontal") * rightPower;
                        }
                        else
                        {
                            thrustVector.x += Input.GetAxis("Horizontal") * leftPower;
                        }
                    }

                    bool isBoosting = BoostFuncion(Input.GetKey(KeyCode.LeftShift));

                    float steerForce = Input.GetAxis("Steer");
                    if (isBoosting)
                    {
                        thrustVector = thrustVector * boostMP;
                        steerForce = steerForce * steerBostMP;
                    }

                    //Steering input
                    rb.AddRelativeTorque(Vector3.up * steerForce * torquePower * 10f);
                    float beforeOverHeat = liftPower;
                    if (isOverheated)
                    {
                        thrustVector = thrustVector * overheatedMP;
                        liftPower = liftPower * overheatedMP;
                    }

                    thrustVector.y = CalculateRequiredPower();
                    liftPower = beforeOverHeat;

                    targetHeight += Input.GetAxis("Ascend") * 0.2f;
                    if (useUImanager)
                        uiManager.UpdateTargetHehight(Mathf.Round(transform.position.y * 10f) / 10f, Mathf.Round(targetHeight * 10f) / 10f);
                    StabilizeShip();
                    break;
                }
            case ControllerType.Enemy:
                {
                    if (Vector3.Distance(currentTarget.position, transform.position) <= agroRange)
                    {
                        if (Vector3.Distance(currentTarget.position, transform.position) >= stopRange)
                        {
                            thrustVector = Vector3.forward * forwardPower;
                        }
                        else
                        {

                        }

                        float angleOfTarget;
                        Vector3 flatTransformPos;
                        Vector3 flatTargetPos;

                        flatTransformPos = new Vector3(transform.position.x, 0, transform.position.z);
                        flatTargetPos = new Vector3(currentTarget.position.x, 0, currentTarget.position.z);
                        if (transform.InverseTransformDirection(flatTargetPos).x < 0)
                        {
                            angleOfTarget = -Vector3.Angle(flatTransformPos, flatTargetPos);
                        }
                        else
                        {
                            angleOfTarget = Vector3.Angle(flatTransformPos, flatTargetPos);
                        }

                        targetHeight = currentTarget.position.y;
                        turretController.GiveTurretInput(true, currentTarget.position);
                        rb.AddTorque(Vector3.up * angleOfTarget / 180f * torquePower * 10f,ForceMode.Force);
                    }
                    else
                    {
                        turretController.GiveTurretInput(false,(transform.forward * 2000f) + transform.position);
                    }
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, Vector3.down,out hit, 30f, worldLayer))
                    {
                        targetHeight = hit.point.y + 30f;
                        if (currentTarget.position.y >= transform.position.y)
                        {
                            targetHeight = currentTarget.position.y;
                        }
                    }


                    thrustVector.y = CalculateRequiredPower();

                    
                    StabilizeShip();
                    break;
                }
            case ControllerType.None:
                {
                    thrustVector.y = CalculateRequiredPower();
                    StabilizeShip();
                    break;
                }
            case ControllerType.Destroyed:
                {
                    break;
                }

        }
        
        targetHeight = Mathf.Clamp(targetHeight, 0, MaxHeight);
        Vector3 thrustVectorWhiteoutY = new Vector3(thrustVector.x, 0f, thrustVector.z);
        rb.AddRelativeForce(thrustVectorWhiteoutY, ForceMode.Force);
        rb.AddForce(Vector3.up * thrustVector.y, ForceMode.Force);
    }

    public void RecalculateEnginePower()
    {
        liftPower = 0f;
        forwardPower = 0f;
        backPower = 0f;
        rightPower = 0f;
        leftPower = 0f;
        torquePower = 0f;
        StabilityPower = 0f;


        foreach (Engine engine in AllEngines)
        {
            liftPower += engine.liftPower * engine.PowerMP;
            forwardPower += engine.forwardPower * engine.PowerMP;
            backPower += engine.backPower * engine.PowerMP;
            rightPower += engine.rightPower * engine.PowerMP;
            leftPower += engine.leftPower * engine.PowerMP;
            torquePower += engine.torquePower * engine.PowerMP;
            StabilityPower += engine.StabilityPower * engine.PowerMP;
        }

        RequiredPower = rb.mass * -Physics.gravity.y;
        //Debug.Log(RequiredPower);

        RequiredPower = Mathf.Clamp(RequiredPower, 0f, liftPower);
    }

    float CalculateRequiredPower()
    {
        float VerticalPower;
        VerticalPower = RequiredPower;
        VerticalPower += Mathf.Clamp((targetHeight - transform.position.y) / 10f, -1f, 1f) * liftPower;
        VerticalPower = Mathf.Max(VerticalPower, 0f);
        return VerticalPower;
    }
    
    //Should run every frame
    bool BoostFuncion(bool InputKey)
    {
        bool isBoosting = false;
        //Adding Booster power and handling
        if (currentBoost > 0f)
        {

            if (!isOverheated)
            {
                if (InputKey)
                {
                    boostCooldown = boostPenalty;
                    currentBoost -= Time.deltaTime;

                    isBoosting = true;
                    if (currentBoost <= 0f)
                    {
                        boostCooldown += boostPenalty;
                        isOverheated = true;
                    }

                }

            }
        }

        if (boostCooldown <= 0f)
        {
            currentBoost += Time.deltaTime * boostRegenRate;
        }

        if (currentBoost >= boostCapacity)
        {
            isOverheated = false;
        }
        currentBoost = Mathf.Clamp(currentBoost, 0f, boostCapacity);
        boostCooldown -= Time.deltaTime;
        if (useUImanager)
            uiManager.UpdateBostSlider(currentBoost / boostCapacity);
        return isBoosting;
    }

    private void StabilizeShip()
    {
        Vector3 predictedUp = Quaternion.AngleAxis(rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / stabilityPowerMP,rb.angularVelocity) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        rb.AddTorque(torqueVector * StabilityPower * stabilityPowerMP);
    }

    public void TakeDamage(float damage, float ComponentDamage)
    {
        health -= damage;
        if (health <= 0f)
        {
            if (isInvincible)
            {
                Debug.Log("You are dead " + transform.name);
            }
            else
            {
                if (controllerType == ControllerType.Player)
                {
                    foreach (ICondition condition in deathConditions)
                    {
                        condition.Condition();
                    }
                }
                controllerType = ControllerType.Destroyed;
            }
        }

        health = Mathf.Clamp(health, 0f, maxHealth);

        if (useUImanager)
            uiManager.UpdateHealth(Mathf.Round(health), maxHealth);

    }
    private void OnDrawGizmosSelected()
    {
        if (controllerType == ControllerType.Enemy)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, agroRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }


}
