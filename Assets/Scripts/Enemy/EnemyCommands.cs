using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EnemyBehaviour))]
public class EnemyCommands : MonoBehaviour
{

    private EnemyBehaviour eb;
    [SerializeField] private float minPatrolDelay;
    [SerializeField] private float maxPatrolDelay;
    [Space]
    [SerializeField] private Transform patrolZone;
    [Space]
    [SerializeField] private float awarenessDropPerFrame = 50f;
    [SerializeField] private int maxAlertness = 10;
    [SerializeField] private int playerSpottedAwarenessLevel = 500;
    [SerializeField] private float startDropAwarenessAfterThisTime = 2f;
    [SerializeField] private float investigateAwarenessPercent = 0.6f;
    [SerializeField] private float lookAwarenessPercent = 0.2f;
    [SerializeField] private float accumulatedAwarenessIncreasesAlertnessLevelAmount = 20f;
    [SerializeField] private float corpseVisualMultiplier = 2f;
    [SerializeField] private float maxVisualPointsPerCorpse = 500;

    [Space]
    [SerializeField] private bool patrolByDefault = true;
    [Space]
    [SerializeField] private MultiAimConstraint headIK;
    [SerializeField] private MultiAimConstraint spine2IK;
    [SerializeField] private Transform spine2;
    [SerializeField] private Transform headIKTarget;
    [SerializeField] private Transform spine2IKTarget;
    [SerializeField] private float suspicionLimbRotationSpeed = 2f;
    [SerializeField] private Color highAwarenessFillColor;
    [SerializeField] private Color lowAwarenessFillColor;
    [Space]
    [SerializeField] private float investigateTime = 20f;
    [SerializeField] private float investigateArea = 8f;
    [Space]
    [SerializeField] private SkinnedMeshRenderer xRaySMR;
    [SerializeField] private Material xRayMaterial;
    [Space]

    private Transform playerCamera;
    private PlayerActions player;
    private Vector3 playerLastKnownPosition;

    private Material[] smr_originalMaterials;
    private Material[] smr_xRayMaterials;

    private Canvas enemyCanvas;
    private Image awarenessImage;
    private Image awarenessImageFill;
    private Image fullAwarenessImage;
    private Image playerSpottedImage;

    private EnemyVoices voice;

    private bool inCombat = false;
    private bool isPatrolling = false;
    private bool isInvestigating = false;

    private Transform[] patrolPositions;
    private int patrolPoint_ = 0;

    private int alertness = 0;
    private float awareness = 0;
    private float awarenessAccumulated = 0;
    private float awarenessPercent_ = 0;

    private float currentInvestigationTimePassed = 0f;



    private float lastGainedAwarenessLevel = 0f;

    private float thisFrameAwareness = 0;
    private float lastFrameAwareness = 0;


    private Vector3 lookAtPlayerVector;
    private Vector3 spine2IkTargetVector;

    public enum AwarenessStates { None, Look, Suspicion, Spotted }
    private AwarenessStates currentAwarenessState = AwarenessStates.None;

    private float headIKweight = 0f;

    private EnemyVisionCone visionCone;


    private bool allowLooking;

    public bool playerDiscovered { get; private set; } = false;
 
    public static implicit operator EnemyCommands(EnemyBehaviour _eb) => _eb.GetComponent<EnemyCommands>();

    public EnemyHearing enemyHearing;
    public event Action OnEnemyDeath;

    public float GetMaxVisualPointsPerCorpse()
    {
        return maxVisualPointsPerCorpse;
    }

    public float GetCorpseVisualMultiplier()
    {
        return corpseVisualMultiplier;
    }

   public bool IsStunned()
   {
        return eb.IsStunned();
   }

