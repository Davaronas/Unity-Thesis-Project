using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class Objective 
{
    public int id;
    public string description;
    public bool visible;
    public int experienceGain;
    public bool isDone;
    public bool isFailed;
    public int completedAmount = 0;
    public int amount = 1;
    public bool endsMission = false;
    public int[] prerequisites = null;
    public bool overrideAndCompletePrerequisites = true;
    public bool mainMission = false;

    public static bool operator ==(Objective obj1, Objective obj2) => obj1.id == obj2.id;

    public static bool operator !=(Objective obj1, Objective obj2) => !(obj1.id == obj2.id);
    

    public Objective(int id, string description, bool endsMission = false,
        bool visible = false, int experienceGain = 100, int amount = 1, bool isDone = false,
        int[] prerequisites = null, bool overridePrerequisites = true, bool mainMission = false)
    {
        this.id = id;
        this.description = description;
        this.visible = visible;
        this.experienceGain = experienceGain;
        this.isDone = isDone;
        this.endsMission = endsMission;
        completedAmount = 0;
        this.amount = amount;
        completedAmount = amount;
        this.prerequisites = prerequisites;
        isFailed = false;
        overrideAndCompletePrerequisites = overridePrerequisites;
        this.mainMission = mainMission;
    }

    public void Completed()
    {
        if(completedAmount < amount)
        completedAmount++;

        if(completedAmount >= amount)
        {
            isDone = true;
        }

       
    }

    public void Uncompleted()
    {
        completedAmount--;

        if (completedAmount < amount)
        {
            isDone = false;
        }
    }

    public void SetDone(bool isDone = true)
    {
        this.isDone = isDone;
    }

    public void SetFailed()
    {
        isFailed = true;
    }
}
