using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading.Tasks;

public class PlayerController : MonoBehaviour
{

    
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float horizontalSpeed = 1f;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject cinemachineCamera;
    [SerializeField] private float minXBoundary;
    [SerializeField] private float maxXBoundary;
    [SerializeField] private float minYBoundary;
    [SerializeField] private float maxYBoundary;
    [SerializeField] private SliderController healthBarSliderController;
    [SerializeField] private SliderController gravityBarSliderController;
    [SerializeField] private SliderController tempratureBarSliderController;
    [SerializeField] private GameObject skateBoard;

    public GravityMode gravityMode = GravityMode.Normal;
    private LTDescr backToForward;
    private float controlsDirection = 1f;
    public static PlayerController Instance;
    private bool backingToForward = false;
    private bool started = false;
    private bool canChangeGravity = true;
    private CharacterController playerController;
    private LevelManager levelManager;

    private bool isGravityBarEmpty;
    private bool wasGravityBarEmpty;
    public bool isGrounded => IsGrounded();


    private SoundManager soundManager;
    private PPController ppController;
    private Coroutine damageRoutine;
    private bool tempHot;
    private bool tempCold;
    public enum GravityMode
    {
        Normal,
        Inverted
    }

   


    private void CheckGravityBar()
    {
        float gravityBarValue = gravityBarSliderController.GetSliderValue();

        if(gravityBarValue <= 1)
        {
            wasGravityBarEmpty = true;
        }else if(gravityBarValue >= 99)
        {
            wasGravityBarEmpty = false;
        }
    }

    private void CheckTempratureBar()
    {
        float tempratureBarValue = tempratureBarSliderController.GetSliderValue();

        if (tempratureBarValue <= 25 && !tempHot && !tempCold)
        {

            tempCold = true;
            damageRoutine = StartCoroutine(DamageRoutine());
        }
        else if (tempratureBarValue >= 75 && !tempHot && !tempCold)
        {
            tempHot = true;
            damageRoutine = StartCoroutine(DamageRoutine());
        }
        else if(tempratureBarValue > 25 && tempratureBarValue < 75)
        {
            tempHot = false;
            tempCold = false;
            if(damageRoutine != null)StopCoroutine(damageRoutine);
        }
    }

    private IEnumerator DamageRoutine() {

        while (true) {

            yield return new WaitForSeconds(1f);
            ppController.PlayHitEffect();
            ApplyDamage(levelManager.temperatureDamage);
        }
        

    }
    private void CheckHealthBar()
    {
        float healthBarValue = healthBarSliderController.GetSliderValue();

        if (healthBarValue <= 1 && !levelManager.gameOver)
        {
            levelManager.gameOver = true;
            levelManager.Gameover();
            animator.SetTrigger("Die");
        }

    }

