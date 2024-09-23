using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using ObserverPattern;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("---Enemy Status----------------------")]

    [SerializeField] private int _MaxHp;
    [SerializeField] private int _CurrentHp;
    public int EnermyLevel;
    private float[] _LevelUpScale = {1, 2, 3};
    [SerializeField] private int _MaxDropAmount;

    [Header("---Enemy AI--------------------------")]

    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private LayerMask Surface, PlayerLayer;

    //Mode Lang thang
    [SerializeField] private Vector3 walkPoint;
    [SerializeField] private float walkPointRange;
    [SerializeField] private Cooldown _waitTime;
    private bool walkPointSet;
    private Transform _PlayerTransform;
    private Vector3 directionToPlayer;

    //Tấn công
    [Header("---Enemy Attack----------------------")]
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField]private float sightRange, attackRange;
    private bool alreadyAttacked;
    private bool playerInSightRange, playerInAttackRange;

    private bool _DisplayHpBar = false;

    private GameObject _HpBar;


    void Awake(){
        //Dang ky su kien
        _PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
        
        
    }

    void OnEnable()
    {
        _LevelUpScale = GameManager.Instance.EnemyLevelUpScale;
        _CurrentHp = (int)(_MaxHp * _LevelUpScale[EnermyLevel]);
    }

    private void Update()
    {
       
        _agent.enabled = true;

        if(_DisplayHpBar){
            if(_HpBar == null) return;
            _HpBar.transform.position = transform.position + new Vector3(0, 1.5f, 0);
            
            _HpBar.GetComponent<Slider>().value = _CurrentHp;
        }

        //Kiểm tra Player có trong tầm nhìn và tầm tấn công
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, PlayerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, PlayerLayer);

        directionToPlayer = (_PlayerTransform.position - transform.position).normalized + new Vector3(0, 0.5f, 0);

        Ray ray = new(transform.position, directionToPlayer);
        if (Physics.Raycast(ray, out RaycastHit hit)){
            if (!hit.collider.CompareTag("Player")){

                playerInSightRange = false;
                playerInAttackRange = false;
                _agent.angularSpeed = 120;
            }
        }

        if (!playerInSightRange && !playerInAttackRange) Patroling(); //Mode Lang thang nếu ko phát hiện người chơi
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling(){
        
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            _agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint(){

        if(_waitTime.IsCoolingDown) return;
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, Surface)) //Kiểm tra điểm đến có ở ngoài map ko
            walkPointSet = true;
        _waitTime.StartCooldown();
    }

    private void ChasePlayer(){
        _agent.SetDestination(_PlayerTransform.position);
        _agent.angularSpeed = 120;
    }

    private void AttackPlayer(){
        
        Patroling();
        _agent.angularSpeed = 0;

        transform.LookAt(_PlayerTransform);

        if (alreadyAttacked) return;
        GetComponentInChildren<IEnemyAttack>().Attack(_LevelUpScale[EnermyLevel]);

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void ResetAttack(){

        alreadyAttacked = false;
    }


    public void TakeDamage(int Dmg, Vector3 hitPoint){
        
        if (_agent.enabled == false) return; //kt agent 
        
        // Nhận Damage 
        _CurrentHp -= Dmg; 

         //Hiển thị Popup Damage
        Observer.PostEvent(EvenID.DisplayTextPopup, Dmg, transform.position, Color.red);

        //Hiển thị HP bar
        if (!_DisplayHpBar){
            _HpBar = ObjectPoolManager.Instance.GetObject("EnemyHPBar");
            _HpBar.SetActive(true);
            _HpBar.GetComponent<Slider>().maxValue = _MaxHp * _LevelUpScale[EnermyLevel];
            _DisplayHpBar = true;

        }
        
        //Knockback
        if(hitPoint != null)
            GetComponent<Rigidbody>().AddForce(hitPoint * 3, ForceMode.Impulse);
        
        //Kiểm tra HP còn lại
        if (_CurrentHp > 0) return; 
        
        // Nếu HP về 0:
        _CurrentHp = 0;
        _HpBar.SetActive(false);
        isDead(hitPoint);
        
        
        
    }


    private void isDead(Vector3 hitPoint){
        
        
        //Sinh ra thi thể
        GameObject corpse = ObjectPoolManager.Instance.GetObject("EnemyCorpse");
        if (corpse == null) return;
        
        corpse.SetActive(true);
        corpse.GetComponent<Collider>().isTrigger = false; 
        corpse.transform.position = transform.position;    
        corpse.GetComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;

        //Knockback
        if(hitPoint != null)
            corpse.GetComponent<Rigidbody>().AddForce(hitPoint * 3, ForceMode.Impulse); 

        //Rơi item
        Drop(Random.Range(1, _MaxDropAmount), "Coin");
        Drop(Random.Range(1, _MaxDropAmount), "Ammo");

        //Xử lý buff
        if(GameManager.Instance._HasSoulEatingBuff){
            Drop(1, "Soul");
        }

        //Vô hiệu hóa enemy
        GetComponent<ActiveEnemy>().enabled = true; 
        enabled = false; 
        gameObject.SetActive(false); 
    }


    private void Drop(int amount, string tag){
        for (int i = 0; i < amount; i++){
            GameObject Tmp = ObjectPoolManager.Instance.GetObject(tag);
            if(Tmp == null) return;

            Tmp.SetActive(true);
            Tmp.transform.position = transform.position + new Vector3(0, 0.7f, 0);

            Rigidbody rb = Tmp.GetComponent<Rigidbody>();

            if(rb == null) return;
            Vector3 randomUpDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f)).normalized;
            float randomForce = Random.Range(5f, 10f);
            rb.AddForce(randomUpDirection * randomForce, ForceMode.Impulse);
            Tmp.SetActive(true);
        }
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
       
    }


    void OnDisable()
    {
        _agent.enabled = false;
        _DisplayHpBar = false;
    }
}
