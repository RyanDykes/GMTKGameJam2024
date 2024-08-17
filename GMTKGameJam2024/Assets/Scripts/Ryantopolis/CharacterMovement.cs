using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using roarke;

public class CharacterMovement : MonoBehaviour
{
    public bool IsMoving => inputDirection.magnitude > 0f;

    [Header("General")]
    [SerializeField] private Mouth mouth = null;
    [SerializeField] private bool isCarrying = true;

    [Header("Rayacsts")]
    [SerializeField] private Transform groundRaycastOrigin;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastLength = 0.8f;

    [Header("Movement")]
    [SerializeField] private float speed = 10f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 0.15f;

    private Rigidbody rbody = null;
    private Vector3 inputDirection = Vector3.zero;
    private float horizontal = 0f;
    private float vertical = 0f;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        inputDirection = new Vector3(horizontal, 0f, vertical);
        

        Ray ray = new Ray(groundRaycastOrigin.position, -transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastLength, groundLayer))
        {
            Debug.DrawRay(hit.point, hit.normal * raycastLength);
            
            MoveAlongSurface(hit.normal);
        }
        else
        {
            FallbackMovement();
        }
    }

    private void FixedUpdate()
    {
        rbody.velocity = Vector3.zero;
    }

    private void MoveAlongSurface(Vector3 surfaceNormal)
    {
        Move();
        Rotate(surfaceNormal);
    }

    private void Move()
    {
        if (inputDirection.magnitude > 0)
        {
            Vector3 moveDirection = TryCarry();
            transform.localPosition += moveDirection * speed * Time.deltaTime;
        }
    }

    private void Rotate(Vector3 surfaceNormal)
    {
        Quaternion toGroundRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
        transform.localRotation = toGroundRotation;

        if (inputDirection.magnitude > 0)
        {
            //Vector3 localMoveDirection = transform.InverseTransformDirection(inputDirection);
            //inputDirection = Vector3.ProjectOnPlane(localMoveDirection, transform.up);
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection, transform.up);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private Vector3 TryCarry()
    {
        //return mouth.IsCarrying ? transform.forward : -transform.forward;
        return isCarrying ? transform.forward : -transform.forward;
    }

    #region Fallback Movement
    private float gravity = -10f;
    private void FallbackMovement()
    {
        transform.localPosition += (inputDirection + (Vector3.up * gravity)) * speed * Time.deltaTime;

        Quaternion toGroundRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
        transform.localRotation = toGroundRotation;
        Quaternion targetRotation = Quaternion.LookRotation(inputDirection, transform.up);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    #endregion
}
