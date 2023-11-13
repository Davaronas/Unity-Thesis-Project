using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturalHidingPlace : MonoBehaviour
{
    [SerializeField] private float reactivateTime = 3f;
    private Collider[] cols = null;

    private float timer = 0;
    private Coroutine timerCor = null;

    public event Action OnHidingActive;
    public event Action OnHidingBroken;

    private void Start()
    {
        cols = GetComponents<Collider>();
        timer = reactivateTime;
    }

    

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 8 && timer >= reactivateTime) // player is inside and timer is not counting
        {
            if(OnHidingActive != null)
            OnHidingActive?.Invoke();
        }

        if (other.gameObject.layer != 6) { return; } // enemy
        if (other.GetComponent<EnemyBehaviour>().IsIncapacitated()) { return; }

        timer = 0;

        if(timerCor != null)
        {
            StopCoroutine(timerCor);
        }

        timerCor = StartCoroutine(ReactivationTimer());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8) // player is inside and timer is not counting
        {
            if(OnHidingBroken != null)
            OnHidingBroken?.Invoke();
        }
    }


    private IEnumerator ReactivationTimer()
    {
        if(OnHidingBroken != null)
        OnHidingBroken?.Invoke();

        for(int i = 0; i < cols.Length; i++)
        {
            cols[i].enabled = false;
        }

        print("While is true" + (timer < reactivateTime));
        while(timer < reactivateTime)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        ReactivateCollider();
        timerCor = null;
        yield return null;
    }

    private void ReactivateCollider()
    {
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].enabled = true;
        }
    }
}
