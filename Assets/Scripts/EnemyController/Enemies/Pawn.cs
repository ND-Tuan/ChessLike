using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : EnemyController
{
    [SerializeField] private GunController _gunController;
    public override void AttackPlayer()
    {
        //_agent.angularSpeed = 0;
        Patroling();
        transform.LookAt(_PlayerTransform);

        if(!_attackCooldown.IsCoolingDown){
            _gunController.Attack(_LevelUpScale[EnermyLevel]);
            _attackCooldown.StartCooldown();
        }
    }

    protected override void OnActive() {}
    protected override void RemoveAttack(){}
}
