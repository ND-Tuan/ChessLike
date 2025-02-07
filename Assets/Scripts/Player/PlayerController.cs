using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ObserverPattern;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;

public class PlayerController : MonoBehaviour, IDamageable
{
    public enum PlayerState{
        TeleportIn,
        TeleportOut,
        Alive,
        Transforming,
        Death
    }
    //Player Status
    [Header("------Player Status------")]
    public PlayerState CurrentState;
    private PlayerState PreviousState;
    [SerializeField] private int _MaxHp;
    private int _CurrentHp;
    [SerializeField] private int _amor;
    [SerializeField] private float _Accurate;
    [SerializeField] private float _NormalSpeed;
    private float _movementspeed = 1;
    public bool Freezing { get; set; }
    public bool Burning { get; set; }


    //Dash
    [Header("------Dash------")]
    
    [SerializeField] private float _DashSpeed;
    [SerializeField] private Cooldown _DashCD;
    [SerializeField] private float _DashTime;
    private bool _isDashing;
    private float _DashTimeCD;
    
    //Player physic
    private Rigidbody _rigidbody;
    private Vector3 moveInput;

    private Animator _animator;
    [SerializeField] private GameObject _Model;
    private GameObject _Hand;


    [Header("-------------------")]
    [SerializeField] private GameObject TargetSign;
    [SerializeField] private ParticleSystem TeleportIn;
    [SerializeField] private ParticleSystem TeleportOut;
    private bool TargetMode;
    private GameObject Target;

    // Start is called before the first frame update
    void Start()
    {   
        CurrentState = PlayerState.TeleportIn;
        StateMachine();
        _CurrentHp = _MaxHp;
        Observer.PostEvent(EvenID.DisplayPlayerHP, _CurrentHp, _MaxHp);
    
        _rigidbody = GetComponent<Rigidbody>();
        _animator = _Model.GetComponent<Animator>();

        //Đăng ký Event
        Observer.AddListener(EvenID.HealPlayer, OnHeal);

        _Hand = GetComponentInChildren<HolderController>().gameObject;
    }

   

    void Update()
    {
        if(CurrentState != PreviousState){
            StateMachine();
            PreviousState = CurrentState;
        }

        if(CurrentState != PlayerState.Alive) return;
        Dash();
        Rotation();
    
    }

    void FixedUpdate()
    {
        Movement();
    }

    private async void StateMachine(){
        switch(CurrentState){
            case PlayerState.Alive:
                ToggleVisual(true);
                break;

            case PlayerState.TeleportIn:
                //Tắt visual đề phòng đang bật
                ToggleVisual(false);
                
                //chạy particle
                TeleportIn.transform.position = transform.position;
                TeleportIn.Play();

                //Chuyển trạng thái
                await Task.Delay(400);
                CurrentState = PlayerState.Alive;
                break;

            case PlayerState.TeleportOut:
                ToggleVisual(false);
                TeleportOut.transform.position = transform.position;
                TeleportOut.Play();

                await Task.Delay(1100);
                CurrentState = PlayerState.TeleportIn;
                break;

            case PlayerState.Transforming:
                _animator.SetBool("IsTransforming", true);
                _animator.Play("Transform",-1, 0f);
                break;
            
            case PlayerState.Death:
                GameManager.Instance.GameOver();
                break;
        }
    }


    private void Movement(){
        if(CurrentState != PlayerState.Alive) return;

        _movementspeed = _isDashing? _DashSpeed : _NormalSpeed;

        moveInput =  new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if(moveInput.x!=0 && moveInput.z!=0) moveInput.Normalize();     //Chuẩn hóa vector 
        _rigidbody.velocity = _movementspeed * moveInput.ToIso();

        _animator.SetFloat("Speed", _rigidbody.velocity.sqrMagnitude);

        //_rigidbody.MovePosition(transform.position + moveInput.ToIso() * moveInput.ToIso().normalized.magnitude *_movementspeed * Time.deltaTime);

    }


    private void Rotation(){
        if(Camera.main == null) return;
         
        // Tạo raycast từ vị trí của con trỏ 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Kiểm tra xem va chạm của raycast
        if (!Physics.Raycast(ray, out hit)) return;
    
        
        Vector3 direction;

        if( Input.GetMouseButtonDown(1)){
            Target = hit.transform.gameObject;
           
            TargetMode = Target.CompareTag("Enemy"); 
            TargetSign.SetActive(TargetMode);
            
            Debug.Log(hit.transform.gameObject.name);
        } 

        if(TargetMode){
            TargetSign.transform.position = Target.transform.position;
            TargetSign.transform.rotation = Quaternion.Euler(0, TargetSign.transform.eulerAngles.y+ Time.deltaTime * 100, 0);
            direction = Target.transform.position - transform.position;

            if(!Target.activeInHierarchy){
                TargetSign.SetActive(false);
                TargetMode = false;
            }
        } else {
            direction = hit.point - transform.position;
            TargetSign.SetActive(false);
        }
       
        direction.y = 0; //bỏ qua độ cao

        //xoay player
        if(_isDashing){
            transform.forward = moveInput.ToIso();
            return;
        }
        transform.forward = direction;

    }

    private void Dash()
    {
         if(CurrentState != PlayerState.Alive) return;

        if ( _DashTimeCD <=0 && _isDashing){

            _animator.SetBool("IsDash", false);

            _isDashing = false;
            _Hand.SetActive(true);

        } else {
            _DashTimeCD -=Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space)&& !_DashCD.IsCoolingDown && _DashTimeCD <= 0)
        {   


            _animator.SetFloat("DashTime", 1/_DashTime);
            _animator.SetBool("IsDash", true);

            _DashCD.StartCooldown();
            _DashTimeCD = _DashTime;
            _isDashing = true;

            

            _Hand.SetActive(false);

        }
    }

    public void TakeDamage(int Dmg, Vector3 hitPoint = default){
        if(_isDashing) return;
        if(_amor > 0){
            _amor -= Dmg;
            if(_amor < 0){
                _CurrentHp += _amor;
                _amor = 0;
            }
        } else _CurrentHp -= Dmg;

        Observer.PostEvent(EvenID.DisplayTextPopup, Dmg, transform.position, Color.red);

        Observer.PostEvent(EvenID.DisplayPlayerHP, _CurrentHp, _MaxHp);

        if(_CurrentHp <=0){
            _CurrentHp = 0;
            CurrentState = PlayerState.Death;
        }
    }

    

    private void OnHeal(object[] obj)
    {
        _CurrentHp += (int)obj[0];


        if(_CurrentHp > _MaxHp) _CurrentHp = _MaxHp;
        Observer.PostEvent(EvenID.DisplayPlayerHP, _CurrentHp, _MaxHp);
        Observer.PostEvent(EvenID.DisplayTextPopup, "+" +(int)obj[0], transform.position, Color.green);
    }

    private void ToggleVisual(bool value){
        foreach (var item in GetComponentsInChildren<MeshRenderer>()){
            item.enabled = value;
        }
    }

    public void BuffsTrigger()
    {
        foreach (var buff in GetComponents<BaseEffect>())
        {
            buff.BuffTrigger();
        }
    }
    
}

public static class Helpers 
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
