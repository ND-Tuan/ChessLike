using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveEnemy : MonoBehaviour
{
    [SerializeField] private bool NeedTriggerToActive = false;
    [SerializeField] private EnemyController _EnemyController;
    [SerializeField] private LayerMask PlayerLayer;

    private void Start()
    {
        _EnemyController = GetComponent<EnemyController>();
    }

    void FixedUpdate()
    {   if(_EnemyController.enabled) return;

        if(!NeedTriggerToActive){
            if(transform.position.y < 0.2){
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                _EnemyController.enabled = true;
            }
        } else {
            if(Physics.CheckSphere(transform.position, 7, PlayerLayer)){
                _EnemyController.enabled = true;
                if (TryGetComponent<BossIntroDisplay>(out var bossIntroDisplay))
                {
                    bossIntroDisplay.enabled = true;
                }
            }
        }


    }

}
