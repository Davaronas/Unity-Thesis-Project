using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WorldBoundaryCheck))]
public class PropPhysicsArranger : MonoBehaviour
{
    [SerializeField] private float endTime = 5f;
    [SerializeField] private bool disableCollider = true;
    [SerializeField] private bool useRandomForce = true;
    [SerializeField] private float randomForceMax = 2f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        float _x = Random.Range(-randomForceMax, randomForceMax);
        float _y = Random.Range(-randomForceMax, randomForceMax);
        float _z = Random.Range(-randomForceMax, randomForceMax);

        rb.velocity = new Vector3 (_x,_y,_z);
        Invoke(nameof(EndPhysics), endTime);
    }

    private void EndPhysics()
    {
        if (disableCollider)
        {
            Collider[] _cols = GetComponentsInChildren<Collider>();
            for (int i = 0; i < _cols.Length; i++)
            {
                Destroy(_cols[i]);
            }
        }

        Destroy(rb);
    }
}
