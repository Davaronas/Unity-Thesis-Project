using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBoundaryCheck : MonoBehaviour
{
    private const float deleteIfExceedsThisNumber = 1000;

    void Start()
    {
        StartCoroutine(CheckIfFallenOutOfWorld());
    }


    IEnumerator CheckIfFallenOutOfWorld()
    {
        while(true)
        {
            if(transform.position.y < -deleteIfExceedsThisNumber)
            {
                Destroy(gameObject);
                break;
            }

            yield return new WaitForSeconds(5f);
        }

        yield return null;
    }
}
