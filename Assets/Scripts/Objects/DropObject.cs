using System;
using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;

public class DropObject : MonoBehaviour, IInteractable
{
    [SerializeField] private int _GunID;
    private List<GameObject> _ModelList = new();
    private List<int> _GunIDList = new List<int>();
    [SerializeField] private GameObject _Model;
     [SerializeField] private LayerMask _Surface;

    void Start()
    {
        //Đăng ký Event
        Observer.AddListener(EvenID.DeActiveDropObject, OnDeActiveDropObject);
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) > 30f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDeActiveDropObject(object[] obj)
    {
        gameObject.SetActive(false);
    }

    public string InteractMessage => "Pick up";

    public void TakeAction()
    {
        HolderController holderController = FindObjectOfType<HolderController>();

        if(holderController == null){
            return;
        }

        object[] data = holderController.PickUpGun(_GunID);

        if((int)data[0] == -1){
            OnDeActiveDropObject(null);
            return;
        }

        SetDrop((GameObject)data[1], (int)data[0]);

        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;  
    }

    public void SetDrop(GameObject Model, int id){
        // Gán giá trị _GunID
        _GunID = id;
    
        //Tắt tất cả các model
        foreach(GameObject model in _ModelList)
        {
            if(model != null) model.SetActive(false);
        }
    
        // Kiểm tra xem _GunID có trong _GunIDList không
        if(!_GunIDList.Contains(_GunID))
        {
            // Nếu không có, thêm _GunID vào _GunIDList
            int newID = _GunID;
            _GunIDList.Add(newID);
    
            // Tạo một mô hình mới và thêm vào _ModelList
            GameObject newModel = Instantiate(Model, _Model.transform);
            _ModelList.Add(newModel);
        }
        else
        {
            // Nếu đã có, kích hoạt model tương ứng trong _ModelList
            _ModelList[_GunIDList.IndexOf(_GunID)].SetActive(true);
        }   
    }


    
}
