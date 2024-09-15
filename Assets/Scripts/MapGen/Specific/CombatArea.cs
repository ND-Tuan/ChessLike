using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;

public class CombatArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Observer.PostEvent(EvenID.BeginCombat);
            gameObject.SetActive(false);
        }
    }

}
