using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Splines;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerActions : MonoBehaviour
{
    [SerializeField] private float interactDistance = 1.3f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float normalAssassinationLength = 0.5f;
    [SerializeField] private float dropAssassinationLength = 1f;
    [Space]
    [SerializeField] private float sneakingVisualReductionMultiplier = 0.55f;
    [SerializeField] private float playerUnderSomethingVisualReductionMultiplier = 0.7f;
    [SerializeField] private float heightDifferenceVisualReductionMultiplier = 0.3f;
    [SerializeField] private float significantHeightDifference = 3f;
    [SerializeField] private float maxHeightDifferenceToAllowNormalAssassination = 1f;
    [Space]
    [SerializeField] private float rotateEnemyForAssassinationSpeed = 1f;
    [Space]
    [SerializeField] private float rotCameraForAssassinationSpeed = 5f;
    [SerializeField] private float posCameraForAssassinationSpeed = 5f;

    [SerializeField] private float rotCameraForAssassinationSpeedDropAssassination = 5f;
    [SerializeField] private float posCameraForAssassinationSpeedDropAssassination = 5f;
    [Space]
    [SerializeField] private float runningNoise;
    [SerializeField] private float landingNoise;
    [SerializeField] private float sprintingNoise;
    [SerializeField] private SphereCollider noiseBubbleTrigger;
    [Space]
    [SerializeField] private float awarenessIndicatorFullAmount = 5f;
    [SerializeField] private float awarenessIndicatorArrowDissappear = 0.3f;
    [Space]
    [SerializeField] private Image eyeImage;
    [SerializeField] private float eyeTransparencyFadeSpeed;
    [SerializeField] private Image leftMouseImage;
    [Space]
    [SerializeField] private Image currentAbilityImage_Dash;
    [SerializeField] private Image currentAbilityImage_Vision;
    [SerializeField] private Image currentAbilityImage_ShadowForm;
    [SerializeField] private Image currentAbilityImage_ShadowSentinel;
    [SerializeField] private Image currentAbilityImage_Mirage;
    [SerializeField] private Image currentAbilityImage_DarkHaven;
    [Space]
    [SerializeField] private RectTransform dashAbilityRT;
    [SerializeField] private RectTransform visionAbilityRT;
    [SerializeField] private RectTransform shadowFormAbilityRT;
    [SerializeField] private RectTransform shadowSentinelAbilityRT;
    [SerializeField] private RectTransform mirageAbilityRT;
    [SerializeField] private RectTransform darkHavenAbilityRT;
    [SerializeField] private Vector2 abilitySelectedSize;
    [SerializeField] private Vector2 abilityNotSelectedSize;
    [SerializeField] private TMP_Text dashNumberText;
    [SerializeField] private TMP_Text visionNumberText;
    [SerializeField] private TMP_Text shadowFormNumberText;
    [SerializeField] private TMP_Text shadowSentinelNumberText;
    [SerializeField] private TMP_Text mirageNumberText;
    [SerializeField] private TMP_Text darkHavenNumberText;
    [SerializeField] private Color abilitySelectedColor;
    [SerializeField] private Color abilityNotSelectedColor;
    // [SerializeField] private GameObject awarenessRaisedFromWhereIndicator;
    [SerializeField] private GameObject[] awarenessRaisedIndicators;
    [SerializeField] private Color indicatorLookOrBelowColor = Color.white;
    [SerializeField] private Color indicatorSuspicionColor = Color.yellow;
    [SerializeField] private Color indicatorSpottedColor = Color.red;
    [SerializeField] private Vector2 indicatorBelowLookSize = new Vector2(12, 12);
    [SerializeField] private Vector2 indicatorLookSize = new Vector2(17, 17);
    [SerializeField] private Vector2 indicatorSuspicionSize = new Vector2(22, 22);
    [SerializeField] private Vector2 indicatorSpottedSize = new Vector2(25, 25);
    private List<AwarenessIndicatorData> awarenessRaisedIndicatorDatas;
    [Space]
    [SerializeField] private SkinnedMeshRenderer handsMeshRenderer;
    [SerializeField] private GameObject daggerMeshRenderer;
    [Space]
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private bool canDash = true;
    [SerializeField] private Image dashCooldownFillImage;
    [Space]
    [SerializeField] private float visionRange = 15f;
    [SerializeField] private float visionDuration = 5f;
    [SerializeField] private int visionManaChargesCost = 2;
    [SerializeField] private Image visionFillImage;
    private PlayerActiveAbilities currentlySelectedAbility = PlayerActiveAbilities.Vision;
    [Space] //----------------------------------------------------------------------------------------------------------
    [Space]
    [SerializeField] private Volume visionVolume;
    [SerializeField] private Volume shadowFormVolume;
    [SerializeField] private Volume binocularVolume;
    [SerializeField] private float normalFieldOfView = 70;
    [SerializeField] private float binocularFieldOfView = 20;
    [Space]
    [SerializeField] private Volume damagedVolume;
    [Space]
    [SerializeField] private float damagedVolumeTransitionSpeed = 1f;
    [SerializeField] private Coroutine damagedVisualCoroutine;
    [SerializeField] private ParticleSystem parryParticleSystem = null;
    [Space]
    [Space]
    [SerializeField] private Material[] originalMaterials;
    [SerializeField] private Material shadowFormMaterial;
    [Space]
    [SerializeField][Range(0, 8)] private int manaCharges = 4;
    [SerializeField] private float maxMana = 200f;
    [SerializeField] private float oneManaCharge = 50f;
    [SerializeField] private float manaRegenRate = 1.4f; // 1.4f
    [SerializeField] private Image[] manaFills;
    [Space]
    [SerializeField] private int dashManaChargesCost = 1;
    [SerializeField] private Transform dashTargetVisualize;
    private Transform dashTargetActualPosition;
    [SerializeField] private float dashDistance;
    [SerializeField] private LayerMask dashIgnoreLayers;
    [Space]
    [SerializeField] private int shadowFormManaChargesCost = 1;
    [SerializeField] private float shadowFormDefaultDuration = 25f;
    [SerializeField] private float shadowFormVisualReductionMultiplier = 0.85f;
    [SerializeField] private Image shadowFormFillImage;
    [SerializeField] private float shadowFormEndDisableInputTime = 0.3f;
    [SerializeField] private bool shadowFormAdditionalSpeedEnabled = false;
    [SerializeField] private bool shadowFormAdditionalJumpStrengthEnabled = false;
    [Space]
    [SerializeField] private int shadowSentinelManaChargesCost = 2;
    [SerializeField] private int maxShadowSentinels = 1;
    [SerializeField] private GameObject shadowSentinel;
    [SerializeField] private bool sentinelVisionEnabled = false;
    private bool sentinelVisionOn = false;
    [SerializeField] private GameObject shadowSentinelCamera;
    [SerializeField] private float sentinelArmTime = 2f;
    [SerializeField] private bool isSentinelAlwaysLethal = false;
    [Space]
    [SerializeField] private float mirageManaChargesCost = 1;
    [SerializeField] private float mirageAwarenessCaused = 100f;
    [SerializeField] private float mirageRadius = 8f;
    [SerializeField] private float mirageMaxDistance = 20f;
    [SerializeField] private GameObject mirage;
    [Space]
    [SerializeField] private GameObject darkHavenProjectile;
    [SerializeField] private float darkHavenThrowSpeed;
    [SerializeField] private float darkHavenManaChargesCost = 1;
    [SerializeField] private int darkHavenMaximumAmount = 2;
     private List<GameObject> darkHavenList = new List<GameObject>();
    [SerializeField] private bool darkHavenTunnelsEnabled = true;
    [SerializeField] private GameObject darkHavenTunnel;
    private SplineContainer darkHavenTunnelInstance;
    [SerializeField] private bool darkHavenSizeIncreasedEnabled = false;
    [SerializeField] private Vector3 darkHavenIncreasedScale = new Vector3(1.5f, 1.5f, 1.5f);
    [SerializeField] private float darkHavenTunnelsIncreasedRadius = 1.5f;
    [Space]
    [SerializeField] private float dropAssassinationCheckDistance;
    [SerializeField] private LayerMask dropAssassinationBelowCheckLayers;
    [SerializeField] private float dropAssassinationSphereCastRadius;
    [SerializeField] private float dropAssassinationRaycastMinDistance = 3f;
    [Space]
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private Image healthFill;
    [Space]
    [SerializeField] private LayerMask scanForEnemyCorpseLayer;
    [Space]
    [SerializeField] private GameObject carryPrompt;
    [SerializeField] private GameObject dropObjectPrompt;
    [SerializeField] private TMP_Text carryText_buttonDisplay;
    [SerializeField] private Image carryImage;
    [Space]
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private TextMeshProUGUI interactText;
    private Interactable currentInteractable;
    

    private float currentHealth;

    private Vector3 dashPos_;

    private float currentMana;

    // Normal parry window  23 start 27 end
    // Extended parry window 12 start 30 end

    private enum PlayerActiveAbilities { Dash, Vision, ShadowForm, ShadowSentinel, Mirage, DarkHaven, None }

    public static event Action<bool, float> OnXRayVisionToggled;
    public static event Action OnPlayerDash;

    private Animator playerHandsAnimator;
    private PlayerMovement pm;

    private List<EnemyCommands> enemiesInsideNoiseBubble;
    private float maxNoiseDistance;

    [Space]
    [SerializeField] private Camera playerCam;
    [SerializeField] private Camera armsCam;
    [SerializeField] private Camera xRayCam;
    [Space]
    [SerializeField] private bool playerCanKillIfNotInVision = false;
    [Space]

    [SerializeField] private Transform cameraDeathTransform;
    [SerializeField] private float cameraDeathTransitionSpeed;


    private Coroutine visionCoroutine;
    private bool isVisionActive = false;
    private float visionTimer = 0;

    private float awarenessCurrentlyGenerated = 0f;

    private bool isAimingDash = false;
    private Ray dashAimRay;
    private int findWhichManaFillIsRegeningNumber;

    private bool isShadowFormActive = false;
    private Vector3 shadowFormStartPosition;
    private Quaternion shadowFormStartRotation;
    private float shadowFormStart_MouseXRot;
    private Coroutine shadowFormCoroutine;
    private Material[] shadowFormMaterials;
    private float shadowFormTimer = 0;

    private List<ShadowSentinel> activeShadowSentinels;

    private Ray mirageAimRay;
    private Collider[] mirageHitColliders;

    private Ray dropAssassinationRay;
    private Collider[] collidersHitLastFrameDropAssassination;
    private bool dropAssassinationEnabledMouseImage = false;



    private bool isParryActive = false;
    private bool isParryAnimationHappening = false;

    private CarryObject currentCarryObject = null;
    private LayerMask defaultLayer;
    private Ray dropObjectRay;
    private RaycastHit rh_dropObject;
    private Vector3 _playerLookRotOnly_Y;

    private ObjectiveManager objectiveManager = null;
    [Space]
    [SerializeField] private TextMeshProUGUI objectiveTextPrefab = null;
    [SerializeField] private Transform objectivesPanel = null;
    [SerializeField] private GameObject objectivesRootPanel = null;
    [SerializeField] private TextMeshProUGUI objectiveMessageText = null;
    [SerializeField] private float objectivePopupTime = 3f;
    private Coroutine objectiveDisplayCoroutine = null;

    private bool isAbandoningMission = false;
    [Space]
    [SerializeField] private TextMeshProUGUI abandonMissionText = null;
    [SerializeField] private GameObject abandonMissionPanel = null;
    [SerializeField] private int abandonMissionTime = 5;
    private Coroutine abandonMissionCoroutine;


    /*
    private struct DarkHavenTunnelData
    {
        public GameObject DarkHaven1;
        public GameObject DarkHaven2;
        public GameObject DarkHavenTunnel;

        public void SetData(GameObject _DarkHaven1, GameObject _DarkHaven2, GameObject _DarkHavenTunnel)
        {
            DarkHaven1 = _DarkHaven1;
            DarkHaven2 = _DarkHaven2;
            DarkHavenTunnel = _DarkHavenTunnel;
        }
    }
    */


    public static event Action OnPlayerDied;

    private bool shadowDashUnlocked = true;
    private bool predatorVisionUnlocked = false;
    private bool shadowFormUnlocked = false;
    private bool shadowSentinelUnlocked = false;
    private bool mirageUnlocked = false;
    private bool darkHavenUnlocked = false;

    private class AwarenessIndicatorData
    {
        public GameObject indicator;
        private Image indicatorImage;
        public EnemyCommands enemyConnected;
        public float timer;

        public AwarenessIndicatorData(GameObject _i, EnemyCommands _e)
        {
            indicator = _i;
            indicatorImage = indicator.transform.GetChild(0).GetComponent<Image>(); // the parent is only the direction it should look, I made a child that has an image component
            enemyConnected = _e;
            timer = Mathf.Infinity;
        }

        public void SetTimer(float _t)
        {
            timer = _t;
        }

        public void SetColor(Color _c)
        {
            indicatorImage.color = _c;
        }

        public void SetSize(Vector2 _s)
        {
            indicatorImage.rectTransform.sizeDelta = _s;
        }

    }


    public Vector3 GetCurrentPlayerHeadPosition()
    {
       // return transform.position + (Vector3.up * 0.5f);

        
        if(pm.IsSneaking())
        {
             return transform.position + (Vector3.up * 0.7f);
            //return transform.position;
        }
        else
        {
             return transform.position + (Vector3.up * 0.5f);
           // return transform.position;
        }
        
    }

    public Vector3 GetFuturePlayerPosition(float _velocityDivide = 1)
    {
        return GetCurrentPlayerHeadPosition() + (pm.GetPlayerVelocity() / _velocityDivide);
    }


    private Ray assassinationRay;
    private RaycastHit rh;

    private Vector3 pcpoeRot;
    private PlayerCamPositionsOnEnemy pcpoe;
    private float heightDifferenceWithEnemy;

    private bool actionsEnabled = true;

    private RaycastHit rh_interaction;
    private Ray interactionRay;

    private Vector3 dir_;
    private Quaternion lookRot_;
    private Vector3 northDirection_;
    private Coroutine disableAwarenessLineCoroutine;

    private Color eyeImageColor;
    private bool lastFrameAwarenessRaised = false;

    public bool IndicatorsContainEnemy(EnemyCommands _e, out int _index)
    {
        for (int i = 0; i < awarenessRaisedIndicatorDatas.Count; i++)
        {
            if (awarenessRaisedIndicatorDatas[i].enemyConnected == _e)
            {
                _index = i;
                return true;
            }
        }
        _index = -1;
        return false;
    }

    public bool IndicatorsContainEnemy(EnemyCommands _e)
    {
        for (int i = 0; i < awarenessRaisedIndicatorDatas.Count; i++)
        {
            if (awarenessRaisedIndicatorDatas[i].enemyConnected == _e)
            {
                return true;
            }
        }
        return false;
    }


    public bool ReceiveDamage(float _damage, EnemyBehaviour _attacker)
    {
        if (currentHealth == 0) { return true; }

        if (!isParryActive)
        {
            if (isShadowFormActive)
            {
                ForceCancelShadowForm();
                return true;
            }

            if (damagedVisualCoroutine != null)
            {
                StopCoroutine(damagedVisualCoroutine);
            }
            damagedVisualCoroutine = StartCoroutine(DamagedVisual());

            currentHealth = Mathf.Clamp(currentHealth - _damage, 0, maxHealth);
            healthFill.fillAmount = currentHealth / maxHealth;

            binocularVolume.enabled = false;
            playerCam.fieldOfView = normalFieldOfView;
            armsCam.fieldOfView = normalFieldOfView;
            xRayCam.fieldOfView = normalFieldOfView;

            if(sentinelVisionEnabled && sentinelVisionOn && activeShadowSentinels.Count > 0)
            {
                shadowSentinelCamera.SetActive(true);
            }


            if (damagedVisualCoroutine != null)
            {
                StopCoroutine(damagedVisualCoroutine);
            }

            if (currentHealth <= 0)
            {
                pm.DisableMovement();
                pm.DeathAnimation();


                shadowFormVolume.enabled = false;
                visionVolume.enabled = false;
                actionsEnabled = false;
                damagedVisualCoroutine = StartCoroutine(DamagedVisual(true));
                currentHealth = 0;

                StartCoroutine(MoveCameraToDeathPosition());
                

                OnPlayerDied?.Invoke();
            }
            else
            {
                pm.PlayerHit();
                damagedVisualCoroutine = StartCoroutine(DamagedVisual());
            }

            return true;
        }
        else
        {

            isParryActive = false;
            playerHandsAnimator.SetTrigger("AttackParried");
            if (_attacker != null)
            {
                _attacker.Stunned();
            }

            parryParticleSystem.Play();
            return false;
            //  CancelDash(); // ???
        }
    }

    IEnumerator MoveCameraToDeathPosition() 
    {
        float _t = 0;
        Vector3 _startPos = playerCam.transform.position;
        Vector3 _endPos = pm.IsSneaking() ? transform.position : transform.position - (Vector3.up / 2);
        Quaternion _startRot = Quaternion.identity;

        while (_t <= 1)
        {
            playerCam.transform.position = Vector3.Lerp(_startPos, _endPos, _t);
            playerCam.transform.rotation = Quaternion.Lerp(_startRot, cameraDeathTransform.rotation, _t);
            _t += Time.deltaTime * cameraDeathTransitionSpeed;
            yield return new WaitForEndOfFrame();
        }

        playerCam.transform.position = Vector3.Lerp(_startPos, _endPos, 1);
        playerCam.transform.rotation = Quaternion.Lerp(_startRot, cameraDeathTransform.rotation, 1);

        yield return null;

    }

    IEnumerator DamagedVisual(bool _stayAtFull = false)
    {
        damagedVolume.enabled = true;
        damagedVolume.weight = 0.5f;

        while (damagedVolume.weight <= 1f)
        {
            damagedVolume.weight += Time.deltaTime * damagedVolumeTransitionSpeed;
            yield return new WaitForEndOfFrame();
        }
        damagedVolume.weight = 1;

        if (_stayAtFull) { yield break; }

        while (damagedVolume.weight > 0f)
        {
            damagedVolume.weight -= Time.deltaTime * damagedVolumeTransitionSpeed;
            yield return new WaitForEndOfFrame();
        }
        damagedVolume.weight = 0;

        damagedVolume.enabled = false;

        yield return new WaitForEndOfFrame();
    }

    void Start()
    {
        
        UpgradeTree.Load();

      //  playerCam = GetComponentInChildren<Camera>();
        pm = GetComponent<PlayerMovement>();
        playerHandsAnimator = GetComponentInChildren<Animator>();
        enemiesInsideNoiseBubble = new List<EnemyCommands>();
        maxNoiseDistance = noiseBubbleTrigger.radius;
        eyeImageColor = Color.white;
        leftMouseImage.enabled = false;
        objectiveManager = FindObjectOfType<ObjectiveManager>();

#if UNITY_EDITOR
        if(objectiveManager == null || objectiveTextPrefab == null || objectivesPanel == null || objectivesRootPanel == null || objectiveMessageText == null)
        {
            Debug.LogError("Objectives not set up properly");
            EditorApplication.isPlaying = false; return;
            
        }
#endif



        dashTargetActualPosition = FindObjectOfType<PlayerDashPositionPredicter>().transform;
#if UNITY_EDITOR
        if (dashTargetActualPosition == null)
        {
            Debug.LogError("DashVisualizationTarget does not exist in the scene!");
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
#endif
        dashTargetVisualize.gameObject.SetActive(false);
        SelectDashAbility();

        originalMaterials = handsMeshRenderer.GetComponent<SkinnedMeshRenderer>().materials;
        shadowFormMaterials = new Material[3] { shadowFormMaterial, shadowFormMaterial, shadowFormMaterial };

        activeShadowSentinels = new List<ShadowSentinel>();

        awarenessRaisedIndicatorDatas = new List<AwarenessIndicatorData>();
        for (int i = 0; i < awarenessRaisedIndicators.Length; i++)
        {
            awarenessRaisedIndicatorDatas.Add(new AwarenessIndicatorData(awarenessRaisedIndicators[i], null));
            awarenessRaisedIndicators[i].SetActive(false);
        }

      

        currentHealth = maxHealth;
        healthFill.fillAmount = currentHealth / maxHealth;

        carryText_buttonDisplay.text = KeyCode.R.ToString(); // temp
        carryPrompt.SetActive(false);
        dropObjectPrompt.SetActive(false);
        carryImage.enabled = false;

        defaultLayer = LayerMask.GetMask("Default", "Transparent");

        shadowSentinelCamera.SetActive(false);

        abandonMissionPanel.SetActive(false);
 

        DarkHavenProjectile.OnDarkHavenCreated += DarkHavenCreated;
        DarkHavenObject.OnDarkHavenObjectTouched += DarkHavenDestroyed;
        ShadowSentinel.OnShadowSentinelDestroyed += ShadowSentinelDestroyed;
        objectiveManager.OnObjectiveChanged += DisplayObjectives;


        SetPlayerAttributes();

        currentMana = maxMana;

        for (int i = 0; i < manaFills.Length; i++)
        {
            manaFills[i].transform.parent.gameObject.SetActive(false);
        }

        for (int i = 0; i < manaCharges; i++)
        {
            manaFills[i].transform.parent.gameObject.SetActive(true);
        }

        DisplayObjectives("");
        objectiveMessageText.enabled = false;
        objectivesRootPanel.SetActive(false);

    }

    private void SetPlayerAttributes()
    {
        #region ShadowDash
        shadowDashUnlocked = UpgradeTree.GetAbility(UpgradeTree.ShadowDashUpgrades.AbilityName).IsUnlocked();
        if(!shadowDashUnlocked)
        { 
            dashAbilityRT.gameObject.SetActive(false);
            currentlySelectedAbility = PlayerActiveAbilities.None;
        }
        else
        {
            if(UpgradeTree.GetUpgrade(UpgradeTree.ShadowDashUpgrades.AbilityName, UpgradeTree.ShadowDashUpgrades.Distance2).IsUnlocked())
            {
                dashDistance = UpgradeStats.ShadowDash.Distance.distance2;
            }
            else if(UpgradeTree.GetUpgrade(UpgradeTree.ShadowDashUpgrades.AbilityName, UpgradeTree.ShadowDashUpgrades.Distance1).IsUnlocked())
            {
                dashDistance = UpgradeStats.ShadowDash.Distance.distance1;
            }
            else 
            {
                dashDistance = UpgradeStats.ShadowDash.Distance.def;
            }


            if (UpgradeTree.GetUpgrade(UpgradeTree.ShadowDashUpgrades.AbilityName, UpgradeTree.ShadowDashUpgrades.ManaCost).IsUnlocked())
            {
                dashManaChargesCost = UpgradeStats.ShadowDash.ManaCost.decreased;
            }
            else
            {
                dashManaChargesCost = UpgradeStats.ShadowDash.ManaCost.def;
            }


            if (UpgradeTree.GetUpgrade(UpgradeTree.ShadowDashUpgrades.AbilityName, UpgradeTree.ShadowDashUpgrades.Cooldown).IsUnlocked())
            {
                dashCooldown = UpgradeStats.ShadowDash.Cooldown.decreased;
            }
            else
            {
                dashCooldown = UpgradeStats.ShadowDash.Cooldown.def;
            }
        }
        #endregion

        #region PredatorVision
        predatorVisionUnlocked = UpgradeTree.GetAbility(UpgradeTree.PredatorVisionUpgrades.AbilityName).IsUnlocked();
        if(!predatorVisionUnlocked)
        {
            visionAbilityRT.gameObject.SetActive(false);
        }
        else
        {
            if (UpgradeTree.GetUpgrade(UpgradeTree.PredatorVisionUpgrades.AbilityName, UpgradeTree.PredatorVisionUpgrades.Distance2).IsUnlocked())
            {
                visionRange = UpgradeStats.PredatorVision.Distance.distance2;
            }
            else if (UpgradeTree.GetUpgrade(UpgradeTree.PredatorVisionUpgrades.AbilityName, UpgradeTree.PredatorVisionUpgrades.Distance1).IsUnlocked())
            {
                visionRange = UpgradeStats.PredatorVision.Distance.distance1;
            }
            else
            {
                visionRange = UpgradeStats.PredatorVision.Distance.def;
            }

            if(UpgradeTree.GetUpgrade(UpgradeTree.PredatorVisionUpgrades.AbilityName, UpgradeTree.PredatorVisionUpgrades.ManaCost).IsUnlocked())
            {
                visionManaChargesCost = UpgradeStats.PredatorVision.ManaCost.decreased;
            }
            else
            {
                visionManaChargesCost = UpgradeStats.PredatorVision.ManaCost.def;
            }

            if (UpgradeTree.GetUpgrade(UpgradeTree.PredatorVisionUpgrades.AbilityName, UpgradeTree.PredatorVisionUpgrades.Time2).IsUnlocked())
            {
                visionDuration = UpgradeStats.PredatorVision.Duration.duration2;
            }
            else if (UpgradeTree.GetUpgrade(UpgradeTree.PredatorVisionUpgrades.AbilityName, UpgradeTree.PredatorVisionUpgrades.Time2).IsUnlocked())
            {
                visionDuration = UpgradeStats.PredatorVision.Duration.duration1;
            }
            else
            {
                visionDuration = UpgradeStats.PredatorVision.Duration.def;
            }
        }
        #endregion

        #region ShadowForm
        shadowFormUnlocked = UpgradeTree.GetAbility(UpgradeTree.ShadowFormUpgrades.AbilityName).IsUnlocked();
        if (!shadowFormUnlocked)
        {
            shadowFormAbilityRT.gameObject.SetActive(false);
        }
        else
        {
            if (UpgradeTree.GetUpgrade(UpgradeTree.ShadowFormUpgrades.AbilityName, UpgradeTree.ShadowFormUpgrades.Time2).IsUnlocked())
            {
                shadowFormDefaultDuration = UpgradeStats.ShadowForm.Duration.duration2;
            }
            else if (UpgradeTree.GetUpgrade(UpgradeTree.ShadowFormUpgrades.AbilityName, UpgradeTree.ShadowFormUpgrades.Time1).IsUnlocked())
            {
                shadowFormDefaultDuration = UpgradeStats.ShadowForm.Duration.duration1;
            }
            else
            {
                shadowFormDefaultDuration = UpgradeStats.ShadowForm.Duration.def;
            }

            shadowFormAdditionalSpeedEnabled = UpgradeTree.GetUpgrade(UpgradeTree.ShadowFormUpgrades.AbilityName, UpgradeTree.ShadowFormUpgrades.Speed).IsUnlocked();
            shadowFormAdditionalJumpStrengthEnabled = UpgradeTree.GetUpgrade(UpgradeTree.ShadowFormUpgrades.AbilityName, UpgradeTree.ShadowFormUpgrades.Jump).IsUnlocked();

        }
        #endregion

        #region ShadowSentinel
        shadowSentinelUnlocked = UpgradeTree.GetAbility(UpgradeTree.ShadowSentinelUpgrades.AbilityName).IsUnlocked();
        if(!shadowSentinelUnlocked)
        {
            shadowSentinelAbilityRT.gameObject.SetActive(false);
        }
        else
        {
            isSentinelAlwaysLethal = UpgradeTree.GetUpgrade(UpgradeTree.ShadowSentinelUpgrades.AbilityName, UpgradeTree.ShadowSentinelUpgrades.KillAware).IsUnlocked();

            if(UpgradeTree.GetUpgrade(UpgradeTree.ShadowSentinelUpgrades.AbilityName, UpgradeTree.ShadowSentinelUpgrades.MaxSentinels).IsUnlocked())
            {
                maxShadowSentinels = UpgradeStats.ShadowSentinel.Quantity.increased;
                sentinelVisionEnabled = false;
            }
            else
            {
                maxShadowSentinels = UpgradeStats.ShadowSentinel.Quantity.def;
                sentinelVisionEnabled = UpgradeTree.GetUpgrade(UpgradeTree.ShadowSentinelUpgrades.AbilityName, UpgradeTree.ShadowSentinelUpgrades.SentinelVision).IsUnlocked();
            }

            if (UpgradeTree.GetUpgrade(UpgradeTree.ShadowSentinelUpgrades.AbilityName, UpgradeTree.ShadowSentinelUpgrades.ManaCost).IsUnlocked())
            {
                shadowSentinelManaChargesCost = UpgradeStats.ShadowSentinel.ManaCost.decreased;
            }
            else
            {
                shadowSentinelManaChargesCost = UpgradeStats.ShadowSentinel.ManaCost.def;
            }


        }
        #endregion

        #region Mirage
        mirageUnlocked = UpgradeTree.GetAbility(UpgradeTree.MirageUpgrades.AbilityName).IsUnlocked();
        if(!mirageUnlocked)
        {
            mirageAbilityRT.gameObject.SetActive(false);
        }
        else
        {
            if(UpgradeTree.GetUpgrade(UpgradeTree.MirageUpgrades.AbilityName, UpgradeTree.MirageUpgrades.ManaCost).IsUnlocked())
            {
                mirageManaChargesCost = UpgradeStats.Mirage.ManaCost.decreased;
            }
            else
            {
                mirageManaChargesCost = UpgradeStats.Mirage.ManaCost.def;
            }
        }
        #endregion

        #region DarkHaven
        darkHavenUnlocked = UpgradeTree.GetAbility(UpgradeTree.DarkHavenUpgrades.AbilityName).IsUnlocked();
        if(!darkHavenUnlocked)
        {
            darkHavenAbilityRT.gameObject.SetActive(false);
        }
        else
        {
            darkHavenSizeIncreasedEnabled = UpgradeTree.GetUpgrade(UpgradeTree.DarkHavenUpgrades.AbilityName, UpgradeTree.DarkHavenUpgrades.Size).IsUnlocked();
            
            if(UpgradeTree.GetUpgrade(UpgradeTree.DarkHavenUpgrades.AbilityName, UpgradeTree.DarkHavenUpgrades.HavenPlus).IsUnlocked())
            {
                darkHavenMaximumAmount = UpgradeStats.DarkHaven.Quantity.increased;
                darkHavenTunnelsEnabled = UpgradeTree.GetUpgrade(UpgradeTree.DarkHavenUpgrades.AbilityName, UpgradeTree.DarkHavenUpgrades.HavenTunnel).IsUnlocked();
            }
            else
            {
                darkHavenMaximumAmount = UpgradeStats.DarkHaven.Quantity.def;
            }

            
        }

        darkHavenIncreasedScale = UpgradeStats.DarkHaven.Size.increased;
        darkHavenTunnelsIncreasedRadius = UpgradeStats.DarkHaven.Size.tunnelIncreased;

        #endregion

        #region ManaSlots

        if(UpgradeTree.GetUpgrade(UpgradeTree.ManaSlotsUpgrades.AbilityName, UpgradeTree.ManaSlotsUpgrades.ManaSlots8).IsUnlocked())
        {
            manaCharges = UpgradeStats.ManaSlots.ManaSlots8;
        }
        else if (UpgradeTree.GetUpgrade(UpgradeTree.ManaSlotsUpgrades.AbilityName, UpgradeTree.ManaSlotsUpgrades.ManaSlots7).IsUnlocked())
        {
            manaCharges = UpgradeStats.ManaSlots.ManaSlots7;
        }
        else if (UpgradeTree.GetUpgrade(UpgradeTree.ManaSlotsUpgrades.AbilityName, UpgradeTree.ManaSlotsUpgrades.ManaSlots6).IsUnlocked())
        {
            manaCharges = UpgradeStats.ManaSlots.ManaSlots6;
        }
        else if (UpgradeTree.GetUpgrade(UpgradeTree.ManaSlotsUpgrades.AbilityName, UpgradeTree.ManaSlotsUpgrades.ManaSlots5).IsUnlocked())
        {
            manaCharges = UpgradeStats.ManaSlots.ManaSlots5;
        }
        else if (UpgradeTree.GetUpgrade(UpgradeTree.ManaSlotsUpgrades.AbilityName, UpgradeTree.ManaSlotsUpgrades.ManaSlots4).IsUnlocked())
        {
            manaCharges = UpgradeStats.ManaSlots.ManaSlots4;
        }
        else if (UpgradeTree.GetUpgrade(UpgradeTree.ManaSlotsUpgrades.AbilityName, UpgradeTree.ManaSlotsUpgrades.ManaSlots3).IsUnlocked())
        {
            manaCharges = UpgradeStats.ManaSlots.ManaSlots3;
        }
        else
        {
            manaCharges = UpgradeStats.ManaSlots.def;
        }

        maxMana = manaCharges * oneManaCharge;
        #endregion

        #region ManaRegen
        if (UpgradeTree.GetUpgrade(UpgradeTree.ManaRegenUpgrades.AbilityName, UpgradeTree.ManaRegenUpgrades.ManaRegen4).IsUnlocked())
        {
            manaRegenRate = UpgradeStats.ManaRegen.ManaRegen4;
        }
        else if (UpgradeTree.GetUpgrade(UpgradeTree.ManaRegenUpgrades.AbilityName, UpgradeTree.ManaRegenUpgrades.ManaRegen3).IsUnlocked())
        {
            manaRegenRate = UpgradeStats.ManaRegen.ManaRegen3;
        }
        else if (UpgradeTree.GetUpgrade(UpgradeTree.ManaRegenUpgrades.AbilityName, UpgradeTree.ManaRegenUpgrades.ManaRegen2).IsUnlocked())
        {
            manaRegenRate = UpgradeStats.ManaRegen.ManaRegen2;
        }
        else if (UpgradeTree.GetUpgrade(UpgradeTree.ManaRegenUpgrades.AbilityName, UpgradeTree.ManaRegenUpgrades.ManaRegen1).IsUnlocked())
        {
            manaRegenRate = UpgradeStats.ManaRegen.ManaRegen1;
        }
        else
        {
            manaRegenRate = UpgradeStats.ManaRegen.def;
        }


        #endregion

        #region ViciousAssassin

        playerCanKillIfNotInVision = UpgradeTree.GetUpgrade(UpgradeTree.ViciousAssassinPerk.AbilityName, UpgradeTree.ViciousAssassinPerk.AbilityName).IsUnlocked();

        #endregion
    }

    private void DisplayObjectives(string _message)
    {
        foreach(Transform _child in objectivesPanel.transform)
        {
            Destroy(_child.gameObject);
        }

        Objective[] _objectives = objectiveManager.GetVisibleCurrentObjectives();

        for(int i = 0; i  < _objectives.Length; i++)
        {
            TextMeshProUGUI _newText = Instantiate(objectiveTextPrefab, Vector3.zero, Quaternion.identity, objectivesPanel).GetComponent<TextMeshProUGUI>();
            _newText.text = _objectives[i].description;

            _newText.color = _objectives[i].mainMission ? Color.green : Color.white;
        }

        if(objectiveDisplayCoroutine != null)
        {
            StopCoroutine(objectiveDisplayCoroutine);
            objectiveDisplayCoroutine = null;
        }
        objectiveDisplayCoroutine = StartCoroutine(DisplayObjectiveMessage(_message));
    }

    private IEnumerator DisplayObjectiveMessage(string _message)
    {

        if(_message == "") 
        { 
            objectiveMessageText.enabled = false;
            yield break; 
        }

        objectiveMessageText.enabled = true;
        objectiveMessageText.text = _message;
        yield return new WaitForSeconds(objectivePopupTime);
        objectiveMessageText.enabled = false;
    }

    private void OnDestroy()
    {
        DarkHavenProjectile.OnDarkHavenCreated -= DarkHavenCreated;
        DarkHavenObject.OnDarkHavenObjectTouched -= DarkHavenDestroyed;
        ShadowSentinel.OnShadowSentinelDestroyed -= ShadowSentinelDestroyed;
        objectiveManager.OnObjectiveChanged -= DisplayObjectives;
    }


    public void DarkHavenDestroyed(SplineContainer _sc, GameObject _go)
    {


        if (_sc == null)
        {
            if (darkHavenTunnelsEnabled)
            {
                if (darkHavenTunnelInstance != null)
                {

                    Destroy(darkHavenTunnelInstance.gameObject);
                    darkHavenTunnelInstance = null;
                }
            }

            if (_go != null)
            {
                darkHavenList.Remove(_go);
            }
        }


    }

    public void DarkHavenCreated(GameObject _newDarkHaven)
    {
        if(_newDarkHaven == null)
        {
            return;
        }

        if (darkHavenList.Count >= darkHavenMaximumAmount)
        {
            Destroy(darkHavenList[0]);
            darkHavenList.RemoveAt(0);
        }

        if(darkHavenSizeIncreasedEnabled)
        {
            _newDarkHaven.transform.localScale = darkHavenIncreasedScale;
        }

        darkHavenList.Add(_newDarkHaven);

        if(darkHavenTunnelsEnabled)
        {
            if(darkHavenList.Count > 1)    
            {
                if (darkHavenTunnelInstance != null)
                {
                    Destroy(darkHavenTunnelInstance.gameObject);
                    darkHavenTunnelInstance = null;
                }

                   
                darkHavenTunnelInstance = Instantiate(darkHavenTunnel, darkHavenList[darkHavenList.Count - 1].transform.position, Quaternion.identity)
                .GetComponent<SplineContainer>();

                

                if (darkHavenSizeIncreasedEnabled)
                {
                    darkHavenTunnelInstance.GetComponent<SplineExtrude>().Radius = darkHavenTunnelsIncreasedRadius;
                }


                Vector3 _localCords = darkHavenTunnelInstance.transform.InverseTransformPoint(darkHavenList[darkHavenList.Count - 2].transform.position);

                BezierKnot _bk = new BezierKnot(new Unity.Mathematics.float3(_localCords));
                darkHavenTunnelInstance.Spline.SetKnot(1, _bk);
            }
        }
    }

    public void AddEnemyInsideNoiseBubble(EnemyCommands _ec)
    {
        enemiesInsideNoiseBubble.Add(_ec);
    }

    public void RemoveEnemyInsideNoiseBubble(EnemyCommands _ec)
    {
        if (enemiesInsideNoiseBubble.Contains(_ec))
        {
            enemiesInsideNoiseBubble.Remove(_ec);
        }
    }

    private void SetAwarenessIndicatorAttributes(AwarenessIndicatorData _aid)
    {
        switch (_aid.enemyConnected.GetAwarenessState())
        {
            case EnemyCommands.AwarenessStates.None:
                _aid.SetColor(indicatorLookOrBelowColor);
                _aid.SetSize(indicatorBelowLookSize);
                break;
            case EnemyCommands.AwarenessStates.Look:
                _aid.SetColor(indicatorLookOrBelowColor);
                _aid.SetSize(indicatorLookSize);
                break;
            case EnemyCommands.AwarenessStates.Suspicion:
                _aid.SetColor(indicatorSuspicionColor);
                _aid.SetSize(indicatorSuspicionSize);
                break;
            case EnemyCommands.AwarenessStates.Spotted:
                _aid.SetColor(indicatorSpottedColor);
                _aid.SetSize(indicatorSpottedSize);
                break;
        }
    }



    public void AwarenessRaised(float _amount, Vector3 _from, EnemyCommands _enemy)
    {

        if (_amount <= 0) { return; }

        _amount = Mathf.Clamp(_amount, 0, awarenessIndicatorFullAmount);

        if (_amount > awarenessCurrentlyGenerated)
        {
            awarenessCurrentlyGenerated = _amount;
        }

        lastFrameAwarenessRaised = true;

        dir_ = _from - transform.position;
        lookRot_ = Quaternion.LookRotation(dir_);
        lookRot_.z = -lookRot_.y;
        lookRot_.x = 0;
        lookRot_.y = 0;

        northDirection_ = new Vector3(0, 0, transform.eulerAngles.y);


        if (IndicatorsContainEnemy(_enemy, out int j))
        {
            awarenessRaisedIndicatorDatas[j].indicator.transform.rotation = lookRot_ * Quaternion.Euler(northDirection_);
            awarenessRaisedIndicatorDatas[j].timer = 0f;
            SetAwarenessIndicatorAttributes(awarenessRaisedIndicatorDatas[j]);
            awarenessRaisedIndicatorDatas[j].indicator.SetActive(true);

        }
        else
        {
            for (int i = 0; i < awarenessRaisedIndicatorDatas.Count; i++)
            {
                if (awarenessRaisedIndicatorDatas[i].enemyConnected == null)
                {
                    awarenessRaisedIndicatorDatas[i].enemyConnected = _enemy;
                    awarenessRaisedIndicatorDatas[i].timer = 0f;
                    awarenessRaisedIndicatorDatas[i].indicator.transform.rotation = lookRot_ * Quaternion.Euler(northDirection_);
                    SetAwarenessIndicatorAttributes(awarenessRaisedIndicatorDatas[i]);
                    awarenessRaisedIndicatorDatas[i].indicator.SetActive(true);

                    break;
                }
            }
        }
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isShadowFormActive) { return; }

        if (hit.gameObject.layer == 6) // enemy
        {
            hit.transform.GetComponent<EnemyCommands>().ReceiveAwarenessSignal(9999);
        }
    }


    private void FixedUpdate()
    {


        if (!actionsEnabled) { return; }

        if (isVisionActive)
        {
            OnXRayVisionToggled?.Invoke(true, visionRange);
        }


        if (!isShadowFormActive)
        {
            if (pm.IsPlayerSprintingOnGround())
            {
                for (int i = 0; i < enemiesInsideNoiseBubble.Count; i++)
                {
                    enemiesInsideNoiseBubble[i].ReceiveAwarenessSignal
                        (sprintingNoise * (1 - (enemiesInsideNoiseBubble[i].transform.position - transform.position).magnitude / maxNoiseDistance));
                }
            }
            else if (pm.IsPlayerRunningOnGround())
            {
                for (int i = 0; i < enemiesInsideNoiseBubble.Count; i++)
                {
                    enemiesInsideNoiseBubble[i].ReceiveAwarenessSignal
                        (runningNoise * (1 - (enemiesInsideNoiseBubble[i].transform.position - transform.position).magnitude / maxNoiseDistance));
                }
            }
        }
    }

    public void LookForDropAssassinationTarget()
    {
        if (!actionsEnabled) { return; }

        dropAssassinationEnabledMouseImage = false;

        dropAssassinationRay = new Ray(transform.position - pm.GetPlayerFeetPosition(), Vector3.down);
        Debug.DrawLine(transform.position, Vector3.down * dropAssassinationCheckDistance);
        if (Physics.Raycast(dropAssassinationRay, out RaycastHit _hit, dropAssassinationCheckDistance, dropAssassinationBelowCheckLayers))
        {
            if (_hit.distance < dropAssassinationRaycastMinDistance) { return; }

            collidersHitLastFrameDropAssassination = Physics.OverlapSphere(_hit.point, dropAssassinationSphereCastRadius);
            for (int i = 0; i < collidersHitLastFrameDropAssassination.Length; i++)
            {
                if (collidersHitLastFrameDropAssassination[i].gameObject.layer == 6)
                {
                    EnemyCommands _enemy = collidersHitLastFrameDropAssassination[i].GetComponent<EnemyCommands>();

                    if (IsEnemyInStateToBeAssassinated(_enemy))
                    {
                        leftMouseImage.enabled = true;
                        dropAssassinationEnabledMouseImage = true;
                    }
                    else
                    {
                        leftMouseImage.enabled = false;
                    }

                    if (Input.GetKeyDown(KeyCode.Mouse0) &&
                        IsEnemyInStateToBeAssassinated(_enemy))
                    {
                        actionsEnabled = false;

                        pm.DisableCharacterController();
                        pm.DisableMovement();

                        playerHandsAnimator.SetBool("Sprinting", false);
                        #region TEMPORARY FOR TESTING

                        if (isAimingDash)
                        {
                            CancelDash();
                        }



                        pcpoe = _enemy.GetComponentInChildren<PlayerCamPositionsOnEnemy>();



                        Transform _pcpoeTransform = null;
                        Vector3 _enemyRotVector;

                        if (Vector3.Dot(transform.forward, _enemy.transform.forward) > 0) // you are behind
                        {




                            _enemy.GetComponent<EnemyBehaviour>().Die(3);
                            playerHandsAnimator.SetInteger("AssassinateType", 2); // back animation?
                            playerHandsAnimator.SetTrigger("Assassinate");


                            pcpoeRot = _enemy.transform.position - playerCam.transform.position;
                            pcpoeRot.y = 0;

                            pcpoe.transform.rotation = Quaternion.LookRotation(pcpoeRot);
                            pcpoe.transform.SetParent(null);
                            _pcpoeTransform = pcpoe.GetTransformById(3);
                            _enemyRotVector = _enemy.transform.position - _pcpoeTransform.transform.position;


                        }
                        else // you are in front
                        {
                            _enemy.GetComponent<EnemyBehaviour>().Die(2);
                            playerHandsAnimator.SetInteger("AssassinateType", 2);
                            playerHandsAnimator.SetTrigger("Assassinate");



                            pcpoeRot = playerCam.transform.position - _enemy.transform.position;
                            pcpoeRot.y = 0;

                            pcpoe.transform.rotation = Quaternion.LookRotation(pcpoeRot);
                            pcpoe.transform.SetParent(null);
                            _pcpoeTransform = pcpoe.GetTransformById(2);
                            _enemyRotVector = _pcpoeTransform.transform.position - _enemy.transform.position;


                        }

                        // THIS IS IMPORTANT, PLACE THE PLAYER TO A NEW POSITION
                        transform.position = pcpoe.transform.position - transform.forward;

                        StartCoroutine(PosCameraForAssassination(_pcpoeTransform, false));
                        StartCoroutine(RotCameraForAssassination(_pcpoeTransform, false));
                        //   rh.collider.transform.rotation = Quaternion.LookRotation(enemyRot_);
                        StartCoroutine(RotateEnemyForAssassination(_enemy.transform, Quaternion.LookRotation(pcpoeRot)));
                        playerHandsAnimator.transform.GetChild(0).gameObject.layer = 8;
                        handsMeshRenderer.gameObject.layer = 8;
                        daggerMeshRenderer.layer = 8;
                        Invoke(nameof(EnableMovementWithCamRotationXReset), dropAssassinationLength);
                        #endregion


                    }
                    return;
                }
            }
        }

    }

    public void EnableMovementWithCamRotationXReset()
    {
        playerCam.transform.localPosition = pm.GetOriginalCamPosRot().localPosition;
        //    Vector3 _newCamRotEuler = new Vector3(0, pm.GetOriginalCamPosRot().rotation.eulerAngles.y, pm.GetOriginalCamPosRot().rotation.eulerAngles.z);
        playerCam.transform.localRotation = Quaternion.Euler(Vector3.zero);
        pm.ResetCameraRot();
        handsMeshRenderer.gameObject.layer = 10;
        daggerMeshRenderer.layer = 10;
        pm.EnableCharacterController();
        pm.EnableMovement();

        if (currentHealth > 0)
        {
            actionsEnabled = true;
        }
    }




    public void PlayerGroundedThisFrame()
    {
        leftMouseImage.enabled = false;
        dropAssassinationEnabledMouseImage = false;

    }




    public void Landing(float _strength)
    {
        if (isShadowFormActive) { return; }

        for (int i = 0; i < enemiesInsideNoiseBubble.Count; i++)
        {
            enemiesInsideNoiseBubble[i].ReceiveAwarenessSignal(landingNoise * (1 - (enemiesInsideNoiseBubble[i].transform.position - transform.position).magnitude / maxNoiseDistance));
        }
    }


    public void AE_VisionActivated()
    {
        if (!actionsEnabled) { return; }

        OnXRayVisionToggled?.Invoke(true, visionRange);
        if (visionCoroutine != null)
        {
            StopCoroutine(visionCoroutine);
        }
        visionCoroutine = StartCoroutine(PlayerVisionTimer());

        isVisionActive = true;
        visionVolume.weight = 1;

        visionTimer = visionDuration;
    }

    public void AE_ShadowFormActivated()
    {
        if (!actionsEnabled) { return; }

        if (isShadowFormActive)
        {

        }
        else
        {
            shadowFormStartPosition = transform.position;
            shadowFormStartRotation = transform.rotation;
            shadowFormStart_MouseXRot = pm.GetCameraRotState();
            handsMeshRenderer.materials = shadowFormMaterials;

            shadowFormCoroutine = StartCoroutine(PlayerShadowFormTimer());
            isShadowFormActive = true;
            shadowFormVolume.weight = 1;

            shadowFormTimer = shadowFormDefaultDuration;
            pm.SetShadowFormBuffs(shadowFormAdditionalSpeedEnabled, shadowFormAdditionalJumpStrengthEnabled);
        }
    }

    public void AE_ShadowSentinelActivated()
    {
        if (!actionsEnabled) { return; }

        if (activeShadowSentinels.Count >= maxShadowSentinels)
        {
            Destroy(activeShadowSentinels[0].gameObject);
            activeShadowSentinels.RemoveAt(0);
        }

        ShadowSentinel _newSS = Instantiate(shadowSentinel, transform.position - pm.GetPlayerFeetPosition(), transform.rotation).GetComponent<ShadowSentinel>();
        _newSS.SetSentinelState(sentinelArmTime ,isSentinelAlwaysLethal ,sentinelVisionEnabled, playerCam.transform.rotation);
        activeShadowSentinels.Add(_newSS);

        if (sentinelVisionEnabled)
        {
            if (!binocularVolume.enabled)
            {
                shadowSentinelCamera.SetActive(true);
            }
            sentinelVisionOn = true;
        }
    }

    public void ShadowSentinelDestroyed(ShadowSentinel _ss)
    {
        activeShadowSentinels.Remove(_ss);

        if (sentinelVisionEnabled)
        {
            if (activeShadowSentinels.Count < 1)
            {
                shadowSentinelCamera.SetActive(false);
                sentinelVisionOn = false;
            }
        }
    }

    public void AE_DarkHavenActivated()
    {
        if (!actionsEnabled) { return; }

        Rigidbody _dhrb = Instantiate(darkHavenProjectile, playerCam.transform.position + playerCam.transform.forward, Quaternion.identity).GetComponent<Rigidbody>();
        _dhrb.velocity = playerCam.transform.forward * darkHavenThrowSpeed;
    }

    IEnumerator PlayerVisionTimer()
    {

        yield return new WaitForSeconds(visionDuration);

        visionFillImage.fillAmount = 0;
        OnXRayVisionToggled?.Invoke(false, visionRange);
        isVisionActive = false;
        visionVolume.weight = 0;
    }

    IEnumerator PlayerShadowFormTimer()
    {
        yield return new WaitForSeconds(shadowFormDefaultDuration);

        pm.ExecuteDash();
        shadowFormTimer = 0;
        shadowFormFillImage.fillAmount = 0;
        transform.position = shadowFormStartPosition;
        transform.rotation = shadowFormStartRotation;
        pm.SetCameraRot(shadowFormStart_MouseXRot);
        OnPlayerDash?.Invoke();
        pm.DashEnded();
        CancelDash();
        pm.EndShadowFormBuff();

        handsMeshRenderer.materials = originalMaterials;

        dashAimRay = new Ray(shadowFormStartPosition, transform.up);
        if (Physics.Raycast(dashAimRay, out RaycastHit _hit, 2f, Physics.AllLayers - dashIgnoreLayers))
        {
            pm.ForceSneak(true);
        }

        isShadowFormActive = false;
        //   shadowFormVolume.weight = 0;

        StartCoroutine(ShadowFormEndCoroutine());
    }

    private void Dash()
    {


        dashAimRay = new Ray(dashTargetActualPosition.transform.position, Vector3.up);
        if (Physics.Raycast(dashAimRay, out RaycastHit _hit2, 2f, Physics.AllLayers - dashIgnoreLayers))
        {
            pm.ForceSneak(true);
        }

        isAimingDash = false;
        pm.ExecuteDash();
        transform.position = IsSneaking() ? dashPos_ : dashPos_ + Vector3.up;
        currentMana -= oneManaCharge * dashManaChargesCost;
        dashTargetVisualize.gameObject.SetActive(false);
        OnPlayerDash?.Invoke();
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        canDash = false;

        float _timer = dashCooldown;
        while (_timer > 0)
        {
            _timer -= Time.deltaTime;
            dashCooldownFillImage.fillAmount = _timer / dashCooldown;
            yield return new WaitForEndOfFrame();
        }

        canDash = true;
        dashCooldownFillImage.fillAmount = 0;
        yield return new WaitForEndOfFrame();
    }

    private bool Mirage()
    {
        mirageHitColliders = null;

        mirageAimRay = new Ray(playerCam.transform.position, playerCam.transform.forward);

        if (Physics.Raycast(mirageAimRay, out RaycastHit _hit, mirageMaxDistance, Physics.AllLayers - dashIgnoreLayers))
        {
            Instantiate(mirage, _hit.point, Quaternion.identity);

            print(_hit.collider);
            mirageHitColliders = Physics.OverlapSphere(_hit.point, mirageRadius, Physics.AllLayers - dashIgnoreLayers);
            for (int i = 0; i < mirageHitColliders.Length; i++)
            {
                EnemyCommands _ec = mirageHitColliders[i].GetComponent<EnemyCommands>();
                if (_ec != null)
                {
                    _ec.RemoveLastPlayerLocation();
                    _ec.ReceiveAwarenessSignal(mirageAwarenessCaused, _hit.point + Vector3.up, false, false);
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }


    private void CancelDash()
    {
        isAimingDash = false;
        pm.CancelDash();
        dashTargetVisualize.gameObject.SetActive(false);
    }

    private void SelectDashAbility()
    {
        currentlySelectedAbility = PlayerActiveAbilities.Dash;
        dashNumberText.color = abilitySelectedColor;
        dashAbilityRT.sizeDelta = abilitySelectedSize;


        visionNumberText.color = abilityNotSelectedColor;
        visionAbilityRT.sizeDelta = abilityNotSelectedSize;

        shadowFormNumberText.color = abilityNotSelectedColor;
        shadowFormAbilityRT.sizeDelta = abilityNotSelectedSize;

        shadowSentinelNumberText.color = abilityNotSelectedColor;
        shadowSentinelAbilityRT.sizeDelta = abilityNotSelectedSize;

        mirageNumberText.color = abilityNotSelectedColor;
        mirageAbilityRT.sizeDelta = abilityNotSelectedSize;

        darkHavenNumberText.color = abilityNotSelectedColor;
        darkHavenAbilityRT.sizeDelta = abilityNotSelectedSize;

        currentAbilityImage_Dash.enabled = true;
        currentAbilityImage_Vision.enabled = false;
        currentAbilityImage_ShadowForm.enabled = false;
        currentAbilityImage_ShadowSentinel.enabled = false;
        currentAbilityImage_Mirage.enabled = false;
        currentAbilityImage_DarkHaven.enabled = false;

    }

    private void SelectVisionAbility()
    {
        currentlySelectedAbility = PlayerActiveAbilities.Vision;
        visionNumberText.color = abilitySelectedColor;
        visionAbilityRT.sizeDelta = abilitySelectedSize;

        dashNumberText.color = abilityNotSelectedColor;
        dashAbilityRT.sizeDelta = abilityNotSelectedSize;

        shadowFormNumberText.color = abilityNotSelectedColor;
        shadowFormAbilityRT.sizeDelta = abilityNotSelectedSize;

        shadowSentinelNumberText.color = abilityNotSelectedColor;
        shadowSentinelAbilityRT.sizeDelta = abilityNotSelectedSize;

        mirageNumberText.color = abilityNotSelectedColor;
        mirageAbilityRT.sizeDelta = abilityNotSelectedSize;

        darkHavenNumberText.color = abilityNotSelectedColor;
        darkHavenAbilityRT.sizeDelta = abilityNotSelectedSize;


        currentAbilityImage_Dash.enabled = false;
        currentAbilityImage_Vision.enabled = true;
        currentAbilityImage_ShadowForm.enabled = false;
        currentAbilityImage_ShadowSentinel.enabled = false;
        currentAbilityImage_Mirage.enabled = false;
        currentAbilityImage_DarkHaven.enabled = false;
    }

    private void SelectShadowFormAbility()
    {
        currentlySelectedAbility = PlayerActiveAbilities.ShadowForm;
        shadowFormNumberText.color = abilitySelectedColor;
        shadowFormAbilityRT.sizeDelta = abilitySelectedSize;

        dashNumberText.color = abilityNotSelectedColor;
        dashAbilityRT.sizeDelta = abilityNotSelectedSize;

        visionNumberText.color = abilityNotSelectedColor;
        visionAbilityRT.sizeDelta = abilityNotSelectedSize;

        shadowSentinelNumberText.color = abilityNotSelectedColor;
        shadowSentinelAbilityRT.sizeDelta = abilityNotSelectedSize;

        mirageNumberText.color = abilityNotSelectedColor;
        mirageAbilityRT.sizeDelta = abilityNotSelectedSize;

        darkHavenNumberText.color = abilityNotSelectedColor;
        darkHavenAbilityRT.sizeDelta = abilityNotSelectedSize;

        currentAbilityImage_Dash.enabled = false;
        currentAbilityImage_Vision.enabled = false;
        currentAbilityImage_ShadowForm.enabled = true;
        currentAbilityImage_ShadowSentinel.enabled = false;
        currentAbilityImage_Mirage.enabled = false;
        currentAbilityImage_DarkHaven.enabled = false;
    }

    private void SelectShadowSentinelAbility()
    {
        currentlySelectedAbility = PlayerActiveAbilities.ShadowSentinel;
        shadowSentinelNumberText.color = abilitySelectedColor;
        shadowSentinelAbilityRT.sizeDelta = abilitySelectedSize;


        shadowFormNumberText.color = abilityNotSelectedColor;
        shadowFormAbilityRT.sizeDelta = abilityNotSelectedSize;

        dashNumberText.color = abilityNotSelectedColor;
        dashAbilityRT.sizeDelta = abilityNotSelectedSize;

        visionNumberText.color = abilityNotSelectedColor;
        visionAbilityRT.sizeDelta = abilityNotSelectedSize;

        mirageNumberText.color = abilityNotSelectedColor;
        mirageAbilityRT.sizeDelta = abilityNotSelectedSize;

        darkHavenNumberText.color = abilityNotSelectedColor;
        darkHavenAbilityRT.sizeDelta = abilityNotSelectedSize;

        currentAbilityImage_Dash.enabled = false;
        currentAbilityImage_Vision.enabled = false;
        currentAbilityImage_ShadowForm.enabled = false;
        currentAbilityImage_ShadowSentinel.enabled = true;
        currentAbilityImage_Mirage.enabled = false;
        currentAbilityImage_DarkHaven.enabled = false;
    }

    private void SelectMirageAbility()
    {
        currentlySelectedAbility = PlayerActiveAbilities.Mirage;
        mirageNumberText.color = abilitySelectedColor;
        mirageAbilityRT.sizeDelta = abilitySelectedSize;

        shadowSentinelNumberText.color = abilityNotSelectedColor;
        shadowSentinelAbilityRT.sizeDelta = abilityNotSelectedSize;

        shadowFormNumberText.color = abilityNotSelectedColor;
        shadowFormAbilityRT.sizeDelta = abilityNotSelectedSize;

        dashNumberText.color = abilityNotSelectedColor;
        dashAbilityRT.sizeDelta = abilityNotSelectedSize;

        visionNumberText.color = abilityNotSelectedColor;
        visionAbilityRT.sizeDelta = abilityNotSelectedSize;

        darkHavenNumberText.color = abilityNotSelectedColor;
        darkHavenAbilityRT.sizeDelta = abilityNotSelectedSize;

        currentAbilityImage_Dash.enabled = false;
        currentAbilityImage_Vision.enabled = false;
        currentAbilityImage_ShadowForm.enabled = false;
        currentAbilityImage_ShadowSentinel.enabled = false;
        currentAbilityImage_Mirage.enabled = true;
        currentAbilityImage_DarkHaven.enabled = false;
    }

    private void SelectDarkHavenAbility()
    {
        currentlySelectedAbility = PlayerActiveAbilities.DarkHaven;
        darkHavenNumberText.color = abilitySelectedColor;
        darkHavenAbilityRT.sizeDelta = abilitySelectedSize;


        mirageNumberText.color = abilityNotSelectedColor;
        mirageAbilityRT.sizeDelta = abilityNotSelectedSize;

        shadowSentinelNumberText.color = abilityNotSelectedColor;
        shadowSentinelAbilityRT.sizeDelta = abilityNotSelectedSize;

        shadowFormNumberText.color = abilityNotSelectedColor;
        shadowFormAbilityRT.sizeDelta = abilityNotSelectedSize;

        dashNumberText.color = abilityNotSelectedColor;
        dashAbilityRT.sizeDelta = abilityNotSelectedSize;

        visionNumberText.color = abilityNotSelectedColor;
        visionAbilityRT.sizeDelta = abilityNotSelectedSize;



        currentAbilityImage_Dash.enabled = false;
        currentAbilityImage_Vision.enabled = false;
        currentAbilityImage_ShadowForm.enabled = false;
        currentAbilityImage_ShadowSentinel.enabled = false;
        currentAbilityImage_Mirage.enabled = false;
        currentAbilityImage_DarkHaven.enabled = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown
            (KeyCode.Escape))
        {
            if(!isAbandoningMission)
            {
               abandonMissionCoroutine = StartCoroutine(AbandonMission());
            }
            else
            {
                StopAbandonMission();
            }
        }

        if (!actionsEnabled) 
        {
            leftMouseImage.enabled = false;
            carryPrompt.SetActive(false);
            interactPrompt.SetActive(false);
            return; 
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            objectivesRootPanel.SetActive(!objectivesRootPanel.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && shadowDashUnlocked)
        {
            SelectDashAbility();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2) && predatorVisionUnlocked)
        {
            SelectVisionAbility();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3) && shadowFormUnlocked)
        {
            SelectShadowFormAbility();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4) && shadowSentinelUnlocked)
        {
            SelectShadowSentinelAbility();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && mirageUnlocked)
        {
            SelectMirageAbility();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha6) && darkHavenUnlocked)
        {
            SelectDarkHavenAbility();
        }

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            playerHandsAnimator.SetTrigger("Parry");
            isParryAnimationHappening = true;
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            binocularVolume.enabled = !binocularVolume.enabled;

            float _fov = binocularVolume.enabled ? binocularFieldOfView : normalFieldOfView;

            playerCam.fieldOfView = _fov;
            armsCam.fieldOfView = _fov;
            xRayCam.fieldOfView = _fov;

            if(binocularVolume.enabled)
            {
                if (sentinelVisionEnabled)
                {
                    shadowSentinelCamera.SetActive(false);
                }
            }
            else
            {
                if (sentinelVisionEnabled && sentinelVisionOn)
                {
                    shadowSentinelCamera.SetActive(true);
                }
            }
        }


            visionTimer = Mathf.Clamp(visionTimer - Time.deltaTime, 0, visionDuration);
        shadowFormTimer = Mathf.Clamp(shadowFormTimer - Time.deltaTime, 0, shadowFormDefaultDuration);

        if (isVisionActive || isShadowFormActive)
        {
            visionFillImage.fillAmount = visionTimer / visionDuration;
            shadowFormFillImage.fillAmount = shadowFormTimer / shadowFormDefaultDuration;
        }


        // OBJECT PUT DOWN CHECK
        if (currentCarryObject != null)
        {
            dropObjectRay = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(dropObjectRay, out rh_dropObject, interactDistance, defaultLayer))
            {
                dropObjectPrompt.SetActive(true);

                if (Input.GetKeyDown(KeyCode.R))
                {
              //      _playerLookRotOnly_Y = transform.position - transform.forward;
                    _playerLookRotOnly_Y = Quaternion.LookRotation(-transform.forward).eulerAngles;
                    pm.EndCarry();
                    carryImage.enabled = false;
                    currentCarryObject.PutDown(rh_dropObject.point,Quaternion.Euler(_playerLookRotOnly_Y));
                    currentCarryObject = null;
                    // set enemy animator state to dead, and immediately set it to the end point
                }

            }
            else
            {
                dropObjectPrompt.SetActive(false);
            }
        }
        else
        {
            dropObjectPrompt.SetActive(false);

            // OBJECT PICK UP CHECK
            interactionRay = new Ray(playerCam.transform.position, playerCam.transform.forward);
            if (Physics.Raycast(interactionRay, out rh_interaction, interactDistance, scanForEnemyCorpseLayer))
            {
                if (rh_interaction.collider.gameObject.layer == 14 && currentCarryObject == null)
                {
                    carryPrompt.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        pm.StartCarry();
                        carryImage.enabled = true;
                        currentCarryObject = rh_interaction.collider.GetComponent<CarryObject>();
                        currentCarryObject.PickedUp();
                        carryPrompt.SetActive(false);
                    }
                }
                else
                {
                    carryPrompt.SetActive(false);
                }

                
                
            }
            else
            {
                // turn off all interaction prompts
                carryPrompt.SetActive(false);
            }
        }



        interactionRay = new Ray(playerCam.transform.position, playerCam.transform.forward);
        if (Physics.Raycast(interactionRay, out rh_interaction, interactDistance, defaultLayer))
        {
            Interactable _interactable = rh_interaction.collider.GetComponent<Interactable>();
            if (_interactable != null)
            {
                if (currentInteractable == null)
                {

                    currentInteractable = _interactable;
                    interactText.text = _interactable.GetDisplayText();
                }
                else if (currentInteractable != _interactable)
                {
                    currentInteractable = _interactable;
                    interactText.text = _interactable.GetDisplayText();
                }

                if (currentInteractable.CanInteract())
                {
                    interactPrompt.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        currentInteractable.Interact();
                    }
                }
                else
                {
                    interactPrompt.SetActive(false);
                }

               

            }
            else
            {
                currentInteractable = null;
                interactPrompt.SetActive(false);
            }
        }
        else
        {
            interactPrompt.SetActive(false );
        }




    }

    IEnumerator AbandonMission()
    {
        isAbandoningMission = true;
        int _abandonTimer = abandonMissionTime;
        abandonMissionText.text = "Abandoning Mission... " + _abandonTimer;
        abandonMissionPanel.SetActive(true);

        while (_abandonTimer > 0)
        {

            yield return new WaitForSeconds(1);

            _abandonTimer--;
            abandonMissionText.text = "Abandoning Mission... " + _abandonTimer;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
        yield return null;
    }

    private void StopAbandonMission()
    {
        isAbandoningMission = false;
        if(abandonMissionCoroutine != null)
        {
            StopCoroutine(abandonMissionCoroutine);
        }
        abandonMissionText.text = "";
        abandonMissionPanel.SetActive(false);
    }

    public void AE_ParryActive()
    {
        isParryActive = true;
    }

    public void AE_ParryNotActive()
    {
        isParryActive = false;
    }

    public void AE_ParryEnd()
    {
        isParryAnimationHappening = false;
    }

    public bool IsParryAnimationHappening()
    {
        return isParryAnimationHappening;
    }

  
    private bool IsEnemyInStateToBeAssassinated(EnemyCommands _enemy)
    {
        return (_enemy.GetAwarenessPercent() < 1f || !_enemy.IsWeaponDrawn() || _enemy.IsStunned())
                    || (playerCanKillIfNotInVision && !_enemy.IsPlayerInVision());
    }

    void LateUpdate()
    {
        if (!actionsEnabled) { return; }

        if (!isVisionActive && !isShadowFormActive)
        {
            currentMana = Mathf.Clamp(currentMana + (Time.deltaTime * manaRegenRate), 0, maxMana);
        }


      //  if (currentMana > oneManaCharge)
        {
            findWhichManaFillIsRegeningNumber = Mathf.Clamp(Mathf.FloorToInt(currentMana / oneManaCharge),0,manaFills.Length -1);
            for (int i = findWhichManaFillIsRegeningNumber; i < manaFills.Length; i++)
            {
                manaFills[i].fillAmount = 0;
            }
            
            manaFills[findWhichManaFillIsRegeningNumber].fillAmount = (currentMana - (oneManaCharge * findWhichManaFillIsRegeningNumber)) / oneManaCharge;
        }
      

        


        for (int i = 0; i < awarenessRaisedIndicatorDatas.Count; i++)
        {
            if(awarenessRaisedIndicatorDatas[i].enemyConnected == null) { continue; }

            awarenessRaisedIndicatorDatas[i].timer += Time.deltaTime;
            if(awarenessRaisedIndicatorDatas[i].timer > awarenessIndicatorArrowDissappear)
            {
                awarenessRaisedIndicatorDatas[i].indicator.SetActive(false);
                awarenessRaisedIndicatorDatas[i].enemyConnected = null;
                awarenessRaisedIndicatorDatas[i].timer = Mathf.Infinity;
            }
        }


        if (!lastFrameAwarenessRaised)
        {
            awarenessCurrentlyGenerated -= Time.deltaTime * eyeTransparencyFadeSpeed;
        }
        else
        {
            lastFrameAwarenessRaised = false;
        }


        eyeImageColor.a = awarenessCurrentlyGenerated / awarenessIndicatorFullAmount;
        eyeImage.color = eyeImageColor;


        if (!actionsEnabled) { return; }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (currentlySelectedAbility == PlayerActiveAbilities.ShadowForm)
            {

                if (isShadowFormActive)
                {
                    ForceCancelShadowForm();
                }
                else if (currentMana >= oneManaCharge * shadowFormManaChargesCost)
                {
                    pm.ShadowFormPressed();
                    currentMana -= oneManaCharge * shadowFormManaChargesCost;
                }
            }
            else if (currentlySelectedAbility == PlayerActiveAbilities.Vision)
            {
                if (currentMana >= oneManaCharge * visionManaChargesCost)
                {

                    pm.VisionPressed();
                    currentMana -= oneManaCharge * visionManaChargesCost;
                }
            }
            else if (currentlySelectedAbility == PlayerActiveAbilities.Dash)
            {
                if (canDash)
                {
                    if (currentMana >= oneManaCharge * dashManaChargesCost)
                    {
                        pm.AimingDash();
                        isAimingDash = true;
                        dashTargetVisualize.gameObject.SetActive(true);
                    }
                }
            }
            else if (currentlySelectedAbility == PlayerActiveAbilities.ShadowSentinel)
            {
                if (currentMana >= oneManaCharge * shadowSentinelManaChargesCost)
                {
                    pm.ShadowSentinelPressed();
                    currentMana -= oneManaCharge * shadowSentinelManaChargesCost;
                }
            }
            else if (currentlySelectedAbility == PlayerActiveAbilities.Mirage)
            {
                if (currentMana >= oneManaCharge * mirageManaChargesCost)
                {
                    if(Mirage())
                    {
                        pm.MiragePressed();
                        currentMana -= oneManaCharge * mirageManaChargesCost;
                    }
                }
            }
            else if(currentlySelectedAbility == PlayerActiveAbilities.DarkHaven) 
            {
                if(currentMana >= oneManaCharge * darkHavenManaChargesCost)
                {
                    pm.DarkHavenPressed();
                    currentMana -= oneManaCharge * darkHavenManaChargesCost;
                }
            }
        }

        if(isAimingDash)
        {
            dashAimRay = new Ray(playerCam.transform.position, playerCam.transform.forward);
            if (Physics.Raycast(dashAimRay, out RaycastHit _hit, dashDistance, Physics.AllLayers - dashIgnoreLayers))
            {

                dashTargetVisualize.position = _hit.point;
                dashPos_ = _hit.point;
            }
            else
            {
                dashTargetVisualize.position = playerCam.transform.position + playerCam.transform.forward * dashDistance;
                dashPos_ = playerCam.transform.position + playerCam.transform.forward * dashDistance;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                CancelDash();
            }

            if(Input.GetKeyUp(KeyCode.Mouse1))
            {
                Dash();

                pm.DashEnded();
            }
        }

        if (currentlySelectedAbility == PlayerActiveAbilities.ShadowSentinel)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!binocularVolume.enabled)
                {
                    if (sentinelVisionOn)
                    {
                        sentinelVisionOn = false;
                        shadowSentinelCamera.SetActive(false);
                    }
                    else
                    {
                        sentinelVisionOn = true;
                        shadowSentinelCamera.SetActive(true);
                    }
                }
            }

        }
 
           
            
               
       

        assassinationRay = new Ray(playerCam.transform.position, playerCam.transform.forward * interactDistance);
        if (Physics.Raycast(assassinationRay, out rh,interactDistance,enemyLayer))
        {
            if (pm.IsGrounded())
            {
                EnemyCommands _enemy = rh.collider.GetComponent<EnemyCommands>();

                if (_enemy == null) { return; }
                heightDifferenceWithEnemy = IsSneaking() ?
                    Mathf.Abs(_enemy.transform.position.y - transform.position.y) :
                    Mathf.Abs(_enemy.transform.position.y - (transform.position.y - 1f));


                if (IsEnemyInStateToBeAssassinated(_enemy) && heightDifferenceWithEnemy <= maxHeightDifferenceToAllowNormalAssassination)
                {
                    leftMouseImage.enabled = true;
                }
                else
                {
                    leftMouseImage.enabled = false;
                }


                if (Input.GetKeyDown(KeyCode.Mouse0) && 
                    IsEnemyInStateToBeAssassinated(_enemy) &&
                heightDifferenceWithEnemy <= maxHeightDifferenceToAllowNormalAssassination &&
                pm.IsGrounded())
                {
                    actionsEnabled = false;

                    playerHandsAnimator.SetBool("Sprinting", false);

                    if (isAimingDash)
                    {
                        CancelDash();
                    }

                    pm.DisableMovement();

                    pcpoe = rh.collider.GetComponentInChildren<PlayerCamPositionsOnEnemy>();

                    Transform _pcpoeTransform = null;
                    Vector3 _enemyRotVector;

                    if (Vector3.Dot(transform.forward, rh.collider.transform.forward) > 0) // you are behind
                    {
                        rh.collider.GetComponent<EnemyBehaviour>().Die(1);
                        playerHandsAnimator.SetInteger("AssassinateType", 1);
                        playerHandsAnimator.SetTrigger("Assassinate");


                        pcpoeRot = rh.collider.transform.position - playerCam.transform.position;
                        pcpoeRot.y = 0;

                        pcpoe.transform.rotation = Quaternion.LookRotation(pcpoeRot);
                        pcpoe.transform.SetParent(null);
                        _pcpoeTransform = pcpoe.GetTransformById(1);
                        _enemyRotVector = rh.collider.transform.position - _pcpoeTransform.transform.position;



                    }
                    else // you are in front
                    {
                        rh.collider.GetComponent<EnemyBehaviour>().Die(0);
                        playerHandsAnimator.SetInteger("AssassinateType", 0);
                        playerHandsAnimator.SetTrigger("Assassinate");



                        pcpoeRot = playerCam.transform.position - rh.collider.transform.position;
                        pcpoeRot.y = 0;

                        pcpoe.transform.rotation = Quaternion.LookRotation(pcpoeRot);
                        pcpoe.transform.SetParent(null);
                        _pcpoeTransform = pcpoe.GetTransformById(0);
                        _enemyRotVector = _pcpoeTransform.transform.position - rh.collider.transform.position;


                    }


                    StartCoroutine(PosCameraForAssassination(_pcpoeTransform,true));
                    StartCoroutine(RotCameraForAssassination(_pcpoeTransform, true));






                    //   rh.collider.transform.rotation = Quaternion.LookRotation(enemyRot_);
                    StartCoroutine(RotateEnemyForAssassination(rh.collider.transform, Quaternion.LookRotation(pcpoeRot)));





                    playerHandsAnimator.transform.GetChild(0).gameObject.layer = 8;

                    handsMeshRenderer.gameObject.layer = 8;
                    daggerMeshRenderer.layer = 8;


                    Invoke(nameof(EnableMovement), normalAssassinationLength);
                }
            }
        }
        else if(!dropAssassinationEnabledMouseImage)
        {
            leftMouseImage.enabled = false;
        }
 

    }

    private void ForceCancelShadowForm()
    {
        pm.ExecuteDash();
        transform.position = shadowFormStartPosition;
        transform.rotation = shadowFormStartRotation;
        pm.SetCameraRot(shadowFormStart_MouseXRot);
        OnPlayerDash?.Invoke();
        pm.DashEnded();
        CancelDash();
        pm.EndShadowFormBuff();

        handsMeshRenderer.materials = originalMaterials;

        dashAimRay = new Ray(shadowFormStartPosition, transform.up);
        if (Physics.Raycast(dashAimRay, out RaycastHit _hit2, 2f, Physics.AllLayers - dashIgnoreLayers))
        {
            pm.ForceSneak(true);
        }

        isShadowFormActive = false;
     //   shadowFormVolume.weight = 0;
        shadowFormTimer = 0;
        shadowFormFillImage.fillAmount = 0;
        if (shadowFormCoroutine != null)
        {
            StopCoroutine(shadowFormCoroutine);
        }

        StartCoroutine(ShadowFormEndCoroutine());
    }

    IEnumerator ShadowFormEndCoroutine()
    {
        actionsEnabled = false;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        pm.DisableCharacterController();
        pm.DisableMovement();
        float _timer = 0;
        while(_timer < shadowFormEndDisableInputTime) 
        {
            _timer += Time.deltaTime;
            shadowFormVolume.weight = 1 - (_timer / shadowFormEndDisableInputTime);
            yield return new WaitForEndOfFrame();
        }

        shadowFormVolume.weight = 0;

        if (currentHealth > 0)
        {
            actionsEnabled = true;
            pm.EnableMovement();
            pm.EnableCharacterController();
        }

        yield return new WaitForEndOfFrame();
    }

    IEnumerator RotateEnemyForAssassination(Transform _enemy, Quaternion _lookRot)
    {
        float _iterator = 0;

        while(_iterator < 1f)
        {
           _enemy.rotation = Quaternion.Slerp(_enemy.rotation, _lookRot, _iterator);

            _iterator += Time.deltaTime * rotateEnemyForAssassinationSpeed;
            yield return new WaitForEndOfFrame();
        }

     

        yield return new WaitForEndOfFrame();
    }

    IEnumerator PosCameraForAssassination(Transform _pcpoe, bool _normalAssassination)
    {
        float _iterator = 0;


        while (_iterator < 1f)
        {
            playerCam.transform.position =
                        Vector3.Lerp(playerCam.transform.position, _pcpoe.position, _iterator);

            playerCam.transform.rotation = Quaternion.Slerp(playerCam.transform.rotation, _pcpoe.rotation, _iterator);

            if (_normalAssassination)
            {
                _iterator += Time.deltaTime * posCameraForAssassinationSpeed;
            }
            else
            {
                _iterator += Time.deltaTime * posCameraForAssassinationSpeedDropAssassination;
            }
            yield return new WaitForEndOfFrame();

        }

        yield return new WaitForEndOfFrame();
    }

    IEnumerator RotCameraForAssassination(Transform _pcpoe, bool _normalAssassination)
    {
        float _iterator = 0;


        while (_iterator < 1f)
        {
           

            playerCam.transform.rotation = Quaternion.Slerp(playerCam.transform.rotation, _pcpoe.rotation, _iterator);

            if (_normalAssassination)
            {
                _iterator += Time.deltaTime * rotCameraForAssassinationSpeed;
            }
            else
            {
                _iterator += Time.deltaTime * rotCameraForAssassinationSpeedDropAssassination;
            }
            yield return new WaitForEndOfFrame();

        }

        yield return new WaitForEndOfFrame();
    }


    public void EnableMovement()
    {
        playerCam.transform.localPosition = pm.GetOriginalCamPosRot().localPosition;
        playerCam.transform.rotation = pm.GetOriginalCamPosRot().rotation;
        handsMeshRenderer.gameObject.layer = 10;
        daggerMeshRenderer.layer = 10;
        pm.EnableMovement();

        if (currentHealth > 0)
        {
            actionsEnabled = true;
        }
    }


    //
   

    public bool IsSneaking()
    {
        return pm.IsSneaking();
    }

    public float SneakingVisualReduction()
    {
        return sneakingVisualReductionMultiplier;
    }

    public float PlayerUnderAnObjectVisualReduction()
    {
        return playerUnderSomethingVisualReductionMultiplier;
    }

    public float HeightDifferenceVisualReduction()
    {
        return heightDifferenceVisualReductionMultiplier;
    }

    public float GetShadowFormVisualReduction()
    {
        return shadowFormVisualReductionMultiplier;
    }

    public bool IsShadowFormActive()
    {
        return isShadowFormActive;
    }

    public bool IsUnderSomething()
    {
       return pm.IsPlayerUnderSomething();
    }

    public bool IsHeightDifferenceSignificant(Vector3 _pos)
    {
        if(Mathf.Abs(_pos.y - transform.position.y) > significantHeightDifference)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TeleportToLastGroundedLocation()
    {
        pm.ExecuteDash();
         NavMesh.SamplePosition(pm.GetLastGroundedLocation(), out NavMeshHit _hit, 20f, defaultLayer);
        transform.position = _hit.position + Vector3.up;
        pm.DashEnded();
    }
}
