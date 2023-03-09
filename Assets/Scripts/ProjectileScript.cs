using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    public float damage = 1f;
    [Range(0f,1f)]
    public float componentDamageMP = 0.5f;
    public float speed = 50f;
    public float lifeTime = 5f;
    public float gravity = 9.81f;
    [Range(0f, 5f)]
    public float gravityMP = 1f;

    float GravityVelocity;

    [HideInInspector]
    public LayerMask bulletCheckLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveBullet();
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void MoveBullet()
    {
        Vector3 MoveVector = transform.forward * speed;
        GravityVelocity += gravity * gravityMP * Time.deltaTime;
        MoveVector += GravityVelocity * Vector3.down;
        

        transform.Translate(MoveVector * Time.deltaTime, Space.World);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit,speed * Time.deltaTime, bulletCheckLayer))
        {
            IHealth iHealthComponent = hit.collider.GetComponent<IHealth>();
            //Debug.Log(hit.collider.name);
            if (iHealthComponent != null)
            {
                iHealthComponent.TakeDamage(damage, damage * componentDamageMP);
            }
            else
            {
                iHealthComponent = hit.transform.GetComponentInParent<IHealth>();
                if (iHealthComponent != null)
                {
                    iHealthComponent.TakeDamage(damage, 0);
                }
            }

            Destroy(gameObject);
        }
    }
}
