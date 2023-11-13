using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVoices : MonoBehaviour
{
    private AudioSource ac;

    [SerializeField] private bool useVoice = false;
    [SerializeField] private AudioClip[] lookVoices;
    [SerializeField] private AudioClip[] suspicionVoices;
    [SerializeField] private AudioClip[] spottedVoices;

    private float minTimeBetweenVoices = 4f;
    private float maxTimeBetweenVoices = 8.5f;


    private bool voiceEnabled = true;

    public enum VoiceLevels {None, Look, Suspicion, Spotted };
    private VoiceLevels currentVoiceLevel = VoiceLevels.None;

    private void Start()
    {
        ac = GetComponent<AudioSource>();
    }



  

    public void PlayLookVoice()
    {
        if (!useVoice) { return; }

        ac.Stop();
        ac.clip = lookVoices[Random.Range(0, lookVoices.Length - 1)];
        ac.Play();
    }

    public void PlaySuspicionVoice()
    {
        if (!useVoice) { return; }

        ac.Stop();
        ac.clip = suspicionVoices[Random.Range(0, suspicionVoices.Length - 1)];
        ac.Play();
    }

    public void PlaySpottedVoice()
    {
        if (!useVoice) { return; }

        ac.Stop();
        ac.clip = spottedVoices[Random.Range(0, spottedVoices.Length - 1)];
        ac.Play();
    }

    

}
