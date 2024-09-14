using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class GunController : MonoBehaviour
{
    [Header("-----Gun Status------")]
    [SerializeField] private bool _1HandGun;
    [SerializeField] private Cooldown _CoolDown;
    [SerializeField] private float  _BulletForce;
    [SerializeField] private int  _Dmg;
    [SerializeField] private int  _CritDmg;
    [SerializeField] private Mesh _BulletMesh;

    [SerializeField] private Transform _FirePos;

    public bool _IsPlayer = true;
    private Transform _User;
    private Animator _handleAnimator;
    private Animator _gunAnimator;

    
    // Start is called before the first frame update
    void Start()
    {
        _handleAnimator = GetComponentInParent<Animator>();
        _gunAnimator = GetComponent<Animator>();

        _User = transform.parent;
        if(_IsPlayer){
            _User = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && !_CoolDown.IsCoolingDown && _IsPlayer){
            Fire();
        }
    }

    public void Fire(){
        GameObject BulletTmp = ObjectPoolManager.Instance.GetObject("Bullet");

        if(BulletTmp == null) return;

        BulletTmp.SetActive(true);
        BulletTmp.transform.SetPositionAndRotation(_FirePos.transform.position, _User.transform.rotation);

        BulletTmp.GetComponentInChildren<TrailRenderer>().Clear();    //đặt lại effect
        BulletTmp.GetComponentInChildren<MeshFilter>().mesh = _BulletMesh;   //Đổi loại đạn

        BulletTmp.GetComponent<BulletHit>().Dmg = _Dmg;
        BulletTmp.GetComponent<BulletHit>().PlayerBullet = _IsPlayer;

        Rigidbody rb = BulletTmp.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero; 
        rb.AddForce(_FirePos.transform.right * _BulletForce, ForceMode.Impulse);  //Gắn vector lực
            

        //chạy Animation giật
        _handleAnimator.Play("GunRecoil", -1, 0f);

        //Điều chỉnh tốc độ chạy Animation
        if(_CoolDown.getCD()<0.25)
            _handleAnimator.SetFloat("SpeedCoef", 1/_CoolDown.getCD());
        else
            _handleAnimator.SetFloat("SpeedCoef", 4);

        _CoolDown.StartCooldown();
    }

    public bool GetStyle(){
        return _1HandGun;
    }
}
