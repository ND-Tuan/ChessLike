using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDeactive : MonoBehaviour
{
   public void DeActive(){
      gameObject.SetActive(false);
   }

   public void OnBecameInvisible()
   {
      DeActive();
   }
}
