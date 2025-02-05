using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgWithTrigger : MonoBehaviour
{
    private int _damage = 0;

    public void SetDamage(int damage){
        _damage = damage;

    }

    private void OnTriggerEnter(Collider other)
    {   
        if(_damage <= 0) return;

        if(other.gameObject.CompareTag("Bullet")){
            if(other.gameObject.GetComponent<BulletHit>().PlayerBullet){
                other.gameObject.SetActive(false);
            }
        }

        if(other.gameObject.CompareTag("Player")){
            if(other.TryGetComponent(out IDamageable damageable)){
                damageable.TakeDamage(_damage);
            }
        }
    }
}
