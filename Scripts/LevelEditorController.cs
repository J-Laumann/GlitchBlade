using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelEditorController : MonoBehaviour
{
    public Transform content;
    public GameObject createdRoom, newRoomButton, wallButton;

    public List<GameObject> allRooms;
    List<GameObject> nextRooms;

    public Transform tracker;
    public InputField seedField;

    // Start is called before the first frame update
    void Start()
    {
        seedField.text = PlayerPrefs.GetString("LevelSeed");
        allRooms = new List<GameObject>();
        nextRooms = new List<GameObject>();
        if(seedField.text != "")
        {
            LoadLevel();
        }
        else
            NewRoom(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Undo();
        }
    }

    public void NewRoom(Vector3 pos)
    {
        GameObject newRoom = Instantiate(createdRoom, content);
        newRoom.transform.localPosition = pos;
        RoomData rd = newRoom.GetComponent<RoomData>();
        if (allRooms.Count > 0)
            allRooms[allRooms.Count - 1].GetComponent<Button>().interactable = false;
        if(allRooms.Count > 1)
        {
            if(allRooms[allRooms.Count - 1].GetComponent<RoomData>().typeText.text == "G" && allRooms[allRooms.Count - 2].GetComponent<RoomData>().typeText.text == "G")
            {
                Destroy(allRooms[allRooms.Count - 1].GetComponentInChildren<WallData>().transform.gameObject);
            }
        }
        allRooms.Add(newRoom);
        if(allRooms.Count > 1)
        {
            Vector3 dir = pos - allRooms[allRooms.Count - 2].transform.localPosition;
            GameObject newWall = Instantiate(wallButton, newRoom.transform);
            newWall.transform.localPosition = -dir / 2;
            if (dir.y > -1 && dir.y < 1) {
                RectTransform rt = newWall.GetComponent<RectTransform>();
                float temp = rt.sizeDelta.x;
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rt.sizeDelta.y);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, temp);
            }
            
        }
        
        rd.controller = this;
        GenerateNextRooms(pos, rd.typeText.text);
    }

    public void NewNewRoomButton(Vector3 pos)
    {
        foreach (GameObject room in allRooms)
        {
            if (Vector3.Distance(room.transform.localPosition, pos) <= 1f)
                return;
        }

        GameObject newButton = Instantiate(newRoomButton, content);
        newButton.transform.localPosition = pos;

        Vector3 tempPos = pos;
        newButton.GetComponent<Button>().onClick.AddListener(delegate { NewRoom(tempPos); });

        nextRooms.Add(newButton);
    }

    public void GenerateNextRooms(Vector3 pos, string type)
    {
        foreach (GameObject oldButton in nextRooms)
            Destroy(oldButton);

        nextRooms = new List<GameObject>();

        if (type == "T" || type == "S")
        {
            if (allRooms.Count > 1)
            {
                Vector3 dir = pos - allRooms[allRooms.Count - 2].transform.localPosition;
                NewNewRoomButton(pos + dir);
            }
            else
            {
                NewNewRoomButton(pos + Vector3.up * 100);
            }
        }
        if (type == "R" || type == "G")
        {
            NewNewRoomButton(pos + Vector3.up * 100);
            if(allRooms.Count > 1)
                NewNewRoomButton(pos + Vector3.up * -100);
            NewNewRoomButton(pos + Vector3.right * 100);
            NewNewRoomButton(pos + Vector3.right * -100);
        }
    }

    
    //THE HARD PART
    public void GenerateSeed()
    {
        string seed = "";
        tracker.eulerAngles = Vector3.zero;
        for(int i = 0; i < allRooms.Count; i++)
        {
            GameObject room = allRooms[i];
            RoomData rd = room.GetComponent<RoomData>();
            WallData wd = room.GetComponentInChildren<WallData>();

            seed += rd.typeText.text;
            seed += rd.enemyCount;

            tracker.localPosition = allRooms[i].transform.localPosition;
            int rot = 0;
            float oldAngle = tracker.eulerAngles.z;
            if (rd.typeText.text != "T" && rd.typeText.text != "S" && i < allRooms.Count - 1)
            {
                tracker.up = allRooms[i + 1].transform.localPosition - tracker.localPosition;
                rot = (int)(oldAngle - tracker.eulerAngles.z);
                rot = rot / 90;
                if (rot < 0)
                    rot = rot + 4;
            }
            seed += rot;

            if(rd.typeText.text == "R")
            {
                if (i > 0)
                {

                    if(rot != 3)
                        rot = Mathf.Abs(rot - 2);

                    tracker.up = allRooms[i - 1].transform.localPosition - tracker.localPosition;
                    if (tracker.eulerAngles.z < 0)
                        tracker.eulerAngles += Vector3.forward * 360;
                }

                for(int x = 0; x < 4; x++)
                {
                    if (x == 0)
                    {
                        if (i < allRooms.Count && allRooms[i + 1].GetComponent<RoomData>().typeText.text == "R")
                        {
                            seed += "0";
                        }
                        else
                        {
                            WallData nwd = allRooms[i + 1].GetComponentInChildren<WallData>();
                            int wallValue = nwd.GetWallType();
                            seed += wallValue;
                        }
                    }
                    else if (x == rot)
                    {
                        int wallValue = 0;
                        if (wd)
                        {
                            wallValue = wd.GetWallType();
                        }
                        seed += wallValue;
                    }
                    else
                        seed += 1;
                }
            }
            else if (rd.typeText.text == "T" || rd.typeText.text == "S")
            {
                int wallNumb = 0;
                if(i > 0)
                {
                    if(allRooms[i - 1].GetComponent<RoomData>().typeText.text == "T" || allRooms[i - 1].GetComponent<RoomData>().typeText.text == "S")
                    {
                        wallNumb = wd.GetWallType();
                    }
                }
                seed += wallNumb;
            }

            if(i < allRooms.Count - 1)
                tracker.up = allRooms[i + 1].transform.localPosition - tracker.localPosition;

            if (i < allRooms.Count - 1)
                seed += ".";
        }
        seedField.text = seed;
    }

    public void PlayLevel()
    {
        PlayerPrefs.SetString("LevelSeed", seedField.text);
        SceneManager.LoadScene("Procedural");
    }

    public void Undo()
    {
        if(allRooms.Count > 1)
        {
            Destroy(allRooms[allRooms.Count - 1]);
            allRooms.RemoveAt(allRooms.Count - 1);
            allRooms[allRooms.Count - 1].GetComponent<Button>().interactable = true;
            GenerateNextRooms(allRooms[allRooms.Count - 1].transform.localPosition, allRooms[allRooms.Count - 1].GetComponent<RoomData>().typeText.text);
        }
    }

    public void Clear(bool restart)
    {
        foreach(GameObject room in allRooms)
        {
            Destroy(room);
        }
        allRooms = new List<GameObject>();
        foreach(GameObject room in nextRooms)
        {
            Destroy(room);
        }
        nextRooms = new List<GameObject>();
        tracker.localPosition = Vector3.zero;
        tracker.eulerAngles = Vector3.zero;

        if (restart)
            NewRoom(tracker.localPosition);
    }

    public void LoadLevel()
    {
        Clear(false);
        string loadSeed = seedField.text;

        tracker.localPosition = Vector3.zero;
        tracker.eulerAngles = Vector3.zero;

        string[] rooms = loadSeed.Split('.');

        for(int i = 0; i < rooms.Length; i++)
        {
            string roomString = rooms[i];

            NewRoom(tracker.localPosition);

            RoomData rd = allRooms[allRooms.Count - 1].GetComponent<RoomData>();

            rd.typeText.text = roomString[0] + "";

            int enemyAmount = roomString[1] - '0';
            for (int x = 0; x < enemyAmount; x++)
                rd.AddEnemy();

            tracker.Rotate(Vector3.forward * (roomString[2] - '0') * -90);

            if(roomString[0] == 'R' && i > 0)
            {
                int rot = roomString[2] - '0';
                if(rot != 3)
                    rot = Mathf.Abs(rot - 2);
                allRooms[i].GetComponentInChildren<WallData>().SetType(roomString[3 + rot] - '0');
            }
            else if((roomString[0] == 'T' || roomString[0] == 'S') && i > 0)
            {
                if (rooms[i - 1][0] == 'R')
                    allRooms[i].GetComponentInChildren<WallData>().SetType(rooms[i - 1][3] - '0');
                else
                    allRooms[i].GetComponentInChildren<WallData>().SetType(rooms[i][3] - '0');
                
            }

            tracker.localPosition += tracker.up * 100;
        }
    }

}
