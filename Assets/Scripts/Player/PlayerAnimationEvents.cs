using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerActions pa;
    private PlayerMovement pm;

    private void Start()
    {
        pa = transform.root.GetComponent<PlayerActions>();
        pm = pa.GetComponent<PlayerMovement>();
    }

    public void AE_Vision()
    {
        pa.AE_VisionActivated();
    }

    public void AE_ShadowForm()
    {
        pa.AE_ShadowFormActivated();
    }

    public void AE_ResetRightHandSprintingAnimation()
    {
        pm.AE_ResetRightHandSprintingAnimation();
    }

    public void AE_ShadowSentinel()
    {
        pa.AE_ShadowSentinelActivated();
    }

    public void AE_ParryWindowStart()
    {
        pa.AE_ParryActive();
    }

    public void AE_ParryWindowEnd()
    {
        pa.AE_ParryNotActive();
    }

    public void AE_ParryEnded()
    {
        pa.AE_ParryEnd();
    }

    public void AE_DarkHaven()
    {
        pa.AE_DarkHavenActivated();
    }

    public void AE_AttackParried()
    {

    }


}
