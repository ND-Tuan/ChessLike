using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

[System.Serializable]
public class Burning : BaseEffect
{
    public float DmgMultiplier = 1;
    public float DurationMultiplier = 1;
    public float CooldownMultiplier = 1;

    private int BurnDmg => (int)(GameManager.Instance._StatusEffectData.BurnDmg * DmgMultiplier);
    private float BurnDuration => GameManager.Instance._StatusEffectData.BurnDuration * DurationMultiplier;
    private float BurnCooldown => GameManager.Instance._StatusEffectData.BurnCooldown * CooldownMultiplier;

    private GameObject fire;

    public override void BuffTrigger(){
        if(GetComponent<IDamageable>().Burning) return;
        
        if(Random.Range(0,100) > _BuffValue) return;

        GetComponent<IDamageable>().Burning = true;

       fire = ObjectPoolManager.Instance.GetObject("Fire");

        if(fire != null){
            fire.transform.parent = gameObject.transform;
            fire.transform.localPosition = Vector3.zero;
            fire.SetActive(true);

            StartCoroutine(SetFire());
        }
    }
        
    private IEnumerator SetFire(){
        float time = 0;
        while(time < BurnDuration){
            yield return new WaitForSeconds(BurnCooldown);
            GetComponent<IDamageable>().TakeDamage(BurnDmg, Vector3.zero);
            time += BurnCooldown;
        }
        ResetBuff();
    }

    public override void  ResetBuff()
    {
        fire?.SetActive(false);
        GetComponent<IDamageable>().Burning = false;
        StopAllCoroutines();
    }
    
}
