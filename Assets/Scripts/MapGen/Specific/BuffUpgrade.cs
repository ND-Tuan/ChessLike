using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;

public class BuffUpgrade : MonoBehaviour, IInteractable
{
    [SerializeField]private int _costToUpgrade;

    public string InteractMessage => "Upgrade Buff";

    public void TakeAction(InteractionController Interacter)
    {
        MenuUI.Instance.OnDisplayBuffUpgradeUI(_costToUpgrade);
    }
}
