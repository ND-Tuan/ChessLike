using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using ObserverPattern;

public class EnemyController : MonoBehaviour
{
    [Header("---Enemy Status----------------------")]

    [SerializeField] private int _MaxHp;
    [SerializeField] private int _CurrentHp;
     public int EnermyLevel;
    private float[] _LevelUpScale = {1, 2, 3};

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
    
    void Awake(){

        _PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
        _LevelUpScale = GameManager.Instance.EnemyLevelUpScale;
        
    }

    void OnEnable()
    {

        _CurrentHp = (int)(_MaxHp * _LevelUpScale[EnermyLevel]);
    }

    private void Update()
    {
       
        _agent.enabled = true;
        //Kiểm tra Player có trong tầm nhìn và tầm tấn công
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, PlayerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, PlayerLayer);

        directionToPlayer = (_PlayerTransform.position - transform.position).normalized + new Vector3(0, 0.5f, 0);

        Ray ray = new(transform.position, directionToPlayer);
        if (Physics.Raycast(ray, out RaycastHit hit)){
            Debug.Log(hit.collider.name);
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
        Observer.PostEvent(EvenID.DisplayDamagePopup, Dmg, transform.position);
        
        //Knockback
        GetComponent<Rigidbody>().AddForce(hitPoint * 3, ForceMode.Impulse);
        
        //Kiểm tra HP còn lại
        if (_CurrentHp > 0) return; 
        
        // Nếu HP về 0:
        
        //Sinh ra thi thể
        GameObject corpse = ObjectPoolManager.Instance.GetObject("EnemyCorpse");
        if (corpse == null) return;
        
        corpse.SetActive(true);
        corpse.GetComponent<Collider>().isTrigger = false; 
        corpse.transform.position = transform.position;    
        corpse.GetComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;

        //Knockback
        corpse.GetComponent<Rigidbody>().AddForce(hitPoint * 3, ForceMode.Impulse); 

        //Vô hiệu hóa enemy
        GetComponent<ActiveEnemy>().enabled = true; 
        enabled = false; 
        gameObject.SetActive(false); 
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
    }
}
