using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDeactive : MonoBehaviour
{
   public void DeActive(){
      gameObject.SetActive(false);
      
   }

   public void IsTrigger(){
     gameObject.GetComponent<Collider>().isTrigger = true;
     
   }


   public void OnBecameInvisible()
   {
      DeActive();
   }
}
