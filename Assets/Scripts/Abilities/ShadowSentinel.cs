using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSentinel : MonoBehaviour
{
    private Animator animator;
    private EnemyBehaviour enemy;
    private PlayerMovement pm;
    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float positionAmount = 0.8f;
    [Space]
    [SerializeField] private GameObject colliderPrefab;
    [SerializeField] private float maxDropAssassinationCheckDistance = 20f;
    [SerializeField] private LayerMask layersToCheck;
    [SerializeField] private float minDistanceToCheckForForwardDropAssassination = 3f;
    [Space]
    [SerializeField] private GameObject sentinelCamera;
    [Space]
    [SerializeField] private Material activatedMat;
    private bool activated = false;
    private bool alwaysLethal = false;
    private bool goAllTheWay = false;


    private List<SphereCollider> colliders;
    private float colliderRadius;

    private Quaternion originalRot_;
    private Vector3 targetRotEuler_;
    private Quaternion targetRot_;
    private Vector3 _targetPos;

    private Ray ray_;
    private SphereCollider newCollider;
    private SkinnedMeshRenderer mr;

    public static event Action<ShadowSentinel> OnShadowSentinelDestroyed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        pm = FindObjectOfType<PlayerMovement>();
        mr = GetComponentInChildren<SkinnedMeshRenderer>();

        // position drop assassination checks


        // position drop assassination check only below
        // if sentinel is in the air

        colliders = new List<SphereCollider>
        {
            GetComponent<SphereCollider>()
        };
        colliderRadius = colliders[0].radius;



        animator.SetBool("Grounded", pm.IsGrounded());

        if(!pm.IsGrounded())
        {
            ray_ = new Ray(transform.position, Vector3.down);
            if(Physics.Raycast(ray_,out RaycastHit _hitInfo,maxDropAssassinationCheckDistance,layersToCheck))
            {
                newCollider = Instantiate(colliderPrefab, _hitInfo.point, Quaternion.identity, transform).GetComponent<SphereCollider>();
                newCollider.GetComponent<ShadowSentinelAdditionalTrigger>().SetOwner(this);
                newCollider.radius = colliderRadius;
                colliders.Add(newCollider);
            }
        }
        else
        {
            ray_ = new Ray(transform.position + transform.forward, Vector3.down);
            if (Physics.Raycast(ray_, out RaycastHit _hitInfo, maxDropAssassinationCheckDistance, layersToCheck))
            {
                if (_hitInfo.distance > minDistanceToCheckForForwardDropAssassination)
                {
                    newCollider = Instantiate(colliderPrefab, _hitInfo.point, Quaternion.identity, transform).GetComponent<SphereCollider>();
                    newCollider.GetComponent<ShadowSentinelAdditionalTrigger>().SetOwner(this);
                    newCollider.radius = colliderRadius;
                    colliders.Add(newCollider);
                }
                else
                {
                    ray_ = new Ray(transform.position + (transform.forward * 2), Vector3.down);
                    if (Physics.Raycast(ray_, out RaycastHit _hitInfo2, maxDropAssassinationCheckDistance, layersToCheck))
                    {
                        if (_hitInfo2.distance > minDistanceToCheckForForwardDropAssassination)
                        {
                            newCollider = Instantiate(colliderPrefab, _hitInfo2.point, Quaternion.identity, transform).GetComponent<SphereCollider>();
                            newCollider.GetComponent<ShadowSentinelAdditionalTrigger>().SetOwner(this);
                            newCollider.radius = colliderRadius;
                            colliders.Add(newCollider);
                        }
                        else
                        {
                            /*
                            ray_ = new Ray(transform.position + (transform.forward * 3), Vector3.down);
                            if (Physics.Raycast(ray_, out RaycastHit _hitInfo3, maxDropAssassinationCheckDistance, layersToCheck))
                            {
                                if (_hitInfo3.distance > minDistanceToCheckForForwardDropAssassination)
                                {
                                    newCollider = Instantiate(colliderPrefab, _hitInfo3.point, Quaternion.identity, transform).GetComponent<SphereCollider>();
                                    newCollider.radius = colliderRadius;
                                    colliders.Add(newCollider);
                                    print("Got third check");
                                }
                            }
                            */
                        }
                    }

                }
            }
          
            // 6 vertical check
            // 6 horizontal check
        }

        for(int i = 1; i < colliders.Count; i++)
        {
            colliders[i].enabled = false;
        }
    }

    public void SetSentinelState(float _timeToArm, bool _alwaysLethal = false, bool _activateCamera = false, Quaternion? _cameraLook = null)
    {
        if(_activateCamera == false)
        {
            Destroy(sentinelCamera);
        }
        else
        {
            if(_cameraLook != null)
            {
                sentinelCamera.transform.rotation = (Quaternion)_cameraLook;
            }
        }

        alwaysLethal = _alwaysLethal;

        Invoke(nameof(Activate), _timeToArm);
    }

    private void Activate()
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            colliders[i].enabled = true;
        }

        mr.material = activatedMat;
        activated = true;
    }

    public void SetMidAirBool()
    {
        animator.SetBool("Grounded", false);
        goAllTheWay = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 17)    // enemy projectile layer
        {
            OnShadowSentinelDestroyed?.Invoke(this);
            Destroy(gameObject);
            return;
        }

        if (other.gameObject.layer == 6) // enemy layer
        {
            if (!activated) {
                OnShadowSentinelDestroyed?.Invoke(this);
                Destroy(gameObject);
                return;
            }
            // scan for visibility

            // check for awareness
            enemy = other.GetComponent<EnemyBehaviour>();

            for(int i = 0; i < colliders.Count;i++)
            {
                colliders[i].enabled = false;
            }

            if (sentinelCamera != null)
            {
                sentinelCamera.transform.localRotation = Quaternion.identity;
            }
            animator.SetTrigger("Assassinate");

            if (Vector3.Dot(other.transform.position - transform.position, other.transform.forward) > 0) // you are behind
            {
                if (alwaysLethal)
                {
                    enemy.Die(1);
                }
                else
                {
                    enemy.AttemptKill(1);
                }
            }
            else
            {
                if (alwaysLethal)
                {
                    enemy.Die(0);
                }
                else
                {
                    enemy.AttemptKill(0);
                }
            }
            
            StartCoroutine(RotateToEnemy(enemy.transform.position));

            if (!goAllTheWay)
            {
                _targetPos = transform.position - ((transform.position - enemy.transform.position) * positionAmount);
            }
            else
            {
                _targetPos = transform.position - (transform.position - enemy.transform.position);
            }
            StartCoroutine(PositionToEnemy(_targetPos));

        }

        
        
    }


    IEnumerator RotateToEnemy(Vector3 _pos)
    {
        float iterator = 0;
        originalRot_ = transform.rotation;
        targetRotEuler_ = _pos - transform.position;
        targetRotEuler_.y = 0;
        targetRot_ = Quaternion.LookRotation(targetRotEuler_);
        

        while (iterator < 1)
        {
            transform.rotation = Quaternion.Lerp(originalRot_, targetRot_,iterator);
            iterator += Time.deltaTime * rotateSpeed;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
    }

    IEnumerator PositionToEnemy(Vector3 _pos)
    {
        float iterator = 0;
        Vector3 _originalPos = transform.position;


        while (iterator < 1)
        {
            transform.position = Vector3.Lerp(_originalPos, _pos, iterator);
            iterator += Time.deltaTime * moveSpeed;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
    }

    public void AE_DestroyShadowSentinel()
    {
        OnShadowSentinelDestroyed?.Invoke(this);
        Destroy(gameObject);
    }
}
