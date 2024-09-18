using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;


public class HolderController : MonoBehaviour
{   
    [SerializeField] private List<GameObject> _GunList;
    [SerializeField] private GameObject[] _OwnGuns = new GameObject[2];
    private bool _CurrentGunOrder = false;
    [SerializeField] private GameObject _2Hand;


    void Awake()
    {
        //Khởi tạo
        GameObject[] _GunListTmp = Resources.LoadAll<GameObject>("Gun");
        foreach(GameObject gun in _GunListTmp){
            GameObject gunTmp = Instantiate(gun, transform);
            _GunList.Add(gunTmp);
            gunTmp.SetActive(false);

        }
    }
    void Start()
    {
        _OwnGuns[0] = _GunList[0];
        _OwnGuns[0].SetActive(true);
        Observer.PostEvent(EvenID.DisplayCurrentGunIcon, _OwnGuns[0].GetComponent<GunController>().GetIcon());
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
        if(_OwnGuns[1] != null && Input.GetAxis("Mouse ScrollWheel") != 0f){
            int _Current= _CurrentGunOrder ? 0 : 1;
            int _Next= !_CurrentGunOrder ? 0 : 1;

            ChangeHandStyle(_OwnGuns[_Next].GetComponent<GunController>().GetStyle());

            _OwnGuns[_Current].SetActive(false);
            _OwnGuns[_Next].SetActive(true);
            DisplayGunStatus(_Next);
            
            _CurrentGunOrder = !_CurrentGunOrder;
        }
    }

    public object[] PickUpGun(int id){
        //Nhặt súng
        if(_OwnGuns[1]== null){
            _OwnGuns[1] = _GunList[id];

            _OwnGuns[0].SetActive(false);
            _OwnGuns[1].SetActive(true);

            DisplayGunStatus(1);
            return new object[] { -1 };
        }
    
        int newid = _GunList.IndexOf(_OwnGuns[1]);
        foreach(GameObject gun in _OwnGuns){
            if(gun != null)
                gun.SetActive(false);
        }

        object[] data = new object[] {newid, _OwnGuns[1].GetComponent<GunController>().GetModel()};

        _OwnGuns[1] = _GunList[id];
        _OwnGuns[1].SetActive(true);
        ChangeHandStyle(_OwnGuns[1].GetComponent<GunController>().GetStyle());
        DisplayGunStatus(1); 

        return data;
    }

    private void DisplayGunStatus(int id){   //Hiển thị thông tin súng
        Observer.PostEvent(EvenID.DisplayCurrentGunIcon, _OwnGuns[id].GetComponent<GunController>().GetIcon());
        Observer.PostEvent(EvenID.DisplayPlayerAmmo, _OwnGuns[id].GetComponent<GunController>().GetRemainAmmo());
    }

    public void ReloadGun(){   //Nạp đạn
        
    }

    public List<GameObject> GetGunList(){
        return _GunList;
    }
}
