using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using roarke.interaction;

public class CharacterMovement : MonoBehaviour
{
    public bool IsMoving => inputDirection.magnitude > 0f;

    [Header("General")]
    [SerializeField] private Mouth mouth = null;
    [SerializeField] private bool isCarrying = true;

    [Header("Rayacsts")]
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastLength = 0.6f;
    [SerializeField] private Transform bodyRaycastOrigin;
    [SerializeField] private List<Transform> raycastOrigins = new List<Transform>();
    
    [Header("Movement")]
    [SerializeField] private float speed = 2f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 6f;

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

        if (inputDirection.magnitude > 0)
        {
            Move();
            Rotate();

            //print("GetAverageDistance: " + GetAverageDistance());
            //LimitDistanceFromSurface();
        }

        //If no ground is hit then fallback to basic movement and apply gravity
        Ray ray = new Ray(bodyRaycastOrigin.position, bodyRaycastOrigin.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, raycastLength, groundLayer))
            FallbackMovement();
    }

    private void FixedUpdate()
    {
        rbody.velocity = Vector3.zero;
    }

    private void Move()
    {
        Vector3 moveDirection = TryCarry();
        transform.localPosition += moveDirection * speed * Time.deltaTime;
    }

    private void Rotate()
    {
        Vector3 surfaceNormal = GetAverageNormals();

        //Convert movement direction to normal
        Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
        Vector3 rotatedDirection = surfaceRotation * inputDirection;
        Vector3 moveDirection = rotatedDirection.normalized;

        //Roate to normal
        Quaternion toGroundRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, toGroundRotation, rotationSpeed * Time.deltaTime);

        //Rotate to movement direction
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, transform.up).normalized;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);

#if UNITY_EDITOR
        Debug.DrawRay(transform.position, moveDirection, color: Color.blue);
#endif
    }

    private void LimitDistanceFromSurface()
    {
        float currentDistance = GetAverageDistance();

        // A small threshold to avoid jitter
        if (Mathf.Abs(currentDistance - groundDistance) > 0.01f)
        {
            Vector3 targetPosition = transform.localPosition + transform.up * (groundDistance - currentDistance);
            transform.localPosition = targetPosition;
        }
    }

    private Vector3 TryCarry()
    {
        //return mouth.IsCarrying ? transform.forward : -transform.forward;
        return isCarrying ? transform.forward : -transform.forward;
    }

    private Vector3 GetAverageNormals()
    {
        Vector3 surfaceNormal = transform.up;
        Vector3 averageNormal = Vector3.zero;

        foreach (Transform origin in raycastOrigins)
        {
            Ray ray = new Ray(origin.position, origin.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastLength, groundLayer))
            {
#if UNITY_EDITOR
                Debug.DrawRay(origin.position, origin.forward * raycastLength);
#endif
                averageNormal += hit.normal;
            }
        }

        if (averageNormal.magnitude > 0)
            surfaceNormal = averageNormal / raycastOrigins.Count;

        surfaceNormal = surfaceNormal.normalized;
        return surfaceNormal;
    }

    private float GetAverageDistance()
    {
        float surfaceDistance = groundDistance;
        float averageDistance = 0f;

        foreach (Transform origin in raycastOrigins)
        {
            Ray ray = new Ray(origin.position, origin.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastLength, groundLayer))
            {
#if UNITY_EDITOR
                Debug.DrawRay(origin.position, origin.forward * raycastLength);
#endif
                averageDistance += hit.distance;
            }
        }

        if (averageDistance > 0)
            surfaceDistance = averageDistance / raycastOrigins.Count;

        return surfaceDistance;
    }



    #region Fallback Movement
    private float gravity = -10f;
    private void FallbackMovement()
    {
#if UNITY_EDITOR
        Debug.LogWarning("USING FALLBACK MOVEMENT");
#endif
        transform.localPosition += (inputDirection + (Vector3.up * gravity)) * speed * Time.deltaTime;

        Quaternion toGroundRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
        transform.localRotation = toGroundRotation;
        Quaternion targetRotation = Quaternion.LookRotation(inputDirection, transform.up);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    #endregion
}
