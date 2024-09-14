using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHanderController : MonoBehaviour
{
    [SerializeField] private GameObject[] _OwnGuns;
    private bool _CurrentGunOrder = false;
    [SerializeField] private GameObject _2Hand;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeGun();

    }

   

    private void ChangeHandStyle( bool _1HandGun){   //Đổi kiểu cầm 1 hoặc 2 tay cho từng loại súng
        if(_1HandGun)
            _2Hand.SetActive(false);
        else 
            _2Hand.SetActive(true);
        
    }

    private void ChangeGun(){   //Đổi súng
        if(_OwnGuns.Length ==2 && Input.GetAxis("Mouse ScrollWheel") != 0f){
            int _Current= _CurrentGunOrder ? 0 : 1;
            int _Next= !_CurrentGunOrder ? 0 : 1;

            ChangeHandStyle(_OwnGuns[_Next].GetComponent<GunController>().GetStyle());

            _OwnGuns[_Current].SetActive(false);
            _OwnGuns[_Next].SetActive(true);
            
            _CurrentGunOrder = !_CurrentGunOrder;
        }
    }
}

