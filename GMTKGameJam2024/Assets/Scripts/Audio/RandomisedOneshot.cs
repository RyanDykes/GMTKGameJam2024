using UnityEngine;
namespace roarke.audio
{
    public class RandomisedOneshot : MonoBehaviour
    {
        [SerializeField] bool shouldPlayOnStart;
        [SerializeField] bool shouldRandomizeVolume;
        [SerializeField] float volumeMin, volumeMax;
        [SerializeField] bool shouldRandomizePitch;
        [SerializeField] float minPitch, maxPitch;

        AudioSource source;
        private void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (shouldRandomizePitch)
            {
                source.pitch = Random.Range(minPitch, maxPitch);
            }
            if (shouldRandomizeVolume)
            {
                source.volume = Random.Range(volumeMin, volumeMax);
            }
            if (shouldPlayOnStart)
            {
                source.Play();
            }
        }

    }
}

