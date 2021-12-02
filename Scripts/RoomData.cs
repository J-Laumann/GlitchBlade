using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{

    public Text typeText, enemyText;
    public int enemyCount, rotation;

    public LevelEditorController controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddEnemy()
    {
        enemyText.text = "";
        enemyCount++;
        if (enemyCount > 9)
            enemyCount = 0;

        if (enemyCount == 9)
            enemyText.text = "o-";
        else
        {
            for(int i = 0; i < enemyCount; i++)
            {
                enemyText.text += "I";
            }
        }
    }

    public void ToggleRoomType()
    {
        if (typeText.text == "S")
            typeText.text = "T";
        else if (typeText.text == "T")
            typeText.text = "R";
        else if (typeText.text == "R")
            typeText.text = "G";
        else
            typeText.text = "S";

        if(controller.allRooms[controller.allRooms.Count - 1].Equals(gameObject))
        {
            controller.GenerateNextRooms(transform.localPosition, typeText.text);
        }
    }
}
