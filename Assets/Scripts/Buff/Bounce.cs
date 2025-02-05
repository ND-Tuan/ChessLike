using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;
using Unity.VisualScripting;
using System;

public class Bounce :BaseEffect
{
    [SerializeField] private int currentBounces;
    private LayerMask _LayerMask = ~0;

    void Update()
    {
        if(!GetComponent<BulletHit>().PlayerBullet) return;

        if (Physics.Raycast(transform.position, transform.forward, out var hit, 0.7f + GetComponent<BulletHit>().force/30, _LayerMask)){
            if(hit.collider.CompareTag("Enemy")) return;
            BounceTrigger(hit);
        } 
    }


    public override void BuffTrigger(){}

    private void BounceTrigger(RaycastHit hit){
        if (currentBounces >= _BuffValue) return;
        currentBounces++;
    
        transform.forward = Vector3.Reflect(transform.forward, hit.normal);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().velocity = transform.forward * GetComponent<BulletHit>().force;
    }

    void OnEnable()
    {
        currentBounces = 0;
        _BuffValue = 3;
    }

    public override void ResetBuff(){ }
}
