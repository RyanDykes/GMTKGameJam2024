using Cinemachine;
using DG.Tweening;
using UnityEngine;
using settings;
namespace roarke.feel
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake instance;
        CinemachineVirtualCamera virtualCamera;
        Transform target;
        private void Awake()
        {
            instance = this;
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            target = virtualCamera.Follow;
        }
        public void Shake(float duration, float intensity, int vibrato, float randomness, Ease ease = Ease.Linear)
        {
            if (!Settings.ShouldUseCameraShake)
                return;

            virtualCamera.Follow = null;
            virtualCamera
                .transform
                .DOShakePosition(duration, intensity, vibrato, randomness)
                .SetEase(ease)
                .OnComplete(() => virtualCamera.Follow = target);
        }

    }
}

