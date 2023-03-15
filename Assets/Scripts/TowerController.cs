using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour , IHealth
{
    [SerializeField]
    private float maxHealth = 300f, health;
    [SerializeField]
    TurretInputController turretController;
    [SerializeField]
    BaseCondition[] deathConditions;
    public Transform currentTarget;
    public float agroRange = 500f;
    //public float stopRange = 300f;

    private void Awake()
    {
        health = maxHealth;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        if (distanceToTarget < agroRange)
        {
            turretController.GiveTurretInput(true, currentTarget.position);
        }
    }

    public void TakeDamage(float damage, float componentDamage)
    {
        health -= damage;
        if (health <= 0f)
        {
            foreach (BaseCondition condition in deathConditions)
            {
                condition.Condition();
            }
            this.enabled = false;
        }

        health = Mathf.Clamp(health, 0f, maxHealth);
    }

    private void OnDrawGizmosSelected()
    {
        if (currentTarget != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, agroRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
}
