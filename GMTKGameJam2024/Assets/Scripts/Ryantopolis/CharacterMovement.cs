using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using roarke.interaction;

public class CharacterMovement : MonoBehaviour
{
    public bool IsMoving => inputDirection.magnitude > 0f;

    [Header("General")]
    [SerializeField] private Mouth mouth = null;
    [SerializeField] private Animator characterAnimator;
    readonly int walkingProperty = Animator.StringToHash("IsWalking");
    private bool isEncumbered => mouth.IsEncumbered();

    [Header("Rayacsts")]
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastLength = 0.6f;
    [SerializeField] private float sphereCastSize = 0.2f;
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
        if (!MainMenu.Instance.IsPlaying)
            return;

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        inputDirection = new Vector3(horizontal, 0f, vertical);

        if (inputDirection.magnitude > 0)
        {
            
            Move();
            Rotate();

            LimitDistanceFromSurface();
        }
        Animate(inputDirection.magnitude > 0);

        //If no ground is hit then fallback to basic movement and apply gravity
        Ray ray = new Ray(bodyRaycastOrigin.position, bodyRaycastOrigin.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, raycastLength, groundLayer))
            FallbackMovement();
    }

    private void FixedUpdate()
    {
        rbody.velocity = Vector3.zero;
    }

    private void Animate(bool shouldWalk)
    {
        characterAnimator.SetBool(walkingProperty, shouldWalk);
    }
    private void Move()
    {
        transform.localPosition += transform.forward * speed * Time.deltaTime;
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
        Ray ray = new Ray(bodyRaycastOrigin.position, bodyRaycastOrigin.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            float currentDistance = hit.distance;

            // Adjust the character's position to maintain the desired distance
            if (Mathf.Abs(currentDistance - groundDistance) > 0.01f) // A small threshold to avoid jitter
            {
                Vector3 targetPosition = transform.position + transform.up * (groundDistance - currentDistance);
                transform.position = Vector3.Lerp(transform.position, targetPosition, 10f * Time.deltaTime);
            }
        }
    }

    private Vector3 GetAverageNormals()
    {
        Vector3 surfaceNormal = transform.up;
        Vector3 averageNormal = Vector3.zero;
        float averageNormalCount = 0;

        foreach (Transform origin in raycastOrigins)
        {
            if (Physics.SphereCast(origin.position, sphereCastSize, origin.forward, out RaycastHit hit, raycastLength, groundLayer))
            {
#if UNITY_EDITOR
                Debug.DrawRay(origin.position, origin.forward * raycastLength);
#endif
                //Don't add the normals of anything you're carrying
                if (mouth.CurrentlyHeldBody == null || hit.transform != mouth.CurrentlyHeldBody.transform)
                {
                    averageNormal += hit.normal;
                    averageNormalCount++;
                }
            }
        }

        if (averageNormal.magnitude > 0)
            surfaceNormal = averageNormal / averageNormalCount;

        surfaceNormal = surfaceNormal.normalized;
        return surfaceNormal;
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


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (Transform origin in raycastOrigins)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin.position, sphereCastSize);
            Gizmos.DrawWireSphere(origin.position + origin.forward * raycastLength, sphereCastSize);

            // Draw a line between the spheres
            Gizmos.DrawLine(origin.position, origin.position + origin.forward * raycastLength);
        }
    }
#endif
}
