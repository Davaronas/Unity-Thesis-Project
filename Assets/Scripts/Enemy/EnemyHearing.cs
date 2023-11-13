using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHearing : MonoBehaviour
{
    private EnemyCommands thisEnemy;
    private List<EnemyCommands> otherEnemiesInHearing;
    private EnemyVisionCone visionCone;


    void Start()
    {
        thisEnemy = transform.root.GetComponent<EnemyCommands>();
        visionCone = thisEnemy.GetComponentInChildren<EnemyVisionCone>();

        otherEnemiesInHearing = new List<EnemyCommands>();
    }

    public void TriggerOtherEnemiesToInvestigate(Vector3 _pos)
    {
        for(int i = 0; i < otherEnemiesInHearing.Count; i++)
        {
            if (!otherEnemiesInHearing[i].IsIncapacitated())
                otherEnemiesInHearing[i].InvestigateWithOtherEnemy(_pos);
        }


        // shouldn't trigger an immediate max awareness from visual only
        List<EnemyCommands> _ecVisual = visionCone.GetOtherEnemiesInSight();
        for (int i = 0; i < _ecVisual.Count; i++)
        {
            if (!_ecVisual[i].IsIncapacitated())
                _ecVisual[i].InvestigateWithOtherEnemy(_pos);
        }
    }

    public void TriggerOtherEnemiesMaxAwareness(EnemyCommands _ec)
    {
        for (int i = 0; i < otherEnemiesInHearing.Count; i++)
        {
            if (!otherEnemiesInHearing[i].IsIncapacitated())
            {
                otherEnemiesInHearing[i].InvestigateWithOtherEnemy(_ec.transform.position);
                otherEnemiesInHearing[i].SetOtherEnemyAwarenessToMax(_ec.transform.position);
            }
        }

        // only investigate shouldn't immediately attack
        List<EnemyCommands> _ecVisual = visionCone.GetOtherEnemiesInSight();
        for (int i = 0; i < _ecVisual.Count; i++)
        {
            if (!_ecVisual[i].IsIncapacitated())
                _ecVisual[i].InvestigateWithOtherEnemy(_ec.transform.position);
        }
    }

    public bool IsEnemyInHearing(EnemyCommands _ec)
    {
        return otherEnemiesInHearing.Contains(_ec);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) // Alive enemy
        {
            EnemyCommands _ec = other.GetComponent<EnemyCommands>();
            if(_ec == null) { return; }

            if(!otherEnemiesInHearing.Contains(_ec))
            {
                otherEnemiesInHearing.Add(_ec);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6) // Alive enemy
        {
            EnemyCommands _ec = other.GetComponent<EnemyCommands>();
            if (_ec == null) { return; }

            otherEnemiesInHearing.Remove(_ec);
        }

    }
}
