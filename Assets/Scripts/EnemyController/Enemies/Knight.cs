using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using Unity.VisualScripting;
using UnityEngine;

public class Knight : EnemyController
{   
    [Header("---Knight Attack----------------------")]
    [SerializeField] private Animator _attackAnimator;
    private bool IsAttacking = false;
    [SerializeField] private GameObject _SlashEffect;


    public override void AttackPlayer()
    {
        if(!_attackCooldown.IsCoolingDown){
            StopAllCoroutines();
            _agent.SetDestination(transform.position);
            Attack();
            IsAttacking = true;
            _attackCooldown.StartCooldown();

        }

        if(!IsAttacking){
            Vector3 direction = (_PlayerTransform.position - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10);
        }
    }

    private void Attack(){
        _agent.SetDestination(transform.position);
        IsAttacking = true;
        _attackAnimator.SetBool("Attack", true);
        StartCoroutine(AttackProcess());
    }

    private IEnumerator AttackProcess(){
        yield return new WaitForSeconds(0.77f);
        
        //Chay am thanh
        Observer.PostEvent(EvenID.PlayFxSound, new object[] { _AttackSound, transform });

        _SlashEffect.SetActive(true);
        GetComponentInChildren<DmgWithTrigger>().SetDamage((int)(_damage * _LevelUpScale[EnermyLevel]));

        yield return new WaitForSeconds(0.09f);
        _SlashEffect.SetActive(false);

        yield return new WaitForSeconds(0.1f);
       
        GetComponentInChildren<DmgWithTrigger>().SetDamage(0);

        yield return new WaitForSeconds(1.2f);
        _attackAnimator.SetBool("Attack", false);
        _agent.SetDestination(_PlayerTransform.position);
        IsAttacking = false;
    }

    protected override void OnActive() {}
    protected override void RemoveAttack(){}
}
