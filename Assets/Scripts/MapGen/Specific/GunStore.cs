using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;

public class GunStore : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> _GunList;
    private readonly int[] _GunIndex = new int[3];
    private GunInfo[] _gunInfo = new GunInfo[3];
    private bool _IsRandom = false;
    public string InteractMessage => "Gun Store";


    void Start()
    {
        Observer.AddListener(EvenID.DropGun, OnDropGun);
    }

    public void TakeAction()
    {
        // Lấy danh sách súng từ HolderController
        _GunList = FindObjectOfType<HolderController>().GetGunList();
    
        // Kiểm tra xem có cần chọn ngẫu nhiên không
        if (_IsRandom)
        {
            // Chọn ngẫu nhiên súng trong danh sách
            TakeRandomGunIndex(_GunList.Count);
    
            for (int i = 0; i < 3; i++){
                // Lấy thông tin của súng được chọn và lưu vào _gunInfo
                _gunInfo[i] = _GunList[_GunIndex[i]].GetComponent<GunController>().GetInfo();
            }
    
            _IsRandom = false;
        }
    
        // Gửi Event hiển thị giao diện
        Observer.PostEvent(EvenID.DisplayGunStoreUI, _gunInfo);
    }
    
    void OnEnable()
    {
        _IsRandom = true;
    }

    private void TakeRandomGunIndex(int _NumGun){
        _GunIndex[0] = Random.Range(1, _NumGun);
        _GunIndex[1] = Random.Range(1, _NumGun);
        _GunIndex[2] = Random.Range(1, _NumGun);


        //lấy ngẫu nhiên 3 số khác nhau
        while(_GunIndex[0] == _GunIndex[1] || _GunIndex[0] == _GunIndex[2] || _GunIndex[1] == _GunIndex[2]){
            _GunIndex[1] = Random.Range(1, _NumGun);
            _GunIndex[2] = Random.Range(1, _NumGun);
        }
    }

    private void OnDropGun(object[] data)
    {

        // Đầu vào lựa chọn
        int _GunId = _GunIndex[(int)data[0]];
        
        // Lấy object từ Pool
        GameObject drop = ObjectPoolManager.Instance.GetObject("DropObject");
        if (drop == null) return;
    
       
        drop.SetActive(true);
        drop.transform.position = new Vector3(transform.position.x, 0, transform.position.z - 2);
    
        // Lấy model súng
        GameObject model = _GunList[_GunId].GetComponent<GunController>().GetModel();
    
        // Drop súng
        drop.GetComponent<DropObject>().SetDrop(model, _GunId);
    
        // Gửi Event hiển thị Coin
        Observer.PostEvent(EvenID.DisplayCoin);
    }

}
