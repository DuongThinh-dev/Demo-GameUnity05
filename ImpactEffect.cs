using System.Collections;
using UnityEngine;

public enum TypeEffect { ImpactEffect, ImpactExplosiveEffect, none }

public class ImpactEffect : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip clipImpact;
    public AudioClip clipExplosive;

    [Header("Effect Settings")]
    public TypeEffect typeEffect;
    public float disableDelay = 2f;

    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    public void Init(TypeEffect effectType)
    {
        typeEffect = effectType;
        PlaySound();
        StartCoroutine(DisableAfterTime(disableDelay));
    }

    void PlaySound()
    {
        AudioClip clipToPlay = null;

        switch (typeEffect)
        {
            case TypeEffect.ImpactEffect:
                clipToPlay = clipImpact;
                break;
            case TypeEffect.ImpactExplosiveEffect:
                clipToPlay = clipExplosive;
                break;
        }

        if (clipToPlay != null)
            PlayOneShotAtPosition(clipToPlay, transform.position, sfxVolume);
    }

    void PlayOneShotAtPosition(AudioClip clip, Vector3 position, float volume)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.spatialBlend = 0f; // 2D sound
        aSource.volume = volume;
        aSource.Play();

        Destroy(tempGO, clip.length);
    }

    IEnumerator DisableAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
