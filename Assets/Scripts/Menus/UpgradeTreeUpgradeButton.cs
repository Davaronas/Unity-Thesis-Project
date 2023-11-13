using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UpgradeTreeUpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private UpgradeTreeMenu upgradeTreeMenu;
    private UpgradeTreeUpgradeButton[] upgradeTreeUpgradeButtons;
    private UpgradeTreeAbilityButton[] upgradeTreeAbilityButtons;

    [SerializeField] private string abilityName;
    [SerializeField] private string upgradeName;
    [Space]
    [SerializeField] private string displayName;

    [TextArea(3, 15)]
    [SerializeField] private string description;

    private UpgradeTree.Upgrade upgrade;

    private Image LockedImage;
    private Image UnlockableImage;

    private bool canUnlock = false;

    

    private void Start()
    {
        upgradeTreeMenu = FindObjectOfType<UpgradeTreeMenu>();
        upgradeTreeUpgradeButtons = FindObjectsOfType<UpgradeTreeUpgradeButton>();
        upgradeTreeAbilityButtons = FindObjectsOfType<UpgradeTreeAbilityButton>();

       

        LockedImage = transform.Find("Locked").GetComponent<Image>();
        UnlockableImage = transform.Find("Unlockable").GetComponent<Image>();

        UpgradeTreeMenu.OnRefresh += Refresh;
        UpgradeTreeMenu.OnReset += ResetUpgrade;

        Refresh();
    }

    private void OnDestroy()
    {
        UpgradeTreeMenu.OnRefresh -= Refresh;
        UpgradeTreeMenu.OnReset -= ResetUpgrade;
    }

    private void ResetUpgrade()
    {
        if(upgrade.IsUnlocked())
        {
            LevelSystem.AddShadowTokens(upgrade.cost);
        }
    }

    private void Refresh()
    {
        upgrade = UpgradeTree.GetUpgrade(abilityName, upgradeName);
        // frissítsük a referenciákat
        UpgradeTree.GetUpgrade(upgrade.abilityName, upgrade.upgradeName).UpdateReferences();


        canUnlock = false;



        StartCoroutine(UpdateUpgrade());
    }

    private IEnumerator UpdateUpgrade()
    {
        // várjunk a másik frameig hogy elõször az Upgrade és Abilityk újra megszerzõdjenek
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        LockedImage.enabled = false;
        UnlockableImage.enabled = false;

        if (upgrade.IsUnlocked()) { yield break; }

        if (upgrade.excluder != null)
        {
            if (upgrade.excluder.IsUnlocked())
            {
                LockedImage.enabled = true;
                yield break;
            }
        }

        if (upgrade.abilityRequired != null)
        {
            if (!upgrade.abilityRequired.IsUnlocked())
            {
                LockedImage.enabled = true;
                yield break;
            }
        }

        if (upgrade.prerequisiteUpgrades.Count == 0)
        {
            UnlockableImage.enabled = true;
            canUnlock = true;
        }
        else
        {


            for (int i = 0; i < upgrade.prerequisiteUpgrades.Count; i++)
            {
                if (!upgrade.prerequisiteUpgrades[i].IsUnlocked())
                {
                    LockedImage.enabled = true;
                    yield break;
                }
            }
            UnlockableImage.enabled = true;
            canUnlock = true;
        }
    }

    public bool CheckNames(string _abilityName, string _upgradeName)
    {
        if (abilityName == _abilityName && upgradeName == _upgradeName)
        {
            return true;
        }
        else
            return false;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string _prereq = "";

        if (upgrade.abilityRequired != null)
        {
            _prereq = "Prerequisites: ";

            for (int i = 0; i < upgradeTreeAbilityButtons.Length; i++)
            {
                if (upgradeTreeAbilityButtons[i].CheckName(upgrade.abilityRequired.AbilityName))
                {
                    _prereq += upgradeTreeAbilityButtons[i].GetDisplayName() + ", ";
                }
            }
        }


        if (upgrade.prerequisiteUpgrades != null)
        {
            if (upgrade.prerequisiteUpgrades.Count > 0)
            {
                if(_prereq.Length <= 0)
                {
                    _prereq = "Prerequisites: ";
                }

                for (int j = 0; j < upgrade.prerequisiteUpgrades.Count; j++)
                {
                    
                    for (int i = 0; i < upgradeTreeUpgradeButtons.Length; i++)
                    {
                        if (upgradeTreeUpgradeButtons[i].CheckNames(upgrade.prerequisiteUpgrades[j].abilityName, upgrade.prerequisiteUpgrades[j].upgradeName))
                        {
                            _prereq += upgradeTreeUpgradeButtons[i].GetDisplayName() + ", ";
                        }
                    }
                }
                // vegyük le a vesszõt és a szóközt a végérõl, ha többet is felsorolunk
            }
        }


        if(_prereq.Length > 0)
        {
            _prereq = _prereq.Substring(0, _prereq.Length - 2);
        }

        string _excl = "";
        if(upgrade.excluder != null)
        {
            _excl = "Excluder: ";

            for (int i = 0; i < upgradeTreeUpgradeButtons.Length; i++)
            {
                if (upgradeTreeUpgradeButtons[i].CheckNames(upgrade.excluder.abilityName, upgrade.excluder.upgradeName))
                {
                    _excl += upgradeTreeUpgradeButtons[i].GetDisplayName();
                }
            }
        }

        upgradeTreeMenu.SetUpgradeTexts(displayName, description, upgrade.cost.ToString(), _prereq, _excl);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        upgradeTreeMenu.ClearTexts();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canUnlock && LevelSystem.HasEnoughShadowTokens(upgrade.cost))
        {
            LevelSystem.RemoveShadowTokens(upgrade.cost);
            UpgradeTree.GetUpgrade(upgrade.abilityName, upgrade.upgradeName).SetUnlocked();
            

            UpgradeTree.SaveUpgradeState();

            UpgradeTreeMenu.Refresh();
        }
    }
}
