using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using TMPro;
using UnityEngine;

public class VendingMachine : MonoBehaviour, IInteractable
{
    [SerializeField] private enum VendingType {Poition, Ammo};
    [SerializeField] private VendingType _Type;
    [SerializeField] private int _Price;
    [SerializeField] private int _Amount;
    [SerializeField] private int _Times = 3;
    [SerializeField] TextMeshProUGUI _Text;
    [SerializeField] Transform _DropPos;
    private string _Message;
    private int _timesUsed = 0;

    public string InteractMessage => _Message;
   
       


    public void TakeAction()
    {
        if(!CoinAndAmmoManager.CanSpendCoins(_Price)){
            _Text.text = "Not enough coins T_T";
            return;
        } 

        if(_timesUsed >= _Times){
            return;
        } 

        CoinAndAmmoManager.SpendCoins(_Price);
        Observer.PostEvent(EvenID.DisplayCoin);

        _Text.text = "Thanks! >_<";
        string tag = _Type == VendingType.Poition ? "Potion" : "Ammo";

        if(tag == "Potion"){
            tag = "Potion" + Random.Range(1, 4);
        }

        for(int i = 0; i < _Amount; i++){
            GameObject spawnedItem = ObjectPoolManager.Instance.GetObject(tag);
    
            if(spawnedItem == null) return;
            spawnedItem.SetActive(true);  
    
            spawnedItem.transform.position = _DropPos.position;

            _DropPos.transform.rotation = Quaternion.Euler(Random.Range(-15,-45), Random.Range(-30,30), 0);
    
            Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
            if (rb == null) return;
           
            // Áp dụng lực
            rb.AddForce(-_DropPos.transform.right*Random.Range(3f, 6f), ForceMode.Impulse);

            
        }

        _timesUsed ++;

        if(_timesUsed >= _Times){
            _Text.text = "Sold out! X_X";
            _Message = "Sold out!";
            return;
        } 
    }

    
    
    void OnEnable()
    {
        _Text.text = _Type == VendingType.Poition ? "POTION" : "AMMO";
        _Message = "Buy: " + _Price + "$";
        _timesUsed = 0;
    }

    
}
