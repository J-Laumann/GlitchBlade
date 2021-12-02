using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallData : MonoBehaviour
{

    public Text wallText;

    public void ChangeWallType()
    {
        if(wallText.text == "OPEN")
            wallText.text = "WINDOW";
        else if (wallText.text == "WINDOW")
            wallText.text = "DOOR";
        else
            wallText.text = "OPEN";
    }

    public void SetType(int i)
    {
        if (i == 0)
            wallText.text = "OPEN";
        else if (i == 2)
            wallText.text = "WINDOW";
        else if (i == 3)
            wallText.text = "DOOR";
        else
            wallText.text = "WALL?";
    }

    public int GetWallType()
    {
        if (wallText.text == "OPEN")
            return 0;
        else if (wallText.text == "WINDOW")
            return 2;
        else if (wallText.text == "DOOR")
            return 3;

        return 1;
    }

}
