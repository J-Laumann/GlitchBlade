using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public Image healthBar;
    bool dead;
    public GameObject blood;
    float lastHit;

    private void Start()
    {
        health = maxHealth;
        lastHit = Time.time;
    }

    private void Update()
    {
        healthBar.fillAmount = (float)health / (float)maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        if (Time.time > lastHit + 0.8f)
        {
            health -= dmg;
            lastHit = Time.time;
            blood.SetActive(true);
            Invoke("BloodOff", 0.5f);
            if (health <= 0 && !dead)
            {
                dead = true;
                transform.parent.GetComponentInChildren<PlayerMovement>().enabled = false;
                PlayerController pc = transform.parent.GetComponentInChildren<PlayerController>();
                pc.staticOverlay.SetActive(true);
                pc.blackOverlay.SetActive(true);
                pc.StopAllCoroutines();
                StartCoroutine(pc.BladeTalk("Pathetic...", 5));
                pc.enabled = false;
                transform.parent.GetComponentInChildren<Rigidbody>().constraints = RigidbodyConstraints.None;
                Invoke("ReloadScene", 5);
            }
        }
    }

    public void ReloadScene()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
        if(collision.gameObject.tag == "Killzone")
        {
            TakeDamage(1000);
        }
    }

    void BloodOff()
    {
        blood.SetActive(false);
    }
}
