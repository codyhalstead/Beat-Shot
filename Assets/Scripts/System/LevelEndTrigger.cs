using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{

    void Start()
    {
   
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.Destroy();
            }
            StartCoroutine(EndLevel());
        }
    }

    public void ManualEndLevel()
    {
        StartCoroutine(EndLevel());
    }

    private IEnumerator EndLevel()
    {
        ScreenFader screenFader = FindFirstObjectByType<ScreenFader>();
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (screenFader != null)
        {
            screenFader.missionComplete = true;
            yield return StartCoroutine(screenFader.FadeOut());
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}