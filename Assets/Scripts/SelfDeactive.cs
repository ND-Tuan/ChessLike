using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDeactive : MonoBehaviour
{
   [SerializeField] private bool NeedDeactiveWhenBecomeInvisible = true;
   [SerializeField] private float _timeToDeactive = 1f;

   public void DeActive(){
      gameObject.SetActive(false);
      if(TryGetComponent<Collider>(out var collider)){
         collider.enabled = false;
      }
      Time.timeScale = 1;
   }

   public void IsTrigger(){
      if(TryGetComponent<Collider>(out var collider)){
         collider.enabled = false;
      }
     
   }


   public void OnBecameInvisible()
   {
      if(NeedDeactiveWhenBecomeInvisible)
         DeActive();


         
   }
}
