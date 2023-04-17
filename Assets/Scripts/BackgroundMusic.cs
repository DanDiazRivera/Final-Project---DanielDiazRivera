using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{

    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip backgroundMusic;
    public AudioSource musicSource;

    public void ChangeSound(int clipNum)
    {
        if (clipNum == 0)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
        if (clipNum == 1)
        {
            musicSource.clip = winSound;
            musicSource.Play();
        }
        if (clipNum == 2)
        {
            musicSource.clip = loseSound;
            musicSource.Play();
        }
    }
}
