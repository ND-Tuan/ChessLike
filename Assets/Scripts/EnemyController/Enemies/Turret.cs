using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Turrel : MonoBehaviour, IDamageable
{
    [SerializeField] private int MaxHp;
    [SerializeField] private Cooldown cooldown;
    private int CurHp;
    private bool active = false;
    private Slider HpBar;
    private Transform  PlayerTransform;
    private bool isDisplayHpBar = false;
    [SerializeField] private GunController attack;

    public bool Freezing { get; set; }
    public bool Burning { get; set; }

    void Start(){
        CurHp = MaxHp; 
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update(){
        if(active){
            transform.LookAt(PlayerTransform);

            if(!cooldown.IsCoolingDown){
                attack.Attack(1);
                cooldown.StartCooldown();
            }
        }

        if(transform.position.y < 0.2){
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            active = true;
        }
    }

    public void TakeDamage(int damage, Vector3 hitPoint = default)
    {   
        if(!active) return;

        CurHp -= damage;

        if(!isDisplayHpBar){
            HpBar = ObjectPoolManager.Instance.GetObject("EnemyHPBar").GetComponent<Slider>();
            HpBar.transform.position = transform.position + new Vector3(0, 1.5f, 0);
            HpBar.gameObject.SetActive(true);

            isDisplayHpBar = true;
        }

        HpBar.maxValue = MaxHp;
        HpBar.value = CurHp;

        if(CurHp <=0){
            CurHp=0;

            foreach(var Mesh in GetComponentsInChildren<MeshFilter>()){
                GameObject fragment = ObjectPoolManager.Instance.GetObject("EnemyCorpse");
                fragment.GetComponent<Collider>().enabled = true; 
                fragment.GetComponent<MeshFilter>().mesh = Mesh.mesh;

                fragment.transform.position = Mesh.transform.position;
                fragment.SetActive(true);
                fragment.GetComponent<Rigidbody>().AddForce(hitPoint * 0.7f, ForceMode.Impulse); 
            }

            active = false;
            HpBar.gameObject.SetActive(false);

            CurHp = MaxHp;
            isDisplayHpBar = false;
             
            gameObject.SetActive(false);
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
