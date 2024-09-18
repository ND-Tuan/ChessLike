using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObserverPattern;
using UnityEngine;

public class ChestController : MonoBehaviour, IInteractable
{
    private Animator _animator;
    [SerializeField] private int _CoinAmount;
    [SerializeField] private int _AmmoAmount;

    [Range(0, 100)]
    [SerializeField] private int _ChanceToDropWeapon;
    private bool _IsOpen = false;
    

    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    
    public string InteractMessage => "Open";

    void OnEnable()
    {
        _IsOpen = false;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    public void TakeAction(){
        if(_IsOpen) return;
        _animator.SetBool("Open", true);
        SpawnItem("Coin", _CoinAmount);
        SpawnItem("Ammo", _AmmoAmount);

        DropWeapon();
        _IsOpen = true;
    }

    private async void SpawnItem(string tag, int amount){
       
        await Task.Delay(500);
    
        for(int i = 0; i < amount; i++){
            
            GameObject spawnedItem = ObjectPoolManager.Instance.GetObject(tag);

            if(spawnedItem == null) return;
            spawnedItem.SetActive(true);  
            spawnedItem.transform.position = transform.position + new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));
    
            ApplyRandomForce(spawnedItem);
        }
    }

    private async void DropWeapon(){
        if(Random.Range(0, 100) < _ChanceToDropWeapon){
            await Task.Delay(500);
            List<GameObject> _GunList = FindObjectOfType<HolderController>().GetGunList();

            if(_GunList.Count == 0) return;
            int randomIndex = Random.Range(1, _GunList.Count);
            GameObject drop = ObjectPoolManager.Instance.GetObject("DropObject");

            if (drop == null) return;
            drop.SetActive(true);

            // Lấy model súng
            GameObject model = _GunList[randomIndex].GetComponent<GunController>().GetModel();
    
            // Drop súng
            drop.GetComponent<DropObject>().SetDrop(model, randomIndex);

            drop.transform.position = transform.position + new Vector3(0, 0.5f, 0);
            ApplyRandomForce(drop);

    
        }
    }

    private void ApplyRandomForce(GameObject spawnedItem){

        Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
        if (rb == null) return;
            
        // Tạo hướng ngẫu nhiên áp dụng lực 
        Vector3 randomUpDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f)).normalized;
        float randomForce = Random.Range(5f, 10f);
        rb.AddForce(randomUpDirection * randomForce, ForceMode.Impulse);
    }

    public void DeActiveChest(){
        transform.parent.gameObject.SetActive(false);
    }

   
}
