using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;

public class Bishop : EnemyController
{
    [Header("---Bishop Attack----------------------")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _force;
    [SerializeField] private int _diractionNum;
    [SerializeField] private float spreadAngle;
    [SerializeField] private Animator _handleAnimator;

    public override void AttackPlayer()
    {
        transform.LookAt(_PlayerTransform);
        Patroling();

        if(!_attackCooldown.IsCoolingDown){
            Attack(_LevelUpScale[EnermyLevel]);
            _attackCooldown.StartCooldown();
        }
           
    }

    public void Attack(float multiplier){
        
        if(_handleAnimator != null)
            _handleAnimator.Play("Cast", -1, 0);

        Observer.PostEvent(EvenID.PlayFxSound, new object[] { _AttackSound, _attackPoint });

       // Tính góc bắn
        float initialAngle = -spreadAngle / 2; 
        float angleStep = _diractionNum > 1 ? spreadAngle / (_diractionNum - 1) : 0; 

        for (int i = 0; i < _diractionNum; i++)
        {
            float currentAngle = initialAngle + i * angleStep;

            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 dir = rotation * _attackPoint.forward;

            dir.y = 0; //Đảm bảo đạn bay thẳng, song song với mặt đất
            dir.Normalize();

            // Lấy đạn từ Pool
            GameObject bullet = ObjectPoolManager.Instance.GetObject("BishopBullet");

            if (bullet != null)
            {
                bullet.GetComponentInChildren<TrailRenderer>().Clear();

                // Đặt vào hướng và vị trí bắn
                bullet.transform.position = _attackPoint.position;

                bullet.SetActive(true);

                bullet.GetComponent<Collider>().enabled = true;

                // Đặt sát thương
                bullet.GetComponent<BulletHit>().Dmg = (int)(_damage * multiplier);
                bullet.GetComponent<BulletHit>().PlayerBullet = false;

                // Áp lực
                Rigidbody rg = bullet.GetComponent<Rigidbody>();
                rg.velocity = Vector3.zero; 
                rg.AddForce(dir * _force, ForceMode.Impulse);
            }
        }
    }

    protected override void RemoveAttack(){}
    protected override void OnActive() {}
}

