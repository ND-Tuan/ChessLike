using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

public class SoulEater :BaseEffect
{
    public override void BuffTrigger(){}

    public override void ResetBuff(){}

    private void OnDisable()
    {
        // Lấy Object từ Pool
        GameObject Tmp = ObjectPoolManager.Instance.GetObject("Soul");
        if (Tmp == null) return; 
        // Kích hoạt đối tượng
        Tmp.SetActive(true);
        Tmp.transform.position = transform.position + new Vector3(0, 0.7f, 0);
        Tmp.GetComponent<MoveToPlayer>().Amount = _BuffValue;
    
        Rigidbody rb = Tmp.GetComponent<Rigidbody>();
        if (rb == null) return;
    
        // Tạo một lực ngẫu nhiên
        Vector3 randomUpDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f)).normalized;
        float randomForce = Random.Range(5f, 10f);
        rb.AddForce(randomUpDirection * randomForce, ForceMode.Impulse);
    }

}
