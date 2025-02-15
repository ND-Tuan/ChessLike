using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObserverPattern;
using UnityEngine;

public class MagicBulletCore : MonoBehaviour
{
    [SerializeField] private Transform[] core;
    [SerializeField] private int _diractionNum;
    [SerializeField] private float _speed;
    [SerializeField] private AudioClip _AttackSound;


    public void Attack(int Damage){
        foreach(var c in core){
            c.position += new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));        
            Fire(c, Damage);      
        }
    }


    private async void Fire(Transform firePos, int Damage){
        await Task.Delay(500);

        for (int i = 0; i < _diractionNum; i++)
        {
            float angle = i * (360f / _diractionNum);
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));

            // Lấy đạn từ Pool
            GameObject bullet = ObjectPoolManager.Instance.GetObject("BishopBullet");

            if (bullet != null)
            {
                bullet.GetComponentInChildren<TrailRenderer>().Clear();

                // Đặt vào hướng và vị trí bắn
                bullet.transform.position = firePos.position;

                bullet.SetActive(true);

                bullet.GetComponent<Collider>().enabled = true;

                // Đặt sát thương
                bullet.GetComponent<BulletHit>().Dmg = Damage;
                bullet.GetComponent<BulletHit>().PlayerBullet = false;

                // Áp lực
                Rigidbody rg = bullet.GetComponent<Rigidbody>();
                rg.velocity = Vector3.zero; 
                rg.AddForce(direction * _speed, ForceMode.Impulse);

                //Chạy âm thanh
                Observer.PostEvent(EvenID.PlayFxSound, new object[] { _AttackSound, firePos });
            }

            await Task.Delay(100);
        }
    }

}
