using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretInputController : MonoBehaviour
{
    [SerializeField] Camera PlayerCamera;
    

    [SerializeField]
    private Transform[] turretSpots;
    [SerializeField]
    private List<TurretScript> turrets;

    [SerializeField]
    LayerMask WorldWhitoutPlayer;

    [SerializeField]
    LayerMask EnemyLayer;

    [SerializeField]
    private bool debugMode = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        PlayerCamera = Camera.main;
        GetTurrets();
    }

    // Update is called once per frame
    void Update()
    {
        GiveTurretInput();
    }

    private void OnDrawGizmos()
    {
        if (debugMode == false)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(CameraRayCast(), 2f);
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
        bool FireTurret = false;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            FireTurret = true;
        }
        else
        {
            FireTurret = false;
        }
        foreach (TurretScript turret in turrets)
        {
            TurretScript turretSc = turret;
            if (turretSc != null)
            {
                turretSc.TargetPostion = CameraRayCast();
                if (FireTurret)
                    turretSc.TryShoot();
            }
        }
    }

    Vector3 CameraRayCast()
    {
        Vector3 HitPostion;
        RaycastHit hit;

        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward,out hit, 500f, WorldWhitoutPlayer))
        {
            HitPostion = hit.point;
        }
        else
        {
            HitPostion = PlayerCamera.transform.position + (PlayerCamera.transform.forward * 800f);
        }
        
        return HitPostion;
    }
}
