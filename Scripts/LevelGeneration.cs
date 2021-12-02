using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGeneration : MonoBehaviour
{

    // T for tunnel or R for room or G for gap or S for slide
    // Each will be followed by enemy amount, or 9 for a turret.
    // Afterwards a rotation amount, 90 degress x whatever. Ex. 3 = 270 (next room will be to the left)
    // R or T will then be followed by 4 numbers for wall types. (Front, Right, Back, Left) (0 = Open, 1 = Wall, 2 = Window, 3 = Door)
    // Stop reading this and go use the dope ass level maker you ape.

    public string forceSeed;
    string seed;

    public GameObject tunnelPrefab, roomPrefab, gapPrefab, slidePrefab, wallPrefab;
    public GameObject enemyPrefab, turretPrefab;

    Transform spawnpoint;

    // Start is called before the first frame update
    void Start()
    {
        if (forceSeed != "" && forceSeed != null)
            seed = forceSeed;
        else
            seed = PlayerPrefs.GetString("LevelSeed", "T000");

        string[] rooms = seed.Split('.');
        spawnpoint = transform;
        spawnpoint.position = Vector3.zero;
        foreach (string room in rooms)
        {
            if (room[0] == 'T' || room[0] == 'S')
            {
                int tunnelWall = room[3] - '0';
                if(tunnelWall > 0)
                {
                    GameObject newWall = Instantiate(wallPrefab, spawnpoint.position - Vector3.up + (spawnpoint.forward * -9), spawnpoint.rotation);
                    newWall.transform.GetChild(tunnelWall).gameObject.SetActive(true);
                }
                spawnpoint.position += spawnpoint.forward * 10;
                spawnpoint.Rotate(Vector3.up * 90 * (room[2] - '0'));
                GameObject newRoom;
                if (room[0] == 'T')
                    newRoom = Instantiate(tunnelPrefab, spawnpoint.position, spawnpoint.rotation);
                else if (room[0] == 'S')
                {
                    newRoom = Instantiate(slidePrefab, spawnpoint.position, spawnpoint.rotation);
                    spawnpoint.position += Vector3.down * 10;
                }
                SpawnEnemies(room[1] - '0');

                spawnpoint.position += spawnpoint.forward * 10;
            }
            else if (room[0] == 'R')
            {
                spawnpoint.position += spawnpoint.forward * 10;
                spawnpoint.Rotate(Vector3.up * 90 * (room[2] - '0'));
                GameObject newRoom = Instantiate(roomPrefab, spawnpoint.position, spawnpoint.rotation);
                for (int i = 0; i < 4; i++)
                {
                    int wallNumb = room[i + 3] - '0';
                    newRoom.transform.GetChild(0).GetChild(i).GetChild(wallNumb).gameObject.SetActive(true);
                }
                SpawnEnemies(room[1] - '0');
                
                spawnpoint.position += spawnpoint.forward * 10;
            }
            else if (room[0] == 'G')
            {
                spawnpoint.position += spawnpoint.forward * 10;
                spawnpoint.Rotate(Vector3.up * 90 * (room[2] - '0'));
                GameObject newRoom = Instantiate(gapPrefab, spawnpoint.position, spawnpoint.rotation);

                int wallNumb = room[2] - '0';
                if (wallNumb != 3)
                    wallNumb = Mathf.Abs(wallNumb - 2);

                for (int i = 1; i < 4; i++)
                {
                    if (i != wallNumb)
                        newRoom.transform.GetChild(0).GetChild(i).GetChild(1).gameObject.SetActive(true);
                }

                SpawnEnemies(room[1] - '0');

                spawnpoint.position += spawnpoint.forward * 10;
            }
        }
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    void SpawnEnemies(int amount)
    {
        if (amount == 9)
        {
            RaycastHit hit;
            if (Physics.Raycast(spawnpoint.position + Vector3.up * 5, Vector3.up, out hit, 100f))
            {
                GameObject newT = Instantiate(turretPrefab, hit.point, spawnpoint.rotation);
                newT.transform.LookAt(spawnpoint);
            }
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                Instantiate(enemyPrefab, spawnpoint.position, spawnpoint.rotation);
            }
        }
    }

}
