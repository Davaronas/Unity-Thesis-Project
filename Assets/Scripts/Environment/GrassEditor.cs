using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassEditor : MonoBehaviour
{
    public int asd1 = 5;
    public float randomSizeRangeMin = 0.8f;
    public float randomSizeRangeMax = 1.2f;


    void Start()
    {
        if (!Application.isEditor)
        {
            Destroy(transform.Find("PlacePoint").gameObject);
            Destroy(this);
        }
    }

}
