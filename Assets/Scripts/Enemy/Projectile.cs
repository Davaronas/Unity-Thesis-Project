using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[RequireComponent(typeof(WorldBoundaryCheck))]
public class Projectile : MonoBehaviour
{

    // PARRY WINODW 0.23 - 0.27
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float parriedSpeed = 2f;
    [SerializeField] private float parriedUpAimAmount = 1.5f;

    private bool canDamage = true;
    private Rigidbody rb;
    private Collider cl;
    private TrailRenderer tr;
    [SerializeField] private bool rotateAroundChild = true;
    [SerializeField] private bool forwardRotationFollowVelocity = false;
    [SerializeField] private Transform childToRotate;
    [SerializeField] private bool raycastBetweenPositions;
    [SerializeField] private LayerMask raycastLayers;
    [Space]
    [SerializeField] private bool keepTrailRendererAfterDestroyed = false;
    [SerializeField] private bool destroyOnImpactAlways = false;
    [SerializeField] private bool detachParticleSystem = true;
    [SerializeField] private float particlesScaleMultiplier = 2f;

    private ParticleSystem particles = null;

    private Vector3 positionlastFrame = Vector3.zero;
    private Ray betweenFramesRay;

    private bool projectileParried = false;
    private Transform thrower = null;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cl = GetComponentInChildren<Collider>();  
        tr = GetComponentInChildren<TrailRenderer>();

        betweenFramesRay = new Ray();
        particles = GetComponentInChildren<ParticleSystem>();
    }

    public void SetThrower(Transform _t)
    {
        thrower = _t;
    }


    void Update()
    {
        if (canDamage && rotateAroundChild)
        childToRotate.transform.Rotate(childToRotate.transform.InverseTransformDirection(childToRotate.transform.right * (rotationSpeed * Time.deltaTime)));

        if(canDamage && forwardRotationFollowVelocity)
        transform.rotation = Quaternion.LookRotation(rb.velocity);

        if(canDamage && raycastBetweenPositions)
        {
            if(positionlastFrame != Vector3.zero)
            {

                Debug.DrawRay(positionlastFrame, positionlastFrame - transform.position, Color.red, 5f);

                betweenFramesRay = new Ray(positionlastFrame, positionlastFrame - transform.position);


                if(Physics.Raycast(betweenFramesRay,out RaycastHit _rh, 10f, raycastLayers))
                {
                    canDamage = false;

                    PlayerActions _pa = _rh.collider.GetComponent<PlayerActions>();
                    if (_pa != null)
                    {
                        HitPlayer(_pa, null);
                        
                    }
                    else
                    {
                        if (!keepTrailRendererAfterDestroyed)
                        {
                            Destroy(gameObject);
                        }
                        else
                        {
                            if(childToRotate)
                            Destroy(childToRotate.gameObject);
                            Destroy(rb);
                            Destroy(cl);
                            Destroy(this);
                            Destroy(gameObject,tr.time);
                        }
                    }
                }
            }

            positionlastFrame = transform.position;
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if(projectileParried)
        {
            EnemyBehaviour _enemy = collision.gameObject.GetComponent<EnemyBehaviour>();
            if(_enemy != null)
            {
                _enemy.Stunned();
                cl.gameObject.layer = 18; // visual rigidbody layer
            }
            projectileParried = false;
            Destroy(tr);
        }

        if (!canDamage) { return; }


        PlayerActions _player = collision.gameObject.GetComponent<PlayerActions>();


        HitPlayer(_player, collision.transform);

        canDamage = false;
        if(destroyOnImpactAlways) 
        {
            particles.transform.SetParent(null);
            particles.transform.localScale = particles.transform.localScale * particlesScaleMultiplier ;
            particles.Stop();
            Destroy(particles, particles.main.startLifetime.constant);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, 60f);
        }
        

        if(rotateAroundChild)
        childToRotate.localEulerAngles = Vector3.zero;
    }

    private void HitPlayer(PlayerActions _player, Transform _collisionT)
    {
        if (_player != null)
        {
            if (!_player.ReceiveDamage(damage, null))
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.velocity = !rb.useGravity ? (thrower.position + Vector3.up) - transform.position * parriedSpeed :
                    thrower.position - transform.position + (Vector3.up * parriedUpAimAmount) * parriedSpeed; //-rb.velocity;
                cl.gameObject.layer = 0; // default layer
                projectileParried = true;
            }
            else
            {
                if (!keepTrailRendererAfterDestroyed)
                {
                    Destroy(gameObject);
                }
                else
                {
                    if (childToRotate)
                        Destroy(childToRotate.gameObject);
                    Destroy(rb);
                    Destroy(cl);
                    Destroy(this);
                    Destroy(gameObject, tr.time);
                }
            }
        }
        else
        {
            Destroy(rb);
            Destroy(cl);
            Destroy(tr);
            transform.SetParent(_collisionT);
        }
    }
}

