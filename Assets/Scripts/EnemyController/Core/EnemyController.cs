using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using ObserverPattern;
using UnityEngine.UI;

public abstract class EnemyController : MonoBehaviour, IDamageable
{
    [Header("---Enemy Status----------------------")]

    [SerializeField] private int _MaxHp;
    private int _CurrentHp;
    public int EnermyLevel;
    protected float[] _LevelUpScale = {1, 2, 3};
    [SerializeField] private int _MaxDropAmount;

    [SerializeField] protected AudioClip _AttackSound;

    [Header("---Enemy AI--------------------------")]

    protected NavMeshAgent _agent;
    [SerializeField] private LayerMask Surface, PlayerLayer, RayScan;

    //Mode Lang thang
    public Vector3 walkPoint;
    [SerializeField] private float walkPointRange;
    [SerializeField] private Cooldown _waitTime;
    [SerializeField] private float sightRange, attackRange;
    [SerializeField] protected Cooldown _attackCooldown;
    [SerializeField] protected int _damage;
    public bool walkPointSet;
    protected Transform _PlayerTransform;
    private Vector3 directionToPlayer;
    private bool playerInSightRange, playerInAttackRange;

    protected Animator _animator;
    private bool _DisplayHpBar = false;
    private GameObject _HpBar;
    private GameObject _Ice;

    public bool Freezing { get ;set; }
    public bool Burning { get ;set; }

    protected abstract void OnActive();
    public abstract void AttackPlayer();
    protected abstract void RemoveAttack();

    void Awake(){
        _PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        Observer.AddListener(EvenID.ApplyToTarget, OnApplyBuff);
    }


    void OnEnable()
    {
        _LevelUpScale = GameManager.Instance.EnemyLevelUpScale;
        _CurrentHp = (int)(_MaxHp * _LevelUpScale[EnermyLevel]);
        Freezing = false;
        walkPoint = transform.position;

        OnActive();

        Debug.Log("Enemy: " + this.name);
    }


    private void Update()
    {
       
        _agent.enabled = true;

        if(_DisplayHpBar){
            if(_HpBar == null) return;
            _HpBar.transform.position = transform.position + new Vector3(0, 1.5f, 0);
            
            _HpBar.GetComponent<Slider>().value = _CurrentHp;
        }

        if(Freezing){
            _agent.SetDestination(transform.position);
            _animator.SetFloat("Mul", 0);

            return;
        }

        _animator.SetFloat("Mul", 1);

        //Kiểm tra Player có trong tầm nhìn và tầm tấn công
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, PlayerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, PlayerLayer);

        directionToPlayer = (_PlayerTransform.position - transform.position).normalized + new Vector3(0, 0.5f, 0);

        Ray ray = new(transform.position, directionToPlayer);
        if (Physics.Raycast(ray, out RaycastHit hit, 20, RayScan)){
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


    protected void Patroling(){
        
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet){
            _agent.SetDestination(walkPoint);
            _animator.SetBool("Move", true);
        }
            

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }


    private void SearchWalkPoint(){

        if(_waitTime.IsCoolingDown){
            _animator.SetBool("Move", false);
            return;
        }
        
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


    // private void AttackPlayer(){
    //     _animator.SetBool("Move", false);

    //     if(DontMoveWhenAttack){
    //         _agent.SetDestination(transform.position);
    //     } else {
    //         _agent.angularSpeed = 0;
    //         Patroling();
    //     }

    //     Vector3 direction = (_PlayerTransform.position - transform.position).normalized;
    //     transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10);

    //     if (alreadyAttacked) return;
    //     alreadyAttacked = true;
    //     GetComponentInChildren<IEnemyAttack>().Attack(_LevelUpScale[EnermyLevel]);
        
    //     Invoke(nameof(ResetAttack), timeBetweenAttacks);
    // }

    

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

    public void BuffsTrigger(){
        foreach (var buff in GetComponents<BaseEffect>())
        {
            buff.BuffTrigger();
        }
    }


    private void isDead(Vector3 hitPoint){
        //Sinh ra thi thể
        GameObject corpse = ObjectPoolManager.Instance.GetObject("EnemyCorpse");
        if (corpse == null) return;
        
        corpse.SetActive(true);
        corpse.GetComponent<Collider>().enabled = true; 
        corpse.transform.position = transform.position;    
        corpse.GetComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;

        //Knockback
        if(hitPoint != null)
            corpse.GetComponent<Rigidbody>().AddForce(hitPoint * 3, ForceMode.Impulse); 

        //Rơi item
        Drop(Random.Range(1, _MaxDropAmount), "Coin");
        Drop(Random.Range(1, _MaxDropAmount), "Ammo");

        //Vô hiệu hóa enemy
        GetComponent<ActiveEnemy>().enabled = true; 
        _Ice?.SetActive(false);
        StopAllCoroutines();
        RemoveAttack();
        enabled = false; 

        gameObject.SetActive(false); 
    }
    
    private void Drop(int amount, string tag){
        for (int i = 0; i < amount; i++)
        {
            // Lấy Object từ Pool
            GameObject Tmp = ObjectPoolManager.Instance.GetObject(tag);
            if (Tmp == null) return; 
            // Kích hoạt đối tượng
            Tmp.SetActive(true);
            Tmp.transform.position = transform.position + new Vector3(0, 0.7f, 0);
    
            Rigidbody rb = Tmp.GetComponent<Rigidbody>();
            if (rb == null) return;
    
            // Tạo một lực ngẫu nhiên
            Vector3 randomUpDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f)).normalized;
            float randomForce = Random.Range(5f, 10f);
            rb.AddForce(randomUpDirection * randomForce, ForceMode.Impulse);
    
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
       
    }

    private void OnApplyBuff(object[] data){
        if(data[0] == null) return;
        System.Type buff = (System.Type)data[0];
        if (gameObject.GetComponent(buff)) return;

        BaseEffect effect = gameObject.AddComponent(buff) as BaseEffect;
        effect._BuffValue = (int)data[1];
    }

    void OnDisable()
    {
        _agent.enabled = false;
        _DisplayHpBar = false;
       
    }
}
