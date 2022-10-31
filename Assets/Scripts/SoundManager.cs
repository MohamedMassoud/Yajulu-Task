using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SoundManager : MonoBehaviour
{


    [SerializeField] private AudioClip gravityFlipSound;
    [SerializeField] private AudioClip electriclShockClip;
    [SerializeField] private AudioSource soundEffectAudioSource;


    public enum SoundEffect
    {
        GravityFlip,
        ElectricShock
    }



    public void PlaySoundEffect(SoundEffect soundEffect)
    {
        switch (soundEffect)
        {
            case SoundEffect.GravityFlip:
                soundEffectAudioSource.PlayOneShot(gravityFlipSound);
                break;

            case SoundEffect.ElectricShock:
                soundEffectAudioSource.PlayOneShot(electriclShockClip);
                break;
        }
    }
}
