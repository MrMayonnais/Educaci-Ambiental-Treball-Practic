using System;
using UnityEngine;

namespace Dlcs
{
    public class DialoguePlayer : MonoBehaviour
    {
    
        private AudioSource _currentSource;

        public void PlayClip(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("AudioClip is null");
                return;
            }

            StopCurrentClip();

            _currentSource = gameObject.AddComponent<AudioSource>();
            _currentSource.clip = clip;
            _currentSource.Play();
        }

        public void StopCurrentClip()
        {
            if (_currentSource != null)
            {
                _currentSource.Stop();
                Destroy(_currentSource);
                _currentSource = null;
            }
        }


    }
}