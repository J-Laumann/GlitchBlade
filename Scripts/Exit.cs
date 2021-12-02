using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    
    public float timer;
    public bool end;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerHealth>().health > 0)
            {
                if (end)
                {
                    Time.timeScale = 0;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    PlayerController pc = collision.gameObject.transform.parent.GetComponentInChildren<PlayerController>();
                    pc.youreWinnerUI.SetActive(true);

                    int seconds, minutes;
                    seconds = (int)(timer % 60);
                    string extraSec = "";
                    if (seconds < 10)
                        extraSec = "0";
                    minutes = (int)(timer / 60);
                    pc.timeText.text = "TIME: " + minutes + " MINS and " + extraSec + seconds + " SECS";
                    pc.killsText.text = pc.kills + "";
                    PlayerPrefs.SetInt("Souls", PlayerPrefs.GetInt("Souls", 0) + pc.kills);
                }
                else
                {
                    SceneManager.LoadScene("MainMenu");
                }
            }
        }
    }
}