    void Start()
    {
        
        if (patrolByDefault && patrolZone != null)
        {
            isPatrolling = true;
            Invoke(nameof(Patrol), Random.Range(minPatrolDelay, maxPatrolDelay));
        }

        voice = GetComponentInChildren<EnemyVoices>();

        eb = GetComponent<EnemyBehaviour>();

        enemyCanvas = GetComponentInChildren<Canvas>();
        awarenessImage = enemyCanvas.transform.Find("Awareness").GetComponent<Image>();
        awarenessImageFill = awarenessImage.transform.Find("Fill").GetComponent<Image>();
        playerSpottedImage = enemyCanvas.transform.Find("PlayerSpotted").GetComponent<Image>();
        enemyCanvas.enabled = false;

        if (patrolZone != null)
        {
            patrolPositions = patrolZone.GetComponentsInChildren<Transform>();
        }

        player = FindObjectOfType<PlayerActions>();
        playerCamera = player.transform.GetComponentInChildren<Camera>().transform;

        PlayerActions.OnXRayVisionToggled += XRayVisionToggled;
        PlayerActions.OnPlayerDied += IgnorePlayer;

        smr_originalMaterials = xRaySMR.GetComponent<SkinnedMeshRenderer>().materials;
        smr_xRayMaterials = new Material[9] { xRayMaterial, xRayMaterial, xRayMaterial, xRayMaterial, xRayMaterial, xRayMaterial, xRayMaterial, xRayMaterial, xRayMaterial, };

        visionCone = transform.GetComponentInChildren<EnemyVisionCone>();
        enemyHearing = GetComponentInChildren<EnemyHearing>();
    }

    public void InvestigateWithOtherEnemy(Vector3 _pos)
    {
        eb.GoToPosition(_pos, false);
        currentInvestigationTimePassed = 0;
        isPatrolling = false;
        isInvestigating = true;
    }

    public void SetOtherEnemyAwarenessToMax(Vector3 _pos)
    {
        awareness = playerSpottedAwarenessLevel;
        alertness = maxAlertness;
        eb.ChangeAlertness((float)alertness / maxAlertness);
        currentAwarenessState = AwarenessStates.Spotted;
        inCombat = true;
        eb.TurnToCombatMode(true);
    }

    private void OnDestroy()
    {
        PlayerActions.OnXRayVisionToggled -= XRayVisionToggled;
        PlayerActions.OnPlayerDied -= IgnorePlayer;
    }

    private void IgnorePlayer()
    {
        awareness = 0;
        currentAwarenessState = AwarenessStates.None;
        eb.GoToPosition(transform.position, true);
    }


    private void XRayVisionToggled(bool _state, float _range)
    {
        if(_state )
        {
            if ((player.transform.position - transform.position).magnitude <= _range)
            {
                xRaySMR.gameObject.layer = 12;
                xRaySMR.materials = smr_xRayMaterials;
            }
        }
        else
        {
            xRaySMR.gameObject.layer = 6;
            xRaySMR.materials = smr_originalMaterials;
        }
    }

    public bool IsIncapacitated()
    {
        return eb.IsIncapacitated();
    }

    private void FixedUpdate()
    {
        thisFrameAwareness = awareness / playerSpottedAwarenessLevel;

        CheckUpwardThresholds();
        CheckDownwardThresholds();

        lastFrameAwareness = awareness / playerSpottedAwarenessLevel;
    }

    private void CheckUpwardThresholds()
    {
        if (lastFrameAwareness < lookAwarenessPercent &&
             thisFrameAwareness >= lookAwarenessPercent &&
             thisFrameAwareness < investigateAwarenessPercent)
        {
            //passed threshold to look level this frame

            currentAwarenessState = AwarenessStates.Look;

            //voice.PlayLookVoice();

        }
        else if (lastFrameAwareness < investigateAwarenessPercent &&
             thisFrameAwareness >= investigateAwarenessPercent &&
             thisFrameAwareness < 1f)
        {
            // passed threshold to suspicion level this frame
            alertness = Mathf.Clamp(alertness + 1, 0, maxAlertness);
            eb.ChangeAlertness((float)alertness / maxAlertness);


            currentAwarenessState = AwarenessStates.Suspicion;
            //voice.PlaySuspicionVoice();
        }
        else if (lastFrameAwareness < 1f && thisFrameAwareness >= 1f)
        {
            //passed threshold to spotted level this frame
            alertness = maxAlertness;
            eb.ChangeAlertness((float)alertness / maxAlertness);

            currentAwarenessState = AwarenessStates.Spotted;
         //   voice.PlaySpottedVoice();

            inCombat = true;
            eb.TurnToCombatMode(true);
        }
    }

