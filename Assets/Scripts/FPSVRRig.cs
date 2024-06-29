using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSVRRig : MonoBehaviour
{

    [SerializeField] private InputActionProperty _hmdPosition;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Camera _camera;
    [SerializeField] private bool updateCameraYPos = true;
    [SerializeField] private float updateCameraYPosHeightOffset = 0.0f;
    [SerializeField] private float playerSpeed = 1.0f;
    [SerializeField] private VRFPSHealthCharacter healthComponent;
    [SerializeField] private bool preventMovementWhenDead = true;

    private Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        _hmdPosition.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = _hmdPosition.action.ReadValue<Vector3>();

        if(updateCameraYPos)
        {
            _camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, pos.y + updateCameraYPosHeightOffset, _camera.transform.localPosition.z);
        }

        if(lastPos.x != pos.x || lastPos.z != pos.z)
        {
            Vector3 moveDir = new Vector3((pos.x - lastPos.x) * playerSpeed, 0, (pos.z - lastPos.z) * playerSpeed);

            if (!preventMovementWhenDead || (healthComponent != null && healthComponent.health > 0))
            {
                if (_characterController != null)
                {
                    if (_characterController.isGrounded)
                    {
                        _characterController.Move(moveDir);
                    }
                    else
                    {
                        _characterController.SimpleMove(moveDir);
                    }
                }
                if (_rigidBody != null)
                {
                    _rigidBody.MovePosition(transform.position + moveDir);
                }
            }
        }
        
        lastPos = pos;

        
    }
}
