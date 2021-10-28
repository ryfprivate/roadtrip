using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public class Effect : PooledObject
    {
        private ParticleSystem system;
        private void Awake()
        {
            system = GetComponent<ParticleSystem>();
        }
        public void Play() => system.Play();
        public void Stop() => system.Stop();
        public virtual void PlayOnce()
        {
            Play();
            StartCoroutine(GoHome());
        }
        private IEnumerator GoHome()
        {
            while (system.isPlaying)
            {
                yield return null;
            }
            if (OriginalPool != null) OriginalPool.Return(this);
        }
    }
}