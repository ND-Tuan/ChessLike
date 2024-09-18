
using ObserverPattern;
using UnityEngine;



public class GunController : MonoBehaviour, IEnemyAttack
{
    [Header("-----Gun Info------")]
    [SerializeField] private GameObject Model;
    [SerializeField] private Sprite Icon;
    [SerializeField] private string Name;
    [SerializeField] private int Cost;
    [SerializeField] private int AmmoCapacity;
    private int _RemainAmmo;
    [SerializeField] private Cooldown _FireRate;

    private float _ReloadTime = 1f;
    
    [SerializeField] private float  _BulletForce;
    [SerializeField] private int  _Dmg;
    [SerializeField] private int  _CritDmg;
    [SerializeField] private Mesh _BulletMesh;
    [SerializeField] private bool _1HandGun;
    [SerializeField] private Transform _FirePos;
    private float Multiplier = 1;

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
            _RemainAmmo = AmmoCapacity;

            Observer.PostEvent(EvenID.DisplayPlayerAmmo, _RemainAmmo);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_IsPlayer){
            if(!_FireRate.IsCoolingDown && Input.GetMouseButton(0) )
                Fire();
        
            if(_RemainAmmo == 0 || Input.GetKeyDown(KeyCode.R))
                Reload();
        }
        
    }

    void OnEnable()
    {
        if(_IsPlayer)
            Observer.PostEvent(EvenID.DisplayPlayerAmmo, _RemainAmmo);
    }

    private void Fire(){

        if(_IsPlayer && _RemainAmmo == 0) return;

        GameObject BulletTmp = ObjectPoolManager.Instance.GetObject("Bullet");

        if(BulletTmp == null) return;

        BulletTmp.SetActive(true);
        BulletTmp.transform.SetPositionAndRotation(_FirePos.transform.position, _User.transform.rotation);

        BulletTmp.GetComponentInChildren<TrailRenderer>().Clear();    //đặt lại effect
        BulletTmp.GetComponentInChildren<MeshFilter>().mesh = _BulletMesh;   //Đổi loại đạn

        BulletTmp.GetComponent<BulletHit>().Dmg = (int)(_Dmg * Multiplier);
        BulletTmp.GetComponent<BulletHit>().PlayerBullet = _IsPlayer;

        Rigidbody rb = BulletTmp.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero; 
        rb.AddForce(_FirePos.transform.right * _BulletForce, ForceMode.Impulse);  //Gắn vector lực
            

        //chạy Animation giật
        _handleAnimator.Play("GunRecoil", -1, 0f);

        //Điều chỉnh tốc độ chạy Animation
        if(_FireRate.getCD()<0.25)
            _handleAnimator.SetFloat("SpeedCoef", 1/_FireRate.getCD());
        else
            _handleAnimator.SetFloat("SpeedCoef", 4);


        if(_IsPlayer){
            _RemainAmmo--;
            Observer.PostEvent(EvenID.DisplayPlayerAmmo, _RemainAmmo);
        }
            
        _FireRate.StartCooldown();
    }

    private void Reload(){
        if(CoinAndAmmoManager.GetAmmo() == 0) return;

        Invoke(nameof(ReloadFinish), _ReloadTime);
        
        _RemainAmmo = CoinAndAmmoManager.ReloadAmmo(AmmoCapacity);
        Observer.PostEvent(EvenID.DisplayReloadProgress, _ReloadTime, _RemainAmmo);

        transform.parent.gameObject.SetActive(false);
    }

    private void ReloadFinish(){
        transform.parent.gameObject.SetActive(true);
    }

    

    public void Attack(float multiplier)
    {
        if(_IsPlayer) return;
           
        Multiplier = multiplier;
        Fire();
    }

    public int GetRemainAmmo(){
        return _RemainAmmo;
    }

    public bool GetStyle(){
        return _1HandGun;
    }

    public Sprite GetIcon(){
        return Icon;
    }

     public GameObject GetModel(){
        return Model;
    }

    public GunInfo GetInfo(){
        GunInfo info = new GunInfo
        {
            Icon = Icon,
            Name = Name,
            Damage = _Dmg,
            CritDamage = _CritDmg,
            Cooldown = _FireRate.getCD(),
            AmmoCapacity = AmmoCapacity,
            Cost = Cost
        };

        return info;
    } 

   
}
