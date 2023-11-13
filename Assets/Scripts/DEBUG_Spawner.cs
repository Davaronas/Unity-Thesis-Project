using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private EnemyBehaviour currentEb;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentEb.IsIncapacitated())
        {
            currentEb = Instantiate(enemyPrefab, transform.position, Quaternion.identity).GetComponent<EnemyBehaviour>();
        }
    }
}
