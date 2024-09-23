using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ObserverPattern;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BuffInfo{
    public string BuffName;
    public string description;
    public bool isUpgraded;
    public EvenID buffID;

}
  public class BuffGiver : MonoBehaviour, IInteractable
{
    [SerializeField] private int[] _BuffIndex = new int[3];
    [SerializeField] private BuffInfo[] _buffInfo = new BuffInfo[3];
    private bool _IsRandom = false;
    private string _Message;
    public string InteractMessage => _Message;

    [SerializeField] private GameObject _Crystal;

    void Awake()
    {   
        //đăng ký event
        Observer.AddListener(EvenID.SelectBuff, OnSelectBuff);
        _Message = "Take a buff";
    }

    void Update()
    {
        _Crystal.transform.rotation = Quaternion.Euler(0, _Crystal.transform.eulerAngles.y+ Time.deltaTime * 20, 0);
    }

    public void TakeAction()
    {
        
        // Kiểm tra xem có cần chọn ngẫu nhiên không
        if (!_IsRandom) return;
        List<BuffEffect> _BuffList = GameManager.Instance.GetBuffList(); 
        int count = _BuffList.Count;

        // Chọn ngẫu nhiên buff trong danh sách
        TakeRandomIndex(_BuffList.Count);

       
        if(count >=3) count = 3;

        for (int i = 0; i < count; i++){
            // Lấy thông tin của buff được chọn và lưu vào 
            _buffInfo[i].BuffName = _BuffList[_BuffIndex[i]].BuffName;
            _buffInfo[i].description = _BuffList[_BuffIndex[i]].SetDescription(0);
        }
    
        _IsRandom = false;
        
        _Message = null ;
        // Gửi Event hiển thị giao diện
        Observer.PostEvent(EvenID.DisplayBuffSelectUI, _buffInfo);
       
    }
    
    void OnEnable()
    {
        _Message = "Take a buff";
        _IsRandom = true;
    }

    private void TakeRandomIndex(int _num){
        _BuffIndex[0] = Random.Range(0, _num);

        if(_num <= 1) return;
        _BuffIndex[1] = Random.Range(0, _num);

        while(_BuffIndex[0] == _BuffIndex[1]){
            _BuffIndex[1] = Random.Range(0, _num);
        }

        if(_num <= 2) return;
        _BuffIndex[2] = Random.Range(0, _num);

        while(_BuffIndex[2] == _BuffIndex[0] || _BuffIndex[2] == _BuffIndex[1]){
            _BuffIndex[2] = Random.Range(0, _num);
        }
    }

    private void OnSelectBuff(object[] obj)
    {
        int _OnSelect = (int)obj[0];
        GameManager.Instance.AddBuff(GameManager.Instance.GetBuffList()[_BuffIndex[_OnSelect]]);


        gameObject.SetActive(false);
    }

}
