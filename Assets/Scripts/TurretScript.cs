using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{

    public float BDamage = 10f;
    public float BSpeed = 100f;
    public float BDistance = 300f;

    public float Firerate = 2f;
    float cooldownTime;

    [HideInInspector]
    public Vector3 TargetPostion;
    //[HideInInspector]
    //public Transform TargetTransform;
    //[Tooltip("The script will always use Vector instead of transform")]
    //public bool useTargetVector;

    [SerializeField]
    private GameObject ProjectilePrefab;
        [SerializeField]
    private Transform HorizonaltPivot;
    [SerializeField]
    private Transform VerticalPivot;
    [SerializeField]
    private Transform bulletSpawnPoint;

    [SerializeField]
    LayerMask ProjectileLayermask;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTime -= Time.deltaTime;
        AimTurret();
    }

    void AimTurret()
    {
        Vector3 TargetVectorFlat = transform.InverseTransformPoint(TargetPostion);
        TargetVectorFlat.y = 0f;
        TargetVectorFlat = transform.TransformPoint(TargetVectorFlat);
        HorizonaltPivot.LookAt(TargetVectorFlat, transform.up);

        TargetVectorFlat = TargetPostion;
        VerticalPivot.LookAt(TargetVectorFlat, transform.up);
    }

    public void TryShoot()
    {
        if (cooldownTime <= 0f)
        {
            ShootProjectile();
            cooldownTime = 1f / Firerate;
        }
    }
    public void ShootProjectile()
    {
        GameObject bulletInstance = Instantiate(ProjectilePrefab,bulletSpawnPoint.position,bulletSpawnPoint.rotation);
        ProjectileScript bulletInstanceScript = bulletInstance.GetComponent<ProjectileScript>();
        if (bulletInstanceScript == null)
        {
            Debug.LogError("There is no script in this BulletPrefab" + transform.name);
            return;
        }

        bulletInstanceScript.damage = BDamage;
        bulletInstanceScript.speed = BSpeed;
        bulletInstanceScript.lifeTime = BDistance / BSpeed;
        bulletInstanceScript.bulletCheckLayer = ProjectileLayermask;

    }
}
