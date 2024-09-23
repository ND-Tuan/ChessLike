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

    

    // Start is called before the first frame update
    void Start()
    {
        virtualCamera =  GameObject.FindGameObjectWithTag("Camera Pivot").GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeAction ()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");

        //chuẩn bị ải sắp tới
        Observer.PostEvent(EvenID.BoardPrepare, teleportDiraction);
        
        //dịch chuyển người chơi đến ải
        if(teleportDiraction == TeleportDiraction.Left){
            Player.transform.position += new Vector3(0, 0, 50.5f);
            return;
        }

        if(teleportDiraction == TeleportDiraction.Right){
            Player.transform.position += new Vector3(50.5f, 0, 0);
            return;
        }

        Player.transform.position += new Vector3(47.5f, 0, 47.5f);
        
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
