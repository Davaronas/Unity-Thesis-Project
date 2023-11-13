using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public static class UpgradeTree
{
 

    [System.Serializable]
    public class Ability
    {
        public string AbilityName;
        public bool unlocked;
        public int cost;
        [SerializeReference]
        public Upgrade prerequisiteUpgrade;

        public Ability(string AbilityName, bool unlocked = false, int cost = 1) 
        {
            this.AbilityName = AbilityName;
            this.unlocked = unlocked;
            this.cost = cost;
            prerequisiteUpgrade = null;
        }

        public bool Equals(string  AbilityName)
        {
            if(this.AbilityName == AbilityName)
                return true;
            else
                return false;
        }

        public void SetUnlocked(bool unlocked = true)
        {
            this.unlocked = unlocked;
        }

        public void SetPrerequisite(Upgrade prerequisite)
        {
            prerequisiteUpgrade = prerequisite;
        }

        public bool IsUnlocked()
        {
            return unlocked;
        }

        public void UpdateReferences()
        {
            if(prerequisiteUpgrade != null)
            {
                prerequisiteUpgrade = GetUpgrade(prerequisiteUpgrade.abilityName, prerequisiteUpgrade.upgradeName);
            }

        }
    }

    [System.Serializable]
    public class Upgrade
    {
        public string abilityName;
        public string upgradeName;
        [SerializeReference]
        public List<Upgrade> prerequisiteUpgrades;
        [SerializeReference]
        public Ability abilityRequired;
        public Upgrade excluder;
        public bool unlocked;
        public int cost;

        public Upgrade(string abilityName, string upgradeName, Upgrade[] prerequisites, Upgrade excluder, int cost = 1, bool unlocked = false)
        {
            this.abilityName = abilityName;
            this.upgradeName = upgradeName;
            prerequisiteUpgrades = prerequisites.ToList();
            this.excluder = excluder;
            this.unlocked = unlocked;
            this.cost = cost;
            abilityRequired = null;
        }

        public Upgrade(string abilityName, string upgradeName, int cost)
        {
            this.abilityName = abilityName;
            this.upgradeName = upgradeName;
            prerequisiteUpgrades = new List<Upgrade>();
            excluder = null;
            unlocked = false;
            this.cost = cost;
            abilityRequired = null;
        }

        public Upgrade()
        {

        }


        public void SetPrerequisites(Upgrade[] prerequisites)
        {
            prerequisiteUpgrades = prerequisites.ToList();
        }

        public void AddPrerequisite(Upgrade prerequisites)
        {
            prerequisiteUpgrades.Add(prerequisites); ;
        }

        public void SetExcluder(Upgrade excluder)
        {
            this.excluder = excluder;
        }

        public void SetUnlocked(bool unlocked = true)
        { 
            this.unlocked = unlocked; 
            if(unlocked && excluder != null)
            {
                excluder.SetUnlocked(false);
            }
        }

        public bool Equals(string abilityName, string upgradeName)
        {
            if(this.abilityName == abilityName && this.upgradeName == upgradeName) return true;
            else return false;
        }

        public bool IsUnlocked()
        {
            return unlocked;
        }

        public void SetRequiredAbility(Ability ability)
        {
            abilityRequired = ability;
        }

        public void UpdateReferences()
        {
            if(abilityRequired != null)
            {
                abilityRequired = GetAbility(abilityName);
            }

            if(prerequisiteUpgrades != null)
            {
                if(prerequisiteUpgrades.Count > 0)
                {
                    List<Upgrade> newUpgradeReferences = new List<Upgrade>();
                    for(int i = 0; i < prerequisiteUpgrades.Count; i++)
                    {
                        newUpgradeReferences.Add(prerequisiteUpgrades[i]);
                    }

                    prerequisiteUpgrades.Clear();
                    prerequisiteUpgrades = newUpgradeReferences;
                }
            }
           
        }

    }

    public static Upgrade GetUpgrade(string abilityName, string upgradeName)
    {
        for(int i = 0; i < AllUpgrades.Count; i++)
        {
            if (AllUpgrades[i].Equals(abilityName, upgradeName))
            {
                return AllUpgrades[i];
            }
        }

        return null;
    }

    public static Ability GetAbility(string abilityName)
    {
        for(int i = 0 ; i < AllAbilities.Count; i++)
        {
            if (AllAbilities[i].Equals(abilityName))
            {
                return AllAbilities[i];
            }
        }

        return null;
    }

    public static List<Upgrade> AllUpgrades;
    public static List<Ability> AllAbilities;



    public static class ShadowDashUpgrades
    {
        public static string AbilityName = "ShadowDash";

        public static string Cooldown = "Cooldown";
        public static string Distance1 = "Distance1";
        public static string Distance2 = "Distance2";
        public static string ManaCost = "ManaCost";

    }
    public static class PredatorVisionUpgrades
    {
        public static string AbilityName = "PredatorVision";

        public static string Time1 = "Time1";
        public static string Time2 = "Time2";
        public static string Distance1 = "Distance1";
        public static string Distance2 = "Distance2";
        public static string ManaCost = "ManaCost";
    }
    public static class ShadowFormUpgrades
    {
        public static string AbilityName = "ShadowForm";

        public static string Time1 = "Time1";
        public static string Time2 = "Time2";
        public static string Speed = "Speed";
        public static string Jump = "Jump";
    }
    public static class ShadowSentinelUpgrades
    {
        public static string AbilityName = "ShadowSentinel";

        public static string KillAware = "KillAware";
        public static string SentinelVision = "SentinelVision";
        public static string MaxSentinels = "MaxSentinels";
        public static string ManaCost = "ManaCost";
    }
    public static class MirageUpgrades
    {
        public static string AbilityName = "Mirage";

        public static string ManaCost = "ManaCost";
    }
    public static class DarkHavenUpgrades
    {
        public static string AbilityName = "DarkHaven";

        public static string Size = "Size";
        public static string HavenPlus = "HavenPlus";
        public static string HavenTunnel = "HavenTunnel";
    }

    public static class ManaSlotsUpgrades
    {
        public static string AbilityName = "ManaSlots";

        public static string ManaSlots3 = "ManaSlots3";
        public static string ManaSlots4 = "ManaSlots4";
        public static string ManaSlots5 = "ManaSlots5";
        public static string ManaSlots6 = "ManaSlots6";
        public static string ManaSlots7 = "ManaSlots7";
        public static string ManaSlots8 = "ManaSlots8";
    }

    public static class ManaRegenUpgrades
    {
        public static string AbilityName = "ManaRegen";

        public static string ManaRegen1 = "ManaRegen1";
        public static string ManaRegen2 = "ManaRegen2";
        public static string ManaRegen3 = "ManaRegen3";
        public static string ManaRegen4 = "ManaRegen4";
    }

    public static class ViciousAssassinPerk
    {
        public static string AbilityName = "ViciousAssassin";
    }


    public static void FirstLoad()
    {
        AllAbilities = new List<Ability>()
        {
            new Ability("ShadowDash", true, 0),
            new Ability("PredatorVision"),
            new Ability("ShadowForm"),
            new Ability("ShadowSentinel"),
            new Ability("Mirage"),
            new Ability("DarkHaven"),
        };


        AllUpgrades = new List<Upgrade> {

        #region Ability Upgrades
        new Upgrade(ShadowDashUpgrades.AbilityName, ShadowDashUpgrades.Cooldown, 1),
        new Upgrade(ShadowDashUpgrades.AbilityName,ShadowDashUpgrades.Distance1, 1),
        new Upgrade(ShadowDashUpgrades.AbilityName, ShadowDashUpgrades.Distance2, 2),
        new Upgrade(ShadowDashUpgrades.AbilityName, ShadowDashUpgrades.ManaCost, 2),

        new Upgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.Time1, 1),
        new Upgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.Time2, 2),
        new Upgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.Distance1, 1),
        new Upgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.Distance2, 2),
        new Upgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.ManaCost, 1),

        new Upgrade(ShadowFormUpgrades.AbilityName, ShadowFormUpgrades.Time1, 2),
        new Upgrade(ShadowFormUpgrades.AbilityName, ShadowFormUpgrades.Time2, 3),
        new Upgrade(ShadowFormUpgrades.AbilityName, ShadowFormUpgrades.Speed, 2),
        new Upgrade(ShadowFormUpgrades.AbilityName, ShadowFormUpgrades.Jump, 2),

        new Upgrade(ShadowSentinelUpgrades.AbilityName, ShadowSentinelUpgrades.KillAware, 4),
        new Upgrade(ShadowSentinelUpgrades.AbilityName, ShadowSentinelUpgrades.SentinelVision, 2),
        new Upgrade(ShadowSentinelUpgrades.AbilityName, ShadowSentinelUpgrades.MaxSentinels, 2),
        new Upgrade(ShadowSentinelUpgrades.AbilityName, ShadowSentinelUpgrades.ManaCost, 1),

        new Upgrade(MirageUpgrades.AbilityName, MirageUpgrades.ManaCost, 1),

        new Upgrade(DarkHavenUpgrades.AbilityName, DarkHavenUpgrades.Size, 2),
        new Upgrade(DarkHavenUpgrades.AbilityName, DarkHavenUpgrades.HavenPlus, 2),
        new Upgrade(DarkHavenUpgrades.AbilityName, DarkHavenUpgrades.HavenTunnel, 4),
        #endregion




        #region Ability Independent Upgrades
        new Upgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots3, 1),
        new Upgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots4, 1),
        new Upgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots5, 2),
        new Upgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots6, 1),
        new Upgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots7, 1),
        new Upgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots8, 2),

        new Upgrade(ManaRegenUpgrades.AbilityName, ManaRegenUpgrades.ManaRegen1,1),
        new Upgrade(ManaRegenUpgrades.AbilityName, ManaRegenUpgrades.ManaRegen2,1),
        new Upgrade(ManaRegenUpgrades.AbilityName, ManaRegenUpgrades.ManaRegen3,2),
        new Upgrade(ManaRegenUpgrades.AbilityName, ManaRegenUpgrades.ManaRegen4,3),

        new Upgrade(ViciousAssassinPerk.AbilityName,ViciousAssassinPerk.AbilityName, 6),
        #endregion
    };



        #region Set Prerequisites and Excluders
        GetUpgrade(ShadowDashUpgrades.AbilityName, ShadowDashUpgrades.Distance2).
            AddPrerequisite(GetUpgrade(ShadowDashUpgrades.AbilityName, ShadowDashUpgrades.Distance1));

        GetUpgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.Time2).
            AddPrerequisite(GetUpgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.Time1));
        GetUpgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.Distance2).
           AddPrerequisite(GetUpgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.Distance1));


        GetAbility(ShadowFormUpgrades.AbilityName).SetPrerequisite(GetUpgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots3));

        GetUpgrade(ShadowFormUpgrades.AbilityName, ShadowFormUpgrades.Time2)
            .AddPrerequisite(GetUpgrade(ShadowFormUpgrades.AbilityName, ShadowFormUpgrades.Time1));
        GetUpgrade(ShadowFormUpgrades.AbilityName, ShadowFormUpgrades.Jump)
           .AddPrerequisite(GetUpgrade(ShadowFormUpgrades.AbilityName, ShadowFormUpgrades.Speed));


        GetAbility(ShadowSentinelUpgrades.AbilityName).SetPrerequisite(GetUpgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots3));

        GetUpgrade(ShadowSentinelUpgrades.AbilityName, ShadowSentinelUpgrades.MaxSentinels)
            .SetExcluder(GetUpgrade(ShadowSentinelUpgrades.AbilityName, ShadowSentinelUpgrades.SentinelVision));
        GetUpgrade(ShadowSentinelUpgrades.AbilityName, ShadowSentinelUpgrades.SentinelVision)
           .SetExcluder(GetUpgrade(ShadowSentinelUpgrades.AbilityName, ShadowSentinelUpgrades.MaxSentinels));

        GetUpgrade(DarkHavenUpgrades.AbilityName, DarkHavenUpgrades.HavenTunnel)
            .AddPrerequisite(GetUpgrade(DarkHavenUpgrades.AbilityName, DarkHavenUpgrades.HavenPlus));

        GetUpgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots4)
            .AddPrerequisite(GetUpgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots3));
        GetUpgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots5)
            .AddPrerequisite(GetUpgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots4));
        GetUpgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots6)
            .AddPrerequisite(GetUpgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots5));
        GetUpgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots7)
            .AddPrerequisite(GetUpgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots6));
        GetUpgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots8)
            .AddPrerequisite(GetUpgrade(ManaSlotsUpgrades.AbilityName, ManaSlotsUpgrades.ManaSlots7));

        GetUpgrade(ManaRegenUpgrades.AbilityName, ManaRegenUpgrades.ManaRegen2)
            .AddPrerequisite(GetUpgrade(ManaRegenUpgrades.AbilityName, ManaRegenUpgrades.ManaRegen1));
        GetUpgrade(ManaRegenUpgrades.AbilityName, ManaRegenUpgrades.ManaRegen3)
            .AddPrerequisite(GetUpgrade(ManaRegenUpgrades.AbilityName, ManaRegenUpgrades.ManaRegen2));
        GetUpgrade(ManaRegenUpgrades.AbilityName, ManaRegenUpgrades.ManaRegen4)
            .AddPrerequisite(GetUpgrade(ManaRegenUpgrades.AbilityName, ManaRegenUpgrades.ManaRegen3));

        #endregion

        #region Set Required Abilities
        GetUpgrade(ShadowDashUpgrades.AbilityName, ShadowDashUpgrades.Distance1).SetRequiredAbility(GetAbility(ShadowDashUpgrades.AbilityName));
        GetUpgrade(ShadowDashUpgrades.AbilityName, ShadowDashUpgrades.Distance2).SetRequiredAbility(GetAbility(ShadowDashUpgrades.AbilityName));
        GetUpgrade(ShadowDashUpgrades.AbilityName, ShadowDashUpgrades.Cooldown).SetRequiredAbility(GetAbility(ShadowDashUpgrades.AbilityName));
        GetUpgrade(ShadowDashUpgrades.AbilityName, ShadowDashUpgrades.ManaCost).SetRequiredAbility(GetAbility(ShadowDashUpgrades.AbilityName));

        GetUpgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.Distance1).SetRequiredAbility(GetAbility(PredatorVisionUpgrades.AbilityName));
        GetUpgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.Distance2).SetRequiredAbility(GetAbility(PredatorVisionUpgrades.AbilityName));
        GetUpgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.Time1).SetRequiredAbility(GetAbility(PredatorVisionUpgrades.AbilityName));
        GetUpgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.Time2).SetRequiredAbility(GetAbility(PredatorVisionUpgrades.AbilityName));
        GetUpgrade(PredatorVisionUpgrades.AbilityName, PredatorVisionUpgrades.ManaCost).SetRequiredAbility(GetAbility(PredatorVisionUpgrades.AbilityName));

        GetUpgrade(ShadowFormUpgrades.AbilityName, ShadowFormUpgrades.Time1).SetRequiredAbility(GetAbility(ShadowFormUpgrades.AbilityName));
        GetUpgrade(ShadowFormUpgrades.AbilityName, ShadowFormUpgrades.Time2).SetRequiredAbility(GetAbility(ShadowFormUpgrades.AbilityName));
        GetUpgrade(ShadowFormUpgrades.AbilityName, ShadowFormUpgrades.Speed).SetRequiredAbility(GetAbility(ShadowFormUpgrades.AbilityName));
        GetUpgrade(ShadowFormUpgrades.AbilityName, ShadowFormUpgrades.Jump).SetRequiredAbility(GetAbility(ShadowFormUpgrades.AbilityName));

        GetUpgrade(ShadowSentinelUpgrades.AbilityName, ShadowSentinelUpgrades.KillAware).SetRequiredAbility(GetAbility(ShadowSentinelUpgrades.AbilityName));
        GetUpgrade(ShadowSentinelUpgrades.AbilityName, ShadowSentinelUpgrades.SentinelVision).SetRequiredAbility(GetAbility(ShadowSentinelUpgrades.AbilityName));
        GetUpgrade(ShadowSentinelUpgrades.AbilityName, ShadowSentinelUpgrades.MaxSentinels).SetRequiredAbility(GetAbility(ShadowSentinelUpgrades.AbilityName));
        GetUpgrade(ShadowSentinelUpgrades.AbilityName, ShadowSentinelUpgrades.ManaCost).SetRequiredAbility(GetAbility(ShadowSentinelUpgrades.AbilityName));

        GetUpgrade(MirageUpgrades.AbilityName, MirageUpgrades.ManaCost).SetRequiredAbility(GetAbility(MirageUpgrades.AbilityName));

        GetUpgrade(DarkHavenUpgrades.AbilityName, DarkHavenUpgrades.Size).SetRequiredAbility(GetAbility(DarkHavenUpgrades.AbilityName));
        GetUpgrade(DarkHavenUpgrades.AbilityName, DarkHavenUpgrades.HavenPlus).SetRequiredAbility(GetAbility(DarkHavenUpgrades.AbilityName));
        GetUpgrade(DarkHavenUpgrades.AbilityName, DarkHavenUpgrades.HavenTunnel).SetRequiredAbility(GetAbility(DarkHavenUpgrades.AbilityName));


        #endregion

        SaveUpgradeState();
    }

    public static void Load()
    {
        string saveLoc1 = Application.persistentDataPath + "/upgrades.data";
        string saveLoc2 = Application.persistentDataPath + "/abilities.data";
        if (File.Exists(saveLoc1) && File.Exists(saveLoc2))
        {
            FileStream fileStream1 = new FileStream(saveLoc1, FileMode.Open, FileAccess.Read);
            FileStream fileStream2 = new FileStream(saveLoc2, FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            AllUpgrades = bf.Deserialize(fileStream1) as List<Upgrade>;
            AllAbilities = bf.Deserialize(fileStream2) as List<Ability>;
            fileStream1.Close();
            fileStream2.Close();
        }
        else
        {
            FirstLoad();
        }
    }

    public static void SaveUpgradeState()
    {
        string saveLoc1 = Application.persistentDataPath + "/upgrades.data";
        string saveLoc2 = Application.persistentDataPath + "/abilities.data";
        FileStream fileStream1 = new FileStream(saveLoc1, FileMode.Create);
        FileStream fileStream2 = new FileStream(saveLoc2, FileMode.Create);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fileStream1, AllUpgrades);
        bf.Serialize(fileStream2, AllAbilities);
        fileStream1.Close();
        fileStream2.Close();

    }

    public static int ConvertBoolToInt(bool _value)
    {
        if( _value)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }


    public static void SetAbilityUnlocked(string abilityName)
    {
        GetAbility(abilityName).SetUnlocked();
    }

}
