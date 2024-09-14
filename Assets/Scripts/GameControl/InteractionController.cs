using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private GameObject _interactionUI;
    [SerializeField] private TextMeshProUGUI _interactMessage;
    [SerializeField] private float _interactionDistance = 2f; // Khoảng cách tương tác tối đa
    [SerializeField] private LayerMask _interactionLayer; // LayerMask để thu nhỏ đối tượng tìm kiếm
    private bool _isInteracting = false;
    [SerializeField] private Collider[] _hitColliders = new Collider[3]; // Mảng chứa các collider trong phạm vi tương tác
    private bool _playerNearObject = false;
    private  int _numColliders;
    [SerializeField] private Collider _nearestCollider;
    private float _nearestDistance ;
    [SerializeField] private IInteractable _interactable;

    void Update()
    {
        _numColliders = Physics.OverlapSphereNonAlloc(transform.position, _interactionDistance, _hitColliders, _interactionLayer);

        _nearestDistance = Mathf.Infinity;

        // Tìm collider gần nhất
        for (int i = 0; i < _numColliders; i++)
        {
            float distance = Vector3.Distance(transform.position, _hitColliders[i].transform.position);
            if (distance < _nearestDistance)
            {
                _nearestDistance = distance;
                _nearestCollider = _hitColliders[i];
            }
        }

        //
        if( _nearestCollider != null){
            if( Vector3.Distance(transform.position, _nearestCollider.transform.position) <= _interactionDistance){
                _playerNearObject = true;

                _interactionUI.SetActive(true);
                _interactionUI.transform.position = transform.position;
            } else {
                _playerNearObject = false;

                _interactionUI.SetActive(false);
            }
        }

        // Bắt đầu hoặc kết thúc tương tác dựa trên khoảng cách
        if (_playerNearObject && !_isInteracting)
        {
            StartInteraction();
        }
        else if (!_playerNearObject && _isInteracting)
        {
            EndInteraction();
        }
    }

    void StartInteraction()
    {
        
        _interactable = _nearestCollider.GetComponent<IInteractable>();
        

        if(_interactable == null) return;
        _interactMessage.text = _interactable.InteractMessage;
        if(Input.GetKeyDown(KeyCode.F)){
            _interactable.TakeAction();
            _isInteracting = true;
        }

        
        
    }

    void EndInteraction()
    {
        _isInteracting = false;
        // Thực hiện hành động khi kết thúc tương tác, ví dụ: ẩn thông báo
        Debug.Log("Player is no longer near the object. Interaction ended.");
    }
}
