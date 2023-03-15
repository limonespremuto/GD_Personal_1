using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    UIState uIState;
    public enum UIState
    {
        Playng,
        Paused,
        Victory,
        Defeat
    }

    public static UIManager Instance;

    public Text textUIHealth;
    public Text textUISpeed;
    public Text textUIHeight;
    public UnityEngine.UI.Slider sliderUIBoost;

    [SerializeField]
    private GameObject cineMachineGO;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Multiple UI Manager exist in scene");
        }
        UIManager.Instance = this;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && uIState != UIState.Defeat)
        {
            updateState(UIState.Paused);
        }
    }

    public void updateState(UIState newState)
    {
        uIState = newState;
        switch (uIState)
        {
            case UIState.Playng:
                {
                    //Time.timeScale = 1f;
                    break;
                }
            case UIState.Paused:
                {
                    //Time.timeScale = 0f;
                    break;
                }
            case UIState.Defeat:
                {
                    break;
                }
            case UIState.Victory:
                {
                    Debug.Log("you win");
                    Application.Quit();
                    break;
                }
        }
    }
    public void UpdateHealth(float health, float maxHealth)
    {
        textUIHealth.text = health + " / " + maxHealth + " HP";
    }

    public void UpdateSpeed(float speed)
    {
        textUISpeed.text = speed.ToString() + " m/s";
    }

    public void UpdateTargetHehight(float height, float TargetHeight)
    {
        textUIHeight.text = height + " (" + TargetHeight + ") Height";
    }

    public void UpdateBostSlider(float ValuePercent)
    {
        sliderUIBoost.value = ValuePercent;
    }
}
