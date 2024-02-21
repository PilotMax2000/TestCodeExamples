using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IntoTheGrid.Music
{
    [Serializable]
    public class SfxHandler
    {
        [Header("Randomize Sound")]
        [SerializeField] [Range(0.1f,0.5f)] private float _pitchMultiplier = 0.2f;
        [SerializeField] [Range(0.1f,0.5f)] private float _volumeMultiplier = 0.2f;

        [Header("Pairs")]
        [SerializeField] private List<SfxAndEventPair> _sfxAndEventPairs;
        
        private Dictionary<SfxEventType, SfxAndEventPair> _sfxEventsCache;
        private AudioSource _audioSource;
        
        public void Initialize(AudioSource audioSource)
        {
            _audioSource = audioSource;
            InitializeSfxEvents();
        }

        public void PlaySfx(SfxEventType sfxTypeToSearch, bool randomizeSound, bool debugForceToNotUseCache = false)
        {
            if (TryToFoundSfxPair(sfxTypeToSearch, debugForceToNotUseCache, out SfxAndEventPair foundPair) == false) 
                return;
            
            if (foundPair.RewriteUsingSO)
                PlayUsingSOInstructions(foundPair);
            else
                PlayUsingGeneralRandomizer(randomizeSound, foundPair);
        }

        private void InitializeSfxEvents()
        {
            _sfxEventsCache = new Dictionary<SfxEventType, SfxAndEventPair>();
            _sfxAndEventPairs.ForEach(pair => _sfxEventsCache.Add(pair.sfxEventType, pair));
        }

        private bool TryToFoundSfxPair(SfxEventType sfxTypeToSearch, bool debugForceToNotUseCache, out SfxAndEventPair foundPair)
        {
            bool pairWasFound;
            
            if (debugForceToNotUseCache)
            {
                pairWasFound = TryGetPair(sfxTypeToSearch, out foundPair);
                if (pairWasFound) 
                    return true;
                
                Debug.LogError($"Pair was not found for '{sfxTypeToSearch}' without using cache");
                return false;
            }

            pairWasFound = _sfxEventsCache.TryGetValue(sfxTypeToSearch, out foundPair);
            if (pairWasFound)
                return true;
            
            Debug.LogError($"Pair was not found for '{sfxTypeToSearch}' in cache");
            return false;
        }

        private void PlayUsingGeneralRandomizer(bool randomizeSound, SfxAndEventPair foundPair)
        {
            _audioSource.pitch = randomizeSound ? Random.Range(1f - _pitchMultiplier, 1f + _pitchMultiplier) : 1f;
            _audioSource.volume = randomizeSound ? Random.Range(1f - _volumeMultiplier, 1f + _volumeMultiplier) : 1f;

            _audioSource.PlayOneShot(foundPair.AudioClip);
        }

        private void PlayUsingSOInstructions(SfxAndEventPair foundPair)
        {
            var sfxData = foundPair.SfxData;
            _audioSource.pitch = sfxData.RandomizePitch ? RandomizedPitch(sfxData) : 1f;
            _audioSource.volume = sfxData.RandomizeVolume ? RandomizedVolume(sfxData) : sfxData.Volume;
            _audioSource.PlayOneShot(sfxData.AudioClip);
            

            float RandomizedPitch(SfxData sfxData1) =>
                Random.Range(1f - sfxData1.PitchMultiplier, 1f + sfxData1.PitchMultiplier);

            float RandomizedVolume(SfxData sfxData2) => 
                Random.Range(1f - sfxData2.VolumeMultiplier, 1f + sfxData2.VolumeMultiplier);
        }

        private bool TryGetPair(SfxEventType sfxEventType, out SfxAndEventPair foundPair)
        {
            foreach (SfxAndEventPair pair in _sfxAndEventPairs)
            {
                if (pair.sfxEventType != sfxEventType) 
                    continue;
                
                foundPair = pair;
                return true;
            }

            foundPair = null;
            return false;
        }
    }

    [Serializable]
    public class SfxAndEventPair
    {
        public SfxEventType sfxEventType;
        public AudioClip AudioClip;
        public bool RewriteUsingSO = false;
        public SfxData SfxData;
    }
}