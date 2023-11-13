using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PositionalObjective : MonoBehaviour
{
    [SerializeField] private int objectiveId = -1;
    [SerializeField] private bool objectiveStateSet = true;
    [Space]
    [SerializeField] private GameObject gameObjectNeeded = null;
    
    [SerializeField] private bool oneWayCompletion = true;


    private ObjectiveManager objectiveManager = null;
    private bool completed = false;



    void Start()
    {
        objectiveManager = FindObjectOfType<ObjectiveManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(completed) { return; }

        if(other.gameObject == gameObjectNeeded)
        {
            if (SetObjective(objectiveStateSet))
            {
                completed = true;
            }

        }
    }

    private void FixedUpdate()
    {
        if(oneWayCompletion) { return; }

        if(!gameObjectNeeded.activeSelf && completed)
        {
            completed = false;
            SetObjective(!objectiveStateSet);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(oneWayCompletion) { return; }

        if (other.gameObject == gameObjectNeeded)
        {
            SetObjective(!objectiveStateSet);
            completed = false;
        }
    }

    private bool SetObjective(bool _objectiveStateSet)
    {


        if (_objectiveStateSet)
        {
           return objectiveManager.CompleteObjective(objectiveId);
        }
        else
        {
            objectiveManager.UncompleteObjective(objectiveId);
            return true;
        }

    }


}
