using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] rooms;

    List<string> startRooms = new List<string> { "s-1" };
    List<string> endRooms = new List<string> { "e-1" };
    List<string> linkRooms = new List<string> { "l-1", "l-2", "l-3", "l-4", "l-5", "l-6", "l-7", "l-8", "l-9", "l-10", "l-11", "l-12", "l-13", "l-14", "l-15", "l-16" };
    List<string> cornerRooms = new List<string> { "cr-nw", "cr-ne", "cr-sw", "cr-se" };

    // In real life the name of the room would be unique and therefore the name could be the key
    Dictionary<Vector2, string> roomInfo = new Dictionary<Vector2, string>();

    //Vector2 startingPoints;
    float yStartingPoint = -0.5f;

    float xRoomLength = 16f;
    float yRoomHeight = 9f;

    // Keep as instance variable as it could be used throught the whole class
    int gridLength = 5;

    string[,] grid;

    Vector2[] startAndEndGridPostions;

    // Start is called before the first frame update
    void Start()
    {
        grid = new string[gridLength, gridLength];

        // Generate Start Position
        // Generate End Position
        startAndEndGridPostions = GenerateStartAndEndGridPositions();

        // Loop through grid
        // Get world position and room name for each sqaure in the grid
        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                //print("(" + i + "," + j + ")");

                // Store World Position in Vector2
                // On-screen position for prefab
                Vector2 worldPoint = CalculateWorldPosition(i, j);

                // Store the current position in the grid
                Vector2 currentGridPosition = new Vector2(i, j);

                // Pick a room for that position, Randomizer might come in here
                string roomName = PickRoom(currentGridPosition);

                // Store in a Dictionary<Room Name, World Cooridinate>
                SaveRoomDetailsForLoading(roomName, worldPoint);
            }
        }

        // By this stage, we have a dictionary with the worldPoint / worldPosition as the key
        // And the roomName / Room Code Name as the value
        // This dictionary holds a theoretical "Area" but needs to be passed to a function to 
        // make it 'real' and print to screen
        //ReadDictionary();

        // GameObject Array to store the loaded prefabs
        rooms = new GameObject[roomInfo.Count];

        // ===================================================
        // Method required to filter what type of room to load
        // ===================================================
        rooms = loadRooms(roomInfo);

        int counter = 0;

        foreach (GameObject room in rooms)
        {
            // Set Transform to world coordinates
            // Retrieve the world coordiantes from the Dictionary
            room.transform.position = new Vector3(roomInfo.ElementAt(counter).Key.x, roomInfo.ElementAt(counter).Key.y, 0);

            Instantiate(room, room.transform.position, room.transform.rotation);

            counter++;
        }

    }

    Vector2 CalculateWorldPosition(float i, float j)
    {        
        float wolrdPositionX = j * xRoomLength;
        float worldPositionY = yStartingPoint - (yRoomHeight * i);

        return new Vector2(wolrdPositionX, worldPositionY);
    }

    string PickRoom(Vector2 currentGridPosition)
    {
        // Start Posisiton
        if (startAndEndGridPostions[0] == currentGridPosition)
        {
            return startRooms[0];
        }

        // If End Position
        if (startAndEndGridPostions[1] == currentGridPosition)
        {
            return endRooms[0];
        }

        // Corner Rooms
        Vector2 nw = new Vector2(0, 0);
        Vector2 ne = new Vector2(gridLength - 1, 0);
        Vector2 sw = new Vector2(0, gridLength - 1);
        Vector2 se = new Vector2(gridLength - 1, gridLength - 1);

        // =====================================================================================
        // GET AND RETURN CORNER ROOMS CODE NAME IF CORRECT GRID POSITION IS BEING ITERATED OVER
        // =====================================================================================
        if (nw == currentGridPosition)
        {
            return cornerRooms[0]; 
        }
        else if (ne == currentGridPosition)
        {
            return cornerRooms[1];
        }
        else  if (sw == currentGridPosition)
        {
            return cornerRooms[2];
        }
        else if (se == currentGridPosition)
        {
            return cornerRooms[3];
        }

        // =============================
        // Need to pick a room at random
        // =============================
        int randomRoom = UnityEngine.Random.Range(0, linkRooms.Count);

        return linkRooms[randomRoom];
    }

    void SaveRoomDetailsForLoading(string roomName, Vector2 worldPoint)
    {
        roomInfo.Add(worldPoint, roomName);
    }

    void ReadDictionary()
    {
        for (int i = 0; i < roomInfo.Count; i++)
        {
            Debug.Log("Reading Dictionary");
            Debug.Log("Room Coordinate: " + roomInfo.ElementAt(i).Key);
            Debug.Log("Room Name: " + roomInfo.ElementAt(i).Value);
        }
    }

    GameObject[] loadRooms(Dictionary<Vector2, string> roomInfo)
    {
        // Initialise Array
        GameObject[] rooms = new GameObject[roomInfo.Count];

        // Get the Room Name from Dictionary

        // Parse first part of name and build string to determine what folder to load
        // cr/cr-nw
        // l/l-1

        // Split RoomName based the hyphen
        String[] spearator = { "-" };

        // Loop through the dictionary
        for (int i = 0; i < roomInfo.Count; i++)
        {
            // Split Room Name and Store Each Part In An Array 
            String[] roomCode = roomInfo.ElementAt(i).Value.Split(spearator, StringSplitOptions.RemoveEmptyEntries);

            //Debug.Log("Path Name: " + roomCode[0] + "/" + roomInfo.ElementAt(i).Value);

            // First Element In Array Will Be The Folder To Search In
            // Create A File Path With The Folder Name and Name Of Room
            rooms[i] = Resources.Load<GameObject>(roomCode[0] + "/" + roomInfo.ElementAt(i).Value);

        }

        return rooms;
    
    }

    Vector2[] GenerateStartAndEndGridPositions()
    {
        Vector2[] startAndEndGridPositions = new Vector2[2];

        int randomStartPosition = UnityEngine.Random.Range(1, (gridLength - 1));
        Vector2 start = new Vector2(randomStartPosition, 0);

        int randomEndPosition = UnityEngine.Random.Range(1, (gridLength - 1));
        Vector2 end = new Vector2(randomEndPosition, (gridLength - 1));

        startAndEndGridPositions[0] = start;
        startAndEndGridPositions[1] = end;

        return startAndEndGridPositions;
    }
}
