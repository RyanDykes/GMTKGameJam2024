using UnityEngine;
namespace roarke
{
    public class BasicCharacterMovement : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] float rotationSpeed;
        Rigidbody rb;
        Vector3 moveDirection = new Vector3();
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            moveDirection.z = Input.GetAxis("Vertical");
            moveDirection.x = Input.GetAxis("Horizontal");
            rb.MovePosition(rb.position+moveDirection.normalized * speed * Time.deltaTime);
            rb.transform.LookAt(rb.position + moveDirection.normalized * rotationSpeed*Time.deltaTime, Vector3.up);
        }
    }
}

