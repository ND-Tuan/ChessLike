using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using ObserverPattern;
using System.Threading.Tasks;

public class MapMaker : MonoBehaviour
{
    [SerializeField] private List<GameObject> _BasicBoardList;
    [SerializeField] private SpecificBoard[] _SpecificBoardInfo;
    [SerializeField] private List<GameObject> _SpecificBoardList;
    [SerializeField] private List<GameObject> _BossBoard;
    [SerializeField] private GameObject RestBoard;
    [SerializeField] private GameObject RestOption;
    [SerializeField] private GameObject BossOption;
    [SerializeField] private GameObject CombatCheck;
    [SerializeField] private GameObject surface;

    private GameObject _CurrentActiveBoard;
    private int[] _OptionArray = new int[2];
    private int _randomInt = 0;


    void Start()
    {
        _CurrentActiveBoard = GameObject.FindGameObjectWithTag("Basic Board");

        //load thông tin các ải chơi
        _SpecificBoardInfo = Resources.LoadAll<SpecificBoard>("SpecificBoard"); 

        //Tạo sẵn trong loading screen
        foreach(SpecificBoard board in _SpecificBoardInfo){ 
            GameObject boardTmp = Instantiate(board.Prefab, this.transform);
            boardTmp.SetActive(false);
            _SpecificBoardList.Add(boardTmp);
        }

        //Đăng ký Event
        Observer.AddListener(EvenID.BoardPrepare, PrepareBoard);
        Observer.AddListener(EvenID.BoardDone, PrepareRandomOptions);
        Observer.AddListener(EvenID.BeginCombat, OnBeginCombat);
    }



    //Random các ải chơi tiếp theo
    public async void PrepareRandomOptions(object[] data){

        await Task.Delay(1000);

        if(GameManager.Instance._CurrentProgress == GameManager.Instance._NumBoardBeforeBoss+1){
            RestOption.SetActive(true);
            RestOption.transform.position = _CurrentActiveBoard.transform.position;
            RestOption.GetComponentInChildren<Animator>().SetBool("Play", true);
            return;
        }

        if(GameManager.Instance._CurrentProgress == GameManager.Instance._NumBoardBeforeBoss+2){
            BossOption.SetActive(true);
            BossOption.transform.position = _CurrentActiveBoard.transform.position;
            BossOption.GetComponentInChildren<Animator>().SetBool("Play", true);
            return;
        }

        _OptionArray[0] = Random.Range(0, _SpecificBoardList.Count);

        _OptionArray[1] = Random.Range(0, _SpecificBoardList.Count);
        while(_OptionArray[1] == _OptionArray[0]){
            _OptionArray[1] = Random.Range(0, _SpecificBoardList.Count);
        }

        SpecificBoard[] Opt = {_SpecificBoardInfo[_OptionArray[0]], _SpecificBoardInfo[_OptionArray[1]]};
        _BasicBoardList[_randomInt].GetComponent<BoardController>().DisplayOptions(Opt);
    }



    //Active lại ải chơi, đưa đến vị trí theo hướng lựa chọn (1-trái, 2-phải, 3-trước)
    public void PrepareBoard(object[] data)
    {
        TeleportDiraction Diraction = (TeleportDiraction)data[0];

        Vector3 newPos = MoverBoard(Diraction);
    
        // Chọn ngẫu nhiên một ải chưa được kích hoạt từ danh sách 
        _randomInt = Random.Range(0, _BasicBoardList.Count);
        while (_BasicBoardList[_randomInt].activeInHierarchy)
        {
            _randomInt = Random.Range(0, _BasicBoardList.Count);
        }

       
        // Đặt vị trí cho ải được chọn
        _BasicBoardList[_randomInt].SetActive(true);
        _BasicBoardList[_randomInt].transform.position = newPos;

        // Gọi hàm để vô hiệu hóa ải trước đó sau 0.5 giây
        Invoke(nameof(DeActivePreviousBoard), 0.5f);
       

        if(Diraction == TeleportDiraction.Combat){
            CombatCheck.SetActive(true);
            CombatCheck.transform.position = newPos;

            // Cập nhật vị trí và tạo lại NavMesh
            surface.transform.position = newPos;
            surface.GetComponent<NavMeshSurface>().BuildNavMesh();

            return;
        } 

        if(Diraction == TeleportDiraction.Rest){
            RestBoard.SetActive(true);
            RestBoard.transform.position = newPos;
        }

        if(Diraction == TeleportDiraction.Boss){
            // _BossBoard[GameManager.Instance._CurrentStage].SetActive(true);
            // _BossBoard[GameManager.Instance._CurrentStage].transform.position = newPos;
            // return;
        }
        
        Observer.PostEvent(EvenID.BoardDone);
        
    
       
    }


    //Di chuyển ải chơi đến vị trí mới
    private Vector3 MoverBoard(TeleportDiraction Diraction){

        Vector3 newPos;

         // Xác định vị trí mới dựa trên hướng dịch chuyển
        if (Diraction == TeleportDiraction.Left){
            newPos = _CurrentActiveBoard.transform.position + new Vector3(0, 0, 60);

            _SpecificBoardList[_OptionArray[0]].SetActive(true);
            _SpecificBoardList[_OptionArray[0]].transform.position = newPos;

            return newPos;
        }
        if (Diraction == TeleportDiraction.Right){
            newPos = _CurrentActiveBoard.transform.position + new Vector3(60, 0, 0);

            _SpecificBoardList[_OptionArray[1]].SetActive(true);
            _SpecificBoardList[_OptionArray[1]].transform.position = newPos;

            return newPos;
        }
            
        newPos = _CurrentActiveBoard.transform.position + new Vector3(60, 0, 60);

        return newPos;
    }



    private void OnBeginCombat(object[] data){

        int _currentDifficult = GameManager.Instance._CurrentStage - 1;
        EnemyWaveSetting _waves = GameManager.Instance._enemyWaveSetting[_currentDifficult];
        _waves.DifficultLevel = _currentDifficult;
        
        _BasicBoardList[_randomInt].GetComponent<BoardController>().PrepareCombat(_waves);
    }


    private void DeActivePreviousBoard()
    {
        // Đặt lại và vô hiệu hóa ải hiện tại
        _CurrentActiveBoard.GetComponent<BoardController>().ResetBoard();
        _CurrentActiveBoard.SetActive(false);
        _CurrentActiveBoard = _BasicBoardList[_randomInt];
        RestOption.SetActive(false);
        BossOption.SetActive(false);
        
    }


    void OnDestroy()
    {
        //Hủy đăng ký Event
        Observer.RemoveListener(EvenID.BoardPrepare, PrepareBoard);
        Observer.RemoveListener(EvenID.BoardDone, PrepareRandomOptions);
        Observer.RemoveListener(EvenID.BeginCombat, PrepareBoard);
    }
}
