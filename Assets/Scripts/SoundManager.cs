using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public AudioClip bgmClips;
    public AudioClip successClip;
    public AudioClip failClip;


    private void Start()
    {
        bgmSource.clip = bgmClips;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(string type)
    {
        switch (type)
        {
            case "success":
                sfxSource.PlayOneShot(successClip);
                break;
            case "fall":
                sfxSource.PlayOneShot(failClip);
                break;
        }
    }
}
