using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitching : MonoBehaviour
{
    /// <summary>
    /// Loads the next scene
    /// </summary>
    /// <param name="levelName"></param>
    public void LoadLevel(string levelName)
    {
        StartCoroutine(InvokePause(levelName));
    }

    IEnumerator InvokePause(string levelName)
    {
        yield return new WaitForSecondsRealtime(.1f);

        PauseMenu.isPaused = false;

        SceneManager.LoadScene(levelName);
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
