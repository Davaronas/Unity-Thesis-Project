using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallProtect : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            PlayerActions _pa = other.gameObject.GetComponent<PlayerActions>();
            _pa.TeleportToLastGroundedLocation();
            _pa.ReceiveDamage(damage, null);
            

        }
    }
}
