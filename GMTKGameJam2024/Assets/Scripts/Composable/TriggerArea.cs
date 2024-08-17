using UnityEngine;
using UnityEngine.Events;

namespace roarke
{
    public class TriggerArea : MonoBehaviour
    {
        [SerializeField] UnityEvent<Collider> onTrigger;
        [SerializeField] UnityEvent<Collider> onExit;
        private void OnTriggerEnter(Collider other)
        {
            onTrigger?.Invoke(other);
        }
        private void OnTriggerExit(Collider other)
        {
            onExit?.Invoke(other);
        }
    }
}

