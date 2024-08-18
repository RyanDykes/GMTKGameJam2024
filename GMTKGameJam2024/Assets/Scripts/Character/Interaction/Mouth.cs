using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace roarke.interaction
{
    public class Mouth : MonoBehaviour
    {
        [SerializeField] LayerMask carryLayer;
        
        [SerializeField] Transform holdParent;
        [SerializeField] float movePower = 25;

        List<IInteractable> overlappingInteractables = new List<IInteractable>(); //All possible rigidbodies current overlapping the mouth trigger collider
        public Carryable CurrentlyHeldBody { get; private set; } //Currently held rigidbody (can be null)
        public bool Holding => CurrentlyHeldBody != null;
        public int Strength { get; private set; } = 3;
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
                    Interact();
                }
            }
            if (Holding)
            {
                MoveHeldBody();
            }
        }

        public bool IsEncumbered()
        {
            return CurrentlyHeldBody != null && CurrentlyHeldBody.Weight + 2 >= Strength;
        }
        //Pick up the closest overlapping rigidbody
        void Interact()
        {
            IInteractable closestInteractable = overlappingInteractables
                                    .OrderBy(i => Vector3.Distance(i.Position, transform.position))
                                    .FirstOrDefault();
            if (closestInteractable != null)
            {
                if (closestInteractable is Carryable carryable && carryable.Weight <= Strength)
                {
                    //if strong enough, try to eat immediately. So we can eat cumbs without having to take them home if we are strong enough
                    if ((Strength >= carryable.Weight + 2) && carryable.TryGetComponent<Eatable>(out var eatable))
                    {
                        if (eatable.Interact(holdParent))
                        {
                            Destroy(eatable.gameObject);
                            Strength++;
                            overlappingInteractables.Remove(closestInteractable);
                        }
                        else
                        {
                            CurrentlyHeldBody = carryable;
                            closestInteractable.Interact(holdParent);
                        }
                    }  
                    else
                    {
                        CurrentlyHeldBody = carryable;
                        closestInteractable.Interact(holdParent);
                    }
                }
            }
        }
        //Release the currently held rigidbody
        void Drop()
        {
            if (CurrentlyHeldBody.Drop())
            { 
                CurrentlyHeldBody = null;
            }
        }


        //Keep the currently held rigidbody near to the mouth
        void MoveHeldBody()
        {
            if (Vector3.Distance(CurrentlyHeldBody.transform.position, transform.position) > 0.5f)
            {
                Vector3 moveDir = holdParent.position - CurrentlyHeldBody.transform.position;
                CurrentlyHeldBody.RB.AddForce(moveDir*movePower);
            }
        }

        private void OnTriggerEnter(Collider col)
        {
            if (!col.transform.TryGetComponent<IInteractable>(out var interactable)
                || !(carryLayer == (carryLayer | (1 << col.gameObject.layer))))
            {
                //Don't detect the overlapping body if it is not a rigidbody or does not exist in the Carryable layer
                return;
            }

            if (!overlappingInteractables.Contains(interactable))
                overlappingInteractables.Add(interactable);
        }
        private void OnTriggerExit(Collider col)
        {
            if (!col.transform.TryGetComponent<IInteractable>(out var interactable) 
                || !(carryLayer == (carryLayer | (1 << col.gameObject.layer))))
            {
                //Don't detect the overlapping body if it is not a rigidbody or does not exist in the Carryable layer
                return;
            }
            if (overlappingInteractables.Contains(interactable))
                overlappingInteractables.Remove(interactable);
        }
    }
}
