using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;


public abstract class BaseEffect: MonoBehaviour
{
    public int _BuffValue { get; set; }
    
    void Awake(){
        Observer.AddListener(EvenID.UpgradeBuff, UpgradeBuff);

        Debug.Log(this.GetType());
    }

    protected void UpgradeBuff(object[] data){
        if(this.GetType() == (System.Type)data[0]){
            _BuffValue = (int)data[1];
        }
    }

    public abstract void BuffTrigger();

    public abstract void ResetBuff();

    void OnDestroy(){
        Observer.RemoveListener(EvenID.UpgradeBuff, UpgradeBuff);
    }
}
