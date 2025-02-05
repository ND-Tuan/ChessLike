using System.Collections;
using System.Collections.Generic;
using Ionic.Zip;
using ObserverPattern;
using UnityEngine;


public class MoveToPlayer : MonoBehaviour
{
    [SerializeField] private float _Speed;
    [SerializeField] private float _delay;
    [SerializeField] private enum ObjectType {Coin, Ammo, Hp};
    [SerializeField] private ObjectType _objectType;
    public int Amount = 0;

    [SerializeField] private bool Randomize;
    [SerializeField] private int RandomRange;

    private bool _StartMove = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update(){

        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y+ Time.deltaTime * 200, 0);

        if(_StartMove){
            
            var step =  _Speed * Time.deltaTime;
            Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0, 0.5f, 0);
            transform.position = Vector3.MoveTowards(transform.position, playerPos, step);
        };
        
    }

    void OnEnable()
    {
        _StartMove = false;

        Invoke("StartMove", _delay);
    }

    private void StartMove()
    {

        _StartMove = true;

        if(rb == null) return;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; 
    }

    void OnTriggerEnter(Collider other){
        if(!_StartMove) return;
        if(other.gameObject.CompareTag("Player")){

            int amount = Randomize? Random.Range(Amount-RandomRange, Amount+RandomRange) : Amount;

            if(_objectType == ObjectType.Coin){
                CoinAndAmmoManager.AddCoins(amount);
                Observer.PostEvent(EvenID.DisplayCoin);

            }

            if(_objectType == ObjectType.Ammo){
                CoinAndAmmoManager.AddAmmo(amount);
                Observer.PostEvent(EvenID.DisplayPlayerAmmo, null);
            }

            if(_objectType == ObjectType.Hp){
                Observer.PostEvent(EvenID.HealPlayer, amount);
            }

            GetComponent<TrailRenderer>().Clear();

            _StartMove = false;
            gameObject.SetActive(false);
        }
    }

   
}
