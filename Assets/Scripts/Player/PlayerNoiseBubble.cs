using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNoiseBubble : MonoBehaviour
{
    private PlayerActions pa;

    void Start()
    {
        pa = transform.root.GetComponent<PlayerActions>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyCommands>() != null)
        {
            pa.AddEnemyInsideNoiseBubble(other.GetComponent<EnemyCommands>());
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.GetComponent<EnemyCommands>() != null)
        {
            pa.RemoveEnemyInsideNoiseBubble(other.GetComponent<EnemyCommands>());
        }
    }
}
