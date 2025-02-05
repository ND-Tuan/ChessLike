using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using GLTF.Schema;
using ObserverPattern;
using UnityEngine;

public enum TeleportDiraction {Right, Left, Combat, Rest, Boss}

public class Teleporter : MonoBehaviour, IInteractable
{
    [SerializeField] private TeleportDiraction teleportDiraction;
    private CinemachineVirtualCamera virtualCamera;
    public string _Message;
  
    public string InteractMessage => _Message;
    private PlayerController playerController;

    

    // Start is called before the first frame update
    void Start()
    {
        virtualCamera =  GameObject.FindGameObjectWithTag("Camera Pivot").GetComponent<CinemachineVirtualCamera>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeAction(InteractionController Interacter)
    {
        PlayerController Player = Interacter.gameObject.GetComponent<PlayerController>();

        Vector3 nextPosition;

        //dịch chuyển người chơi đến ải
        if(teleportDiraction == TeleportDiraction.Left){
            nextPosition = new Vector3(0, 0, 50.5f);
        } 

        else if(teleportDiraction == TeleportDiraction.Right){
            nextPosition = new Vector3(50.5f, 0, 0);
        }

        else{
            nextPosition = new Vector3(47.5f, 0, 47.5f);
        }

        Player.CurrentState = PlayerController.PlayerState.TeleportOut;
        TelePlayer(Player.gameObject, nextPosition);
        
    }

    private async void TelePlayer(GameObject player, Vector3 target){
        await Task.Delay(800);
        
        //chuẩn bị ải sắp tới
        Observer.PostEvent(EvenID.BoardPrepare, teleportDiraction);
        player.transform.position += target;

    }


    // async void  CamZoomIn(){
       
    //     while(virtualCamera.m_Lens.OrthographicSize > 6){
    //         virtualCamera.m_Lens.OrthographicSize -=0.3f;
    //         await Task.Delay(1);
    //     }
    //     virtualCamera.m_Lens.OrthographicSize = 6;
    // }

    // async void  CamZoomOut(){
       
    //     while(virtualCamera.m_Lens.OrthographicSize <= 20){
    //         virtualCamera.m_Lens.OrthographicSize +=0.2f;
    //         await Task.Delay(1);
    //     }

        
    // }


}
