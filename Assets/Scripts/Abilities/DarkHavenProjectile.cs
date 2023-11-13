using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkHavenProjectile : MonoBehaviour
{
    [SerializeField] private GameObject darkHavenObject;
    [SerializeField] private float overlapSphereEnemyCheckRadius = 2.2f;

    private Rigidbody rb;
    private Collider cl;
    private TrailRenderer tr;
    private MeshRenderer mr;

    public static event Action<GameObject> OnDarkHavenCreated;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cl = GetComponent<Collider>();
        tr = GetComponent<TrailRenderer>();
        mr = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(rb);
        Destroy(cl);
        mr.enabled = false;
        Destroy(gameObject, tr.time);

        Collider[] _colsHit = Physics.OverlapSphere(collision.GetContact(0).point, overlapSphereEnemyCheckRadius);
        
        for (int i = 0; i < _colsHit.Length; i++) 
        { 
            if(_colsHit[i].gameObject.layer == 6)
            {
                return;
            }

        } // 6 is enemy layer

        

        OnDarkHavenCreated?.Invoke(Instantiate(darkHavenObject, collision.GetContact(0).point, Quaternion.identity));

    }
}
