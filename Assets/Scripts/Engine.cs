using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour, IHealth
{
    [Header ("Stats")]
    public float liftPower = 10f;
    public float forwardPower = 0f;
    public float backPower = 0f;
    public float rightPower = 0f;
    public float leftPower = 0f;
    public float torquePower = 1f;

    public float StabilityPower = 0f;

    [Tooltip("The power of the engine at this moment")]
    public float PowerMP = 1f;

    public float maxComponentHealth = 10f, componentHealth = 10f;

    public void TakeDamage(float damage, float ComponentDamage)
    {
        componentHealth -= ComponentDamage;
        componentHealth = Mathf.Clamp(componentHealth, 0f, maxComponentHealth);
        PowerMP = componentHealth / maxComponentHealth;
        ShipController pInputManager = GetComponentInParent<ShipController>();
        if (pInputManager != null)
        {
            pInputManager.RecalculateEnginePower();
        }

        HealthScript healthScript = GetComponentInParent<HealthScript>();
        if (healthScript != null)
        {
            healthScript.TakeDamage(damage, 0f);
        }
    }


    //[Header("setup")]
    //protected Rigidbody rb;

    //protected Vector3 DesiredVectorThrust = Vector3.zero;


}