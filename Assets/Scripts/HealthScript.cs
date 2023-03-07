using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour , IHealth
{
    [SerializeField]
    private float MaxHealth, health;

    [SerializeField]
    private bool isInvincible;


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
                Debug.Log("You are dead" + transform.name);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

}
public interface IHealth
{
    public void TakeDamage(float damage, float ComponentDamage);
}
