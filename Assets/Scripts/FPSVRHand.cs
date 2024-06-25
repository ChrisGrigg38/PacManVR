using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class FPSVRHand : MonoBehaviour
{
    //basic hand pos/rotation
    [SerializeField] private InputActionProperty _handPosition;
    [SerializeField] private InputActionProperty _hmdPosition;
    [SerializeField] private InputActionProperty _handRotation;
    [SerializeField] private Camera _camera;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Rigidbody _rigidBody;

    //teleporter
    [SerializeField] private bool _useTeleporter;
    [SerializeField] private InputActionProperty _teleporterAxis;
    [SerializeField] private GameObject _teleporterMarker;
    [SerializeField] private float _teleporterAxisButtonActivate = 0.1f;
    [SerializeField] private bool _teleporterFlatSurface;
    [SerializeField] private float _teleporterFlatSurfaceAngle = 5.0f;
    [SerializeField] private float _teleporterMoveSpeed = 6.0f;
    [SerializeField] private float _teleporterMinDistanceComplete = 1.0f;
    [SerializeField] private float _teleporterMarkerYOffset = 1.0f;
    [SerializeField] private float _teleporterMaxDistance = 10f;

    //free movement
    [SerializeField] private bool _useFreeMovement;
    [SerializeField] private InputActionProperty _freeMovementAxis;
    [SerializeField] private float _freeMovementMoveSpeed = 6.0f;
    [SerializeField] private float _freeMovementAxisTrigger = 0.1f;

    //locals
    private bool moveTeleporter;
    private bool moveTeleporterPending;
    private Vector3 moveTeleporterLocation;


    void MoveCharacter(Vector3 dir)
    {
        if (_characterController != null) 
        {
            _characterController.Move(dir);
        }
        if(_rigidBody != null)
        {
            _rigidBody.AddForce(dir, ForceMode.Acceleration);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _handPosition.action.Enable();
        _handPosition.EnableDirectAction();
        _handRotation.action.Enable();
        _handRotation.EnableDirectAction();
        _hmdPosition.action.Enable();

        if(_useTeleporter && _teleporterAxis != null)
        {
            _teleporterAxis.action.Enable();
            _teleporterAxis.EnableDirectAction();
        }

        if(_useFreeMovement && _freeMovementAxis != null)
        {
            _freeMovementAxis.action.Enable();
            _freeMovementAxis.EnableDirectAction();
        }
    }

    void RunFreeMovementLogic()
    {
        Vector2 axisButton = _freeMovementAxis.action.ReadValue<Vector2>();

        if(axisButton.y > _freeMovementAxisTrigger)
        {
            Vector3 moveDir = transform.forward * _freeMovementMoveSpeed * Time.deltaTime;
            Vector3 moveDirY = new Vector3(moveDir.x, 0, moveDir.z);
            MoveCharacter(moveDirY);
        }
        
    }

    void RunTeleporterLogic()
    {
        Vector2 axisButton = _teleporterAxis.action.ReadValue<Vector2>();
        
        if (axisButton.y > _teleporterAxisButtonActivate)
        {
            RaycastHit[] hitInfo = Physics.RaycastAll(transform.position, transform.forward, _teleporterMaxDistance);
            RaycastHit acceptedHit = new RaycastHit();
            float shortestDistance = _teleporterMaxDistance;
            bool foundHit = false;

            foreach(RaycastHit hit in hitInfo)
            {
                if(!hit.collider.isTrigger && hit.distance < shortestDistance)
                {
                    acceptedHit = hit;
                    shortestDistance = hit.distance;
                    foundHit = true;
                }
            }

            if (foundHit && (!_teleporterFlatSurface || Vector3.Angle(acceptedHit.normal, Vector3.up) <= _teleporterFlatSurfaceAngle))
            {
                if (_teleporterMarker != null)
                {
                    _teleporterMarker.SetActive(true);
                    _teleporterMarker.transform.position = new Vector3(acceptedHit.point.x, acceptedHit.point.y + _teleporterMarkerYOffset, acceptedHit.point.z);
                    _teleporterMarker.transform.rotation = Quaternion.identity;
                }

                moveTeleporter = false;
                moveTeleporterPending = true;
                moveTeleporterLocation = acceptedHit.point;
            }
            else 
            {
                moveTeleporter = false;
                moveTeleporterPending = false;

                if (_teleporterMarker != null)
                {
                    _teleporterMarker.SetActive(false);
                }
            } 
        }
        else if(moveTeleporterPending && !moveTeleporter)
        {
            moveTeleporterPending = false;
            moveTeleporter = true;
        }
            
        if (moveTeleporter)
        {
            Vector3 diffToTele = moveTeleporterLocation - _characterController.transform.position;
            bool completeThisFrame = false;
            diffToTele.y = 0;

            float disThisFrame = diffToTele.magnitude;
            diffToTele.Normalize();
            Vector3 turnSpeed = diffToTele * _teleporterMoveSpeed * Time.deltaTime;
            CollisionFlags collision = CollisionFlags.None;

            if (turnSpeed.magnitude >= disThisFrame)
            {
                MoveCharacter(turnSpeed);
                moveTeleporter = false;
            }
            else
            {
                MoveCharacter(turnSpeed);
            }

            Transform trans = _characterController != null ? _characterController.transform : _rigidBody.transform;

            if (Vector3.Distance(trans.position, moveTeleporterLocation) < _teleporterMinDistanceComplete)
            {
               moveTeleporter = false;
            }
        }

        if (_teleporterMarker != null && !moveTeleporterPending)
        {
            _teleporterMarker.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 handpos = _handPosition.action.ReadValue<Vector3>();
        Vector3 headpos = _hmdPosition.action.ReadValue<Vector3>();
        Quaternion handrot = _handRotation.action.ReadValue<Quaternion>();

        Vector3 handCamDiff = handpos - headpos;

        transform.position = _camera.transform.position + handCamDiff;
        transform.rotation = handrot;

        if (_useTeleporter && _teleporterAxis != null)
        {
            RunTeleporterLogic();
        }

        if (_useFreeMovement && _freeMovementAxis != null)
        {
            RunFreeMovementLogic();
        }
    }
}
