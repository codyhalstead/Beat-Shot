using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackgroundAudioSelector : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip[] songs = new AudioClip[4];
    public float[] songBPM = new float[4];
    private PlayerInput playerInput;

    public event Action<float> onSongChanged;
    int currentIndex = 0;

    void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();
    }

    void Update()
    {
        if (playerInput.actions["SelectSong1"].triggered)
        {
            if (currentIndex != 0)
            {
                PlaySong(0);
            }
        }    
        else if (playerInput.actions["SelectSong2"].triggered)
        {
            if (currentIndex != 1)
            {
                PlaySong(1);
            }
        }
        else if (playerInput.actions["SelectSong3"].triggered)
        {
            if (currentIndex != 2)
            {
                PlaySong(2);
            }
        }
        else if (playerInput.actions["SelectSong4"].triggered)
        {
            if (currentIndex != 3)
            {
                PlaySong(3);
            }
        }
    }

    public void PlaySong(int index)
    {
        if (songs == null || index < 0 || index >= songs.Length || songs[index] == null)
        {
            return;
        }
        currentIndex = index;
        musicSource.Stop();                
        musicSource.clip = songs[index];
        musicSource.time = 0f;         
        musicSource.Play();

        onSongChanged?.Invoke(songBPM[index]);
    }

    public float getCurrentSongBPM()
    {
        return songBPM[currentIndex];
    }
}
