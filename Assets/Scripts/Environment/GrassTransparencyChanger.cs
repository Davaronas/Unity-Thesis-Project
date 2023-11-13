using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTransparencyChanger : MonoBehaviour
{
    [SerializeField] private Material opaqueMat;
    [SerializeField] private Material transparentMat;

    private Renderer[] renderers = null;
    private NaturalHidingPlace hidingPlace = null;

    private bool opaque = true;

    // Start is called before the first frame update
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        hidingPlace = GetComponentInChildren<NaturalHidingPlace>();
        hidingPlace.OnHidingActive += SetTransparent ;
        hidingPlace.OnHidingBroken += SetOpaque;

        PlayerActions.OnPlayerDash += SetOpaque;
    }

    private void OnDestroy()
    {
        hidingPlace.OnHidingActive -= SetTransparent;
        hidingPlace.OnHidingBroken -= SetOpaque;

        PlayerActions.OnPlayerDash -= SetOpaque;
    }

    private void SetOpaque()
    {
        

        if(opaque ) { return; }

        opaque = true;

        for(int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = opaqueMat;
        }
    }

    private void SetTransparent()
    {
        

        if (!opaque ) { return; }

        opaque = false;

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = transparentMat;
        }
    }


}
