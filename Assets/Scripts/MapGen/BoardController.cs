using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class EnemyWaveSetting {
    [Range(1,5)]
    public int DifficultLevel;
    [Range(1,5)]
    public int NumberOfWaves;
    [Range(1,10)]
    public int[] NumberEnemiesOfWave ;
}

public class BoardController : MonoBehaviour
{
    [SerializeField] private GameObject _Options;
    [SerializeField] private GameObject[] _OptionsDisplay = new GameObject[2];
    [SerializeField] private Teleporter[] teleporter;

    void Awake()
    {
        
    }

    //hiển thị các lựa chọn ải tiếp theo
    public void DisplayOptions(SpecificBoard[] OptionInfo){
        _Options.SetActive(true);
        teleporter = GetComponentsInChildren<Teleporter>();

        //đổi icon cho các lựa chọn khu vực tiếp theo
        for(int i = 0; i<2; i++){
            _OptionsDisplay[i].GetComponent<MeshRenderer>().material = OptionInfo[i].BoardIcon;
            teleporter[i]._Message = OptionInfo[i].Message;
        }
        teleporter[2]._Message = "Combat";
    
        _Options.GetComponent<Animator>().SetBool("Play", true);
        
    }

    //Làm mới ải nhằm tái sử dụng
    public void ResetBoard(){
        _Options.transform.position = new Vector3(0, -1.28f, 0);
        _Options.SetActive(false);
    }

    public void SetupWave(EnemyWaveSetting waveSetting){
        for(int i = 0; i < waveSetting.NumberOfWaves; i++){
            SpawnEnermy(waveSetting.NumberEnemiesOfWave[i], i+1);
        }
    }

    public void PrepareCombat(EnemyWaveSetting waveSetting){
        StartCoroutine(StartWave(waveSetting));

        Debug.Log("PrepareCombat");
    }

    private IEnumerator StartWave(EnemyWaveSetting waveSetting){

        for(int i = 0; i < waveSetting.NumberOfWaves; i++){

            SpawnEnermy(waveSetting.NumberEnemiesOfWave[i], waveSetting.DifficultLevel);
            Debug.Log("Wave " + i);
            yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Enemy") == null);
            //yield return new WaitForSeconds(5);
        }

        Observer.PostEvent(EvenID.BoardDone);
    }

    private void SpawnEnermy(int quantity, int difficultLevel){

        // Lấy danh sách các Enermy từ Pool
        List<GameObject> enemyList = ObjectPoolManager.Instance.GetAllObjects("Enemy");
    
        // Chọn ngẫu nhiên các Enermy khác nhau từ List
        List<int> selectedIndices = new();
        while (selectedIndices.Count < quantity)
        {
            int randomIndex = Random.Range(0, enemyList.Count);
            if (!selectedIndices.Contains(randomIndex))
            {
                selectedIndices.Add(randomIndex);
            }
        }
    
        
        foreach (int index in selectedIndices)
        {
            // SetActive và thiết lập độ khó
            enemyList[index].SetActive(true);
            enemyList[index].GetComponent<EnemyController>().EnermyLevel = difficultLevel;

            // Set vị trí ngẫu nhiên
            enemyList[index].transform.position = TakeRandomPosition();
        }
    }

    private Vector3 TakeRandomPosition(){

        // lấy vị trí ngẫu nhiên
        Vector3 position = transform.position + new Vector3(Random.Range(-10, 10), 15, Random.Range(-10, 10));
    
        // bắn Raycast để xác định điểm rơi
        Ray ray = new Ray(position, Vector3.down);
        Physics.Raycast(ray, out RaycastHit hit);
    
        int num;
        Collider[] hitColliders = new Collider[3];
        int mask = 0x01 << LayerMask.NameToLayer("Surface");
    
        // Kiểm tra các vật cản tại điểm rơi
        num = Physics.OverlapSphereNonAlloc(hit.point, 1, hitColliders, mask);
    
        //tìm cho đến khi được vị trí hợp lệ (dưới mặt đất và không có vật cản)
        while (hit.point.y > 0 || num > 1)
        {
            // Tạo lại vị trí ngẫu nhiên
            position = transform.position + new Vector3(Random.Range(-10, 10), 15, Random.Range(-10, 10));
            ray = new Ray(position, Vector3.down);
            Physics.Raycast(ray, out hit, 15);
            num = Physics.OverlapSphereNonAlloc(hit.point, 1, hitColliders, mask);
        }
    
        // Trả về vị trí hợp lệ
        return position;
    }
}

