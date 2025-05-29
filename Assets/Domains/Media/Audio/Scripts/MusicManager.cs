using UnityEngine;

namespace Domains.Audio
{
    public class MusicManager : MonoBehaviour
    {
        private AudioSource musicSource;

        public static MusicManager Instance { get; private set; }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            Instance = this;


            musicSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}