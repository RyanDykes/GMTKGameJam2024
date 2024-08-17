using Cinemachine;
using UnityEngine;
namespace roarke
{
    public class CameraInitialize : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera virtualCamera;

        private void Start()
        {
            var player = GameObject.FindWithTag("Player");
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }
    }
}

