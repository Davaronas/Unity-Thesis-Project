using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DamageArea : MonoBehaviour
{
    [SerializeField] private float interval = 1f;
    [SerializeField] private float damage = 10f;

    private ParticleSystem ps = null;

    private bool canDamage = true;

    private void Awake()
    {
        ps = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 8)  // player
        {
            if (canDamage)
            {
                other.GetComponent<PlayerActions>().ReceiveDamage(damage, null);
                canDamage = false;
                Invoke(nameof(EnableDamage), interval);
            }
        }
    }

    private void EnableDamage()
    {
        canDamage = true;
    }

    public void DetachParticleSystem()
    {
        if(ps != null)
        {
            ps.Stop();
            ps.transform.SetParent(null, true);
            Destroy(ps, ps.main.startLifetime.constant);
        }
    }

}
