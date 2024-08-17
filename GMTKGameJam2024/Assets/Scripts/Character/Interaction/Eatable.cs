using UnityEngine;
namespace roarke.interaction
{
    public class Eatable : MonoBehaviour, IInteractable
    {
        [SerializeField] ParticleSystem eatEffect;
        public Vector3 Position => transform.position;
        public bool Interact(Transform interactionParent)
        {
            if (eatEffect != null)
            {
                Instantiate(eatEffect, interactionParent.position, Quaternion.identity);
            }
            else
            {
                Debug.Log($"NO EAT EFFECT FOR {transform.name}");
            }
            return true;
        }
    }

}
