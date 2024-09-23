
using ObserverPattern;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="Buff", menuName = "ScriptableObject/Buff")]
public class BuffEffect : ScriptableObject
{
    public string BuffName;
    public EvenID BuffID;
    public string description;
    [SerializeField] private int[] _BuffValue = new int[2];

    public bool _isUpgraded {private set; get;}


    public void ApplyBuff(){
        ActiveBuff(_BuffValue[0], BuffID);
        _isUpgraded = false;
   }

    public void UpgradeBuff(){
        ActiveBuff(_BuffValue[1], BuffID);
        _isUpgraded = true;
   }

    public string SetDescription(int Lvl){
        string desc = description.Replace("#", _BuffValue[Lvl].ToString());

        return desc;
    }

    private void ActiveBuff(int value, EvenID evenID){
        Observer.PostEvent(evenID, value);
        
    }
   
}


