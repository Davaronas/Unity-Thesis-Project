using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class UpgradeTreeAbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private UpgradeTreeMenu upgradeTreeMenu;
    private UpgradeTreeUpgradeButton[] upgradeTreeUpgradeButtons;

    [SerializeField] private string abilityName;
    [SerializeField] private string displayName;

    [TextArea(3,15)]
    [SerializeField] private string description;

    private UpgradeTree.Ability ability;

    private Image LockedImage;
    private Image UnlockableImage;

    private bool canUnlock = false;

    

    private void Start()
    {
        upgradeTreeMenu = FindObjectOfType<UpgradeTreeMenu>();
        upgradeTreeUpgradeButtons = FindObjectsOfType<UpgradeTreeUpgradeButton>();

       

        LockedImage = transform.Find("Locked").GetComponent<Image>();
        UnlockableImage = transform.Find("Unlockable").GetComponent<Image>();

        UpgradeTreeMenu.OnRefresh += Refresh;
        UpgradeTreeMenu.OnReset += ResetAbility;

        Refresh();
    }

    private void OnDestroy()
    {
        UpgradeTreeMenu.OnRefresh -= Refresh;
        UpgradeTreeMenu.OnReset -= ResetAbility;
    }

    private void ResetAbility()
    {
        if(ability.IsUnlocked())
        {
            LevelSystem.AddShadowTokens(ability.cost);
        }
    }

    private void Refresh()
    {
        ability = UpgradeTree.GetAbility(abilityName);
        // frissítsük a referenciákat
        UpgradeTree.GetAbility(ability.AbilityName).UpdateReferences();
        canUnlock = false;
       

        StartCoroutine(UpdateAbility());
    }

    private IEnumerator UpdateAbility()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();


        LockedImage.enabled = false;
        UnlockableImage.enabled = false;

        if (ability.IsUnlocked()) { yield break; }

        if (ability.prerequisiteUpgrade != null)
        {
            print("Prerequisite Unlocked: " + ability.prerequisiteUpgrade.upgradeName + " " + ability.prerequisiteUpgrade.IsUnlocked());

            if (!ability.prerequisiteUpgrade.IsUnlocked())
            {
                LockedImage.enabled = true;
            }
            else
            {
                UnlockableImage.enabled = true;
                canUnlock = true;
            }
        }
        else
        {
            UnlockableImage.enabled = true;
            canUnlock = true;
        }
    }


    public bool CheckName(string _abilityName)
    {
        if (abilityName == _abilityName)
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



        if(ability.prerequisiteUpgrade != null)
        {
            _prereq = "Prerequisites: ";
            for(int i = 0; i < upgradeTreeUpgradeButtons.Length; i++)
            {
                if (upgradeTreeUpgradeButtons[i].CheckNames(ability.prerequisiteUpgrade.abilityName, ability.prerequisiteUpgrade.upgradeName))
                {
                    _prereq += upgradeTreeUpgradeButtons[i].GetDisplayName() + ", ";
                }
            }
            // vegyük le a vesszõt és a szóközt a végérõl
            _prereq = _prereq.Substring(0, _prereq.Length - 2);
        }

        upgradeTreeMenu.SetUpgradeTexts(displayName, description, ability.cost == 0 ? "" : ability.cost.ToString(), _prereq, "");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        upgradeTreeMenu.ClearTexts();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canUnlock && LevelSystem.HasEnoughShadowTokens(ability.cost))
        {
            LevelSystem.RemoveShadowTokens(ability.cost);
            UpgradeTree.GetAbility(ability.AbilityName).SetUnlocked();
           


            UpgradeTree.SaveUpgradeState();

            UpgradeTreeMenu.Refresh();
        }
    }
}
