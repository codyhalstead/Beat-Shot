using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public AudioSource backgroundAudioSource;

    void Update()
    {
        bool wasPaused =  Keyboard.current.pKey.wasPressedThisFrame;
        if (wasPaused)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (Time.timeScale == 0f)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        backgroundAudioSource.UnPause();
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        backgroundAudioSource.Pause();
    }

    public void QuitGame()
    {
        
    }

}