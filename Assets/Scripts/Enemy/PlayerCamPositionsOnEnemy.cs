using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamPositionsOnEnemy : MonoBehaviour
{
    [SerializeField] private List<Transform> transforms;


    private void Start()
    {
        foreach (Transform tr in transform)
        {
            transforms.Add(tr);
        }

    }

    public Transform GetTransformById(int _id)
    {
        return transforms[_id];
    }
}
