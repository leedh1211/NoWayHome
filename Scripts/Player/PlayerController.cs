using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum LeftclickMode
{
    Normal,
    Building,
    MoveOnly
}

public class PlayerController : MonoBehaviour
{
    [Header("플레이어")]
    [Tooltip("움직이는 방향으로 회전하는 속도")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    [Tooltip("가속도")]
    public float SpeedChangeRate = 10.0f;

    [Tooltip("자체 중력 적용")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("점프 간 최소 딜레이")]
    public float JumpTimeout = 0.50f;

    [Tooltip("추락 상태로 돌입하는 시간")]
    public float FallTimeout = 0.15f;

    [Header("구르기")]
    public float RollSpeed = 6.0f;          // 구르기 속도
    public float RollDuration = 0.5f;       // 구르기 지속 시간 (애니메이션 길이와 맞추는 것이 좋음)
    public float RollCooldown = 0.8f;       // 구르기 후 다음 구르기까지의 쿨타임

    private bool _isRolling = false;        // 현재 구르기 중인지 상태 플래그
    private float _rollCooldownDelta = 0f;  // 현재 남은 쿨타임
    private Vector3 _rollDirection = Vector3.zero; // 구르기 방향
    private int _animIDRoll;

    [Header("스태미나 소모/화복량")]
    public float staminaRecoverPerTic = 1.5f;
    public float runStaminaPerTic = 1f;
    public float jumpStamina = 10f;
    public float rollStamina = 15f;


    [Header("땅")]
    [Tooltip("플레이어가 땅에 닿은 상태인지 체크")]
    public bool Grounded = true;

    [Tooltip("울퉁불퉁한 땅에 유용한 오프셋")]
    public float GroundedOffset = -0.14f;

    [Tooltip("CharacterController 의 Radius와 같은 값이어야 함")]
    public float GroundedRadius = 0.28f;

    [Tooltip("땅(Ground) 레이어")]
    public LayerMask GroundLayers;  

    [Tooltip("적(Enemy) 레이어")]
    public LayerMask EnemyLayer;

    [Header("Cinemachine")]
    [Tooltip("Cinemachine 카메라가 따라갈 오브젝트")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("마우스 감도 설정")]
    [Range(0.1f, 10f)] public float MouseSensivity = 1.0f;

    [Tooltip("위쪽으로 바라볼 수 있는 최대 각도")]
    public float TopClamp = 30.0f;

    [Tooltip("아래쪽으로 바라볼 수 있는 최대 각도")]
    public float BottomClamp = -30.0f;

    [Tooltip("카메라 고정")]
    public bool LockCameraPosition = false;

    [Header("총 설정")]
    public GunCtrl[] guns;
    public GunCtrl currentGun;
    public GameObject pickAxe;
    private int curGunIndex = 0;

    [Header("LeftClick")]
    public LeftclickMode currentLeftClickMode = LeftclickMode.Normal;

    [SerializeField]
    private GameObject aimObject;       //플레이어의 총, 머리가 바라보는 오브젝트
    [SerializeField] private LayerMask targetLayer;
    
    [Header("Scroll")] 
    [Tooltip("How much the mouse has scrolled")]
    public float scrollValue;
    
    [Tooltip("How fast the scroll moves")]
    public float scrollSpeed;
    

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    
    // player
    private float _speed;
    private Vector2 _moveDir = Vector2.zero;
    private Vector2 _lookDir = Vector2.zero;
    private bool _jump = false;
    private bool _sprint = false;
    private bool isAiming = false;
    private bool isShooting = false;
    private bool canMove = true;
    private bool isAimingMonster = false;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private bool isAbleInputSystem = true;
    private PlayerStat playerStat;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float shootingTimeDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    private int _animIDAiming;
    private int _animIDHoriznotal;
    private int _animIDVertical;
    private int _animIDShoot;
    private int _animIDReload;
    private int _animIDMining;
    
    private PlayerInput _playerInput;
    private PlayerStat _playerStat;
    private Animator _animator;
    private CharacterController _controller;
    private GameObject _mainCamera;
    private Coroutine aimingRoutine;
    private ObjectPreview _preview;

    private const float _threshold = 0.01f;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        
        _animator = GetComponent<Animator>();
        _playerStat = GetComponent<PlayerStat>();
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _preview = GetComponent<ObjectPreview>();
        playerStat = GetComponent<PlayerStat>();

        AssignAnimationIDs();

        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        EventBus.Subscribe<StartGame>(HandleGameStart);
    }

