using DG.Tweening;
using UnityEngine;
namespace roarke.interaction
{
    public enum CarryType
    { 
        Food_small,
        Food_medium,
        Food_large,
        Trash,
        Special,
    }
    public class Carryable : MonoBehaviour, IInteractable
    {
        [SerializeField] CarryType type = CarryType.Trash;
        [SerializeField] int weight = 1;
        public CarryType Type => type;
        public int Weight => weight;
        public Vector3 Position => transform.position;
        public Rigidbody RB { get; private set; }
        float originalDrag;
        void Awake()
        {
            RB = GetComponent<Rigidbody>();
            originalDrag = RB.drag;
        }

        public bool Drop()
        {
            RB.drag = originalDrag;
            RB.useGravity = true;
            RB.transform.parent = null;
            return true;
        }
        public bool Interact(Transform interactionParent)
        {
            RB.drag = 10;
            RB.useGravity = false;
            RB.transform.parent = interactionParent;
            transform.DOPunchScale(new Vector3(-.2f, -.2f, -.2f), .33f, 5, 5).SetEase(Ease.InOutExpo);
            transform.DOShakeRotation(.33f, 25, 25, 90).SetEase(Ease.InOutExpo);
            return true;
        }
    }
}

