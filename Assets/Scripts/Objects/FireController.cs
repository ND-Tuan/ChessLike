using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{

    public void SetFire(int dmg, EnemyController enemyController, float DmgCooldown, float Duration){
        StartCoroutine(_DmgCoroutine(dmg, enemyController, DmgCooldown));
        Invoke(nameof(DeActive), Duration);
    }
    
    private IEnumerator _DmgCoroutine( int dmg, EnemyController enemyController, float DmgCooldown){
        while(true){
            yield return new WaitForSeconds(DmgCooldown);
            enemyController.TakeDamage(dmg, Vector3.zero);
        }
    }

    private void DeActive()
    {
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
