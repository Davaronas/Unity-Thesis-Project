using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class DarkHavenObject : MonoBehaviour
{

    private SplineContainer sc;

    private bool locked = false;

    private void Awake()
    {
        sc = GetComponent<SplineContainer>();
    }

    public static event Action<SplineContainer, GameObject> OnDarkHavenObjectTouched;

    private void OnTriggerEnter(Collider other)
    {
        if(locked) return;

        if(other.gameObject.layer != 8) // player layer
        {
            
            StartCoroutine(DestroyObjectNextFrame());
            locked = true;
        }
    }

    IEnumerator DestroyObjectNextFrame()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        OnDarkHavenObjectTouched?.Invoke(sc, gameObject);
        Destroy(gameObject);
    }
}
