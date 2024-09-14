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
    [Range(1,3)] public int EnermyLevel;

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
        
    }

    void Start()
    {
        _CurrentHp = _MaxHp;
    }

    private void Update()
    {
       
        _agent.enabled = true;
        //Kiểm tra Player có trong tầm nhìn và tầm tấn công
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, PlayerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, PlayerLayer);

        directionToPlayer = (_PlayerTransform.position - transform.position).normalized;

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
    
        GetComponentInChildren<GunController>().Fire();
            

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void ResetAttack(){

        alreadyAttacked = false;
    }


    public void TakeDamage(int Dmg){
        
        if(_agent.enabled ==false) return;

        _CurrentHp -=Dmg;

        Observer.PostEvent(EvenID.DisplayDamagePopup, Dmg, transform.position);

        if(_CurrentHp >0) return;
        _CurrentHp = _MaxHp;
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
