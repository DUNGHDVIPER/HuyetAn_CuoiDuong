using UnityEngine;

public class PlayerCombatSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip swingClip;
    public AudioClip hitClip;

    void Awake()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    public void PlaySwing()
    {
        if (swingClip) audioSource.PlayOneShot(swingClip, 0.9f);
    }

    public void PlayHit()
    {
        if (hitClip) audioSource.PlayOneShot(hitClip, 1.0f);
    }
}
