
using System.Collections.Generic;
using ObserverPattern;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="Buff", menuName = "ScriptableObject/Buff")]
public class BuffEffect : ScriptableObject
{
    public BuffID BuffID;
    public string BuffName;
    public string description;
    public BaseEffect BuffComponent ;
    [SerializeField] private EvenID ApplyTo;
    [SerializeField] private int[] _BuffValue = new int[2];


    public bool _isUpgraded {private set; get;}

    public void ApplyBuff(){

        object[] data =  {BuffDictionary.GetBuffType(BuffID), _BuffValue[0]};

        Observer.PostEvent(ApplyTo, data );
        _isUpgraded = false;
    }

    public void UpgradeBuff(){
        object[] data =  {BuffDictionary.GetBuffType(BuffID), _BuffValue[1]};
        Observer.PostEvent(ApplyTo, data );
        _isUpgraded = true;
    }

    public string SetDescription(int Lvl){
        string desc = description.Replace("#", _BuffValue[Lvl].ToString());
        return desc;
    }
}


