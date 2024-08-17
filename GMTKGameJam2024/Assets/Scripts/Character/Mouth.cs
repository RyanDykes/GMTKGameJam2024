using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace roarke
{
    public class Mouth : MonoBehaviour
    {
        [SerializeField] LayerMask carryLayer;
        
        [SerializeField] Transform holdParent;
        [SerializeField] float movePower = 25;

        List<Rigidbody> overlappingRigidbodies = new List<Rigidbody>(); //All possible rigidbodies current overlapping the mouth trigger collider
        public Rigidbody CurrentlyHeldBody { get; private set; } //Currently held rigidbody (can be null)
        public bool Holding => CurrentlyHeldBody != null;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Holding)
                {
                    Drop();
                }
                else
                {
                    PickUp();
                }
            }
            if (Holding)
            {
                MoveHeldBody();
            }
        }

        //Pick up the closest overlapping rigidbody
        void PickUp()
        {
            var grabBody = overlappingRigidbodies
                                    .OrderBy(rb => Vector3.Distance(rb.transform.position, transform.position))
                                    .FirstOrDefault();
            if (grabBody != null)
            {
                grabBody.drag = 10;
                grabBody.useGravity = false;
                grabBody.transform.parent = holdParent;
                CurrentlyHeldBody = grabBody;
            }
        }
        //Release the currently held rigidbody
        void Drop()
        {
            CurrentlyHeldBody.transform?.SetParent(null);
            CurrentlyHeldBody.drag = 0;
            CurrentlyHeldBody.useGravity = true;
            CurrentlyHeldBody = null;
        }


        //Keep the currently held rigidbody near to the mouth
        void MoveHeldBody()
        {
            if (Vector3.Distance(CurrentlyHeldBody.transform.position, transform.position) > 0.5f)
            {
                Vector3 moveDir = holdParent.position - CurrentlyHeldBody.transform.position;
                CurrentlyHeldBody.AddForce(moveDir*movePower);
            }
        }

        private void OnTriggerEnter(Collider col)
        {
            if (!col.transform.TryGetComponent<Rigidbody>(out var rb)
                || !(carryLayer == (carryLayer | (1 << col.gameObject.layer))))
            {
                //Don't detect the overlapping body if it is not a rigidbody or does not exist in the Carryable layer
                return;
            }

            if (!overlappingRigidbodies.Contains(rb))
                overlappingRigidbodies.Add(rb);
        }
        private void OnTriggerExit(Collider col)
        {
            if (!col.transform.TryGetComponent<Rigidbody>(out var rb) 
                || !carryLayer.value.Equals(col.gameObject.layer))
            {
                //Don't detect the overlapping body if it is not a rigidbody or does not exist in the Carryable layer
                return;
            }

            if (overlappingRigidbodies.Contains(rb))
                overlappingRigidbodies.Remove(rb);
        }
    }
}