    //이벤트 구독,해제는 여기서!
    private void OnEnable()
    {
        EventBus.Subscribe<EnablePlayerInputEvent>(EnablePlayerInputHandler);
        EventBus.Subscribe<DisablePlayerInputEvent>(DisablePlayerInputHandler);
        EventBus.Subscribe<MouseSensitivityChangedEvent>(OnMouseSensitivityChanged);
        EventBus.Subscribe<GameOverEvent>(GameOverHandler);

        SceneManager.sceneLoaded += OnSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<EnablePlayerInputEvent>(EnablePlayerInputHandler);
        EventBus.UnSubscribe<DisablePlayerInputEvent>(DisablePlayerInputHandler);
        EventBus.UnSubscribe<MouseSensitivityChangedEvent>(OnMouseSensitivityChanged);
        EventBus.UnSubscribe<GameOverEvent>(GameOverHandler);

        SceneManager.sceneLoaded -= OnSceneLoadedEvent;
    }

    private void HandleGameStart(StartGame _)
    {
        GameInit();
    }

    private void GameInit()
    {
        Cursor.lockState = CursorLockMode.Locked;   //마우스 잠금
    }

    private void Update()
    {
        // 쿨타임 감소
        if (_rollCooldownDelta > 0f)
        {
            _rollCooldownDelta -= Time.deltaTime;
        }

        // 지면 체크 및 중력/점프 로직은 _verticalVelocity와 Grounded 상태를 계속 업데이트
        GroundedCheck();
        JumpAndGravity();

        // 플레이어의 일반적인 이동 및 액션 처리
        if (!_isRolling && canMove)
        {
            Move(); 
        }

        // 사격 로직 (구르기 중이 아니고, 움직일 수 있고, 조준 및 사격 상태일 때)
        if (isAiming && isShooting && canMove && !_isRolling)
        {
            Shoot();
        }
        if (!_sprint && Grounded && !_isRolling)    //달리기 중, 점프 중, 구르기 중이 아니라면 스태미나 자연 화복
        {
            playerStat.RecoverStamina(staminaRecoverPerTic * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void AssignAnimationIDs()   //애니메이션 ID 할당
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDAiming = Animator.StringToHash("Aiming");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDHoriznotal = Animator.StringToHash("X");
        _animIDVertical = Animator.StringToHash("Y");
        _animIDShoot = Animator.StringToHash("Shoot");
        _animIDReload = Animator.StringToHash("Reload");
        _animIDMining = Animator.StringToHash("Mining");
        _animIDRoll = Animator.StringToHash("Roll");
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
        _animator.SetBool(_animIDGrounded, Grounded);
    }

    private void CameraRotation()
    {     
        if (isAiming)
        {
            Vector3 targetPosition = Vector3.zero;
            RaycastHit hit;
            float rayDistance = 100f;
            if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward,
                out hit, rayDistance, EnemyLayer))
            {
                if (!isAimingMonster)
                {
                    isAimingMonster = true;
                    EventBus.Raise<PlayerAimMonsterEvent>(new PlayerAimMonsterEvent(true));
                }
              
            }
            else if (isAimingMonster)
            {
                isAimingMonster = false;
                EventBus.Raise<PlayerAimMonsterEvent>(new PlayerAimMonsterEvent(false));
            }

            targetPosition = _mainCamera.transform.position + _mainCamera.transform.forward * 100f;
            aimObject.transform.position = targetPosition;
      
            Vector3 targetAim = targetPosition;
            targetAim.y = transform.position.y;
            Vector3 aimDir = targetAim - transform.position;
            aimDir = aimDir.normalized;

            currentGun.transform.LookAt(aimObject.transform);
            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 30f);
        }
        else if(isAimingMonster)
        {
            isAimingMonster = false;
        }
       
