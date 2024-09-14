using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using ObserverPattern;

public class MapMaker : MonoBehaviour
{
    [SerializeField] private List<GameObject> _BasicBoardList;
    [SerializeField] private SpecificBoard[] _SpecificBoardInfo;
    [SerializeField] private List<GameObject> _SpecificBoardList;
    [SerializeField] private List<GameObject> _BossBoardList;
    [SerializeField] private GameObject RestBoard;
    [SerializeField] private GameObject surface;

    private GameObject _CurrentActiveBoard;
    private int[] _OptionArray = new int[2];
    private int _randomInt;


    void Start()
    {
        _CurrentActiveBoard = GameObject.FindGameObjectWithTag("Basic Board");
        _CurrentActiveBoard.GetComponent<BoardController>().PrepareCombat(GameManager.Instance._enemyWaveSetting[0]);

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

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //Random các ải chơi tiếp theo
    public void PrepareRandomOptions(object[] data){
        _OptionArray[0] = Random.Range(0, _SpecificBoardList.Count);

        _OptionArray[1] = Random.Range(0, _SpecificBoardList.Count);
        while(_OptionArray[1] == _OptionArray[0]){
            _OptionArray[1] = Random.Range(0, _SpecificBoardList.Count);
        }

        SpecificBoard[] Opt = {_SpecificBoardInfo[_OptionArray[0]], _SpecificBoardInfo[_OptionArray[1]]};
        _CurrentActiveBoard.GetComponent<BoardController>().DisplayOptions(Opt);
    }


    //Active lại ải chơi, đưa đến vị trí theo hướng lựa chọn (1-trái, 2-phải, 3-trước)
    public void PrepareBoard(object[] data)
    {
        TeleportDiraction Diraction = (TeleportDiraction)data[0];
        Vector3 newPos;
    
        // Xác định vị trí mới dựa trên hướng dịch chuyển
        if (Diraction == TeleportDiraction.Left){
            newPos = _CurrentActiveBoard.transform.position + new Vector3(0, 0, 60);
            _SpecificBoardList[_OptionArray[0]].SetActive(true);
            _SpecificBoardList[_OptionArray[0]].transform.position = newPos;

        }else if (Diraction == TeleportDiraction.Right){
            newPos = _CurrentActiveBoard.transform.position + new Vector3(60, 0, 0);
            _SpecificBoardList[_OptionArray[1]].SetActive(true);
            _SpecificBoardList[_OptionArray[1]].transform.position = newPos;

        }else{
            newPos = _CurrentActiveBoard.transform.position + new Vector3(60, 0, 60);
        }
    
        // Chọn ngẫu nhiên một ải chưa được kích hoạt từ danh sách 
        _randomInt = Random.Range(0, _BasicBoardList.Count);
        while (_BasicBoardList[_randomInt].activeInHierarchy)
        {
            _randomInt = Random.Range(0, _BasicBoardList.Count);
        }
    
        // Đặt vị trí cho ải được chọn
        _BasicBoardList[_randomInt].SetActive(true);
        _BasicBoardList[_randomInt].transform.position = newPos;
        if(Diraction == TeleportDiraction.Combat){
            _BasicBoardList[_randomInt].GetComponent<BoardController>().PrepareCombat(GameManager.Instance._enemyWaveSetting[0]);
        }
    
        // Cập nhật vị trí và tạo lại NavMesh
        surface.transform.position = newPos;
        surface.GetComponent<NavMeshSurface>().BuildNavMesh();
    
        GameManager.Instance._CurrentProsses ++;
    
        // Gọi hàm để vô hiệu hóa ải trước đó sau 0.5 giây
        Invoke(nameof(DeActivePreviousBoard), 0.5f);
    }

    void OnDestroy()
    {
        //Hủy đăng ký Event
        Observer.RemoveListener(EvenID.BoardPrepare, PrepareBoard);
        Observer.RemoveListener(EvenID.BoardDone, PrepareRandomOptions);
    }
    
    void DeActivePreviousBoard()
    {
        // Đặt lại và vô hiệu hóa ải hiện tại
        _CurrentActiveBoard.GetComponent<BoardController>().ResetBoard();
        _CurrentActiveBoard.SetActive(false);
        _CurrentActiveBoard = _BasicBoardList[_randomInt];
    }
}
