using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveEnemy : MonoBehaviour
{
    void FixedUpdate()
    {
        if(transform.position.y < 0.2){
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            GetComponent<EnemyController>().enabled = true;
        }
    }

}
