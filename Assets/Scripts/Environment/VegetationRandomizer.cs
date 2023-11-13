using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationRandomizer : MonoBehaviour
{

    [SerializeField] private float minSize = 0.8f;
    [SerializeField] private float maxSize = 1.2f;


    [ContextMenu("Randomize")]
    public void Randomize()
    {
        float _r = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(_r, _r, _r);

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Random.Range(0, 361), transform.rotation.eulerAngles.z);
    }
}
