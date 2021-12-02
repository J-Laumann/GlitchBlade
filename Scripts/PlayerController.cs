using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    AudioSource audioSource;
    public Rigidbody rb;
    public float glitchDistance, glitchCooldown;
    float glitchTimer, hitTimer;
    public GameObject staticOverlay, bloodEffect, blackOverlay, youreWinnerUI;
    public float hitDist, hitCooldown, knockback;
    public Text bladeText, timeText, killsText;
    public Image glitchIcon;
    public int dmg;
    public AudioClip slashSound, hitFleshSound, swordTalkSound;
    public int kills;
    public LayerMask unhittableLayer;
    public string[] introMessage;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        
        StartCoroutine(IntroMessage());
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale <= 0.1f)
            return;

        if (Input.GetButton("Fire1") && Time.time >= hitTimer)
        {
            Attack();
        }

        if (Input.GetButtonDown("Glitch") && Time.time >= glitchTimer)
        {
            Glitch();
        }

        
        glitchIcon.fillAmount = 1 - (glitchTimer - Time.time) / glitchCooldown;
    }

    void Attack()
    {
        hitTimer = Time.time + hitCooldown;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("SwordSlash1"))
        {
            anim.Play("SwordSlash2");
            hitTimer += 0.3f;
        }
        else
        {
            anim.Play("SwordSlash1");
        }

        audioSource.PlayOneShot(slashSound, 0.5f);
        StartCoroutine(DealDamage(dmg));
    }

    IEnumerator DealDamage(int dmg)
    {
        yield return new WaitForSeconds(0.2f);
        //NEW COOL OVERLAP METHOD
        Collider[] cols = Physics.OverlapCapsule(transform.position, transform.position + transform.forward * hitDist, 3f);
        foreach (Collider col in cols)
        {
            if (col.gameObject.tag == "Enemy")
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, col.transform.position - transform.position, out hit, hitDist))
                {
                    if (hit.transform.gameObject == col.gameObject)
                    {
                        Enemy enemy = col.transform.GetComponent<Enemy>();
                        if (enemy)
                        {
                            
                            enemy.TakeDamage(dmg, transform.forward * knockback);
                            
                        }
                        //Effects! (blood and sound)
                        GameObject blood = Instantiate(bloodEffect, hit.point, Quaternion.identity, col.transform);
                        blood.transform.LookAt(transform);
                        AudioSource.PlayClipAtPoint(hitFleshSound, col.transform.position);
                    }
                }
            }
        }

        /* OLD RAYCASTING
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, hitDist))
        {
            if(hit.transform.gameObject.tag == "Enemy")
            {
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy)
                    enemy.TakeDamage(10, transform.forward * knockback);
            }
        }
        */
    }

    

    void Glitch()
    {
        //Loop for unglitchable wall
        float distFromPlayer = 0;
        Vector3 dir = Vector3.zero;
        dir += transform.forward * Input.GetAxis("Vertical");
        dir += transform.right * Input.GetAxis("Horizontal");
        dir.y = 0;
        dir = dir.normalized;
        while(distFromPlayer < glitchDistance)
        {
            Vector3 telePoint = rb.transform.position + (dir * distFromPlayer);
            telePoint.y -= rb.transform.localScale.y - 0.55f;
            Collider[] cols = Physics.OverlapCapsule(telePoint, telePoint + Vector3.up * (rb.transform.localScale.y * 2 - 0.55f), 0.5f, ~unhittableLayer);
            bool flag = false;
            foreach(Collider col in cols)
            {
                if (col.gameObject.tag == "Unglitchable")
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
                break;
            distFromPlayer += 0.1f;
        }

        //Now that weve found max distance (either maxDist or until an unglitchable wall)
        //Loop from max distance in until you wont be stuck in stuff and then glitch
        while(distFromPlayer > 1)
        {
            Vector3 telePoint = rb.transform.position + (dir * distFromPlayer); 
            telePoint.y -= rb.transform.localScale.y - 0.55f;
            Collider[] cols = Physics.OverlapCapsule(telePoint, telePoint + Vector3.up * (rb.transform.localScale.y * 2 - 0.55f), 0.5f, ~unhittableLayer);
            if (cols.Length == 0)
            {
                rb.transform.position = telePoint;
                break;
            }
            distFromPlayer -= 0.1f;
        }

        glitchTimer = Time.time + glitchCooldown;

        staticOverlay.SetActive(true);
        Invoke("StaticOff", 0.1f);
    }

    void StaticOff()
    {
        staticOverlay.SetActive(false);
    }

    public IEnumerator BladeTalk(string message, float readTime)
    {
        bladeText.text = "";
        foreach(char c in message)
        {
            //audioSource.PlayOneShot(swordTalkSound);
            bladeText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(readTime);
        bladeText.text = "";
    }

    public IEnumerator IntroMessage()
    {
        foreach(string message in introMessage)
        {
            StartCoroutine(BladeTalk(message, 2));
            yield return new WaitForSeconds(message.Length * 0.05f + 3f);
        }
    }

    public void GetKill()
    {
        kills++;
    }
}
