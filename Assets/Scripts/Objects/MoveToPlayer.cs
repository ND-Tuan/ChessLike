using System.Collections;
using System.Collections.Generic;
using Ionic.Zip;
using ObserverPattern;
using UnityEngine;


public class MoveToPlayer : MonoBehaviour
{
    [SerializeField] private float _Speed;
    [SerializeField] private float _delay;
    [SerializeField] private enum ObjectType {Coin, Ammo, Soul};
    [SerializeField] private ObjectType _objectType;
    private int healthAmount = 0;
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
        healthAmount = GameManager.Instance._HealAmount;
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

            if(_objectType == ObjectType.Coin){
                int coin = Random.Range(4, 8);
                CoinAndAmmoManager.AddCoins(coin);
                Observer.PostEvent(EvenID.DisplayCoin);

            }

            if(_objectType == ObjectType.Ammo){
                CoinAndAmmoManager.AddAmmo(Random.Range(2, 6));
                Observer.PostEvent(EvenID.DisplayPlayerAmmo, null);
            }

            if(_objectType == ObjectType.Soul){
                Observer.PostEvent(EvenID.HealPlayer, healthAmount);
            }

            GetComponent<TrailRenderer>().Clear();

            _StartMove = false;
            gameObject.SetActive(false);
        }
    }

   
}
