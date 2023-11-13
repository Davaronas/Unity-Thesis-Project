using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirage : MonoBehaviour
{
    void Awake()
    {
        Vector3 _eulers = new Vector3(0, Random.Range(0, 360), 0);
        transform.rotation = Quaternion.Euler(_eulers);
    }

    public void AE_DestroyMirage()
    {
        Destroy(gameObject);
    }

}
