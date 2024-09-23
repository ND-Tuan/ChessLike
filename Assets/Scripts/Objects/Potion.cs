using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;

public class Potion : MonoBehaviour, IInteractable
{
    [SerializeField] private int _HealAmount;
    [SerializeField] private LayerMask _Surface;

    public string InteractMessage => "Drink";

    void Update()
    {
        if(Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) > 30f)
        {
            gameObject.SetActive(false);
        }
    }

    public void TakeAction()
    {
        Observer.PostEvent(EvenID.HealPlayer, _HealAmount);
        gameObject.SetActive(false);
    }
}
