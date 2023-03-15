using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    public GameObject uiPauseGO;
    public GameObject uiGameOverGO;
    public GameObject uiVictoryGO;

    [SerializeField]
    private GameObject cineMachineGO;

    //[SerializeField]
    //private string mainMenuSceneName;
    //[SerializeField]
    //private string levelSceneName;
    void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Multiple UI Manager exist in scene");
        }
        Instance = this;
    }



    // Start is called before the first frame update
    void Start()
    {
        UpdateState(uIState = UIState.Playng);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (uIState != UIState.Defeat || uIState != UIState.Victory)
                UpdateState(UIState.Paused);
        }
    }

    public void UpdateStateInt(int i)
    {
        if (i == 0)
        {
            UpdateState(UIState.Playng);
        }
        if (i == 1)
        {
            UpdateState(UIState.Paused);
        }
        if (i == 2)
        {
            UpdateState(UIState.Defeat);
        }
        if (i == 3)
        {
            UpdateState(UIState.Victory);
        }
    }

    public void UpdateState(UIState newState)
    {
        uIState = newState;
        switch (uIState)
        {
            case UIState.Playng:
                {
                    uiPauseGO.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Time.timeScale = 1f;
                    break;
                }
            case UIState.Paused:
                {
                    uiPauseGO.SetActive(true);
                    Cursor.lockState = CursorLockMode.Confined;
                    Time.timeScale = 0f;
                    break;
                }
            case UIState.Defeat:
                {
                    uiGameOverGO.SetActive(true);
                    Cursor.lockState = CursorLockMode.Confined;
                    Time.timeScale = 0.1f;
                    break;
                }
            case UIState.Victory:
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    uiVictoryGO.SetActive(true);
                    Time.timeScale = 0.1f;
                    //LoadScene(mainMenuSceneName);
                    //Debug.Log("you win");
                    //Application.Quit();
                    break;
                }
        }
    }

    #region ShipUI
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

    public void UpdateBoostSlider(float ValuePercent)
    {
        sliderUIBoost.value = ValuePercent;
    }
    #endregion

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
