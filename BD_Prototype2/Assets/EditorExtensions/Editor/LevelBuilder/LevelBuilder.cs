using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BulletDance.LevelBuilder
{

public class LevelBuilder
{
    public static GameObject[] GetRooms()
    {
        return GameObject.FindGameObjectsWithTag("Room");
    }

    public static GameObject GetRoomFromChild(GameObject gameObject)
    {
        if(gameObject == null) return null;

        Transform inspectedTrf = gameObject.transform;

        while(true)
        {
            //Check if the tag is room
            if(inspectedTrf.tag == "Room")
                return inspectedTrf.gameObject;

            //Reached root but still not found
            if(inspectedTrf == gameObject.transform.root)
                return null;

            //If current inspected transform is not room, inspect parent
            inspectedTrf = inspectedTrf.parent;
        }
    }
}


}