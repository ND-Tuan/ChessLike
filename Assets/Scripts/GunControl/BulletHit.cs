using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHit : MonoBehaviour
{
    public bool PlayerBullet = false;
    public int Dmg;
    private EnemyController _enemyController;
    private PlayerController _playerController;

    void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("Player")) return;

        //Bắn vào kẻ thù
        if(PlayerBullet ){
            _enemyController = other.gameObject.GetComponent<EnemyController>();
            if(_enemyController == null) return;

            _enemyController.TakeDamage(Dmg, transform.forward);
        }
        //Bắn vào người chơi
        else if(!PlayerBullet){
            _playerController = other.gameObject.GetComponent<PlayerController>();
            if(_playerController == null) return;

            _playerController.TakeDamage(Dmg);
        }

        gameObject.SetActive(false);
    }
}
