using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    [SerializeField] private string displayText = "Interact";
    [SerializeField] private float blockInteractionTimeAfterInteract = 1f;
    [SerializeField] private bool oneWayInteraction = false;
    public event Action OnInteract;
    private bool canInteract = true;



    public void Interact()
    {
        if (canInteract)
        {
            OnInteract?.Invoke();

            if (oneWayInteraction)
            {
                BlockInteraction(0);
            }
            else
            {
                BlockInteraction(blockInteractionTimeAfterInteract);
            }
        }
    }



    public string GetDisplayText()
    {
        return displayText;
    }
 
    public void BlockInteraction(float _duration = 0)
    {
        canInteract = false;

        if(_duration > 0)
        { 
            Invoke(nameof(EnableInteraction), _duration);
        }
    }

    public bool CanInteract()
    {
        return canInteract;
    }

    public void EnableInteraction()
    {
        canInteract = true;
    }

}
