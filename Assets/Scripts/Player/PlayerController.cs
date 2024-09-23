using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ObserverPattern;
using System;

public class PlayerController : MonoBehaviour
{
    //Player Status
    [Header("------Player Status------")]

    [SerializeField] private int _MaxHp;
    private int _CurrentHp;
    [SerializeField] private int _amor;
    [SerializeField] private float _Accurate;
    [SerializeField] private float _movementspeed = 1;

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
    private bool TargetMode;
    private GameObject Target;

    // Start is called before the first frame update
    void Start()
    {
        _CurrentHp = _MaxHp;
    
        _rigidbody = GetComponent<Rigidbody>();
        _animator = _Model.GetComponent<Animator>();

        //Đăng ký Event
        Observer.AddListener(EvenID.HealPlayer, OnHeal);

        _Hand = GetComponentInChildren<HolderController>().gameObject;
    }

   

    void Update()
    {
        
        Dash();
        Rotation();
    
    }

    void FixedUpdate()
    {
        Movement();
    }

    private void Movement(){
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
            TargetSign.SetActive(true);
            TargetMode = Target.CompareTag("Enemy"); 
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

        if ( _DashTimeCD <=0 && _isDashing){

            _movementspeed -= _DashSpeed;
            _animator.SetBool("IsDash", false);

            _isDashing = false;
            _Hand.SetActive(true);

        } else {
            _DashTimeCD -=Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space)&& !_DashCD.IsCoolingDown && _DashTimeCD <= 0)
        {   

            _movementspeed += _DashSpeed;

            _animator.SetFloat("DashTime", 1/_DashTime);
            _animator.SetBool("IsDash", true);

            _DashCD.StartCooldown();
            _DashTimeCD = _DashTime;
            _isDashing = true;

            

            _Hand.SetActive(false);

        }
    }

    public void TakeDamage(int Dmg){
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
            Observer.PostEvent(EvenID.DisplayGameOver);
        }
    }

    

    private void OnHeal(object[] obj)
    {
        _CurrentHp += (int)obj[0];


        if(_CurrentHp > _MaxHp) _CurrentHp = _MaxHp;
        Observer.PostEvent(EvenID.DisplayPlayerHP, _CurrentHp, _MaxHp);
        Observer.PostEvent(EvenID.DisplayTextPopup, "+" +(int)obj[0], transform.position, Color.green);
    }

    
}

public static class Helpers 
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
