using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarryObject : MonoBehaviour
{
    public event Action OnPickedUp;
    public event Action OnPutDown;

    public void PickedUp()
    {
        OnPickedUp?.Invoke();
        transform.root.gameObject.SetActive(false);
    }

    public void PutDown(Vector3 _newPos,Quaternion _newRot)
    {
        transform.root.position = _newPos;
        transform.root.rotation = _newRot;
        transform.root.gameObject.SetActive(true);
        OnPutDown?.Invoke();
    }
}
