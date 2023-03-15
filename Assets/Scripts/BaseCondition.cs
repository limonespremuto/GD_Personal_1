using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCondition : MonoBehaviour, ICondition
{
    // his script handles WinCondittion for now also contain its Interface
    [SerializeField]
    ConditionType conditionType = ConditionType.DisableGO;
    public enum ConditionType
    {
        DisableGO,
        EnableGO,
        LevelEnd
    }

    [Header("In caso of Gameobjects")]
    public GameObject[] gameObjects;

    [Header("In case of level end")]
    public UIManager uIManager;
    public ShipController shipController;
    public void Condition()
    {
        switch (conditionType)
        {
            case ConditionType.DisableGO:
                {
                    foreach (GameObject go in gameObjects)
                    {
                        go.SetActive(false);
                    }
                    break;
                }
            case ConditionType.EnableGO:
                {
                    foreach (GameObject go in gameObjects)
                    {
                        go.SetActive(true);
                    }
                    break;
                }
            case ConditionType.LevelEnd:
                {
                    shipController.controllerType = ShipController.ControllerType.None;
                    uIManager.UpdateState(UIManager.UIState.Victory);
                    break;
                }
        }

    }



}

public interface ICondition
{
    public void Condition();
}