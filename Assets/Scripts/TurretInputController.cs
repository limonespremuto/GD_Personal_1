using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretInputController : MonoBehaviour
{
    public enum ControllerType
    {
        Player,
        Enemy
    }

    [SerializeField]
    private ControllerType controllerType = ControllerType.Player;

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

    [SerializeField]
    private Cinemachine.CinemachineFreeLook MainCamera;

    // Start is called before the first frame update
    void Start()
    {
        PlayerCamera = Camera.main;
        GetTurrets();
    }

    // Update is called once per frame
    void Update()
    {
        if (controllerType == ControllerType.Player)
        {
            CameraControlls();
        }
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

    public void GiveTurretInput(bool boolInput, Vector3 targetVector)
    {
        bool FireTurret;
        if (boolInput)
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
                turretSc.TargetPostion = targetVector;
                if (FireTurret)
                    turretSc.TryShoot();
            }
        }
    }

    public Vector3 CameraRayCast()
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

    void CameraControlls()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            MainCamera.m_Lens.FieldOfView = 40f / 2f;
        }
        else
        {
            MainCamera.m_Lens.FieldOfView = 40f;
        }
    }
}
