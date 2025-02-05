using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

public class Freeze : BaseEffect
{
    public float DurationMultiplier = 1;

    private float FreezeDuration => GameManager.Instance._StatusEffectData.FreezeDuration * DurationMultiplier;

    private GameObject _Ice;

    public override void BuffTrigger(){
        if(GetComponent<IDamageable>().Freezing) return;

        if(Random.Range(0,100) > _BuffValue) return;

        GetComponent<IDamageable>().Freezing = true;

        _Ice =  ObjectPoolManager.Instance.GetObject("FreezeEffect");
        if(_Ice == null) return;
        _Ice.SetActive(true);
        _Ice.transform.position = transform.position;
        _Ice.GetComponent<Animator>().Play("iceeee");
        
        StartCoroutine(ExiFreeze(FreezeDuration));
    }


    private IEnumerator ExiFreeze(float time){
        yield return new WaitForSeconds(time);
        ResetBuff();
    }

    public override void ResetBuff()
    {
        _Ice.SetActive(false);
        GetComponent<IDamageable>().Freezing = false;
        StopAllCoroutines();
    }
}
