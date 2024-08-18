using UnityEngine;
namespace roarke.feel
{
    public class Randomizer : MonoBehaviour
    {
        [SerializeField] bool shouldRadnomizeScale;
        [SerializeField] float min, max;
        [SerializeField] bool shouldRandomizeRotation;
        private void Start()
        {
            if(shouldRadnomizeScale)
                transform.localScale *= Random.Range(min, max);
            if (shouldRandomizeRotation)
                transform.rotation = Random.rotation;

        }
    }
}

