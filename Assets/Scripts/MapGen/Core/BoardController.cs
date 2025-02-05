using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class EnemyWaveSetting {
    [Range(0,2)]
    public int DifficultLevel;
    [Range(1,5)]
    public int NumberOfWaves;
    [Range(1,10)]
    public int[] NumberEnemiesOfWave ;
}

public class BoardController : MonoBehaviour
{
   
    public void PrepareCombat(EnemyWaveSetting waveSetting){
        StartCoroutine(StartWave(waveSetting));

        Debug.Log("PrepareCombat");
    }

    private IEnumerator StartWave(EnemyWaveSetting waveSetting){

        for(int i = 0; i < waveSetting.NumberOfWaves; i++){

            SpawnEnermy(waveSetting.NumberEnemiesOfWave[i], waveSetting.DifficultLevel);
            yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Enemy") == null);
            //yield return new WaitForSeconds(5);
        }

        Observer.PostEvent(EvenID.CombatDone);
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

    public Vector3 TakeRandomPosition(){

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

