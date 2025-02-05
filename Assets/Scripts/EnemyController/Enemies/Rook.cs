using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : EnemyController
{
    [SerializeField] private LineRenderer _cautionLaser;
    [SerializeField] private LineRenderer _Laser;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask dmgLayer;
    [SerializeField] private Animator _handleAnimator; 
    [SerializeField] private ParticleSystem _FireEffect;

    private Vector3 _hitPos;
    private bool IsAttacking = false;

    void LateUpdate()
    {
        Aim();
    }

    public override void AttackPlayer()
    {
        if(!_attackCooldown.IsCoolingDown){
            StopAllCoroutines();
            _agent.SetDestination(transform.position);
            
            StartCoroutine(AttackProcess());     
            _attackCooldown.StartCooldown();      
        }

        if(!IsAttacking){
            Patroling();
            Vector3 direction = (_PlayerTransform.position - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10);
        }
    }

    private void Aim(){
    
        if(Physics.Raycast(_attackPoint.position, _attackPoint.forward, out var _hit ,100, layerMask)){
            _hitPos = _hit.point;
        } else {
            _hitPos =_attackPoint.position + transform.forward*100;
        } 
        //bắn laser cảnh báo
        _cautionLaser.SetPositions(new Vector3[] { _attackPoint.position, _hitPos });
        _Laser.SetPositions(new Vector3[] { _attackPoint.position, _hitPos });
    }

    private IEnumerator AttackProcess(){
        _cautionLaser.gameObject.SetActive(true);
        _FireEffect.Play();

        //Nháy cảnh báo sắp bắn
        yield return new WaitForSeconds(0.5f);
       
        float elapsed = 0f;
        IsAttacking = true;

        while (elapsed < 0.4)
        {
            _cautionLaser.gameObject.SetActive(!_cautionLaser.gameObject.activeSelf);
            yield return new WaitForSeconds(0.04f);
            elapsed += 0.05f;
        }

        //bắn
        _cautionLaser.gameObject.SetActive(false);
        
        _handleAnimator?.Play("LaserGunRecoil", -1, 0f);
     
        _Laser.gameObject.SetActive(true);

        // SphereCast để gây sát thương
        if (Physics.SphereCast(_attackPoint.position,  0.2f, _attackPoint.forward, out var _hit, 100, dmgLayer))
        {
            Debug.Log(_hit.collider.name);
            if (_hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage((int)(_damage * _LevelUpScale[EnermyLevel]), _hitPos);
            }
        }

        yield return new WaitForSeconds(0.34f);
        _Laser.gameObject.SetActive(false);
        IsAttacking = false;
        
    }

    protected override void OnActive() {}
    protected override void RemoveAttack(){}
}
