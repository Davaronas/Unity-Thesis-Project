using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float gravity = -9.81f; // -12
    [Space]
    [SerializeField] private float baseMovementSpeed = 20f;
    [SerializeField] private float jumpStrenght = 40f;
    [SerializeField] private float loseJumpLevelSpeed = 0.1f; // 100
    [SerializeField] private float flyTimeThatStillAllowsJumping = 0.1f;
    [SerializeField] [Range(0f, 1f)] private float movementStrenghtMidJump = 0.45f;
    [SerializeField] private float fallingTimeForLandingToCreateNoise = 1f;
    [Space]
    [SerializeField] private float shadowFormSpeedBuffMultiplier = 1.3f;
    [SerializeField] private float shadowFormJumpBuffMultiplier = 1.5f;
    [Space]
    [SerializeField] private float rotateSensitivity = 50f;
    [SerializeField] private int maxY_Rotation = 80;
    [SerializeField] private int minY_Rotation = -80;
    [Space(10)]
    [SerializeField] private bool isDoubleJumpUnlocked = false;
    [Space]
    [SerializeField] private float leanSpeed = 0.05f;
    [SerializeField] private Transform cameraLeftLeanPosRot;
    [SerializeField] private Transform cameraRightLeanPosRot;
    [SerializeField] private Transform cameraTopLeanPosRot;

    [SerializeField] private Transform cameraLeftLeanRestrictedPosRot;
    [SerializeField] private Transform cameraRightLeanRestrictedPosRot;
    [SerializeField] private Transform cameraTopLeanRestrictedPosRot;

    [SerializeField] private Transform cameraOriginalPosRot;
    [SerializeField] private float maxLeanRayDistance_X = 0.7f;
    [SerializeField] private float maxLeanRayDistance_Y = 1.2f;
    [SerializeField] private LayerMask leanCheckLayers;
    [Space(5)]
    [SerializeField] private float sneakCooldown = 0.2f; // is this needed?
    [SerializeField] private float sneakMovementSpeed = 4f;
    [Space(5)]
    [SerializeField] private float sprintingMovementSpeed = 12f;
    [Space]
    [SerializeField] private float carryingSpeedMultiplier = 0.6f;
    [Space]
    [SerializeField] private float hitSpeedDebuffMultiplier = 0.5f;
    [SerializeField] private float speedDebuffLength = 1f;
    [SerializeField] private float zeroSpeedTime = 0.3f;
    [Space]

    private Animator playerHandsAnimator;





    private PlayerActions playerActions;
    private CharacterController cc = null;
    private Camera playerCamera = null;
    private Transform playerCameraRig = null;
  //  private PlayerGrapple playerGrappling = null;

    private int stopMovement = 0;



    private Vector3 movementVector_;
    private float horizontalInput_;
    private float verticalInput_;
    private Vector3 horizontalVector_;
    private Vector3 verticalVector_;
    private float movementMultiplier_;
    private bool isMoving = false;

    private float movementSpeed_;

    private Vector3 jumpVector_;
    private float jumpLevel_ = 0;
    private int doubleJumpCounter = 1;
    private float movementVector_X_BeforeJump_;
    private float movementVector_Y_BeforeJump_;
    private float midJumpHorizontalDif_ = 0;
    private float midJumpVerticalDif_ = 0;
    private float flyTime = 0;


    private float x_MouseInput_ = 0;
    private float y_MouseInput_ = 0;
    private Vector3 rotationVector_ = Vector3.zero;
    private float yLook_ = 0;
    private float xLook_ = 0;
    private float currentCameraRotation_X = 0;

    private bool isLeaning = false;
    private bool isSneaking = false;
    private float x_leanState = 0f;
    private float y_leanState = 0f;
    private Vector3 leaningEuler;
    private Vector3 leaningPositionVector_;
    private Vector3 cameraBasePosVectorOffset_;
    private Ray leanRay;
    private Ray constantUnderSomethingCheck;
    private bool isPlayerUnderSomething = false;

    private bool isSprinting = false;
    private bool sneakInterruptedSprint_BlockThisButtonSprinting = false;



    private Vector3 sneakCC_Center;
    private Vector3 standingCC_Center;

    private bool isCarrying = false;
    private float currentCarrySpeedMultiplier = 1f;
    
    

    private bool lastFrameGroundedState_ = true;
    private Coroutine headBobCoroutine = null;

    private Vector3 cameraBasePos;
    private Quaternion cameraBaseRot;
    private float headBobProgress = 0f;
    [Space]
    [SerializeField] private Vector3 y_cameraMoveHeadBob = new Vector3(0,0.2f,0);
    [SerializeField] private float headBobDownSpeed = 5f;
    [SerializeField] private float headBobUpSpeed = 5f;



    private bool firstMoveDone = false;
    public static Action OnFirstMovement;


    private float currentSpeedMultiplier = 1f;
    private float currentJumpMultiplier = 1f;
    private Coroutine speedDebuffCoroutine;


    private Vector3 lastGroundedPosition;

    private void FixedUpdate()
    {

        if(isSneaking)
        {
            constantUnderSomethingCheck = new Ray(cameraOriginalPosRot.position, transform.up * maxLeanRayDistance_Y);
            if (Physics.Raycast(constantUnderSomethingCheck, maxLeanRayDistance_X, leanCheckLayers))
            {
                isPlayerUnderSomething = true;

            }
            else
            {
                isPlayerUnderSomething = false;

            }
        }
        else
        {
            isPlayerUnderSomething = false;

        }
       
        
    }

    public void PlayerHit()
    {
        if (speedDebuffCoroutine != null)
        {
            StopCoroutine(speedDebuffCoroutine);
        }

        speedDebuffCoroutine = StartCoroutine(SpeedDebuff());
    }

    IEnumerator SpeedDebuff()
    {

        currentSpeedMultiplier = 0f;
        yield return new WaitForSeconds(zeroSpeedTime);

        currentSpeedMultiplier = hitSpeedDebuffMultiplier;
        while (currentSpeedMultiplier < 1f)
        {
            currentSpeedMultiplier += Time.deltaTime * speedDebuffLength;
            yield return new WaitForEndOfFrame();
        }

        currentSpeedMultiplier = 1f;


        yield return null;
    }
 

    void Start()
    {
        lastGroundedPosition = Vector3.zero;

        Application.targetFrameRate = 120;

        cc = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        playerCameraRig = playerCamera.transform.parent;
        playerHandsAnimator = GetComponentInChildren<Animator>();
        jumpLevel_ = gravity;


        cameraBasePos = playerCameraRig.transform.localPosition;
        cameraBaseRot = playerCameraRig.transform.localRotation;

        playerActions = GetComponent<PlayerActions>();

        Cursor.lockState = CursorLockMode.Locked;

        

        currentCameraRotation_X = 0f;

        sneakCC_Center = new Vector3(0, 0.5f, 0);
        standingCC_Center = new Vector3(0, 1f, 0);

        movementSpeed_ = baseMovementSpeed;
        isLeaning = true;

        cameraBasePosVectorOffset_ = new Vector3(0, 0.7f, 0);

        shadowFormSpeedBuffMultiplier = UpgradeStats.ShadowForm.Speed.increasedSpeed;
        shadowFormJumpBuffMultiplier = UpgradeStats.ShadowForm.Jump.increasedJump;


    }

    public void AE_ResetRightHandSprintingAnimation()
    {
        if (!playerActions.IsParryAnimationHappening())
        {
            playerHandsAnimator.SetTrigger("ResetRightHand");
        }
    }

    public void StartCarry()
    {
        isCarrying = true;
        currentCarrySpeedMultiplier = carryingSpeedMultiplier;
    }

    public void EndCarry()
    {
        isCarrying = false;
        currentCarrySpeedMultiplier = 1f;
    }

    public bool IsCarrying()
    {
        return isCarrying;
    }

    public Transform GetOriginalCamPosRot()
    {
        return cameraOriginalPosRot;
    }

    public void DisableCharacterController()
    {
        cc.enabled = false;
    }

    public void EnableCharacterController()
    {
        cc.enabled = true;
    }



    public void DisableMovement()
    {
        stopMovement = 1;
        playerHandsAnimator.SetBool("Moving", false);
        playerHandsAnimator.SetBool("Sprinting", false);
        playerHandsAnimator.SetFloat("Horizontal", 0);
        playerHandsAnimator.SetFloat("Vertical", 0);

        if(headBobCoroutine != null)
        {
            StopCoroutine(headBobCoroutine);
        }
    }

    public void DeathAnimation()
    {
        playerHandsAnimator.SetTrigger("Death");
    }

    public void VisionPressed()
    {
        playerHandsAnimator.SetTrigger("Vision");
    }

    public void ShadowFormPressed()
    {
        playerHandsAnimator.SetTrigger("ShadowForm");
    }

    public void MiragePressed()
    {
        playerHandsAnimator.SetTrigger("Mirage");
    }

    public void ShadowSentinelPressed()
    {
        playerHandsAnimator.SetTrigger("ShadowSentinel");
    }

    public void DarkHavenPressed()
    {
        playerHandsAnimator.SetTrigger("DarkHaven");
    }

    public void AimingDash()
    {
        playerHandsAnimator.SetBool("AimingDash",true);
    }


    public void ExecuteDash()
    {
        playerHandsAnimator.SetTrigger("Dash");
        jumpLevel_ = 0;
        cc.enabled = false;
    }

    public void DashEnded()
    {
        playerHandsAnimator.SetBool("AimingDash", false);
        cc.enabled = true;
    }

    public void CancelDash()
    {
        playerHandsAnimator.SetBool("AimingDash", false);
    }

    public void EnableMovement()
    {
        stopMovement = 0;
    }

    public void SetShadowFormBuffs(bool _speedEnabled, bool _jumpEnabled)
    {
        if (_speedEnabled)
        {
            currentSpeedMultiplier = shadowFormSpeedBuffMultiplier;
        }

        if(_jumpEnabled)
        {
            currentJumpMultiplier = shadowFormJumpBuffMultiplier;
        }
    }

    public void EndShadowFormBuff()
    {
        currentSpeedMultiplier = 1;
        currentJumpMultiplier = 1;
    }

    void Update()
    {
       
     

        if (IsMovementAllowed())
        {
           
            cc.Move(DetermineMovementVector());
             
            
            Rotation();
            Lean();
            Sneak();

            if(cc.isGrounded)
            {
                jumpLevel_ = gravity;
                doubleJumpCounter = 1;
            }

            jumpLevel_ -= Time.deltaTime * loseJumpLevelSpeed;
            jumpLevel_ = Mathf.Clamp(jumpLevel_, gravity, Mathf.Infinity);
        }

      
  

        if (cc.isGrounded)
        {
            if (lastFrameGroundedState_ != cc.isGrounded)
            {
                playerActions.PlayerGroundedThisFrame();
                {
                    if (headBobCoroutine != null)
                        StopCoroutine(headBobCoroutine);
                    headBobCoroutine = StartCoroutine(HeadBob());

                    if(flyTime >=  fallingTimeForLandingToCreateNoise)
                    {
                        LoudLanding();
                    }

                    
                }

            }

            flyTime = 0;
        }
        else
        {
            flyTime += Time.deltaTime;
            playerActions.LookForDropAssassinationTarget();
        }

        if (!cc.isGrounded && lastFrameGroundedState_ == true)
        {
            lastGroundedPosition = transform.position;
        }

            lastFrameGroundedState_ = cc.isGrounded;

    }


    private void Lean()
    {
        //if(headBobCoroutine != null) { print("Not null"); return; }

       

        if (Input.GetKey(KeyCode.Mouse2))
        {
            isLeaning = true;

            x_MouseInput_ = Input.GetAxis("Mouse X");
            y_MouseInput_ = Input.GetAxis("Mouse Y");

            x_leanState = Mathf.Clamp(x_leanState + (x_MouseInput_ * leanSpeed * Time.deltaTime),0,1);
            y_leanState = Mathf.Clamp(y_leanState + (y_MouseInput_ * leanSpeed * Time.deltaTime), 0, 1);

            // check restrictions

            #region LeanCheck

            if (x_leanState < 0.5f) // we are leaning left
            {

                leanRay = new Ray(cameraOriginalPosRot.position, -transform.right * maxLeanRayDistance_X);
                if (Physics.Raycast(leanRay, maxLeanRayDistance_X, leanCheckLayers)) // left restricted, something in the way
                {
                   
                    if (isSneaking) 
                    {
                        leanRay = new Ray(cameraOriginalPosRot.position, transform.up * maxLeanRayDistance_Y);
                        if (Physics.Raycast(leanRay, maxLeanRayDistance_Y, leanCheckLayers)) // if we are sneaking check if something is above or head
                        {
                            SetLeanPosVector(cameraLeftLeanRestrictedPosRot.localPosition, cameraRightLeanPosRot.localPosition, cameraTopLeanRestrictedPosRot.localPosition);
                            
                        }
                        else
                        {
                            SetLeanPosVector(cameraLeftLeanRestrictedPosRot.localPosition, cameraRightLeanPosRot.localPosition, cameraTopLeanPosRot.localPosition);
                        }
                    }
                    else // we don't need to check if we are not sneaking, standing is restricted top by default
                    {
                        SetLeanPosVector(cameraLeftLeanRestrictedPosRot.localPosition, cameraRightLeanPosRot.localPosition, cameraTopLeanRestrictedPosRot.localPosition);
                    }
                }
                else // left not restricted, AND we are leaning left
                {
                   

                    if (isSneaking)
                    {
                        leanRay = new Ray(cameraOriginalPosRot.position, transform.up * maxLeanRayDistance_Y);
                        if (Physics.Raycast(leanRay, maxLeanRayDistance_Y, leanCheckLayers)) // if we are sneaking check if something is above or head
                        {
                            SetLeanPosVector(cameraLeftLeanPosRot.localPosition, cameraRightLeanPosRot.localPosition, cameraTopLeanRestrictedPosRot.localPosition);
                            
                        }
                        else
                        {
                            SetLeanPosVector(cameraLeftLeanPosRot.localPosition, cameraRightLeanPosRot.localPosition, cameraTopLeanPosRot.localPosition);
                        }
                    }
                    else // we don't need to check if we are not sneaking, standing is restricted top by default
                    {
                        SetLeanPosVector(cameraLeftLeanPosRot.localPosition, cameraRightLeanPosRot.localPosition, cameraTopLeanRestrictedPosRot.localPosition);
                    }


                }

            }
            else if(x_leanState > 0.5f)
            {
        




                leanRay = new Ray(cameraOriginalPosRot.position, transform.right * maxLeanRayDistance_X);
                if (Physics.Raycast(leanRay, maxLeanRayDistance_X, leanCheckLayers)) // right restricted, something in the way
                {
                    if (isSneaking)
                    {

                        leanRay = new Ray(cameraOriginalPosRot.position, transform.up * maxLeanRayDistance_Y);
                        if (Physics.Raycast(leanRay, maxLeanRayDistance_Y, leanCheckLayers)) // if we are sneaking check if something is above or head
                        {
                            SetLeanPosVector(cameraLeftLeanPosRot.localPosition, cameraRightLeanRestrictedPosRot.localPosition, cameraTopLeanRestrictedPosRot.localPosition);
                            
                        }
                        else
                        {
                            SetLeanPosVector(cameraLeftLeanRestrictedPosRot.localPosition, cameraRightLeanRestrictedPosRot.localPosition, cameraTopLeanPosRot.localPosition);
                        }
                    }
                    else // we don't need to check if we are not sneaking, standing is restricted top by default
                    {
                        SetLeanPosVector(cameraLeftLeanPosRot.localPosition, cameraRightLeanRestrictedPosRot.localPosition, cameraTopLeanRestrictedPosRot.localPosition);
                    }
                }
                else // right not restricted, AND we are leaning left
                {
                    if (isSneaking)
                    {
                        leanRay = new Ray(cameraOriginalPosRot.position, transform.up * maxLeanRayDistance_Y);
                        if (Physics.Raycast(leanRay, maxLeanRayDistance_Y, leanCheckLayers)) // if we are sneaking check if something is above or head
                        {
                            SetLeanPosVector(cameraLeftLeanPosRot.localPosition, cameraRightLeanPosRot.localPosition, cameraTopLeanRestrictedPosRot.localPosition);
                            
                        }
                        else
                        {
                            SetLeanPosVector(cameraLeftLeanPosRot.localPosition, cameraRightLeanPosRot.localPosition, cameraTopLeanPosRot.localPosition);
                        }
                    }
                    else // we don't need to check if we are not sneaking, standing is restricted top by default
                    {
                        SetLeanPosVector(cameraLeftLeanPosRot.localPosition, cameraRightLeanPosRot.localPosition, cameraTopLeanRestrictedPosRot.localPosition);
                    }
                }
            }
            else
            {
                if (isSneaking)
                {
                    leanRay = new Ray(cameraOriginalPosRot.position, cameraTopLeanPosRot.position);
                    if (Physics.Raycast(leanRay, maxLeanRayDistance_Y, leanCheckLayers)) // if we are sneaking check if something is above or head
                    {
                        SetLeanPosVector(cameraLeftLeanPosRot.localPosition, cameraRightLeanPosRot.localPosition, cameraTopLeanRestrictedPosRot.localPosition);
                        
                    }
                    else
                    {
                        SetLeanPosVector(cameraLeftLeanPosRot.localPosition, cameraRightLeanPosRot.localPosition, cameraTopLeanPosRot.localPosition);

                    }
                }
                else// we don't need to check if we are not sneaking, standing is restricted top by default
                {
                    SetLeanPosVector(cameraLeftLeanPosRot.localPosition, cameraRightLeanPosRot.localPosition, cameraTopLeanRestrictedPosRot.localPosition);
                }
                            
            }
            #endregion

            playerCamera.transform.localPosition = leaningPositionVector_;

            leaningEuler = Quaternion.Lerp(cameraLeftLeanPosRot.rotation, cameraRightLeanPosRot.rotation, x_leanState).eulerAngles;
            leaningEuler.x = playerCamera.transform.rotation.eulerAngles.x;
            playerCamera.transform.rotation = Quaternion.Euler(leaningEuler);


        }
        else
        {
            if (isLeaning)
            {
                x_leanState = 0.5f;
                y_leanState = 0;

                playerCamera.transform.localPosition = cameraOriginalPosRot.localPosition;

                leaningEuler = Quaternion.Lerp(cameraLeftLeanPosRot.rotation, cameraRightLeanPosRot.rotation, x_leanState).eulerAngles;
                leaningEuler.x = playerCamera.transform.rotation.eulerAngles.x;
                playerCamera.transform.rotation = Quaternion.Euler(leaningEuler);

                isLeaning = false;

              
            }
        }
    }

    private void SetLeanPosVector(Vector3 _leftLean, Vector3 _rightLean,Vector3 _topLean)
    {
        leaningPositionVector_ = Vector3.Lerp(_leftLean, _rightLean, x_leanState) +
                            Vector3.Lerp(cameraOriginalPosRot.localPosition, _topLean, y_leanState) - cameraBasePosVectorOffset_; 
    }

    private void Sneak()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(!isSneaking)
            {
                cc.center = sneakCC_Center;
                cc.height = 1;
                movementSpeed_ = sneakMovementSpeed;
              //  transform.position += Vector3.down;
                isSneaking = true;
                playerHandsAnimator.SetBool("Sneaking", true);
                if(isSprinting)
                {
                    sneakInterruptedSprint_BlockThisButtonSprinting = true;
                    isSprinting = false;
                    playerHandsAnimator.SetBool("Sprinting", false);
                }
                

            }
            else
            {
                leanRay = new Ray(cameraOriginalPosRot.position, transform.up * maxLeanRayDistance_Y);
                if (!Physics.Raycast(leanRay, maxLeanRayDistance_Y, leanCheckLayers))
                {
                    transform.position += Vector3.up * 1.5f;
                    cc.center = Vector3.zero; //standingCC_Center;
                    cc.height = 2;
                    movementSpeed_ = baseMovementSpeed;
                    isSneaking = false;
                    playerHandsAnimator.SetBool("Sneaking", false);
                    
                }
            }
        }
    }

    public bool ForceSneak(bool _state)
    {
        if (!isSneaking && _state)
        {
            cc.center = sneakCC_Center;
            cc.height = 1;
            movementSpeed_ = sneakMovementSpeed;
            //  transform.position += Vector3.down;
            isSneaking = true;
            playerHandsAnimator.SetBool("Sneaking", true);
            return true;
        }
        else if(isSneaking && !_state)
        {
            leanRay = new Ray(cameraOriginalPosRot.position, transform.up * maxLeanRayDistance_Y);
            if (!Physics.Raycast(leanRay, maxLeanRayDistance_Y, leanCheckLayers))
            {
                transform.position += Vector3.up * 1.5f;
                cc.center = Vector3.zero; //standingCC_Center;
                cc.height = 2;
                movementSpeed_ = baseMovementSpeed;
                isSneaking = false;
                playerHandsAnimator.SetBool("Sneaking", false);
                return true;
            }
            return false;
        }
        return false;
    }


    IEnumerator HeadBob()
    {
        headBobProgress = 0;
        while (headBobProgress < 1)
        {
            playerCameraRig.transform.localPosition = Vector3.Lerp(cameraBasePos, cameraBasePos - y_cameraMoveHeadBob, headBobProgress + (headBobDownSpeed * Time.deltaTime));
            headBobProgress += headBobDownSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
           
        }

        headBobProgress = 0;
        while (headBobProgress < 1)
        {
            playerCameraRig.transform.localPosition = Vector3.Lerp(cameraBasePos - y_cameraMoveHeadBob, cameraBasePos, headBobProgress + (headBobUpSpeed * Time.deltaTime));
            headBobProgress += headBobUpSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }


        yield return null;
    }


 
        

    private Vector3 DetermineMovementVector()
    {
        

        horizontalInput_ = Input.GetAxis("Horizontal");
        verticalInput_ = Input.GetAxis("Vertical");

        if(horizontalInput_ == 0 && verticalInput_ == 0)
        {
            playerHandsAnimator.SetBool("Moving", false);
            isMoving = false;
        }
        else
        {
            isMoving = true;
            playerHandsAnimator.SetBool("Moving", true);
            playerHandsAnimator.SetFloat("Horizontal", horizontalInput_);
            playerHandsAnimator.SetFloat("Vertical", verticalInput_);

            if (verticalInput_ > 0)
            {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    if (!sneakInterruptedSprint_BlockThisButtonSprinting)
                    {
                        if (ForceSneak(false) || !isSneaking)
                        {
                            movementSpeed_ = sprintingMovementSpeed;
                            playerHandsAnimator.SetBool("Sprinting", true);
                            isSprinting = true;
                        }
                        else
                        {
                            playerHandsAnimator.SetBool("Sprinting", false);
                            isSprinting = true;
                        }
                    }
                }
                else
                {
                    playerHandsAnimator.SetBool("Sprinting", false);
                    isSprinting = true;
                    sneakInterruptedSprint_BlockThisButtonSprinting = false;
                    if (!isSneaking)
                    {
                        movementSpeed_ = baseMovementSpeed;
                    }
                }
            }
            else
            {
                playerHandsAnimator.SetBool("Sprinting", false);
                isSprinting = true;
                if (!isSneaking)
                {
                    movementSpeed_ = baseMovementSpeed;
                }
            }
           
         
        }


        movementMultiplier_ = Time.deltaTime * movementSpeed_;



        if (cc.isGrounded || flyTime < flyTimeThatStillAllowsJumping)
        {
            horizontalVector_ = transform.right * (horizontalInput_ * movementMultiplier_);
            verticalVector_ = transform.forward * (verticalInput_ * movementMultiplier_);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpLevel_ = jumpStrenght * currentJumpMultiplier;
                movementVector_X_BeforeJump_ = horizontalInput_;
                movementVector_Y_BeforeJump_ = verticalInput_;
            }
            else
            {
                if (isDoubleJumpUnlocked)
                {
                    if (doubleJumpCounter > 0)
                    {
                        jumpLevel_ = jumpStrenght;
                        doubleJumpCounter--;
                    }
                }
            }
        }
        else
        {
            // carry *before jump* movement

            midJumpHorizontalDif_ = Mathf.Abs(horizontalInput_ - movementVector_X_BeforeJump_);
            if(movementVector_X_BeforeJump_ < 0.2f && movementVector_X_BeforeJump_ > -0.2f)
            {
                midJumpHorizontalDif_ = 2;
            }

            if (midJumpHorizontalDif_ < 1) // Equal direction
            {
                horizontalVector_ = (transform.right * (movementVector_X_BeforeJump_ * movementMultiplier_));
            }
            else if(movementVector_X_BeforeJump_ > 0.1f || movementVector_X_BeforeJump_ < 0.1f)
            {
                horizontalVector_ = (transform.right * (horizontalInput_ * movementMultiplier_) * movementStrenghtMidJump) +
               (transform.right * (movementVector_X_BeforeJump_ * movementMultiplier_));
            }
           


            midJumpVerticalDif_ = Mathf.Abs(verticalInput_ - movementVector_Y_BeforeJump_);
            if (movementVector_Y_BeforeJump_ < 0.2f && movementVector_Y_BeforeJump_ > -0.2f)
            {
                midJumpVerticalDif_ = 2;
            }


            if (midJumpVerticalDif_ < 1) // If you press the same movement button
            {
                verticalVector_ = (transform.forward * (movementVector_Y_BeforeJump_ * movementMultiplier_));
            }
            else if(movementVector_Y_BeforeJump_ > 0.1f || movementVector_Y_BeforeJump_ < 0.1f)
            {
                verticalVector_ = (transform.forward * (verticalInput_ * movementMultiplier_) * movementStrenghtMidJump) +
                (transform.forward * (movementVector_Y_BeforeJump_ * movementMultiplier_));
            }
           
                
                

        }

        
        jumpVector_ = transform.up * jumpLevel_ * Time.deltaTime;

        

        movementVector_ = (Vector3.ClampMagnitude(horizontalVector_ + verticalVector_, Mathf.Max(horizontalVector_.magnitude,verticalVector_.magnitude)) * currentCarrySpeedMultiplier * currentSpeedMultiplier) + jumpVector_;


        if (!firstMoveDone)
        {
            if (horizontalInput_ > 0 || verticalInput_ > 0 || Input.GetKeyDown(KeyCode.Space))
            {
                firstMoveDone = true;
                OnFirstMovement?.Invoke();
            }
        }

       

        return movementVector_;
    }


    #region Rotation
    private void Rotation()
    {
        if (isLeaning) { return; }

        if (stopMovement == 1) { return; }

        if(playerCamera == null) {UnityEngine.Debug.LogError("There's no camera attached to the player"); return; }

        x_MouseInput_ = Input.GetAxis("Mouse X");
        y_MouseInput_ = Input.GetAxis("Mouse Y");

        RotatePlayer(x_MouseInput_);
        RotateCamera(y_MouseInput_);
    }

    public void RotatePlayer(float _x)
    {
        if (stopMovement == 1) { return; }

        if (_x == 0) { return; }

        yLook_ = _x * rotateSensitivity * Time.deltaTime;

        rotationVector_ = transform.up * yLook_;

        transform.rotation *= Quaternion.Euler(rotationVector_);
    }

    public void RotateCamera(float _y)
    {

        xLook_ = _y * rotateSensitivity * Time.deltaTime;

        currentCameraRotation_X -= xLook_;
        currentCameraRotation_X = Mathf.Clamp(currentCameraRotation_X, minY_Rotation, maxY_Rotation);


        playerCamera.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(currentCameraRotation_X, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z)),
         Quaternion.Euler(new Vector3(currentCameraRotation_X, 0, 0)),
          Time.deltaTime);

    }
    #endregion

    public void ResetCameraRot()
    {
        currentCameraRotation_X = 0;
    }

    public void SetCameraRot(float _x)
    {
        currentCameraRotation_X = _x;
    }
    public float GetCameraRotState()
    {
        return currentCameraRotation_X;
    }



    private bool IsMovementAllowed()
    {
        if (stopMovement == 0 && cc.enabled)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector3 GetPlayerFeetPosition()
    {
        if (isSneaking)
        {
            return Vector3.zero;
        }
        else
        {
            return Vector3.up;
        }
    }

    public bool IsGrounded()
    {
        return cc.isGrounded;
    }

    public bool IsPlayerSprintingOnGround()
    {
        if (!isSneaking && cc.isGrounded && isSprinting && isMoving)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsPlayerRunningOnGround()
    {
        if(!isSneaking && cc.isGrounded && isMoving)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void LoudLanding()
    {

        playerActions.Landing(jumpLevel_);
    }

    public Vector3 GetLastGroundedLocation()
    {
        return lastGroundedPosition;
    }


    public Vector3 GetPlayerVelocity()
    {
        return cc.velocity;
    }


    public bool IsSneaking()
    {
        return isSneaking;
    }

    public bool IsPlayerUnderSomething()
    {
        return isPlayerUnderSomething;
    }

  
}
