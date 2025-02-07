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
    [SerializeField] private GameObject FinalBoard;
    [SerializeField] private GameObject _NormalOptions;
    [SerializeField] private GameObject[] _NormalOptionsDisplay = new GameObject[2];
    [SerializeField] private Teleporter[] teleporter;
    [SerializeField] private GameObject RestOption;
    [SerializeField] private GameObject BossOption;
    [SerializeField] private GameObject FinalOption;
    [SerializeField] private GameObject CombatCheck;
    [SerializeField] private GameObject surface;

    private GameObject _CurrentActiveBoard;
    private int[] _OptionArray = new int[2];
    private int _randomInt = 0;
    


    void Awake()
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

        foreach(GameObject basicBoard in Resources.LoadAll<GameObject>("BasicBoard")){
            GameObject boardTmp = Instantiate(basicBoard, this.transform);
            boardTmp.SetActive(false);
            _BasicBoardList.Add(boardTmp);
        }

        //Đăng ký Event
        Observer.AddListener(EvenID.BoardPrepare, PrepareBoard);
        Observer.AddListener(EvenID.BoardDone, PrepareRandomOptions);
        Observer.AddListener(EvenID.BeginCombat, OnBeginCombat);
    }

    

    //Random các ải chơi tiếp theo
    public async void PrepareRandomOptions(object[] data){
        await Task.Delay(100);

        //chuẩn bị ải nghỉ ngơi trước khi đến ải Boss
        if(GameManager.Instance._CurrentProgress == GameManager.Instance._NumBoardBeforeBoss+1){
            RestOption.SetActive(true);
            RestOption.transform.position = _CurrentActiveBoard.transform.position;
            RestOption.GetComponentInChildren<Animator>().SetBool("Play", true);
            return;
        }

        //chuẩn bị ải Boss
        if(GameManager.Instance._CurrentProgress == GameManager.Instance._NumBoardBeforeBoss+2){
            BossOption.SetActive(true);
            BossOption.transform.position = _CurrentActiveBoard.transform.position;
            BossOption.GetComponentInChildren<Animator>().SetBool("Play", true);
            return;
        }

        if(GameManager.Instance._CurrentStage > _BossBoard.Count){
            FinalOption.SetActive(true);
            FinalOption.transform.position = _CurrentActiveBoard.transform.position;
            FinalOption.GetComponentInChildren<Animator>().SetBool("Play", true);
            return;
        }

        _OptionArray[0] = Random.Range(0, _SpecificBoardList.Count);

        _OptionArray[1] = Random.Range(0, _SpecificBoardList.Count);
        while(_OptionArray[1] == _OptionArray[0]){
            _OptionArray[1] = Random.Range(0, _SpecificBoardList.Count);
        }

        SpecificBoard[] Opt = {_SpecificBoardInfo[_OptionArray[0]], _SpecificBoardInfo[_OptionArray[1]]};
        DisplayOptions(Opt);
    }

    
    //hiển thị các lựa chọn ải tiếp theo
    public void DisplayOptions(SpecificBoard[] OptionInfo){
        _NormalOptions.SetActive(true);
        _NormalOptions.transform.position = _CurrentActiveBoard.transform.position;

        teleporter = GetComponentsInChildren<Teleporter>();

        //đổi icon cho các lựa chọn khu vực tiếp theo
        for(int i = 0; i<2; i++){
            _NormalOptionsDisplay[i].GetComponent<MeshRenderer>().material = OptionInfo[i].BoardIcon;
            teleporter[i]._Message = OptionInfo[i].Message;
        }
        teleporter[2]._Message = "Combat";
    
        _NormalOptions.GetComponentInChildren<Animator>().SetBool("Play", true);
    }


    //Active lại ải chơi, đưa đến vị trí theo hướng lựa chọn (1-trái, 2-phải, 3-trước)
    public void PrepareBoard(object[] data)
    {
        TeleportDiraction Diraction = (TeleportDiraction)data[0];

        Vector3 newPos = MoverBoard(Diraction);
        GameManager.Instance.CurrentBoardPosition = newPos;

        //Vô hiệu hóa ải cũ
        DeActivePreviousBoard();

        // Chọn ngẫu nhiên một ải chưa được kích hoạt từ danh sách 
        if(Diraction != TeleportDiraction.Boss && Diraction != TeleportDiraction.Final){
            
            _randomInt = Random.Range(0, _BasicBoardList.Count);
            while (_BasicBoardList[_randomInt].activeInHierarchy)
            {
                _randomInt = Random.Range(0, _BasicBoardList.Count);
            }

            // Đặt vị trí cho ải được chọn
            _BasicBoardList[_randomInt].SetActive(true);
            _BasicBoardList[_randomInt].transform.position = newPos;
            _CurrentActiveBoard = _BasicBoardList[_randomInt];
        }

        switch (Diraction){
            case TeleportDiraction.Left:
                _SpecificBoardList[_OptionArray[0]].SetActive(true);
                _SpecificBoardList[_OptionArray[0]].transform.position = newPos;
                break;

            case TeleportDiraction.Right:
                _SpecificBoardList[_OptionArray[1]].SetActive(true);
                _SpecificBoardList[_OptionArray[1]].transform.position = newPos;
                break;
            
            case TeleportDiraction.Combat:
                CombatCheck.SetActive(true);
                CombatCheck.transform.position = newPos;
                break;

            case TeleportDiraction.Rest:
                RestBoard.SetActive(true);
                RestBoard.transform.position = newPos;
                break;
            
            case TeleportDiraction.Boss:
                _BossBoard[GameManager.Instance._CurrentStage-1].SetActive(true);
                _BossBoard[GameManager.Instance._CurrentStage-1].transform.position = newPos;
                _CurrentActiveBoard = _BossBoard[GameManager.Instance._CurrentStage-1];
                break;

            case TeleportDiraction.Final:
                FinalBoard.SetActive(true);
                FinalBoard.transform.position = newPos;
                return;
        }


        if(Diraction == TeleportDiraction.Boss || Diraction == TeleportDiraction.Combat){
            surface.transform.position = newPos;
            surface.GetComponent<NavMeshSurface>().BuildNavMesh();
        } else {
            Observer.PostEvent(EvenID.BoardDone);
        }

        
    }


    //Di chuyển ải chơi đến vị trí mới
    private Vector3 MoverBoard(TeleportDiraction Diraction){
        
        Vector3 newPos;

         // Xác định vị trí mới dựa trên hướng dịch chuyển
        if (Diraction == TeleportDiraction.Left){
            newPos = _CurrentActiveBoard.transform.position + new Vector3(0, 0, 60);
            return newPos;
        }
        if (Diraction == TeleportDiraction.Right){
            newPos = _CurrentActiveBoard.transform.position + new Vector3(60, 0, 0);
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
        // Đặt lại và vô hiệu hóa ải hiện tạiS
        _NormalOptions.SetActive(false);

        _CurrentActiveBoard.SetActive(false);

        foreach(var Object in _SpecificBoardList){
            Object.SetActive(false);
        }

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
