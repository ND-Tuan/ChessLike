using System;
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
    
    // private EnemyController _enemyController;
    private PlayerController _playerController;


    void Awake()
   {
        Observer.AddListener(EvenID.ApplyToBullet, OnApplyBuff);
   }

    void OnTriggerEnter(Collider other)
    {
            
        if(!other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("Player")) return;
        
        //Bắn vào kẻ thù
        if(PlayerBullet && other.gameObject.CompareTag("Enemy")){
            IDamageable enemy = other.gameObject.GetComponent<IDamageable>();
            
            enemy.TakeDamage(Dmg, transform.forward);
            enemy.BuffsTrigger();

            gameObject.SetActive(false);
            
        }
        //Bắn vào người chơi
        else if(!PlayerBullet){
            _playerController = other.gameObject.GetComponent<PlayerController>();
            if(_playerController == null) return;

            _playerController.TakeDamage(Dmg, Vector3.zero);
            gameObject.SetActive(false);
        }
    }

    //
    private void OnApplyBuff(object[] data){
        if(data[0] == null) return;
        Type buff = (Type)data[0];
        if (gameObject.GetComponent(buff)) return;

        BaseEffect effect = gameObject.AddComponent(buff) as BaseEffect;
        effect._BuffValue = (int)data[1];
    }

    
    void OnDestroy()
    {
        
    }
}