        if (_lookDir.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            _cinemachineTargetYaw += _lookDir.x * MouseSensivity;
            _cinemachineTargetPitch += _lookDir.y * MouseSensivity;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        CinemachineCameraTarget.transform.rotation = 
            Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
        
    }

    private void Move()
    {
        float targetSpeed = _playerStat.CurrentWalkSpeed;

        if (_sprint) // 달리기 버튼을 누르고 있으면 더 빠르게 움직임
        {
            if(playerStat.UseStamina(runStaminaPerTic * Time.deltaTime)) //달리기 시 스태미나 소모
            {
                targetSpeed = _playerStat.CurrentRunSpeed;
            }
            else
            {
                _sprint = false;
            }
        }
       
       

        // 인풋 값이 없으면, 이동속도 0으로 조절
        if (_moveDir == Vector2.zero) targetSpeed = 0.0f;

        // 플레이어의 수직 velocity 값 참조
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _moveDir.magnitude;

        // 목표 속도로 감속/가속
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        if (_moveDir != Vector2.zero)
        {

            // 방향 벡터 normalize
            Vector3 inputDirection = new Vector3(_moveDir.x, 0.0f, _moveDir.y).normalized;

            if (isAiming)
            {
                inputDirection = transform.right * _moveDir.x + transform.forward * _moveDir.y;
                _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }

            else
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                          _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
        }
        else
        {
            _controller.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

      
     
        _animator.SetFloat(_animIDHoriznotal, _moveDir.x);
        _animator.SetFloat(_animIDVertical, _moveDir.y);
        _animator.SetFloat(_animIDSpeed, _animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }


    private void JumpAndGravity()
    {
        if (Grounded)
        {
            _fallTimeoutDelta = FallTimeout;
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDFreeFall, false);

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // 점프!
            if (_jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(_playerStat.CurrentJumpHeight * -2f * Gravity);
                _animator.SetBool(_animIDJump, true);
            }

            // 점프 타임아웃
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // 점프 타이머 초기화
            _jumpTimeoutDelta = JumpTimeout;

            // 추락 타임아웃
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _animator.SetBool(_animIDFreeFall, true);
            }
            _jump = false;
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }


    private void Shoot()
    {
        if (currentGun.Fire())
        {
            _animator.SetTrigger(_animIDShoot);
        }
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }

    private void StopMove(AnimationEvent animationEvent)   //이동 불가 애니메이션 이벤트
    {
       canMove = false;
    }

    private void StartMove(AnimationEvent animationEvent)   //이동 가능 애니메이션 이벤트
    {
        canMove = true;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDir = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookDir = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if (canMove && Grounded)  //움직일 수 있고, 점프 상태가 아닌 경우
            {
                if (playerStat.UseStamina(jumpStamina))
                {
                    _jump = true;
                }
            }
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            _sprint = true;
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            _sprint = false;
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        // 구르기는 땅에 있고, 현재 구르기 중이 아니며, 쿨타임이 아니고, 일반적인 움직임이 가능할 때만 실행
        if (context.phase == InputActionPhase.Performed && Grounded && !_isRolling && _rollCooldownDelta <= 0f && canMove)
        {
            if (playerStat.UseStamina(rollStamina))
            {
                StartCoroutine(RollRoutine());
            }
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (!canMove || _isRolling) return;

        switch (currentLeftClickMode)
        {
            case LeftclickMode.Normal:
                //견착
                if (context.phase == InputActionPhase.Started)
                {
                    EventBus.Raise(new PlayerAimEvent(true));
                    isAiming = true;
                    _animator.SetBool(_animIDAiming, true);
                }
                else if (context.phase == InputActionPhase.Canceled)
                {
                    EventBus.Raise(new PlayerAimEvent(false));
                    isShooting = false;
                    isAiming = false;
                    _animator.SetBool(_animIDAiming, false);
                }
                break;
            case LeftclickMode.Building:
                // Building UI에서 켜진 Building Object Preview 끄기
                // 이때 인벤토리에서 제거한 아이템의 수만큼 다시 추가해주는 작업 필요
                if (context.phase == InputActionPhase.Started)
                {
                    _preview.OffPreview();
                    BuildingManager.Instance.ui_building.ingredientCostControll(1);
                    EventBus.Raise(new CameraLayerRemoveEvent("InstallableField"));
                }
                break;
        }
        
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            isShooting = true;   
            //isAiming = true;
            //aimingRoutine = StartCoroutine(AimRoutine());
            //_animator.SetBool(_animIDAiming, true);
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            isShooting = false;
            //StopCoroutine(aimingRoutine);
            //isAiming = false;
            //_animator.SetBool(_animIDAiming, false);
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started) 
        {
            if (currentGun.Reload())
            {   //장전 성공
                _animator.SetTrigger(_animIDReload);
            }
        }
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (_isRolling) return;

        switch (currentLeftClickMode)
        {
            case LeftclickMode.Normal:
                if (isAiming)   //조준 중이면 총 발사
                {
                    if (context.phase == InputActionPhase.Started)
                    {
                        isShooting = true;
                        //isAiming = true;
                        //aimingRoutine = StartCoroutine(AimRoutine());
                        //_animator.SetBool(_animIDAiming, true);
                    }
                    else if (context.phase == InputActionPhase.Canceled)
                    {
                        isShooting = false;
                        //StopCoroutine(aimingRoutine);
                        //isAiming = false;
                        //_animator.SetBool(_animIDAiming, false);
                    }
                }
                else if(Grounded)   //조준 중이 아니고 땅에 있으면 도끼질? 시작
                {
                    if (context.phase == InputActionPhase.Started)
                    {
                        _animator.SetBool(_animIDMining, true);
                        currentGun.gameObject.SetActive(false);
                        pickAxe.SetActive(true);
                        canMove = false;
                    }
                    else if (context.phase == InputActionPhase.Canceled)
                    {
                        _animator.SetBool(_animIDMining, false);
                        currentGun.gameObject.SetActive(true);
                        pickAxe.SetActive(false);
                        canMove = true;
                    }
                }
                break;
            case LeftclickMode.Building:
                
                if (context.phase == InputActionPhase.Started)
                {
                    // BuildingManager에서 오브젝트 생성 호출
                    Onbuilding();
                }
                break;
            case LeftclickMode.MoveOnly:
                break;
        }
    }

    private void AnimatorReset()        //모든 애니메이터 파라미터를 리셋하는 함수
    {
        foreach (AnimatorControllerParameter param in _animator.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Float:
                    _animator.SetFloat(param.nameHash, param.defaultFloat); 
                    break;
                case AnimatorControllerParameterType.Int:
                    _animator.SetInteger(param.nameHash, param.defaultInt);
                    break;
                case AnimatorControllerParameterType.Bool:
                    _animator.SetBool(param.nameHash, param.defaultBool);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    _animator.ResetTrigger(param.nameHash); 
                    break;
                default:
                    break;
            }
        }
    }
    
