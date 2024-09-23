using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;

public class BuffUpgrade : MonoBehaviour, IInteractable
{
    private List<BuffEffect> _buffEffects;
    [SerializeField]private int _costToUpgrade;

    public string InteractMessage => "Upgrade Buff";

    public void TakeAction()
    {
        Observer.PostEvent(EvenID.DisplayBuffUpgradeUI,_costToUpgrade);
    }
}
