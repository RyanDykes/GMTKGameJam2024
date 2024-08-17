using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyCharacterController : MonoBehaviour
{
    public bool IsMoving => moveDirection.magnitude > 0f;

    [Header("General")]
    [SerializeField] private Transform groundRaycastOrigin;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastLength = 0.8f;

    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float gravity = 0.15f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 0.15f;

    private Camera mainCam = null;
    private Rigidbody rbody = null;
    private Vector3 moveDirection = Vector2.zero;
    private float horizontal = 0f;
    private float vertical = 0f;
    private const float axisInputRange = 0.2f;

    private void Awake()
    {
        mainCam = Camera.main;
        rbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(horizontal, 0f, vertical);

        Ray ray = new Ray(groundRaycastOrigin.position, -transform.up);
        RaycastHit hit;
        Debug.DrawRay(groundRaycastOrigin.position, -transform.up * 2f);

        if (Physics.Raycast(ray, out hit, raycastLength, groundLayer))
        {
            Vector3 groundNormal = hit.normal;
            Debug.DrawRay(hit.point, groundNormal * raycastLength, color: Color.green);

            Quaternion toGroundRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            transform.rotation = toGroundRotation;

            transform.localPosition += transform.forward * moveDirection.magnitude * speed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, transform.up);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 previousDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
            transform.position += previousDirection * speed * Time.deltaTime;
        }
    }
}
