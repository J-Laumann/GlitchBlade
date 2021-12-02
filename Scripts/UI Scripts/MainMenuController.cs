using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public GameObject mainUI, levelsUI;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenLevels()
    {
        levelsUI.SetActive(true);
        mainUI.SetActive(false);
    }

    public void CloseLevels()
    {
        levelsUI.SetActive(false);
        mainUI.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
