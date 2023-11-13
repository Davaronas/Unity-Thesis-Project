using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public static class LevelSystem 
{



    public static int experience = 0;
    public static int level = 1;
    public static int shadowTokens = 0;

    public static event Action OnShadowTokenChanged;
    


    private static int experienceNeededForLevelUp = 250;


    public static void Load()
    {
        level = PlayerPrefs.GetInt("level", 1);
        shadowTokens = PlayerPrefs.GetInt("shadowTokens", 0);
        experience = PlayerPrefs.GetInt("experience", 0);
    }

    public static void ResetLevel()
    {
        level = 1;
        shadowTokens = 0;
        experience = 0;
        

        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetInt("shadowTokens", shadowTokens);
        PlayerPrefs.SetInt("experience", experience);
    }

    public static int GetExperienceNeededForNextLevelUp()
    {
        return experienceNeededForLevelUp * level;
    }


    public static void LevelUp()
    {
        experience += (level * experienceNeededForLevelUp) - experience;

        if (experience >= experienceNeededForLevelUp * level)
        {
            level++;
            shadowTokens++;
            experience = 0;

            PlayerPrefs.SetInt("level",level);
            PlayerPrefs.SetInt("shadowTokens",shadowTokens);
            PlayerPrefs.SetInt("experience", experience);
        }
    }



    public static void GainExperience(int _amount)
    {
        experience += _amount;

        while (experience >= experienceNeededForLevelUp * level)
        {
            experience -= experienceNeededForLevelUp * level;
            shadowTokens++;
            level++;
        }


        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetInt("shadowTokens", shadowTokens);
        PlayerPrefs.SetInt("experience", experience);
    }




    public static bool HasEnoughShadowTokens(int _amount)
    {
        if(shadowTokens >= _amount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void RemoveShadowTokens(int _amount)
    {
        shadowTokens -= _amount;
        PlayerPrefs.SetInt("shadowTokens", shadowTokens);
        OnShadowTokenChanged?.Invoke();
    }

    public static void AddShadowTokens(int _amount)
    {
        shadowTokens += _amount;
        PlayerPrefs.SetInt("shadowTokens", shadowTokens);
        OnShadowTokenChanged?.Invoke();
    }
}
