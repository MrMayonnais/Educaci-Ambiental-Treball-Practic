using System;
using UnityEngine;

namespace Dlcs
{
    public class AudioManager : MonoBehaviour
    {
        public static event Action OnClipFinished;
    
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

            Invoke(nameof(NotifyClipFinished), clip.length);
        }

        public void StopCurrentClip()
        {
            if (_currentSource != null)
            {
                CancelInvoke(nameof(NotifyClipFinished));
                _currentSource.Stop();
                Destroy(_currentSource);
                _currentSource = null;
            }
        }

        private void NotifyClipFinished()
        {
            OnClipFinished?.Invoke();
        
            if (_currentSource != null)
            {
                Destroy(_currentSource);
                _currentSource = null;
            }
        }
    }
}