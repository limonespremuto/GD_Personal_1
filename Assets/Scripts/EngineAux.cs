using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineAux : EngineBase, IHealth, IEngine
{
    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //ApplyForceVector();
    }

    new public void GetNewVector(Vector3 inputVector)
    {
        DesiredVectorThrust += inputVector;
    }
    new public void ApplyForceVector()
    {
        if (DesiredVectorThrust != Vector3.zero)
        {
            Debug.DrawRay(transform.position, 10 * transform.TransformDirection(DesiredVectorThrust), Color.red, 0.05f);
            rb.AddForceAtPosition(transform.TransformDirection(DesiredVectorThrust) * ThrusterPower * CurrentPowerMP, transform.position, ForceMode.Force);
            DesiredVectorThrust = Vector3.zero;

        }

    }
    void CalculatePowerMP()
    {
        CurrentPowerMP = 1f * (componentHealth / maxComponentHealth);
    }
    public void TakeDamage(float damage, float ComponentDamage)
    {
        componentHealth -= damage;
        componentHealth = Mathf.Max(0, componentHealth);
        CalculatePowerMP();
        gameObject.GetComponentInParent<HealthScript>().TakeDamage(damage, 0);
    }

}
