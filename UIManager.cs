using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Level Select");
    }

    public void LoadToSceneMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadToSceneTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void LoadToSceneLevel(string level)
    {
        SceneManager.LoadScene("Level " + level);
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Kiem tra xem scene tiep theo co nam trong build settings khong
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene found in Build Settings.");
        }
    }

    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
