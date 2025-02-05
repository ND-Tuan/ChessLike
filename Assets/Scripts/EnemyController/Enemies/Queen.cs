using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;

public class Queen : EnemyController
{   
    [SerializeField] Transform _Symbol;
    [SerializeField] GameObject _PawmAttack;
    [SerializeField] GameObject _RookAttack;
    [SerializeField] GameObject _BishopAttack;
    [SerializeField] private Animator[] _animators = new Animator[3];
    private bool IsAttacking = false;
    private int _currentAttack = 0;

    // Start is called before the first frame update
    protected override void OnActive()
    {
        _RookAttack.transform.parent = null;
        _RookAttack.transform.localScale = Vector3.one;
        _PawmAttack.transform.parent = null;
        _PawmAttack.transform.localScale = Vector3.one;
        _BishopAttack.transform.parent = null;
        _BishopAttack.transform.localScale = Vector3.one;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        AttackPlayer();
        if(!IsAttacking)
            _Symbol.Rotate(0, 150 * Time.deltaTime, 0);
    }

    public override void AttackPlayer()
    {
        if(!_attackCooldown.IsCoolingDown){
            IsAttacking = true;
        
            _currentAttack = Random.Range(0, 3);

            //chừng nào _PawmAttack còn tồn tại sẽ không triển khai tấn công kiểu này nữa
            while(_currentAttack == 0 && _PawmAttack.activeSelf)
            {
                _currentAttack = Random.Range(0, 3);
            }

            StartCoroutine(PerformAttack());
            _attackCooldown.StartCooldown();
        }

        Patroling();
    }

    private IEnumerator PerformAttack()
    {
        float elapsedTime = 0;
        while (elapsedTime < 0.5)
        {
            elapsedTime += Time.deltaTime;
            float newYRotation = Mathf.Lerp(_Symbol.transform.rotation.eulerAngles.y, 40 + 120*_currentAttack, elapsedTime / 0.5f);
            _Symbol.transform.rotation = Quaternion.Euler(0, newYRotation, 0);
            yield return null;
        }

        _animators[_currentAttack].Play("SymbolSelected");

        yield return new WaitForSeconds(0.5f);
        
        // Step 2: Perform the attack
        switch (_currentAttack)
        {
            case 0:
                PawnAttack();
                break;
            case 1:
                RookAttack();
                break;
            case 2:
                BishopAttack();
                break;
        }
        yield return new WaitForSeconds(0.8f);
    
        _animators[_currentAttack].Play("SymbolSelectedR");

        yield return new WaitForSeconds(0.2f);
        _RookAttack.SetActive(false);
        
        IsAttacking = false;

        yield return new WaitForSeconds(1f);
        _BishopAttack.SetActive(false);
    }

    private void PawnAttack()
    {
        _PawmAttack.transform.position = FindObjectOfType<BoardController>().GetComponent<BoardController>().TakeRandomPosition();
        _PawmAttack.SetActive(true);
    }

    private void RookAttack()
    {
        _RookAttack.SetActive(true);
        _RookAttack.transform.position = GameManager.Instance.CurrentBoardPosition;

        int Direction = Random.Range(0, 1);
        _RookAttack.transform.rotation = Quaternion.Euler(0, 90 + 90*Direction, 0);

        DmgWithTrigger Laser = _RookAttack.GetComponentInChildren<DmgWithTrigger>();
        Laser.SetDamage((int)(_damage * _LevelUpScale[EnermyLevel]));
    }

    private void BishopAttack()
    {
        _BishopAttack.SetActive(true);
        _BishopAttack.GetComponent<MagicBulletCore>().Attack((int)(_damage * _LevelUpScale[EnermyLevel]));  
    }


    protected override void RemoveAttack()
    {
        _RookAttack.SetActive(false);
        _BishopAttack.SetActive(false);
        _PawmAttack.GetComponent<IDamageable>().TakeDamage(999, Vector3.zero);
        Observer.PostEvent(EvenID.CombatDone, transform.position);
    }

}
