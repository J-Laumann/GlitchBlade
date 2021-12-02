using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // feel free to change this if this isn't the vibe, i'm still learning 3d lol
    NavMeshAgent agent;
    Transform player;
    Animator anim;
    public Transform head, gun, gunTip;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    public LayerMask whatIsEnemy;

    public GameObject projectile;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    bool hasFound;

    //States
    public float fov, rotSpeed;
    public bool willChase;
    public bool willPatrol;
    public bool willRemember;
    public bool canMove;
    public float sightRange;
    public float attackRange;
    public float ohShitRange;
    bool playerInSight;
    bool playerInAttack, attacking;

    public int health;
    Rigidbody rb;
    public GameObject[] drops;

    float footstepTimer;
    public AudioClip[] footsteps;

    public void Awake()
    {
        //searches for player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        // check for sight and attack range
        playerInSight = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        if (!playerInSight)
        {
            hasFound = false;
        }

        //Check if behind wall then if within field of view
        if (playerInSight)
        {
            //FOV (distance/vector3 math)
            if (playerInSight)
            {
                Vector3 relativePos = (player.position - transform.position).normalized;
                float dot = Vector3.Dot(relativePos, transform.forward);
                float angle = Mathf.Acos(dot);
                if (angle > fov)
                    playerInSight = false;
            }

        }

        //Super close radius (ignores FOV)
        if (!playerInSight)
        {
            playerInSight = Physics.CheckSphere(transform.position, ohShitRange, whatIsPlayer);
            
        }

        //WALL CHECK (raycast)
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, sightRange, ~whatIsEnemy))
        {
            if (hit.transform.gameObject.tag != "Player")
                playerInSight = false;
        }

        //Triggers the found bool, which will enable "memory" of player existing.
        if (willRemember && playerInSight && !hasFound)
            hasFound = true;

        playerInAttack = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!playerInSight && !playerInAttack && willPatrol && canMove)
        {
            Patroling();
        }
        if(canMove && (playerInSight && !playerInAttack && willChase || !playerInSight && hasFound && willChase))
        {
            ChasePlayer();
        }
        if(playerInSight && playerInAttack)
        {
            AttackPlayer();
        }
    }

    public void TakeDamage(int dmg, Vector3 knockback)
    {
        if (health > 0)
        {
            health -= dmg;
            rb.AddForce(knockback);
            if (health <= 0)
            {
                //Drops
                foreach (GameObject obj in drops)
                {
                    GameObject drop = Instantiate(obj, transform.position, transform.rotation);
                    drop.GetComponent<Rigidbody>().AddForce(Vector3.up * 500 + transform.forward * 100);
                }

                //drop gun
                if (gun)
                {
                    gun.transform.parent = null;
                    gun.gameObject.AddComponent<Rigidbody>();

                }

                //Allow falling, untag enemy, and disable AI
                rb.constraints = RigidbodyConstraints.None;
                //gameObject.tag = "Untagged";
                if (canMove)
                    agent.enabled = false;
                player.transform.parent.GetComponentInChildren<PlayerController>().GetKill();
                //Destroy(gameObject, 5);
                anim.enabled = false;
                this.enabled = false;
            }
        }
    }
    /// <summary>
    /// Waiting for player to be in range
    /// </summary>
    public void Patroling()
    {
        if(!walkPointSet)
        {
            SearchWalkPoint();
        }

        if(walkPointSet == true)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //walkpoint reached
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    /// <summary>
    /// Chasing player once in sight range
    /// </summary>
    public void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    /// <summary>
    /// attacking player once in attack range
    /// </summary>
    public void AttackPlayer()
    {
        //Makes it wait once the player enters shoot range.
        if (!attacking)
        {
            attacking = true;
            if (!alreadyAttacked)
            {
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
                if (anim)
                    anim.Play("Reload");
            }
        }

        // Make sure enemy doens't move
        if(canMove)
            agent.SetDestination(transform.position);

        //Whole body look
        var rotation = Quaternion.LookRotation(player.position - transform.position);
        rotation.x = 0;
        rotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotSpeed);

        //Gun look
        if (gun)
        {
            rotation = Quaternion.LookRotation(player.position - gun.position);
            gun.rotation = Quaternion.Slerp(gun.rotation, rotation, Time.deltaTime * rotSpeed * 1.2f);
        }

        //Head look
        if (head)
        {
            rotation = Quaternion.LookRotation(player.position - head.position);
            head.rotation = Quaternion.Slerp(head.rotation, rotation, Time.deltaTime * rotSpeed * 1.2f);
        }

        if (!alreadyAttacked)
        {
            //jackson can you please put your pizzazz here?

            Rigidbody rb = Instantiate(projectile, gunTip.position, Quaternion.identity).GetComponent<Rigidbody>();

            //feel free to adjust values as needed
            rb.AddForce(gunTip.forward * 100f, ForceMode.Impulse);
            rb.AddForce(gunTip.up * 6f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            if (anim)
                anim.Play("Reload");
        }
    }

    public void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void SearchWalkPoint()
    {
        //Calculate random point in range
        float randX = Random.Range(-walkPointRange, walkPointRange);
        float randZ = Random.Range(-walkPointRange, walkPointRange);

        // keeps y the same
        walkPoint = new Vector3(transform.position.x + randX, transform.position.y, transform.position.z + randZ);

        // makes sure it's on the map
        Collider[] cols = Physics.OverlapCapsule(new Vector3(transform.position.x + randX, transform.position.y, transform.position.z + randZ), new Vector3(transform.position.x + randX, transform.position.y + 1, transform.position.z + randZ), 1);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround) && cols.Length == 0)
        {
            walkPointSet = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Killzone")
        {
            TakeDamage(1000, Vector3.zero);
        }
    }
}
