using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private Objective[] objectives = null;
    [SerializeField] private float timeToLoadNextMap = 3f;

    public event Action<string> OnObjectiveChanged;

    public Objective[] GetObjectives() { return objectives; }

    public Objective[] GetVisibleCurrentObjectives()
    {
        List<Objective> _visibleObjectives = new List<Objective>();
        for(int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i].visible && !objectives[i].isDone && !objectives[i].isFailed)
            {
                _visibleObjectives.Add(objectives[i]);
            }
        }

        return _visibleObjectives.ToArray();
    }

    public Objective FindObjective(int _id)
    {

        for (int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i].id == _id)
            {
                return objectives[i];
            }
        }

        return null;
    }

    public bool ArePrerequisitesDone(Objective _obj)
    {
        if (_obj.prerequisites == null)
        {
            return true;
        }
        else if (_obj.prerequisites.Length == 0)
        {
            return true;
        }

        for (int i = 0; i < _obj.prerequisites.Length; i++)
        {
            if (!FindObjective(_obj.prerequisites[i]).isDone)
            {
                return false;
            }

            /*
            if (!_obj.overrideAndCompletePrerequisites)
            {

                if (!FindObjective(_obj.prerequisites[i]).isDone)
                {
                    return false;
                }
            }
            else
            {
                FindObjective(_obj.prerequisites[i]).SetDone();
            }
            */
        }

        return true;
    }

    public bool CompleteObjective(int _id)
    {
        Objective _obj = FindObjective(_id);
        print(_obj.description);
        if(_obj.isDone) { return false; }

        if (_obj.overrideAndCompletePrerequisites)
        {
            for (int i = 0; i < _obj.prerequisites.Length; i++)
            {
                FindObjective(_obj.prerequisites[i]).SetDone();
            }
            
        }

        if (ArePrerequisitesDone(_obj))
        {
            _obj.Completed();
            

            if (_obj.isDone && _obj.endsMission)
            {
                Invoke(nameof(InvokeWinScreen), timeToLoadNextMap);
            }

            UpdateVisiblity();

            OnObjectiveChanged?.Invoke((_obj.isDone ? "Completed" : ("Completed (" + _obj.completedAmount + "/" + _obj.amount + ")")) + ": " + _obj.description);
            return true;
        }
        else
        {
            return false;
        }
       
    }

    private void InvokeWinScreen()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(7);
    }

    private void UpdateVisiblity()
    {
        for (int i = 0; i < objectives.Length; i++)
        {
            if(ArePrerequisitesDone(objectives[i]))
            {
                objectives[i].visible = true;
            }
            else
            {
                objectives[i].visible = false;
            }
        }
    }

    public void UncompleteObjective(int _id)
    {
        Objective _obj = FindObjective(_id);
        _obj.Uncompleted();

        UpdateVisiblity();

        OnObjectiveChanged?.Invoke("Uncompleted: " + _obj.description);
    }

    public void FailObjective(int _id)
    {
        Objective _obj = FindObjective(_id);
        _obj.SetFailed();
        UpdateVisiblity();

        OnObjectiveChanged?.Invoke("Failed: " + _obj.description);
    }


}
