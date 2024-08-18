using System.Collections.Generic;
using UnityEngine;
namespace roarke.interaction
{
    public class Eatable : MonoBehaviour, IInteractable
    {
        [SerializeField] List<GameObject> eatEffects;
        [SerializeField] private SceneController.EatingFoods foodType = SceneController.EatingFoods.None;

        public SceneController.EatingFoods FoodType => foodType;
        public Vector3 Position => transform.position;
        public bool Interact(Transform interactionParent)
        {
            if (eatEffects.Count > 0)
            {
                foreach (var effect in eatEffects)
                { 
                    Instantiate(effect, interactionParent.position, Quaternion.identity);
                }
            }
            else
            {
                Debug.Log($"NO EAT EFFECT FOR {transform.name}");
            }
            return true;
        }
    }

}
