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
    [SerializeField] private float raycastLength = 0.6f;
    [SerializeField] private float groundDistance = 0.2f;

    [Header("Movement")]
    [SerializeField] private float speed = 2f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 6f;
    [SerializeField] private AnimationCurve additionalAngleCurve = null;

    private Camera mainCam = null;
    private Rigidbody rbody = null;
    private Vector3 inputDirection = Vector3.zero;
    private float horizontal = 0f;
    private float vertical = 0f;

    private void Awake()
    {
        mainCam = Camera.main;
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
            MoveAlongSurface(hit.point, hit.normal);
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

    private void MoveAlongSurface(Vector3 surfacePoint, Vector3 surfaceNormal)
    {
        if (inputDirection.magnitude > 0)
        {
            Move();
            Rotate(surfaceNormal);

            LimitDistanceFromSurface();
        }
    }

    private void Move()
    {
        Vector3 moveDirection = TryCarry();
        transform.localPosition += moveDirection * speed * Time.deltaTime;
    }

    private void Rotate(Vector3 surfaceNormal)
    {
        Quaternion toGroundRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, toGroundRotation, rotationSpeed * Time.deltaTime);

        Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
        Vector3 rotatedDirection = surfaceRotation * inputDirection;
        Vector3 moveDirection = rotatedDirection.normalized;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, transform.up).normalized;
        Debug.DrawRay(groundRaycastOrigin.position, moveDirection, color: Color.blue);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void LimitDistanceFromSurface()
    {
        Ray ray = new Ray(groundRaycastOrigin.position, -transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastLength, groundLayer))
        {
            float currentDistance = hit.distance;

            // A small threshold to avoid jitter
            if (Mathf.Abs(currentDistance - groundDistance) > 0.01f)
            {
                Vector3 targetPosition = transform.localPosition + transform.up * (groundDistance - currentDistance);
                transform.localPosition = targetPosition;
            }
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
