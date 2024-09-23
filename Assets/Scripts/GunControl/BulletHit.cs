using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

public class BulletHit : MonoBehaviour
{
    public bool PlayerBullet = false;
    public int Dmg;
    public float force ;
    [SerializeField] private Transform pos;
    
    private EnemyController _enemyController;
    private PlayerController _playerController;

    //buff status
    private bool canBounce = false;
    private bool canPierce = false;
    private int maxBounces ;
    private int pierceChance; 
    private int currentBounces = 0;
    private RaycastHit hit;

    private bool isOnFire = false;
    [SerializeField]private int FireChance;
    private int BurnDmg;
    private float BurnCooldown;
    private float BurnDuration;

    


    void Awake()
   {
        Observer.AddListener(EvenID.BuffPierce, ApplyPierce);
        Observer.AddListener(EvenID.BuffBounce, ApplyBounce);
        Observer.AddListener(EvenID.BuffOnFire, ApplyFire);
   }

    void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("Player")) return;

        //Bắn vào kẻ thù
        if(PlayerBullet ){
            _enemyController = other.gameObject.GetComponent<EnemyController>();
            if(_enemyController == null) return;
            
            _enemyController.TakeDamage(Dmg, transform.forward);

            SetFire(_enemyController);
            SetPierce();
            
            
           
            
        }
        //Bắn vào người chơi
        else if(!PlayerBullet){
            _playerController = other.gameObject.GetComponent<PlayerController>();
            if(_playerController == null) return;

            _playerController.TakeDamage(Dmg);
            gameObject.SetActive(false);
        }
    }

    //Begin region=======Apply buff xuyên quái ====================================
    private void ApplyPierce(object[] data){
        pierceChance = (int)data[0];
        canPierce = true;
    }

    private void SetPierce(){
        if( Random.Range(0,100) < pierceChance && canPierce){
            return;
        }

        gameObject.SetActive(false);
    }

    //End region ==============================================

    //Begin region=======buff bật nẩy =========================

    private void ApplyBounce(object[] data){
        canBounce = true;
        maxBounces = (int)data[0];
    }

    void Update()
    {
        if(!PlayerBullet || !canBounce) return;
       
        if (Physics.Raycast(pos.position, transform.forward, out hit, 0.7f + force/30)){
            if(hit.collider.CompareTag("Enemy")) return;
            HandleBounce(hit);
        }
        
    }

    private void HandleBounce(RaycastHit hit){
        if (currentBounces >= maxBounces) return;
       
        currentBounces++;
    
        transform.forward = Vector3.Reflect(transform.forward, hit.normal);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().velocity = transform.forward * force;
        
    }

    //End region ==============================================

    //Begin region=======buff cháy =========================
    private void ApplyFire(object[] data){
        isOnFire = true;
        FireChance = (int)data[0];
        ApplyBurn();
    }

    private void SetFire(EnemyController enemyController){
        if(!isOnFire || Random.Range(0,100) > FireChance) return;
        if(enemyController.gameObject.GetComponentInChildren<FireController>() != null) return; 

        GameObject fire = ObjectPoolManager.Instance.GetObject("Fire");

        if(fire != null){
            fire.transform.parent = enemyController.gameObject.transform;
            fire.transform.localPosition = Vector3.zero;
            fire.SetActive(true);
            fire.GetComponent<FireController>().SetFire(BurnDmg, _enemyController, BurnCooldown, BurnDuration);
        }

    }

    private void ApplyBurn(){
       object[] data = GameManager.Instance.GetBurnInfo();

        BurnDmg = (int)data[0];
        BurnDuration = (float)data[1];
        BurnCooldown = (float)data[2];
    }




    void OnEnable()
    {
        currentBounces = 0;
    }

    void OnDestroy()
    {
        Observer.RemoveListener(EvenID.BuffPierce, ApplyPierce);
        Observer.RemoveListener(EvenID.BuffBounce, ApplyBounce);
    }
}
