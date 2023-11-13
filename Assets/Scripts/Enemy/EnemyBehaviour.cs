using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyCommands))]



public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float rotateToTargetSpeed = 2f;
    [Space]
    [SerializeField] private float combatActionDelayMin = 0.4f;
    [SerializeField] private float combatActionDelayMax = 1.2f;
    [SerializeField] private int maxCombatIndex = 2;
    [SerializeField] private int[] meleeCombatActionsAgaintsCrouching = { 5, 9 };
    private const int rangedCombatIndex = 99;
    [SerializeField] private float damage = 35f;
    [SerializeField] private float damageCheckCapsuleCastThickness = 0.5f;
    [SerializeField] private LayerMask playerLayer = 8;
    [Space]
    [SerializeField] private float dodgeStrength = 2f;
    [SerializeField] private float dodgeForwardMultiplier = 0.3f;
    [SerializeField] private float stunnedVelocityMultiplier = 1f;
    [Space]
    [SerializeField] private Transform weapon;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Vector3 weaponHandPos;
    [SerializeField] private Vector3 weaponHandRotEuler;
    [Space]
    [SerializeField] private Transform incapacitatedColliderParent;
    [Space]
    [SerializeField] private float bodyPutDownSpeed = 4f;
    [Space]
    [SerializeField] private float maxCoverDistanceMagnitudeOnMeleeAttacks = 2f;
    [SerializeField] private float attackTrackingValue = 3f;
    private bool physicsControlled = false;
    [Space]
    [SerializeField] private bool onlyRanged = false;
    [SerializeField][Range(0, 5)] private int rangedAttacks = 1;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawn = null;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float noCalculationThrowDistance = 5f;
    [Space]
    [SerializeField] private bool disableProjectileInHandWhenFiring = true;
    [SerializeField] private GameObject projectileInHandVisualObject = null;
    [SerializeField] private bool rangedNoArc = false;
    [SerializeField] private GameObject rangedWeaponBeforeUse = null;
    [SerializeField] private ParticleSystem rangedWeaponParticles = null;

    [SerializeField] private bool isUsingBow = false;
    [SerializeField] private GameObject projectileOnRangedWeaponVisualObject = null;
    [SerializeField] private float rangedCombatActionDelayMin = 0.8f;
    [SerializeField] private float rangedCombatActionDelayMax = 1.6f;
    [SerializeField] private float minInnaccuracy = -0.2f;
    [SerializeField] private float maxInnaccuracy = 0.2f;
    [SerializeField] private float rangedFuturePlayerPositionCalculationDivision = 3f;
    private bool allowLooking = true;
    [Space]
    [SerializeField] private GameObject mageDefenseBarrier = null;
    [SerializeField] private GameObject mageOffensiveDamageAreaPrefab = null;
    [SerializeField] private float mageOffensiveAreaNavMeshSampleMaxDistance = 20;
    [SerializeField] private float mageOffensiveAreaDuration = 3f;
    [SerializeField] private LayerMask defaultLayerMask;

    private BoxCollider[] incapacitatedColliders;

    private Transform weaponDamageRaycastStart;
    private Transform weaponDamageRaycastEnd;

    private NavMeshAgent nma;
    private Animator animator;
    private EnemyCommands ec;
    private CapsuleCollider capsuleCol;
    private Rigidbody rb;
    private int death_id;


    private bool hasMovementOrder = false;

    private Vector3 lastDest;
    private Vector3 rotateToDestEulers;

    private float alertnessPercent;

    private NavMeshPath nextPath_;

    private bool inMeleeRange = false;

    private int combatIndex = 0;
    private bool combatActionAllowed = true;
    private bool isWeaponDrawn = false;
    private float weaponLength;
    private bool checkingForDealingDamage = false;
    private Ray damageCheckRay;
    private Ray damageCheckRay2;
    private bool isStunned;

    private bool isIncapacitated = false;

    private CarryObject[] carryObjects;

    private PlayerActions player;
    private EnemyVisionCone visionCone;

    private Vector3 lastFrameWeaponStartPos;

    private Animator bowAnimator;

    private Coroutine InProcessActionCoroutine;
    private bool combatActionTimerIsGoing = false;
    private bool rangedCombatActionTimerIsGoing = false;

    private void Start()
    {
        ec = GetComponent<EnemyCommands>();
        animator = GetComponent<Animator>();
        nma = GetComponent<NavMeshAgent>();
        capsuleCol = GetComponent<CapsuleCollider>();
        player = FindObjectOfType<PlayerActions>();
        visionCone = GetComponentInChildren<EnemyVisionCone>();
        rb = GetComponent<Rigidbody>();

        if (isUsingBow)
        {
            bowAnimator = GetComponentInChildren<BowAnimator>().GetComponent<Animator>();
        }


        if (!onlyRanged)
        {
            weaponDamageRaycastStart = weapon.transform.Find("RaycastStart");
            weaponDamageRaycastEnd = weapon.transform.Find("RaycastEnd");


            weaponLength = (weaponDamageRaycastEnd.position - weaponDamageRaycastStart.position).magnitude;
        }

       incapacitatedColliders = incapacitatedColliderParent.GetComponentsInChildren<BoxCollider>();


       for(int i = 0; i < incapacitatedColliders.Length;i++)
       {
            incapacitatedColliders[i].gameObject.SetActive(false);
       }

        defaultLayerMask = LayerMask.GetMask("Default");

        PlayerActions.OnPlayerDash += PlayerDashed;

        carryObjects = GetComponentsInChildren<CarryObject>(true);
        for (int i = 0; i < carryObjects.Length;i++)
        {
            carryObjects[i].OnPutDown += AdjustAnimatorAfterCarry;
        }

    }

 

    private void AdjustAnimatorAfterCarry()
    {
        // Debug mode: Keep animator state on disable

        // animator.GetAnimatorTransitionInfo(0).normalizedTime = 1;
        //animator.SetInteger("AssassinatedType", death_id);
        //  animator.SetTrigger("Assassinated");
        // animator.keepAnimatorStateOnDisable = false;

        animator.speed = bodyPutDownSpeed;
        animator.SetInteger("AssassinatedType", -1); // -1 is body drop down anim without assassination anim
        animator.SetTrigger("Assassinated");

        incapacitatedColliders[0].gameObject.SetActive(true);
        incapacitatedColliders[1].gameObject.SetActive(false);




    }


    private void OnDestroy()
    {
        PlayerActions.OnPlayerDash -= PlayerDashed;

        if(carryObjects == null)
        {
            print(gameObject.name);
            return;
        }

        for (int i = 0; i < carryObjects.Length; i++)
        {
            if (carryObjects[i] != null)
            {
                carryObjects[i].OnPutDown -= AdjustAnimatorAfterCarry;
            }
        }
    }

    public void DisableCombatLayer()
    {
        animator.SetLayerWeight(1, 0);
    }

    public void Stunned()
    {
        PhysicsMovementEnded();

        if(InProcessActionCoroutine != null)
        {
            StopCoroutine(InProcessActionCoroutine);
            combatActionTimerIsGoing = false;
            rangedCombatActionTimerIsGoing = false;
        }


        combatActionAllowed = false;
        checkingForDealingDamage = false;
        animator.SetInteger("CombatIndex",-1);
        isStunned = true;

        if(projectileInHandVisualObject != null)
        {
            if(projectileInHandVisualObject.activeInHierarchy)
            {
                projectileInHandVisualObject.SetActive(false);
            }
        }

        if (projectileOnRangedWeaponVisualObject != null)
        {
            if (projectileOnRangedWeaponVisualObject.activeInHierarchy)
            {
                projectileOnRangedWeaponVisualObject.SetActive(false);
            }
        }
    }

    private bool CanMeleeAttack()
    {
        if (inMeleeRange && ec.InCombat() && combatActionAllowed && isWeaponDrawn && visionCone.IsPlayerInVision() && nma.hasPath)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CanRangedAttack()
    {
       // print(!inMeleeRange + " " + ec.InCombat() + " " + combatActionAllowed + " " + isWeaponDrawn + " " + visionCone.IsPlayerInVision());
        if((!inMeleeRange || onlyRanged) && ec.InCombat() && combatActionAllowed && isWeaponDrawn && visionCone.IsPlayerInVision())
        {
            return true;
        }
        else 
        {
            return false;

        }
    }

    private void FixedUpdate()
    {
       
        if(InProcessActionCoroutine != null)
        {
            if (combatActionTimerIsGoing)
            {
                return;
            }
        }

        if (CanMeleeAttack() && !onlyRanged)
        {
            combatActionAllowed = false;
            checkingForDealingDamage = false;
            // Invoke(nameof(CombatAction), Random.Range(combatActionDelayMin, combatActionDelayMax));
            InProcessActionCoroutine = StartCoroutine(CombatAction());
        }
        else if (CanRangedAttack())
        {
            combatActionAllowed = false;
            checkingForDealingDamage = false;
            // Invoke(nameof(RangedAttack), Random.Range(rangedCombatActionDelayMin, rangedCombatActionDelayMax));
            InProcessActionCoroutine = StartCoroutine(RangedAttackCombatAction());
        }
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    private void LateUpdate()
    {
        if(checkingForDealingDamage)
        {
            // errõl is beszélhetsz: régi iteráció nem volt elég jó

            /*
            damageCheckRay = new Ray(weaponDamageRaycastStart.position,  weaponDamageRaycastEnd.position - weaponDamageRaycastStart.position);
            damageCheckRay2 = new Ray(lastFrameWeaponStartPos, weaponDamageRaycastEnd.position - lastFrameWeaponStartPos);
            Debug.DrawRay(weaponDamageRaycastStart.position, weaponDamageRaycastEnd.position - weaponDamageRaycastStart.position,Color.green,1f);
            Debug.DrawRay(lastFrameWeaponStartPos, weaponDamageRaycastEnd.position - lastFrameWeaponStartPos, Color.green, 1f);
            if (Physics.Raycast(damageCheckRay,weaponLength,playerLayer) || Physics.Raycast(damageCheckRay2, weaponLength, playerLayer)) // only check for player
            {
                player.ReceiveDamage(damage,this);
                checkingForDealingDamage = false;
            }
            */

            

            if(Physics.OverlapCapsule(weaponDamageRaycastStart.position,weaponDamageRaycastEnd.position,damageCheckCapsuleCastThickness,playerLayer).Length > 0)
            {
                player.ReceiveDamage(damage, this);
                checkingForDealingDamage = false;
            }

        }
    }

    /*
    private void CombatAction()
    {
        
        if(!inMeleeRange || !visionCone.IsPlayerInVision() || isStunned || isIncapacitated)
        {
            combatActionAllowed = true;
            return;
        }

        GiveControlToPhysics();
        combatIndex = Random.Range(1, maxCombatIndex + 1);
        animator.SetInteger("CombatIndex", combatIndex);
        checkingForDealingDamage = false;
    }
    */


    private IEnumerator CombatAction()
    {
        combatActionTimerIsGoing = true;
        yield return new WaitForSeconds(Random.Range(combatActionDelayMin, combatActionDelayMax));

        if (!inMeleeRange || !visionCone.IsPlayerInVision() || isStunned || isIncapacitated)
        {
            combatActionAllowed = true;
            combatActionTimerIsGoing = false;
            yield break;
        }


        combatActionTimerIsGoing = false;

        GiveControlToPhysics();
        combatIndex = Random.Range(1, maxCombatIndex + 1);

        // Only do melee attacks that can hit a crouched player : felesleges mióta nagyobb a fegyver ellenõrzése
        if(player.IsSneaking())
        {
            combatIndex = FindClosestLowAttackNumber(combatIndex);
        }

        animator.SetInteger("CombatIndex", combatIndex);
        checkingForDealingDamage = false;
        yield break;
    }

    private int FindClosestLowAttackNumber(int _combatIndex)
    {
        int _leastDif = 999;
        for(int i = 0; i < meleeCombatActionsAgaintsCrouching.Length;i++)
        {
            if(Mathf.Abs(_combatIndex - meleeCombatActionsAgaintsCrouching[i]) < _leastDif)
            {
                _leastDif = meleeCombatActionsAgaintsCrouching[i];
            }
        }

        return _leastDif;
    }

    private IEnumerator RangedAttackCombatAction()
    {
        rangedCombatActionTimerIsGoing = true;

        combatActionTimerIsGoing = true;
        yield return new WaitForSeconds(Random.Range(rangedCombatActionDelayMin, rangedCombatActionDelayMax));

        if ((inMeleeRange && !onlyRanged) || !visionCone.IsPlayerInVision() || isStunned || isIncapacitated)
        {
            combatActionAllowed = true;
            combatActionTimerIsGoing = false;

            rangedCombatActionTimerIsGoing = false;
            yield break;
        }

        rangedCombatActionTimerIsGoing = false;
        combatActionTimerIsGoing = false;
        GiveControlToPhysics();
        combatIndex = Random.Range(rangedCombatIndex, rangedCombatIndex + rangedAttacks);
        animator.SetInteger("CombatIndex", combatIndex);
        yield break;
    }

    /*
    private void RangedAttack()
    {
        if ((inMeleeRange && !onlyRanged) || !visionCone.IsPlayerInVision() || isStunned || isIncapacitated)
        {
            combatActionAllowed = true;
            return;
        }

        GiveControlToPhysics();
        combatIndex = rangedCombatIndex;
        animator.SetInteger("CombatIndex", combatIndex);
        
    }
    */

    public void AE_ShowProjectileInHandVisualObject()
    {
        if (!physicsControlled) { return; }
        projectileInHandVisualObject.SetActive(true);
    }

    public void AE_HideProjectileInHandVisualObject()
    {
        projectileInHandVisualObject.SetActive(false);
    }

    public bool NmaHasPath()
    {
        if(nma.isActiveAndEnabled)
        return nma.hasPath;
        else
        {
            return true;
        }
    }

    public bool InMeleeRange()
    {
        return inMeleeRange;
    }

    public void AE_FireProjectile()
    {
        if(!physicsControlled) { return; }

        if (!player.IndicatorsContainEnemy(ec) && visionCone.IsPlayerInVision())
        {
            return;
        }

        float _d = 0;
        Vector3 _Vel = rangedNoArc ? (player.GetFuturePlayerPosition(rangedFuturePlayerPositionCalculationDivision) - projectileSpawn.position).normalized * projectileSpeed:
            CalculateVelocity(projectileSpawn.position, player.GetFuturePlayerPosition(), 1f, 2f, out _d);



        Rigidbody _newProjectile =
       Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.LookRotation(player.transform.position - transform.position)).GetComponent<Rigidbody>();
        _newProjectile.GetComponent<Projectile>().SetThrower(transform);

        if (_d < noCalculationThrowDistance)
        {
            _newProjectile.velocity = (player.GetFuturePlayerPosition(rangedFuturePlayerPositionCalculationDivision) - 
                _newProjectile.transform.position).normalized * projectileSpeed;
        }
        else
        {
            Vector3 _innacuracy =
                new Vector3
                (Random.Range(minInnaccuracy, maxInnaccuracy) * _d, Random.Range(minInnaccuracy, maxInnaccuracy) * _d, Random.Range(minInnaccuracy, maxInnaccuracy) * _d);
            _newProjectile.velocity = _Vel + _innacuracy; // 0.5f 2.0f
        }

        if (projectileOnRangedWeaponVisualObject != null)
        {
            if (projectileOnRangedWeaponVisualObject.activeSelf)
            {
                projectileOnRangedWeaponVisualObject.SetActive(false);
            }
        }

        if (rangedWeaponParticles != null)
        {
            rangedWeaponParticles.Play();
        }

        //(player.transform.position - _newProjectile.transform.position).normalized * projectileSpeed;

    }

    public void AE_SummonDefensiveDamageArea()
    {
        mageDefenseBarrier.SetActive(true);
        isWeaponDrawn = true;
    }

    public void AE_SummonOffensiveDamageArea()
    {
        Vector3 _pos = ec.IsAwareOfThePlayerAndHasVisionOfHim() ? player.GetFuturePlayerPosition() : ec.GetLastPlayerLocation();
        

        _pos = NavMesh.SamplePosition(_pos, out NavMeshHit _nmh, mageOffensiveAreaNavMeshSampleMaxDistance, defaultLayerMask) ? _nmh.position : _pos;

        DamageArea _da = Instantiate(mageOffensiveDamageAreaPrefab, _pos, Quaternion.identity).GetComponent<DamageArea>();

       StartCoroutine(DestroyOffensiveDamageArea(_da));
    }

    private IEnumerator DestroyOffensiveDamageArea(DamageArea _da)
    {
        yield return new WaitForSeconds(mageOffensiveAreaDuration);

        _da.DetachParticleSystem();
        Destroy(_da.gameObject, mageOffensiveAreaDuration);
    }


    public void AE_PlayBowAnimation()
    {
        if(combatIndex != rangedCombatIndex) { return; }

        projectileOnRangedWeaponVisualObject.SetActive(true);


        bowAnimator.SetTrigger("Fire");

    }


    public void AE_DrawPistol()
    {
        rangedWeaponBeforeUse.SetActive(false);
        projectileInHandVisualObject.SetActive(true);
       
    }

    public void AE_HolsterPistol()
    {
        projectileInHandVisualObject.SetActive(false);
        rangedWeaponBeforeUse.SetActive(true);
    }


    private Vector3 CalculateVelocity(Vector3 _original, Vector3 _target, float _time, float _arc, out float _distance)
    {
        Vector3 distance = (_target - _original);
        _distance = distance.magnitude;

        if(_distance < noCalculationThrowDistance ) { return Vector3.zero; }

        Vector3 distance_XZ = distance;
        distance_XZ.y = 0f;

        float Sy = distance.y;
        float Sxz = distance_XZ.magnitude;

  
        float Vxz =  Sxz - _time;
        float Vy = Sy / _time + 0.5f * Mathf.Abs(Physics.gravity.y) * _time;

        Vector3 _result = distance_XZ.normalized;
        _result *= Vxz;
        _result.y = Vy;

        return _result;
    }

    public void AE_CombatActionEnded()
    {
        if (InProcessActionCoroutine != null)
        {
            StopCoroutine(InProcessActionCoroutine);
            combatActionTimerIsGoing = false;
            rangedCombatActionTimerIsGoing = false;

            
        }

        animator.SetInteger("CombatIndex", 0);
        combatIndex = 0;
        checkingForDealingDamage = false;
        isStunned = false;



        combatActionAllowed = true;
        PhysicsMovementEnded();
    }



    private void GiveControlToPhysics()
    {
        nma.enabled = false;
        nma.updateRotation = false;
     //   rb.isKinematic = false;
     //   rb.useGravity = true;

        physicsControlled = true;
    }

    private void AllowPhysicsMovement()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public void PhysicsMovementEnded()
    {
        if(!rb.isKinematic)
        rb.velocity = Vector3.zero;

        rb.isKinematic = true;
        rb.useGravity = false;
        nma.enabled = true;
        nma.updateRotation = true;

        physicsControlled = false;
    }

    public void AE_StopRigidbody()
    {
        if (!rb.isKinematic)
            rb.velocity = Vector3.zero;

        rb.isKinematic = true;
        rb.useGravity = false;
    }


  

   
  

    public void AE_MoveAndRotateTowardsPlayer()
    {
       if(!visionCone.IsPlayerInVision() && !player.IndicatorsContainEnemy(ec)) { return; }

        Vector3 _pPos = player.transform.position;
        _pPos.y = 0;
        Vector3 _ePos = transform.position;
        _ePos.y = 0;
        Vector3 _playerEnemyVector = _pPos - _ePos;

       // transform.LookAt(_pPos);
       transform.rotation = Quaternion.LookRotation(_playerEnemyVector);


        //  print((_playerEnemyVector * attackTrackingValue).magnitude + " magn");
        //  _playerEnemyVector.y = 0; // Move before velocity change
        //rb.MoveRotation(Quaternion.LookRotation(_playerEnemyVector));
        //  transform.rotation = Quaternion.LookRotation(_playerEnemyVector);


        StartCoroutine(MoveAfterRotation(_playerEnemyVector));
    }

    private IEnumerator MoveAfterRotation(Vector3 _playerEnemyVector) 
    {
        // for some reason toration is reset if it happens in the same frame as velocity change

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        AllowPhysicsMovement();
        rb.velocity = Vector3.ClampMagnitude(_playerEnemyVector * attackTrackingValue, maxCoverDistanceMagnitudeOnMeleeAttacks);
    }


    public void AE_RotateTowardsPlayer()
    {
        if (!visionCone.IsPlayerInVision()) { return; }

        Vector3 _pPos = player.transform.position;
        _pPos.y = 0;
        Vector3 _ePos = transform.position;
        _ePos.y = 0;
        Vector3 _playerEnemyVector = _pPos - _ePos;

        transform.rotation = Quaternion.LookRotation(_playerEnemyVector);


       // transform.LookAt(_pPos);

    }

    public void AE_DodgeLeft()
    {
        AllowPhysicsMovement();
        rb.velocity = (-transform.right + (transform.forward * dodgeForwardMultiplier)) * dodgeStrength;
    }

    public void AE_DodgeRight()
    {
         AllowPhysicsMovement();
        rb.velocity = (transform.right + (transform.forward * dodgeForwardMultiplier)) * dodgeStrength;
    }


    public void AE_StunnedPhysicsMovement()
    {
        GiveControlToPhysics();
        AllowPhysicsMovement();
        rb.velocity = -transform.forward * stunnedVelocityMultiplier;
    }


 


    

    public void AE_ParentSwordToHand()
    {
        if (weapon != null)
        {
            weapon.parent = rightHand;
            weapon.localPosition = weaponHandPos;
            weapon.localRotation = Quaternion.Euler(weaponHandRotEuler);
        }
        isWeaponDrawn = true;
    }

    public void AE_WeaponDamageCheckStarted()
    {
        checkingForDealingDamage = true;
        lastFrameWeaponStartPos = weaponDamageRaycastStart.position;
    }

    public void AE_WeaponDamageCheckEnded()
    {
        checkingForDealingDamage = false;
    }



    public bool IsWeaponDrawn()
    {
        return isWeaponDrawn;
    }


    public void AE_DrawSwordFinished()
    {
        if(nma.isActiveAndEnabled)
        nma.isStopped = false;
    }

    public void TurnToCombatMode(bool _state)
    {
        if(_state)
        {
            animator.SetBool("InCombat", true);
            nma.speed = runSpeed;
        }
        else
        {
            animator.SetBool("InCombat", false);
            nma.speed = walkSpeed;
        }
    }


    public void PlayerDashed()
    {

        if (ec.IsAwareOfThePlayerAndHasVisionOfHim())
        {
            ec.RemoveLastPlayerLocation();
            GoToPosition(transform.position, true);
            animator.SetInteger("Moving", 0);
            animator.SetTrigger("PlayerDashed");
        }
    }



    public void ChangeAlertness(float _percentage)
    {

        if(alertnessPercent >= 1) // no need to change
        {
            return; 
        }

        alertnessPercent = _percentage;
       

        animator.SetFloat("Alertness", _percentage);

        if (_percentage >= 1)
        {
            nma.isStopped = true;
            animator.SetTrigger("ReachedMaxAlertness");
        }
    }



    public void GoToPosition(Vector3 _pos, bool _forceOrder)
    {
        if (!nma.isActiveAndEnabled) { return; }

        lastDest = _pos;

        // if ( !_forceOrder) { return; }

        nextPath_ = new NavMeshPath();
       if( nma.CalculatePath(_pos, nextPath_))
       {
            nma.SetDestination(_pos);
            if ((nma.destination - transform.position).magnitude >= nma.stoppingDistance)
            {
                animator.SetInteger("Moving", 1);
                hasMovementOrder = true;
                nma.updateRotation = true;
            }
            else if (_forceOrder)
            {
                hasMovementOrder = true;
            }
        }
        else
        {
            animator.SetInteger("Moving", 0);
            hasMovementOrder = false;
            nma.updateRotation = true;
            nma.SetDestination(transform.position);
        }

       /*
        print(nma.hasPath);
        if (nma.SetDestination(_pos))
        {
            if ((nma.destination - transform.position).magnitude >= nma.stoppingDistance)
            {
                animator.SetInteger("Moving", 1);
                hasMovementOrder = true;
                nma.updateRotation = true;
            }
            else if(_forceOrder)
            {
                hasMovementOrder = true;
            }
        }
        else
        {
            animator.SetInteger("Moving", 0);
            hasMovementOrder = false;
            nma.updateRotation = true;
            nma.SetDestination(transform.position);
        }
       */
    }

    private void Update()
    {

        if(physicsControlled) { return; }

        // if(nma.remainingDistance <= nma.stoppingDistance) // Reached Dest
        //    if((lastDest - transform.position).magnitude <= nma.stoppingDistance * 1.5f)


         if (nma.remainingDistance <= nma.stoppingDistance * 1.5f && nma.hasPath)
       // if ((nma.destination - transform.position).magnitude <= nma.stoppingDistance * 1.5f)
        {
            if(hasMovementOrder)
            {
                hasMovementOrder = false;
                ec.ReachedPosition();  
            }

            animator.SetInteger("Moving", 0);

            nma.updateRotation = false;

            
            if (lastDest != null)
            {
                rotateToDestEulers = (lastDest - transform.position);
                rotateToDestEulers.y = 0;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rotateToDestEulers, Vector3.up),Time.deltaTime * rotateToTargetSpeed);
            }
            


            inMeleeRange = true;

            if(!onlyRanged && rangedCombatActionTimerIsGoing)
            {
                StopCoroutine(InProcessActionCoroutine);
                combatActionTimerIsGoing = false;
                rangedCombatActionTimerIsGoing = false;

                combatActionAllowed = true;
            }

       }
       else
        {
            inMeleeRange = false;
        }
    }


  
    public void AttemptKill(int _assassinationId)
    {
        if(ec.GetAwarenessState() == EnemyCommands.AwarenessStates.Spotted)
        {
            Stunned();
        }
        else
        {
            Die(_assassinationId);
        }
    }

    public void Die(int _assassinationId)
    {
        if(_assassinationId % 2 == 0)
        {
            incapacitatedColliders[0].gameObject.SetActive(true);
        }
        else
        {
            incapacitatedColliders[1].gameObject.SetActive(true);
        }


        PhysicsMovementEnded();
        ec.Die();
        //ec.enabled = false;
        nma.enabled = false;
       // Destroy(rb);
        //  capsuleCol.enabled = false;
        // Destroy(capsuleCol);
        gameObject.layer = 18; // visual rigidbody layer
        checkingForDealingDamage = false;

        DisableCombatLayer();
        animator.SetInteger("CombatIndex", 0);
        animator.SetInteger("AssassinatedType", _assassinationId);
        animator.SetTrigger("Assassinated");
        death_id = _assassinationId;

        if(mageDefenseBarrier != null)
        {
            Destroy(mageDefenseBarrier);
        }
        

        isIncapacitated = true;
        this.enabled = false;
    }

    public bool IsIncapacitated()
    {
        return isIncapacitated;
    }


}
