using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Interactable))]
public class InteractableObjective : MonoBehaviour
{
    [SerializeField] private int objectiveId = -1;
    [SerializeField] private bool objectiveStateSet = true;
    [SerializeField] private bool destroyOnInteract = true;

    private Interactable interactable = null;
    private ObjectiveManager objectiveManager = null;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        objectiveManager = FindObjectOfType<ObjectiveManager>();
        interactable.OnInteract += SetObjective;
    }

    private void OnDestroy()
    {
        interactable.OnInteract -= SetObjective;
    }

    private void SetObjective()
    {
        if (objectiveStateSet)
        {
            objectiveManager.CompleteObjective(objectiveId);
        }
        else
        {
            objectiveManager.UncompleteObjective(objectiveId);
        }

        if(destroyOnInteract)
        {
            Destroy(gameObject);
        }
    }

}
