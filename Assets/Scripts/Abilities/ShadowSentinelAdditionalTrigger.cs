using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSentinelAdditionalTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    private ShadowSentinel ss;


    public void SetOwner(ShadowSentinel _ss)
    {
        ss = _ss;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(ss == null) { return; }

        ss.SetMidAirBool();
        ss.OnTriggerEnter(other);
    }
}
