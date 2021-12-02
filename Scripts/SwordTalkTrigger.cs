using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTalkTrigger : MonoBehaviour
{
    public string[] messages;
    public float readTime;

    public int killsRequired;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerController pc = other.transform.parent.GetComponentInChildren<PlayerController>();
            if (killsRequired == -1 || pc.kills == killsRequired)
            {
                StartCoroutine(SayMessages(pc));
                GetComponent<Collider>().enabled = false;
            }
        }
    }

    IEnumerator SayMessages(PlayerController pc)
    {
        foreach(string message in messages)
        {
            pc.StopAllCoroutines();
            pc.StartCoroutine(pc.BladeTalk(message, readTime));
            readTime += (message.Length * 0.05f) + 1f;
            yield return new WaitForSeconds(readTime);
        }
        Destroy(gameObject);
    }
}
