using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Image gammaPanel;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1);
        byte _a = (byte)Mathf.Clamp((byte)(255 - (255 * brightnessSlider.value)), 0, 240);
        gammaPanel.color = new Color32(0, 0, 0, _a);
    }

    public void Remote_ChangeBrightnessValue()
    {
        PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);
        byte _a = (byte)Mathf.Clamp((byte)(255 - (255 * brightnessSlider.value)), 0, 240);
        gammaPanel.color = new Color32(0, 0, 0, _a);
    }

    public void RemoteCall_DevMap1()
    {
        SceneManager.LoadScene(1);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
    }

    public void RemoteCall_DevMap2()
    {
        SceneManager.LoadScene(2);
      //  SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(2));

    }

    public void RemoteCallMap1()
    {
        SceneManager.LoadScene(3);
      //  SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(3));
    }
    public void RemoteCall_GeneralDev()
    {
        SceneManager.LoadScene(4);
        //ceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(4));
    }

    public void RemoteCall_UpgradeTree()
    {
        SceneManager.LoadScene(5);
      //  SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
    }

    public void RemoteCall_ObjectiveDev()
    {
        SceneManager.LoadScene(6);
    }

    public void RemoteCall_Exit()
    {
        Application.Quit();
    }
}