    // ObjectPreview에서 오브젝트 생성 메서드 호출
    private void Onbuilding()
    {
        _preview.InstallObject();
    }
    
    // 마우스 휠키 입력 시, float 값으로 Objectpreview y 값 조절
    public void OnScrolling(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<float>();
        scrollValue += scroll * scrollSpeed; 
        scrollValue = Mathf.Max (0f, scrollValue);
        Debug.Log(scrollValue);
    }
    
    private void DisablePlayerInputHandler(DisablePlayerInputEvent args)
    {
        isAbleInputSystem = false;
        Cursor.lockState = CursorLockMode.None;
        _playerInput.DeactivateInput();
    }
    
    
    private void EnablePlayerInputHandler(EnablePlayerInputEvent args)
    {
        isAbleInputSystem = true;
        Cursor.lockState = CursorLockMode.Locked;
        _playerInput.ActivateInput();
    }

    
    private IEnumerator AimRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        isShooting = true;
        
    }

    // B 버튼 입력 시, 플레이어 인풋 비활성화 + UI 켜기
    public void OnBuilding(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            EventBus.Raise(new DisablePlayerInputEvent());
            // UI_Building 불러오기
            BuildingManager.Instance.ui_building.OpenBuildingWindow();
        }
    }
    
    // building UI 꺼질 때 플레이어 인풋 활성화
    public void offBuilding()
    {
        EventBus.Raise(new EnablePlayerInputEvent());
    }
    public void OnGunChange(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            curGunIndex++;
            if (curGunIndex >= guns.Length)
            {
                curGunIndex = 0;
            }

            currentGun.GunOff();
            currentGun = guns[curGunIndex];
            currentGun.GunOn();
        }
    }
    private void OnMouseSensitivityChanged(MouseSensitivityChangedEvent e)
    {
        MouseSensivity = e.Sensitivity;
        //currentLeftClickMode = LeftclickMode.Normal;
    }

    // 좌클릭 인풋을 Normal 상태(총쏘기, 곡괭이)로 변경
    public void SwitchleftInputNormal()
    {
        currentLeftClickMode = LeftclickMode.Normal;
        //CameraSwitcher.Instance.RemoveLayerFromCullingMask("InstallableField");
        EventBus.Raise(new CameraLayerRemoveEvent("InstallableField"));
    }

    // 좌클릭 인풋을 Building 상태(건축하기)로 변경
    public void SwitchleftInputBuilding()
    {
        currentLeftClickMode = LeftclickMode.Building;
        //CameraSwitcher.Instance.AddLayerToCullingMask("InstallableField");
        EventBus.Raise(new CameraLayerAddEvent("InstallableField"));
    }

    private void GameOverHandler(GameOverEvent evnt)
    {
        _moveDir = Vector2.zero;
        canMove = false;
        AnimatorReset();
    }
    private IEnumerator RollRoutine()       //구르기 루틴
    {
        _isRolling = true;
        isAiming = false;
        canMove = false;

        playerStat.SetInvincible(true);     //구르기 시 무적
        if (_moveDir.sqrMagnitude > _threshold) // 현재 이동 입력이 있다면 해당 방향으로
        {
            Vector3 inputDirection = new Vector3(_moveDir.x, 0.0f, _moveDir.y).normalized;
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            _rollDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            // 구르기 시작 시 캐릭터가 해당 방향을 즉시 또는 부드럽게 보도록 설정 가능
            transform.rotation = Quaternion.LookRotation(_rollDirection);
        }
        else // 이동 입력이 없다면 캐릭터의 현재 앞 방향으로 구르기
        {
            _rollDirection = transform.forward;
        }

        _animator.SetTrigger(_animIDRoll); 

        float startTime = Time.time;
        while (Time.time < startTime + RollDuration)
        {
            _controller.Move(_rollDirection * RollSpeed * Time.deltaTime + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            yield return null;
        }

        _isRolling = false;
        canMove = true; // 구르기 종료 후 일반 이동 가능하도록 복원
        _rollCooldownDelta = RollCooldown; // 쿨타임 설정
        playerStat.SetInvincible(false);
    }

    private void OnSceneLoadedEvent(Scene scene, LoadSceneMode mode)    //씬이 바뀔 때 호출되는 함수
    {
        switch (scene.name)
        {
            case "LobbyScene":
                Cursor.lockState = CursorLockMode.None; 
                currentLeftClickMode = LeftclickMode.MoveOnly;
                break;
            case "GameScene":
                currentLeftClickMode= LeftclickMode.Normal;
                break;
            default:
                break;
        }
    }
}