using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DEBUG_script : MonoBehaviour
{



    void Start()
    {
        UniversalAdditionalCameraData acd = GetComponent<UniversalAdditionalCameraData>();
        acd.SetRenderer(1);
    }


    void Update()
    {
        
    }
}
