using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeTreeMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI upgradeDesc;
    [SerializeField] private TextMeshProUGUI upgradeCost;
    [SerializeField] private TextMeshProUGUI upgradePrerequisites;
    [SerializeField] private TextMeshProUGUI upgradeExcluder;
    [Space]
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI shadowTokens;
    [SerializeField] private TextMeshProUGUI experience;

    [SerializeField] private Image experienceFill;

    public static event Action OnRefresh;
    public static event Action OnReset;

    private int _exp;
    private int _levelUpExp;

    private void Awake()
    {
        // MOVE THIS TO GAME LOAD, MAIN MENU! --------------------------------------------------------
        UpgradeTree.Load();
        LevelSystem.Load();
        // ------------------------------------------

    }

    private void Start()
    {
        UpdatePlayerInfo();
        LevelSystem.OnShadowTokenChanged += UpdatePlayerInfo;
    }

    private void OnDestroy()
    {
        LevelSystem.OnShadowTokenChanged -= UpdatePlayerInfo;
    }



    private void UpdatePlayerInfo()
    {
        _exp = LevelSystem.experience;
        _levelUpExp = LevelSystem.GetExperienceNeededForNextLevelUp();

        playerLevel.text = "Level " + LevelSystem.level.ToString();
        shadowTokens.text = "Shadow Tokens: " + LevelSystem.shadowTokens.ToString();
        experience.text = _exp + " / " + _levelUpExp;
        experienceFill.fillAmount = (float)_exp / _levelUpExp;
    }

    public void SetUpgradeTexts(string _name, string _desc, string _cost, string _prerequisites, string _excluder)
    {
        upgradeName.text = _name;
        upgradeDesc.text = _desc;
        upgradeCost.text = _cost == "" ? "" : ("Unlock Cost: " + _cost + " Shadow Tokens");
        upgradePrerequisites.text = _prerequisites;
        upgradeExcluder.text = _excluder;
    }

    public void ClearTexts()
    {
        upgradeName.text = "";
        upgradeDesc.text = "";
        upgradeCost.text = "";
        upgradeName.text = "";
        upgradePrerequisites.text = "";
        upgradeExcluder.text = "";
    }

    public void Remote_ResetTree()
    {
        OnReset?.Invoke();
        UpgradeTree.FirstLoad();
        OnRefresh?.Invoke();
    }

    public void Remote_LevelUp()
    {
        LevelSystem.LevelUp();

        if (LevelSystem.level > 20)
        {
            LevelSystem.ResetLevel();
        }

        UpdatePlayerInfo();
    }

    public void Remote_AddExp()
    {
        LevelSystem.GainExperience(120);

        if (LevelSystem.level > 20)
        {
            LevelSystem.ResetLevel();
        }

        UpdatePlayerInfo();
    }

    public void Remote_Back()
    {
        SceneManager.LoadScene(0);
    }

    public static void Refresh()
    {
        OnRefresh?.Invoke();
    }
}
