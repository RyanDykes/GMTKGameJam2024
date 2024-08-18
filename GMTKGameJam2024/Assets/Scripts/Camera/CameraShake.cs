using DG.Tweening;
using UnityEngine;

namespace roarke.feel
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake instance;
        private void Awake()
        {
            instance = this;
        }
        public void Shake(float duration, float intensity, int vibrato, float randomness)
        {
            transform.DOShakeRotation(duration, intensity, vibrato, randomness);
        }
    }
}

