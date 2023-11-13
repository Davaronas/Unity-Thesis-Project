using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathObjective : MonoBehaviour
{
    [SerializeField] private int objectiveId = -1;
    [SerializeField] private bool objectiveStateSet = true;


    private EnemyCommands enemy = null;
    private ObjectiveManager objectiveManager = null;



    void Start()
    {
        enemy = GetComponent<EnemyBehaviour>();
        objectiveManager = FindObjectOfType<ObjectiveManager>();
        enemy.OnEnemyDeath += SetObjective;
    }



    private void OnDestroy()
    {
        enemy.OnEnemyDeath -= SetObjective;
        if(objectiveStateSet == false && !objectiveManager.FindObjective(objectiveId).isFailed) 
        {
            objectiveManager.CompleteObjective(objectiveId);
        }
    }



    private void SetObjective()
    {
        if (objectiveStateSet)
        {
            objectiveManager.CompleteObjective(objectiveId);
        }
        else
        {
            objectiveManager.FailObjective(objectiveId);
        }

    }
}