    private void CheckDownwardThresholds()
    {
        if(lastFrameAwareness >= 1f &&
            thisFrameAwareness < 1f)
        {
            currentAwarenessState = AwarenessStates.Suspicion;

            inCombat = false;
            eb.TurnToCombatMode(false);
        }
        else if(lastFrameAwareness >= investigateAwarenessPercent && thisFrameAwareness < investigateAwarenessPercent)
        {
            currentAwarenessState = AwarenessStates.Look;
        }
        else if(lastFrameAwareness >= lookAwarenessPercent && thisFrameAwareness < lookAwarenessPercent)
        {
            currentAwarenessState = AwarenessStates.None;
        }
    }

    public float GetAwarenessPercent()
    {
        return awareness / playerSpottedAwarenessLevel;
    }

    public bool IsAwareOfThePlayerAndHasVisionOfHim()
    {
        if(visionCone.IsPlayerInVision() && awareness >= playerSpottedAwarenessLevel)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsPlayerInVision()
    {
        return visionCone.IsPlayerInVision();
    }

    public void RemoveLastPlayerLocation()
    {
        playerLastKnownPosition = transform.position + transform.forward;
        headIKweight = 0;
        allowLooking = false;
    }

    private void Update()
    {
       

        lastGainedAwarenessLevel =  Mathf.Clamp(lastGainedAwarenessLevel + Time.deltaTime, 0, startDropAwarenessAfterThisTime + 10f);
        if(lastGainedAwarenessLevel >= startDropAwarenessAfterThisTime)
        {
            awareness = Mathf.Clamp(awareness - (awarenessDropPerFrame * Time.deltaTime), 0, playerSpottedAwarenessLevel);
            awarenessImageFill.fillAmount = awarenessPercent_;
        //    print(awareness + "/ " + playerSpottedAwarenessLevel);
        }

        awarenessPercent_ = awareness / playerSpottedAwarenessLevel;

        if (awareness > 0)
        {
            

            if (!enemyCanvas.isActiveAndEnabled)
            {
                enemyCanvas.enabled = true;
            }

        //    lookAtPlayerVector = playerCamera.transform.position;

          //  lookAtPlayerVector.y = 0;


            enemyCanvas.transform.LookAt(playerCamera.transform.position, Vector3.up);
            enemyCanvas.transform.Rotate(Vector3.up, 180);


            if (awarenessPercent_ > lookAwarenessPercent && allowLooking && (!inCombat || (!eb.InMeleeRange() && !eb.NmaHasPath())))
            {
                headIKTarget.position = playerLastKnownPosition;

                spine2IkTargetVector = playerLastKnownPosition;
                spine2IkTargetVector.y = spine2.transform.position.y;
                spine2IKTarget.position = spine2IkTargetVector;
                headIKweight = Mathf.Clamp(headIKweight + (Time.deltaTime * suspicionLimbRotationSpeed), 0, 1);
            }
            else
            {
                headIKweight = Mathf.Clamp(headIKweight - (Time.deltaTime * suspicionLimbRotationSpeed), 0, 1);
            }


            if (awarenessPercent_ > investigateAwarenessPercent)
            {
                eb.GoToPosition(playerLastKnownPosition, false);
                enemyHearing.TriggerOtherEnemiesToInvestigate(playerLastKnownPosition);
                currentInvestigationTimePassed = 0;
                isPatrolling = false;
                isInvestigating = true;
                awarenessImageFill.color = highAwarenessFillColor;
            }
            else
            {
                awarenessImageFill.color = lowAwarenessFillColor;
            }

            if(awarenessPercent_ >= 1f)
            {
                

                if(awarenessImage.gameObject.activeSelf)
                {
                    awarenessImage.gameObject.SetActive(false);
                    playerSpottedImage.enabled = true;
                    enemyHearing.TriggerOtherEnemiesMaxAwareness(this);
                }
            }
            else
            {
                if(playerSpottedImage.isActiveAndEnabled)
                {
                    awarenessImage.gameObject.SetActive(true);
                    playerSpottedImage.enabled = false;
                }
            }

         
        }
        else
        {
            if (enemyCanvas.isActiveAndEnabled)
            {
                enemyCanvas.enabled = false;
            }
            headIKweight = Mathf.Clamp(headIKweight - (Time.deltaTime * suspicionLimbRotationSpeed), 0, 1);
        }

        currentInvestigationTimePassed = Mathf.Clamp(currentInvestigationTimePassed + Time.deltaTime,0,investigateTime);

        if(currentInvestigationTimePassed >= investigateTime)
        {
            if (isPatrolling == false)
            {
                isPatrolling = true;
                isInvestigating = false;
                if (patrolByDefault && patrolZone != null)
                {
                    Invoke(nameof(Patrol), Random.Range(minPatrolDelay, maxPatrolDelay));
                }
            }
        }

        headIK.weight = Mathf.Lerp(0, 1, headIKweight);
        spine2IK.weight = Mathf.Lerp(0, 1, headIKweight);
    }

    private void Patrol()
    {
        if (inCombat || !isPatrolling) { return; }

        if (patrolPositions != null)
        {
            eb.GoToPosition(patrolPositions[Random.Range(0,patrolPositions.Length-1)].position,true);
        }


    }


    public void ReachedPosition()
    {
      //  if(alertness ) // sometimes look behind

        if (!inCombat && isPatrolling && !isInvestigating)
        {
            Invoke(nameof(Patrol), Random.Range(minPatrolDelay, maxPatrolDelay));
        }
        else if(isInvestigating)
        {
            Invoke(nameof(InvestigateAroundLastKnownPosition), Random.Range(minPatrolDelay, maxPatrolDelay));
        }

    }

    private void InvestigateAroundLastKnownPosition()
    {
        if(awareness >= playerSpottedAwarenessLevel) { return; }

        NavMesh.SamplePosition(playerLastKnownPosition + new Vector3(Random.Range(-investigateArea, investigateArea), 0f, Random.Range(-5f, 5f)), out NavMeshHit _hit, 10f, Physics.AllLayers);
        eb.GoToPosition(_hit.position,true);
    }

    public float GetAlertnessMultiplier()
    {
        return (1 + ((float)alertness / maxAlertness));
    }

    public bool InCombat()
    {
        return inCombat;
    }

    public bool IsWeaponDrawn()
    {
        return eb.IsWeaponDrawn();
    }

    public Vector3 GetLastPlayerLocation()
    {
        return playerLastKnownPosition;
    }



    public void ReceiveAwarenessSignal(float _points, Vector3? _pos = null, bool _softOverride = false, bool _notifyPlayer = true)
    {
        if (eb.IsIncapacitated()) { return; }

        _points *= (1 + ((float)alertness / maxAlertness));

        //_points = 0; // ---------

        if (alertness < maxAlertness)
        {
            awarenessAccumulated += _points;
            if (awarenessAccumulated >= accumulatedAwarenessIncreasesAlertnessLevelAmount)
            {
                awarenessAccumulated = 0;
                alertness = Mathf.Clamp(alertness + 1, 0, maxAlertness);
                eb.ChangeAlertness((float)alertness / maxAlertness);
                if (alertness == maxAlertness)
                {
                    inCombat = true;
                }
            }
        }


        if (_softOverride && (isInvestigating || inCombat))
        {

        }
        else if (_pos == null)
        {
            playerLastKnownPosition = playerCamera.position;
        }
        else
        {
            playerLastKnownPosition = (Vector3)_pos;
        }



        awareness = Mathf.Clamp(awareness + _points, 0, playerSpottedAwarenessLevel);
        //  print(awareness + "/ " + playerSpottedAwarenessLevel);
        awarenessImageFill.fillAmount = awareness / playerSpottedAwarenessLevel;
        lastGainedAwarenessLevel = 0;

        if (_notifyPlayer)
        {
            player.AwarenessRaised(_points, transform.position, this);
        }


        allowLooking = true;
    }


    public float GetAlertnessPercent()
    {
        return alertness / maxAlertness;
    }

    public void Die()
    {


        headIK.weight = 0;
        spine2IK.weight = 0;
        enemyCanvas.enabled = false;
        
        // player.RemoveEnemyInsideNoiseBubble(this);

        this.enabled = false;

        OnEnemyDeath?.Invoke();
    }




    public AwarenessStates GetAwarenessState()
    {
        return currentAwarenessState;
    }

   

}
  