    private bool IsGrounded()
    {
        
        //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + (playerController.height / 2 + 0.1f), transform.position.z), Vector3.down, Color.green, 100);
        
        return Physics.Raycast(new Vector3(transform.position.x, transform.position.y + (playerController.height / 2 + 0.1f), transform.position.z), Vector3.down, LayerMask.NameToLayer("Ground")); // raycast down to look for ground is not detecting
    }
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //Physics.gravity *= 2;
        playerController = GetComponent<CharacterController>();
        levelManager = SingletonFactory.Instance.levelManager;
        soundManager = SingletonFactory.Instance.soundManager;
        ppController = SingletonFactory.Instance.ppController;
    }

    public void StartSkating()
    {
        animator.SetTrigger("Start Game");
    }
    [System.Obsolete]
    void Update()
    {
        if (!started || levelManager.gameOver || !levelManager.gameStarted) return;
        MoveForward();
        HandleMovementInput();        
        ApplyGravity();
        CheckGravityBar();
        CheckHealthBar();
        CheckTempratureBar();
        ConsumeGravity();
        HandleFlipMovement();
        HandleFalling();
        HandleLanding();
        
    }

    private void ConsumeGravity()
    {
        if (isGrounded)
        {
            gravityBarSliderController.AddValueToSlider(1000 * Time.deltaTime);
        }
        else
        {
            gravityBarSliderController.AddValueToSlider(-500 * Time.deltaTime);
        }

        
    }

    private void HandleFalling()
    {
        if (!isGrounded)
        {
            animator.ResetTrigger("Land");
            animator.SetTrigger("Fall");
        }
    }

    private void HandleLanding()
    {


        if ( isGrounded)
        {
            animator.ResetTrigger("Fall");
            animator.SetTrigger("Land");
        }

    }

    private void ApplyGravity()
    {
        playerController.Move(- Vector3.down * Time.deltaTime * Physics.gravity.y/2);
    }


    private void MoveForward()
    {
        playerController.Move(transform.forward * Time.deltaTime * movementSpeed);
    }

    [System.Obsolete]
    private void HandleMovementInput()
    {
        float xInput = Input.GetAxis("Horizontal");
        playerController.Move(Vector3.right * Time.deltaTime * xInput * horizontalSpeed * controlsDirection);
        

        if (Mathf.Abs(xInput) <= 0.01 && !backingToForward)
        {
            backingToForward = true;
            backToForward = LeanTween.rotateY(gameObject, 0, 0.1f).setOnComplete(() => { backingToForward = false; });
        }
        else
        {
            if (backToForward != null) {
                backToForward.cancel(gameObject);
                backingToForward = false;
            }
            transform.Rotate(Vector3.up, xInput * Time.deltaTime * rotationSpeed * controlsDirection, Space.World);
            if (transform.eulerAngles.y > 30 && transform.eulerAngles.y < 180)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 30, transform.eulerAngles.z);
            }
            else if (transform.eulerAngles.y > 180 && transform.eulerAngles.y < 330)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, -30, transform.eulerAngles.z);
            }
        }
        
    }

    private void HandleFlipMovement()
    {
        if (!canChangeGravity || isGravityBarEmpty || wasGravityBarEmpty) return;
        

        GravityMode prevGravityMode = gravityMode;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            gravityMode = GravityMode.Normal;
        }else if (Input.GetKeyDown(KeyCode.E))
        {
            gravityMode = GravityMode.Inverted;
        }
        if (gravityMode != prevGravityMode) FlipAll();
    }

    private void FlipAll()
    {


        soundManager.PlaySoundEffect(SoundManager.SoundEffect.GravityFlip);
        canChangeGravity = false;
        FlipGravity(gravityMode);
        FlipCamera(gravityMode);
        FlipControls();
    }

    private void FlipControls()
    {
        controlsDirection *= -1;
    }
    private void FlipGravity(GravityMode gravityMode)
    {

        switch (gravityMode)
        {
            case GravityMode.Normal:
                Physics.gravity = -VectorAbsY(Physics.gravity);
                break;

            case GravityMode.Inverted:
                Physics.gravity = VectorAbsY(Physics.gravity);
                break;
        }        

        
    }

    public void ResetGravity()
    {
        FlipGravity(GravityMode.Normal);
    }


    private void FlipCamera(GravityMode gravityMode)
    {
        switch (gravityMode)
        {
            case GravityMode.Normal:

                SlowDown();

                LeanTween.rotateZ(cinemachineCamera, 0f, 0.75f).setOnComplete(() => { canChangeGravity = true; SpeedUp(); });
                LeanTween.rotateZ(gameObject, 0f, 0.75f);
                break;

            case GravityMode.Inverted:

                SlowDown();

                LeanTween.rotateZ(cinemachineCamera, 180f, 0.75f).setOnComplete(() => { canChangeGravity = true; SpeedUp(); }); ;
                LeanTween.rotateZ(gameObject, 180f, 0.75f);

                break;
        }
        CinemachineTransposer cmTrans = cinemachineCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
        LeanTween.value(cmTrans.m_FollowOffset.y, -cmTrans.m_FollowOffset.y, 0.75f).setOnUpdate((float value) =>
        {
            cmTrans.m_FollowOffset = new Vector3(cmTrans.m_FollowOffset.x, value, cmTrans.m_FollowOffset.z);
        });

        
    }
    private Vector3 VectorAbsY(Vector3 vector3)
    {
        Vector3 result = new Vector3(vector3.x, Mathf.Abs(vector3.y), vector3.z);
        return result;
    }


    private void SlowDown()
    {

    }

    private void SpeedUp()
    {

    }

    public void StartFinish()
    {

        started = true;
    }

    public void StartForce()
    {
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Environment Layer Trigger"))
        {
            levelManager.UpdateEnvironmentLayers();
        }
        else if (other.gameObject.tag.Contains("Electricity") && !levelManager.gameOver && levelManager.gameStarted)
        {
            ApplyDamage(levelManager.electricityDamage);
            animator.SetTrigger("Hit");
            soundManager.PlaySoundEffect(SoundManager.SoundEffect.ElectricShock);
            ppController.PlayHitEffect();
        }
        
    }


    private void ApplyDamage(float damage)
    {
        healthBarSliderController.AddValueToSlider(- damage);
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        
        if (hit.gameObject.CompareTag("Fire Ground") && !levelManager.gameOver && levelManager.gameStarted)
        {
            tempratureBarSliderController.AddValueToSlider(400 * Time.deltaTime);

        }
        else if (hit.gameObject.CompareTag("Frost Ground") && !levelManager.gameOver && levelManager.gameStarted)
        {
            tempratureBarSliderController.AddValueToSlider(-400 * Time.deltaTime);

        }
    }

    public void DitachSkateBoard()
    {
    }

}
