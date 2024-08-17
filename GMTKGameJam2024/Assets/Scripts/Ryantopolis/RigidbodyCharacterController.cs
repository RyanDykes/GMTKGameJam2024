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

    private Rigidbody rbody = null;
    private Vector2 moveDirection = Vector2.zero;
    private const float axisInputRange = 0.2f;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");
        moveDirection.Normalize();
        moveDirection *= speed;

        Ray ray = new Ray(groundRaycastOrigin.position, -transform.up);
        RaycastHit hit;
        Debug.DrawRay(groundRaycastOrigin.position, -transform.up * 2f);

        if (Physics.Raycast(ray, out hit, raycastLength, groundLayer))
        {
            Vector3 groundNormal = hit.normal;
            Debug.DrawRay(hit.point, groundNormal * raycastLength, color: Color.green);

            Quaternion toGroundRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            transform.rotation = toGroundRotation;

            //Cross
            //moveDirection.y *= Vector3.Cross(transform.right, groundNormal);
            //moveDirection.x *= Vector3.Cross(transform.right, groundNormal).normalized;
        }
    }

    private float maxSpeed = 2f;
    private void FixedUpdate()
    {
        if (moveDirection.magnitude >= axisInputRange)
            rbody.AddForce(new Vector3(moveDirection.x, 0f, moveDirection.y), ForceMode.VelocityChange);
        else
            rbody.velocity = new Vector3(0f, rbody.velocity.y, 0f);

        rbody.AddForce(-transform.up * gravity, ForceMode.Force);

        float xClamped = Mathf.Clamp(rbody.velocity.x, -maxSpeed, maxSpeed);
        float yClamped = Mathf.Clamp(rbody.velocity.y, -40f, 40f);
        float zClamped = Mathf.Clamp(rbody.velocity.z, -maxSpeed, maxSpeed);
        rbody.velocity = new Vector3(xClamped, yClamped, zClamped);
    }

    private void LateUpdate()
    {
        if (moveDirection.magnitude > 0)
        {
            Vector3 lookDirection = new Vector3(moveDirection.x, transform.position.y, moveDirection.y);
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
