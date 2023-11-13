using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Door : MonoBehaviour
{
    private Interactable interactable;

    [SerializeField] private Vector3 openRotation;
    [SerializeField] private Vector3 closedRotation;
    [SerializeField] private float transitionSpeed;

    [SerializeField] private bool closed = true;

    [SerializeField] private bool enemyOpenWithContact = true;

    void Start()
    {
        interactable = GetComponent<Interactable>();

        interactable.OnInteract += OpenOrClose;


        StartCoroutine(transition(!closed));
    }

    private void OnDestroy()
    {

        interactable.OnInteract -= OpenOrClose;

    }

    private void OpenOrClose()
    {
        StartCoroutine(transition(closed));
        closed = !closed;
    }

    IEnumerator transition(bool _state)
    {
        float _time = 0;
        Quaternion _startRot = transform.localRotation;

        if(_state) // open
        {
            while(_time < 1)
            {
                transform.localRotation = Quaternion.Lerp(_startRot, Quaternion.Euler(openRotation), _time);
                _time += Time.deltaTime * transitionSpeed;
                yield return new WaitForEndOfFrame();
            }
        } 
        else // close
        {
            while (_time < 1)
            {
                transform.localRotation = Quaternion.Lerp(_startRot, Quaternion.Euler(closedRotation), _time);
                _time += Time.deltaTime * transitionSpeed;
                yield return new WaitForEndOfFrame();
            }
        }

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) // enemy layer
        {
            StartCoroutine(transition(true));
            closed = false;
        }
    }
}
