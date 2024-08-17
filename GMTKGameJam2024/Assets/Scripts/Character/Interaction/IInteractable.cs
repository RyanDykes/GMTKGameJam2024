using UnityEngine;
namespace roarke.interaction
{
    public interface IInteractable
    {
        public Vector3 Position { get; }
        bool Interact(Transform interactionParent);
    }
}

