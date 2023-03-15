using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour, IHealth
{
    [SerializeField]
    private float MaxHealth, health;

    [SerializeField]
    private bool isInvincible;

    [SerializeField]
    ShipController shipController;

    [SerializeField]
    private float collisionMinSpeed;

    [SerializeField]
    private float collisionDamageMP;

    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage, float componentDamage)
    {
        health -= damage;
        if (health <= 0f)
        {
            if (isInvincible)
            {
                if (shipController != null)
                {
                    //shipController.controllerType = ShipController.ControllerType.Destroyed;
                }
                Debug.Log("You are dead " + transform.name);
            }
            else
            {
                if (shipController != null)
                {
                    shipController.controllerType = ShipController.ControllerType.Destroyed;
                }
            }
        }

        health = Mathf.Clamp(health, 0f, MaxHealth);
    }

    
    private void  OnCollisionEnter(Collision otherCollidingShip)
    {
        //Debug.Log("there has been a collision whit " + collision.transform.root.name);
        Rigidbody rb = transform.GetComponentInParent<Rigidbody>();
        //Debug.Log("the Speed of the colliding ship is " + rb.velocity.magnitude);
        if (rb != null)
        {
            Rigidbody otherRB = otherCollidingShip.collider.GetComponentInParent<Rigidbody>();
            
            float damageTaken = 0f;

            if (otherRB != null)
            {
                if ((rb.velocity - otherRB.velocity).magnitude >= collisionMinSpeed)
                {
                    damageTaken = (rb.velocity - otherRB.velocity).magnitude - collisionMinSpeed;
                    damageTaken = damageTaken * collisionDamageMP;
                    TakeDamage(damageTaken / 2f, 0);

                    IHealth otherIhealt = otherCollidingShip.transform.GetComponentInParent<IHealth>();
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

}
public interface IHealth
{
    public void TakeDamage(float damage, float ComponentDamage);
}
