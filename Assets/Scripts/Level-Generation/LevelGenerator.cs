using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Room Elements
    readonly List<string> roomOpenExitElements = new List<string> { "n", "no", "nne", "ne", "ene", "e", "eo", "ese", "se", "sse", "s", "so", "ssw", "sw", "wsw", "w", "wo", "wnw", "nw", "nnw" };
    readonly List<string> adjoiningRoomOpenExitElements = new List<string> { "s", "so", "sse", "se", "wnw", "w", "wo", "wsw", "ne", "nne", "n", "no", "nnw", "nw", "ese", "e", "eo", "ene", "sw", "ssw" };
    readonly List<string> roomClosedExitElements = new List<string> { "nc", "ec", "sc", "wc" };

    // Open Exit Elements Mapper
    Dictionary<string, string> roomOpenExitsElementsMapper = new Dictionary<string, string>();

    // Required Room Elements For Connecting Adjoining Room
    List<string> requiredRoomElementsForConnectingAdjoiningRoom = new List<string>();

    // Corner Rooms
    readonly List<string> nwCornerRooms = new List<string> { "cr-nc-eo-sc-wc" };
    readonly List<string> neCornerRooms = new List<string> { "cr-nc-ec-sc-wo" };
    readonly List<string> swCornerRooms = new List<string> { "cr-nc-eo-sc-wc" };
    readonly List<string> seCornerRooms = new List<string> { "cr-nc-ec-sc-wo" };

    // Start and End Rooms
    readonly List<string> startRooms = new List<string> { "s-1" };
    readonly List<string> endRooms = new List<string> { "e-1" };

    // Link Room Codes
    readonly List<string> linkRooms = new List<string> { "lk-nc-eo-sc-wo", "lk-n-eo-s-wo", "lk-nc-eo-sc-wc", "lk-n-eo-sc-wc", "lk-nc-eo-s-wo", "lk-nc-ec-s-wo", "lk-nc-ec-sc-wo" };

    // Contains The World Position and Room Name To Place In That World Position
    Dictionary<Vector2, string> worldPositionAndRoomNameForLoadingLevel = new Dictionary<Vector2, string>();

    // Stores the required room elements for upcoming grid positions
    Dictionary<Vector2, List<string>> requiredRoomElementsForGridPositions = new Dictionary<Vector2, List<string>>();

    // Involved in determining the world positions of each room
    const float worldStartingPointOnYAxis = -0.5f; // The NW corner room will have the coordinate (0, -0.5f)
    const float roomLengthOnXAxis = 16f;
    const float roomLengthOnYAxis = 9f;

    // Determines the size of the grid
    const int gridLength = 5;

    // Variables For Rules
    Vector2[] startAndEndRoomGridPositions = new Vector2[2];
    Vector2[] cornerRoomGridPositions = new Vector2[4];

    Vector2 gridStartingPosition;
    string currentRoom;

    // Variables For Debugging
    public bool levelGeneratorSetupDebuggingEnabled = false;
    public bool gridGeneratorDebuggingEnabled = false;
    public bool removingUnwantedRoomNameElementsDebuggingEnabled = false;
    public bool mapCurrentRoomElementsToAdjoiningRoomElementsDebuggingEnabled = false;
    public bool mapRoomElementsToAdjoiningRoomGridPositionDebuggingEnabled = false;
    public bool debuggingEnabled = false;
    public bool lowLevelDebuggingEnabled = false; 

    // Used To Break Room Names Into Indiviual Strings
    readonly string[] roomNameSpearator = { "-" };

    // Store prefabs for loading rooms
    readonly GameObject[] rooms;

    void Awake()
    {
        PrintLevelGeneratorSetup();

        populateRoomOpenExitsElementsMapper();

        // Rules Setup
        StartAndEndRoomGridPositionRule();
        CornerRoomGridPositionRule();

        // Initialisation
        gridStartingPosition = new Vector2(0, 0);

        PrintLevelGeneratorSetupCompleted();
    }

    void Start()
    {
        PrintGridGenerationStarting();

        for (int row = 0; row < gridLength; row++)
        {
            for (int column = 0; column < gridLength; column++)
            {
                Vector2 currentGridPosition = new Vector2(row, column);
                Vector2 worldPosition = CalculateWorldPositionBasedOnGridPosition(currentGridPosition);

                SelectRoomForCurrentGridPosition(currentGridPosition);

                CollectRequiredRoomElementsForAdjoiningRooms(currentGridPosition, currentRoom);

                MapRequiredElementsForAjoiningRoomsToCorrectGridPositions(currentGridPosition, requiredRoomElementsForConnectingAdjoiningRoom);

                SaveRoomDetailsForLoading(currentRoom, worldPosition);
            }
        }

        // WORKING CODE: THIS MIGHT BE ABLE TO BE PUT INTO A SINGLE METHOD TO CLEAN IT UP
      
        // GameObject Array to store the loaded prefabs
        /*rooms = new GameObject[roomInfo.Count];

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
        }*/

    }

    /*
     *      STEP 1: INITIALISATION AND RULE SETUP
     */

    // Step 1a: Initialisation
    void populateRoomOpenExitsElementsMapper()
    {
        for (int i = 0; i < roomOpenExitElements.Count; i++)
        {
            roomOpenExitsElementsMapper.Add(roomOpenExitElements[i], adjoiningRoomOpenExitElements[i]);
        }

        PrintExitMapperCompletion();
    }

    // Step 1b: Rule Setup
    void StartAndEndRoomGridPositionRule()
    {
        int randomStartPosition = UnityEngine.Random.Range(1, (gridLength - 1));
        Vector2 start = new Vector2(randomStartPosition, 0);

        int randomEndPosition = UnityEngine.Random.Range(1, (gridLength - 1));
        Vector2 end = new Vector2(randomEndPosition, (gridLength - 1));

        startAndEndRoomGridPositions[0] = start;
        startAndEndRoomGridPositions[1] = end;

        PrintStartAndEndRoomsGeneratedGridPositions(start, end);
    }

    void CornerRoomGridPositionRule()
    {
        // In the order the room will be iterated over in the loop

        // nw
        cornerRoomGridPositions[0] = new Vector2(0, 0);

        // ne
        cornerRoomGridPositions[1] = new Vector2(0, gridLength - 1);

        // sw
        cornerRoomGridPositions[2] = new Vector2(gridLength - 1, 0);

        // se
        cornerRoomGridPositions[3] = new Vector2(gridLength - 1, gridLength - 1);

        PrintCornerRoomsGeneratedGridPositions(cornerRoomGridPositions[0], cornerRoomGridPositions[1], cornerRoomGridPositions[2], cornerRoomGridPositions[3]);
    }

    /*
     *      STEP 2: GRID GENERATION
     */

    // Step 2a: Calculate World Positions For Rooms
    Vector2 CalculateWorldPositionBasedOnGridPosition(Vector2 currentGridPosition)
    {
        float wolrdPositionX = currentGridPosition.y * roomLengthOnXAxis;
        float worldPositionY = worldStartingPointOnYAxis - (roomLengthOnYAxis * currentGridPosition.x);

        return new Vector2(wolrdPositionX, worldPositionY);
    }

    // Step 2b: Select a room for the current grid position
    void SelectRoomForCurrentGridPosition(Vector2 currentGridPosition)
    {
        if (currentGridPosition == gridStartingPosition)
        {
            currentRoom = FirstRoomSelector();
        }
        else
        {
            currentRoom = AllOtherRoomsSelector(currentGridPosition);
        }

        PrintCurrentRoomAndGridPositionInformation(currentGridPosition, currentRoom);
    }

    string FirstRoomSelector()
    {
        return RandomRoomSelection(nwCornerRooms);
    }

    // TODO: THIS ENTIRE METHOD NEEDS TO BE LOOK AT MORE CLOSELY AND CLEANED UP
    string AllOtherRoomsSelector(Vector2 currentGridPosition)
    {
        // North-East Corner Room
        if (cornerRoomGridPositions[1] == currentGridPosition)
        {
            return RandomRoomSelection(neCornerRooms);
        }

        // South-West Corner Room
        if (cornerRoomGridPositions[2] == currentGridPosition)
        {
            return RandomRoomSelection(swCornerRooms);
        }

        // South-East Corner Room
        if (cornerRoomGridPositions[3] == currentGridPosition)
        {
            return RandomRoomSelection(seCornerRooms);
        }

        List<string> roomsWhichMeetTheExitRequirements = new List<string>();
        List<string> exitsRequiredForCurrentGridPosition;

        // This Dictionary contains the mapping of the grid position to the list of exits it may require
        if (requiredRoomElementsForGridPositions.ContainsKey(currentGridPosition))
        {
            exitsRequiredForCurrentGridPosition = requiredRoomElementsForGridPositions[currentGridPosition];
        }
        else
        {
            // No exits to check
            // This happens when on (1,0) where (0,0) has no south exit, need a default for these Rooms
            //// TODO: STRAIGHT UP HARD CODING THE DEFAULT ROOM CURRENTLY, FIND BETTER SOLUTION
            exitsRequiredForCurrentGridPosition = CreateDefaultRoomElementsForCurrentGridPosition(currentGridPosition);
        }

        // -------------------------------------------
        // Need A Better Way To Handle This NULL Check
        // -------------------------------------------
        if (exitsRequiredForCurrentGridPosition == null)
        {
            // If NULL does that mean any room can be selected?
            int tempRandomRoom = UnityEngine.Random.Range(0, linkRooms.Count);
            return linkRooms[tempRandomRoom];
        }
        else 
        {
            // Working on LinkedRooms for now but it will need to be able to handle any type of room it is provided
            foreach (string roomName in linkRooms)
            {
                bool doesRoomMeetExitRequirements = false;

                // loop though exitsRequiredForCurrentGridPosition and compare each to the room name
                for (int i = 0; i < exitsRequiredForCurrentGridPosition.Count; i++)
                {
                    if (roomName.Contains(exitsRequiredForCurrentGridPosition[i]))
                    {
                        doesRoomMeetExitRequirements = true;
                    }
                    else
                    {
                        // If room doesn't contain exit, it's not a suitable room so stop checking it
                        doesRoomMeetExitRequirements = false;
                        break;
                    }
                }

                if (doesRoomMeetExitRequirements)
                {
                    // Put into list of suitable rooms
                    roomsWhichMeetTheExitRequirements.Add(roomName);
                }
            }
        }

        //PrintSuitableRoomsForSelection(roomsWhichMeetTheExitRequirements, exitsRequiredForCurrentGridPosition, currentGridPosition);

        return SelectRandomRoomWhichMeetsElementsRequirements(roomsWhichMeetTheExitRequirements, currentGridPosition);
    }

    string RandomRoomSelection(List<string> listOfRooms)
    {
        int selectRandomNwCornerRoom = UnityEngine.Random.Range(0, listOfRooms.Count);
        PrintInitialRoomSelectedToStartLevelGenerator(selectRandomNwCornerRoom);
        return listOfRooms[selectRandomNwCornerRoom];
    }

    // TODO: AVOID HARD CODING THE DEFAULT ROOM
    List<string> CreateDefaultRoomElementsForCurrentGridPosition(Vector2 currentGridPosition)
    {

        Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Creating Default Room For Grid Position: " + currentGridPosition);

        List<string> requiredRoomElements = new List<string>();

        if (currentGridPosition.y == 0) // First Column
        {
            Debug.Log("Default Room For First Column");
            // Enter Into The Grid at the current Grid Position A Default Requirement Of A wc
            requiredRoomElements.Add("wc");
            requiredRoomElements.Add("nc");
            requiredRoomElements.Add("sc");
            requiredRoomElements.Add("eo");
            requiredRoomElementsForGridPositions.Add(currentGridPosition, requiredRoomElements);
        }

        if (currentGridPosition.y > 0 && currentGridPosition.y < (gridLength - 1)) // In-between end columns
        {
            Debug.Log("Default Room For In-between Column");
            requiredRoomElements.Add("wo");
            requiredRoomElements.Add("nc");
            requiredRoomElements.Add("sc");
            requiredRoomElements.Add("eo");
            requiredRoomElementsForGridPositions.Add(currentGridPosition, requiredRoomElements);
        }

        if (currentGridPosition.y == (gridLength - 1)) // In-between end columns
        {
            Debug.Log("Default Room For Last Column");
            requiredRoomElements.Add("wo");
            requiredRoomElements.Add("nc");
            requiredRoomElements.Add("sc");
            requiredRoomElements.Add("ec");
            requiredRoomElementsForGridPositions.Add(currentGridPosition, requiredRoomElements);
        }

        PrintDeafultRoomCreated(currentGridPosition, requiredRoomElements);

        for (int i = 0; i < requiredRoomElements.Count; i++)
        {
            Debug.Log("Default Room Element Required: " + requiredRoomElements[i]);
        }

        return requiredRoomElements;
    }

    string SelectRandomRoomWhichMeetsElementsRequirements(List<string> roomsWhichMeetTheExitRequirements, Vector2 currentGridPosition)
    {
        Debug.Log("             In SelectRandomRoomWhichMeetsElementsRequirements               ");
        Debug.Log("             Selecting Room From roomsWhichMeetTheExitRequirements               ");
        Debug.Log(" List Size = " + roomsWhichMeetTheExitRequirements.Count);
        int selectRandomRoom = UnityEngine.Random.Range(0, roomsWhichMeetTheExitRequirements.Count);
        Debug.Log(" Random Number Generated = " + selectRandomRoom);
        PrintRoomSelectedAtGridPosition(roomsWhichMeetTheExitRequirements[selectRandomRoom], currentGridPosition);
        return roomsWhichMeetTheExitRequirements[selectRandomRoom];
    }


    // Step 2c: Collect the required exits for the adjoining rooms
    void CollectRequiredRoomElementsForAdjoiningRooms(Vector2 currentGridPosition, string currentRoom)
    {
        // New Iteration Of Loop Requires Empty List
        ClearRequiredRoomElementsForConnectingAdjoiningRoomList();
    
        string[] allSeparatedRoomElements = currentRoom.Split(roomNameSpearator, StringSplitOptions.RemoveEmptyEntries);

        List<string> roomElementsAfterFiltering = RemoveUnneededRoomElements(currentGridPosition, allSeparatedRoomElements);

        PrintRoomElementsAfterFiltering(roomElementsAfterFiltering);

        DetermineRequiredAdjoiningRoomElements(roomElementsAfterFiltering);
    }

    void ClearRequiredRoomElementsForConnectingAdjoiningRoomList()
    {
        requiredRoomElementsForConnectingAdjoiningRoom.Clear();
    }

    List<string> RemoveUnneededRoomElements(Vector2 currentGridPosition, string[] allSeparatedRoomElements)
    {
        List<string> listOfAllRoomNameElements = allSeparatedRoomElements.ToList();

        for (int i = 0; i < listOfAllRoomNameElements.Count; i++)
        {
            RemoveRoomTypeElement(i, listOfAllRoomNameElements);

            // Rules For Building The Grid With Outer Walls

            RemoveNorthClosedElementUnlessInTheTopRowOfGrid(i, currentGridPosition.x, listOfAllRoomNameElements);

            RemoveWestClosedElementUnlessInTheFirstColumnOfGrid(i, currentGridPosition.y, listOfAllRoomNameElements);

            RemoveSouthClosedElementUnlessInTheLastRowOfGrid(i, currentGridPosition.x, listOfAllRoomNameElements);

            RemoveEastClosedElementUnlessInTheLastColumnOfGrid(i, currentGridPosition.y, listOfAllRoomNameElements);
        }

        // Advanced Rules For Removing Exits Which Are Not Required

        RemoveEastExitRequirement(listOfAllRoomNameElements);

        return listOfAllRoomNameElements;
    }

    void RemoveRoomTypeElement(int i, List<string> listOfAllRoomNameElements)
    {
        if (i == 0)
        {
            PrintRemovingRoomTypeElement(listOfAllRoomNameElements[i]);
            listOfAllRoomNameElements.Remove(listOfAllRoomNameElements[i]);
        }
    }

    void RemoveNorthClosedElementUnlessInTheTopRowOfGrid(int i, float currentGridRow, List<string> listOfAllRoomNameElements)
    {
        if (currentGridRow != 0) // If Not In Top Row Of Grid
        {
            if (listOfAllRoomNameElements[i].Equals("nc")) // If Room Has A Closed North Exit Then Remove It (Not Required)
            {
                PrintRemovingRoomExitElement(listOfAllRoomNameElements[i]);
                listOfAllRoomNameElements.Remove(listOfAllRoomNameElements[i]);
            }
        }
    }

    void RemoveWestClosedElementUnlessInTheFirstColumnOfGrid(int i, float currentGridColumn, List<string> listOfAllRoomNameElements)
    {
        if (currentGridColumn != 0) // If Not In The First Column Of Grid
        {
            if (listOfAllRoomNameElements[i].Equals("wc")) // If Room Has A Closed West Exit Then Remove It (Not Required)
            {
                PrintRemovingRoomExitElement(listOfAllRoomNameElements[i]);
                listOfAllRoomNameElements.Remove(listOfAllRoomNameElements[i]);
            }
        }
    }

    void RemoveSouthClosedElementUnlessInTheLastRowOfGrid(int i, float currentGridRow, List<string> listOfAllRoomNameElements)
    {
        if (currentGridRow != (gridLength - 1)) // If Not In The Last Row Of Grid 
        {

            //Debug.Log("Checking If The Removal Of Non Needed sc Occurs At Grid Position: " + currentGridRow);

            if (listOfAllRoomNameElements[i].Contains("sc")) // If Room Has A Closed South Exit Then Remove It (Not Required)
            {
                PrintRemovingRoomExitElement(listOfAllRoomNameElements[i]);
                listOfAllRoomNameElements.Remove(listOfAllRoomNameElements[i]);
            }  
        }
    }

    void RemoveEastClosedElementUnlessInTheLastColumnOfGrid(int i, float currentGridColumn, List<string>  listOfAllRoomNameElements)
    {

        // THIS MAY NEED TO BE THE SECOND LAST COLUMN TO WORK- NEED TO CHECKS
        if (currentGridColumn != (gridLength - 1)) // If Not In The Last Column Of Grid
        {
            if (listOfAllRoomNameElements[i].Contains("ec")) // If Room Has A Closed East Exit Then Remove It (Not Required)
            {
                PrintRemovingRoomExitElement(listOfAllRoomNameElements[i]);
                listOfAllRoomNameElements.Remove(listOfAllRoomNameElements[i]);
            }
        }
    }

    void RemoveEastExitRequirement(List<string> listOfAllRoomNameElements)
    {
        if (listOfAllRoomNameElements.Count >= 2) // If The Room Contains 2 Or More Exits E.G "lk-nc-eo-sc-wo"
        {
            for (int i = 0; i < listOfAllRoomNameElements.Count; i++)
            {
                // Remove West Exits As They Will Trigger The Requirement Of An East Exit
                // As Of 8/12/2020
                // East Exits Are Not Required For Any Room Except For Rooms In The Last Column
                if (listOfAllRoomNameElements[i].Contains("wo")) 
                {
                    PrintRemovingRoomExitElement(listOfAllRoomNameElements[i]);
                    listOfAllRoomNameElements.Remove(listOfAllRoomNameElements[i]);
                }
            }
        }
    }

    void DetermineRequiredAdjoiningRoomElements(List<string> roomElementsAfterFiltering)
    {
        PrintMapCurrentExitsToAdjoiningExits();

        for (int i = 0; i < roomElementsAfterFiltering.Count; i++)
        {
            if (roomElementsAfterFiltering[i].Equals("nc") || roomElementsAfterFiltering[i].Equals("ec") || roomElementsAfterFiltering[i].Equals("sc") || roomElementsAfterFiltering[i].Equals("wc"))
            {
                PrintRoomElementAcceptedIntoRequiredElementsList(roomElementsAfterFiltering[i]);
                requiredRoomElementsForConnectingAdjoiningRoom.Add(roomElementsAfterFiltering[i]);
            }

            if (!roomElementsAfterFiltering[i].Equals("nc") && !roomElementsAfterFiltering[i].Equals("wc") && !roomElementsAfterFiltering[i].Equals("ec") && !roomElementsAfterFiltering[i].Equals("sc"))
            {
                PrintRoomElementWithTheMappedElement(roomElementsAfterFiltering[i], roomOpenExitsElementsMapper[roomElementsAfterFiltering[i]]);
                requiredRoomElementsForConnectingAdjoiningRoom.Add(roomOpenExitsElementsMapper[roomElementsAfterFiltering[i]]);
            }            
        }
     }

     void MapRequiredElementsForAjoiningRoomsToCorrectGridPositions(Vector2 currentGridPosition, List<string> roomElementsRequiredForAdjoiningRoom)
     {
        PrintMappingRequiredElementsForAdjoiningRooms();

        for (int i = 0; i < roomElementsRequiredForAdjoiningRoom.Count; i++)
        {
            // Reset To Current Grid Position On Each Iteration
            Vector2 calculatedGridPosition = currentGridPosition;

            // Grid Position Movement
            Vector2 gridPositionForNorthExit = new Vector2(1, 0);
            Vector2 gridPositionForWestExit = new Vector2(0, 1);

            // Possible if statement could reduce how often this code is checked
            // E.G Only is i == 0
            MapTopRowRoomsToHaveNorthClosedElement(i, calculatedGridPosition, gridPositionForWestExit, roomElementsRequiredForAdjoiningRoom);

            MapRoomsWhichRequireEastClosedElement(i, calculatedGridPosition, gridPositionForWestExit, roomElementsRequiredForAdjoiningRoom);

            MapBottomRowRoomsToHaveSouthClosedElement(i, calculatedGridPosition, gridPositionForWestExit, roomElementsRequiredForAdjoiningRoom);

            MapRoomsWhichRequireWestElementToGridPosition(i, calculatedGridPosition, gridPositionForWestExit, roomElementsRequiredForAdjoiningRoom);

        }

        //PrintDictionaryWithVector2KeyAndStringListValue(roomGridPositionsWithRequiredExits);

    }

    void MapTopRowRoomsToHaveNorthClosedElement(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForWestExit, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        if (calculatedGridPosition.x == 0)
        {
            if (roomElementsRequiredForAdjoiningRoom[i].Equals("nc"))
            {
                // The room to the right must also have a closed north exit 'nc'
                calculatedGridPosition += gridPositionForWestExit;
                PrintExitToGridPositionMapping(roomElementsRequiredForAdjoiningRoom[i], calculatedGridPosition, "MapTopRowRoomsToHaveNorthClosedElement");

                if (!requiredRoomElementsForGridPositions.ContainsKey(calculatedGridPosition))
                {
                    // List of exits for specific grid positions
                    List<string> gridPositionExitList = new List<string>();
                    gridPositionExitList.Add(roomElementsRequiredForAdjoiningRoom[i]);
                    requiredRoomElementsForGridPositions.Add(calculatedGridPosition, gridPositionExitList);
                }
            }
        }
    }

    void MapRoomsWhichRequireEastClosedElement(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForWestExit, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        // CODE NOT WORKING
        // The List roomElementsRequiredForAdjoiningRoom Must Contain 'ec' To Determine that the east side of grid needs an east wall
        if (calculatedGridPosition.y == (gridLength - 2)) // If second last column
        {
            if (roomElementsRequiredForAdjoiningRoom[i].Equals("ec"))
            {
                // The room to the right (Last Column In The Grid) must also have a closed north exit 'ec'
                calculatedGridPosition += gridPositionForWestExit;
                PrintExitToGridPositionMapping(roomElementsRequiredForAdjoiningRoom[i], calculatedGridPosition, "MapRoomsWhichRequireEastClosedElement");

                if (!requiredRoomElementsForGridPositions.ContainsKey(calculatedGridPosition))
                {
                    // List of exits for specific grid positions
                    List<string> gridPositionExitList = new List<string>();
                    gridPositionExitList.Add(roomElementsRequiredForAdjoiningRoom[i]);
                    requiredRoomElementsForGridPositions.Add(calculatedGridPosition, gridPositionExitList);
                }
                else
                {
                    requiredRoomElementsForGridPositions[calculatedGridPosition].Add(roomElementsRequiredForAdjoiningRoom[i]);
                }
            }
        }
    }

    void MapBottomRowRoomsToHaveSouthClosedElement(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForWestExit, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        if (calculatedGridPosition.x == (gridLength - 1))
        {
            if (roomElementsRequiredForAdjoiningRoom[i].Equals("sc"))
            {
                // The room to the right must also have a closed north exit 'sc'
                calculatedGridPosition += gridPositionForWestExit;
                PrintExitToGridPositionMapping(roomElementsRequiredForAdjoiningRoom[i], calculatedGridPosition, "MapBottomRowRoomsToHaveSouthClosedElement");

                if (!requiredRoomElementsForGridPositions.ContainsKey(calculatedGridPosition))
                {
                    // List of exits for specific grid positions
                    List<string> gridPositionExitList = new List<string>();
                    gridPositionExitList.Add(roomElementsRequiredForAdjoiningRoom[i]);
                    requiredRoomElementsForGridPositions.Add(calculatedGridPosition, gridPositionExitList);
                }
                else
                {
                    requiredRoomElementsForGridPositions[calculatedGridPosition].Add(roomElementsRequiredForAdjoiningRoom[i]);
                }
            }
        } 
    }

    void MapRoomsWhichRequireWestElementToGridPosition(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForWestExit, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        if (roomElementsRequiredForAdjoiningRoom[i].StartsWith("w") && !roomElementsRequiredForAdjoiningRoom[i].Equals("wc"))
        {
            // The room to the right must also have a closed north exit 'nc'
            calculatedGridPosition += gridPositionForWestExit;
            PrintExitToGridPositionMapping(roomElementsRequiredForAdjoiningRoom[i], calculatedGridPosition, "MapRoomsWhichRequireWestElementToGridPosition");

            if (!requiredRoomElementsForGridPositions.ContainsKey(calculatedGridPosition))
            {
                // List of exits for specific grid positions
                List<string> gridPositionExitList = new List<string>();
                gridPositionExitList.Add(roomElementsRequiredForAdjoiningRoom[i]);
                requiredRoomElementsForGridPositions.Add(calculatedGridPosition, gridPositionExitList);
            }
            else
            {
                requiredRoomElementsForGridPositions[calculatedGridPosition].Add(roomElementsRequiredForAdjoiningRoom[i]);
            }

        }
    }

    // ***************************************************************************
    //                        ROOM SAVING AND LOADING
    // ***************************************************************************

    void SaveRoomDetailsForLoading(string roomName, Vector2 worldPoint)
    {
        worldPositionAndRoomNameForLoadingLevel.Add(worldPoint, roomName);
    }

    GameObject[] loadRooms(Dictionary<Vector2, string> roomInfo)
    {
        // Initialise Array
        GameObject[] rooms = new GameObject[roomInfo.Count];

        // Get the Room Name from Dictionary

        // Parse first part of name and build string to determine what folder to load
        // cr/cr-nw
        // l/l-1
        // lk-nc-eo-sc-wo

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

    // ***************************************************************************
    //                              Debugging Methods
    // ***************************************************************************

    void PrintLevelGeneratorSetup()
    {
        if (levelGeneratorSetupDebuggingEnabled)
        {
            Debug.Log(" =========================== Setting Up Level Generator Rules ===========================");
        }
    }

    void PrintExitMapperCompletion()
    {
        if (levelGeneratorSetupDebuggingEnabled)
        {
            Debug.Log("Exit Mapper Generation Competed");
        }
    }

    void PrintStartAndEndRoomsGeneratedGridPositions(Vector2 startGridPosition, Vector2 endGridPosition)
    {
        if (levelGeneratorSetupDebuggingEnabled)
        {
            Debug.Log(" =========================== Setting Up Start And End Room Rules ===========================");
            Debug.Log("Start Room Grid Position Generated At: " + startGridPosition);
            Debug.Log("End Room Grid Position Generated At: " + endGridPosition);
        }
    }

    void PrintCornerRoomsGeneratedGridPositions(Vector2 nwGridPosition, Vector2 neGridPosition, Vector2 swGridPosition, Vector2 seGridPosition)
    {
        if (levelGeneratorSetupDebuggingEnabled)
        {
            Debug.Log(" =========================== Setting Up Corner Room Rules ===========================");
            Debug.Log("NW Room Grid Position Generated At: " + nwGridPosition);
            Debug.Log("NE Room Grid Position Generated At: " + neGridPosition);
            Debug.Log("SW Room Grid Position Generated At: " + swGridPosition);
            Debug.Log("SE Room Grid Position Generated At: " + seGridPosition);
        }
    }

    void PrintLevelGeneratorSetupCompleted()
    {
        if (levelGeneratorSetupDebuggingEnabled)
        {
            Debug.Log(" =========================== Level Generator Rules Setup Completed =========================== ");
        }
    }

    void PrintGridGenerationStarting()
    {
        if (levelGeneratorSetupDebuggingEnabled)
        {
            Debug.Log("--------------------------- Grid Generation Starting --------------------------- ");
        }
    }

    void PrintInitialRoomSelectedToStartLevelGenerator(int selectRandomNwCornerRoom)
    {
        if (gridGeneratorDebuggingEnabled && lowLevelDebuggingEnabled)
        {
            Debug.Log("Initial Corner Room Selected: " + nwCornerRooms[selectRandomNwCornerRoom]);
        }  
    }

    void PrintCurrentRoomAndGridPositionInformation(Vector2 currentGridPosition, string roomName)
    {
        if (true)
        {
            Debug.Log("**********     Current Grid Position: " + currentGridPosition + "    |    Current Room Selected: " + roomName + "     **********");
        }
    }


    void PrintRemovingRoomTypeElement(string roomTypeElement)
    {
        if (removingUnwantedRoomNameElementsDebuggingEnabled)
        {
            Debug.Log("Room Type Element Removed:  " + roomTypeElement);
        }
    }

    void PrintRemovingRoomExitElement(string roomExitElement)
    {
        if (removingUnwantedRoomNameElementsDebuggingEnabled)
        {
            Debug.Log("Room Exit Element Removed:  " + roomExitElement);
        }
    }

    void PrintRoomElementsAfterFiltering(List<string> filteredRoomElements)
    {
        if (removingUnwantedRoomNameElementsDebuggingEnabled)
        {
            string elements = "";

            foreach (string element in filteredRoomElements)
            {
                elements += element + " ";
            }

            Debug.Log("Room Elements After Filtering: " + elements);
        }
    }

    void PrintMapCurrentExitsToAdjoiningExits()
    {
        if (mapCurrentRoomElementsToAdjoiningRoomElementsDebuggingEnabled)
        {
            Debug.Log("=========================== Map Room Elements To Ajoining Room Elements ===========================");
        }
    }

    void PrintRoomElementWithTheMappedElement(string roomElementAfterFiltering, string roomElementMapped)
    {
        if (mapCurrentRoomElementsToAdjoiningRoomElementsDebuggingEnabled)
        {
            Debug.Log("Current Room Element: " + roomElementAfterFiltering + "     |     Adjoining Room Element: " + roomElementMapped);
        }
    }

    void PrintRoomElementAcceptedIntoRequiredElementsList(string roomElementAfterFiltering)
    {
        if (mapCurrentRoomElementsToAdjoiningRoomElementsDebuggingEnabled)
        {
            Debug.Log("Non Mappable But Required Room Element Added To Required Elements List: " + roomElementAfterFiltering);
        }
    }

    void PrintMappingRequiredElementsForAdjoiningRooms()
    {
        if (mapRoomElementsToAdjoiningRoomGridPositionDebuggingEnabled)
        {
            Debug.Log("=========================== Map Room Elements To Ajoining Room Grid Position ===========================");
        }
    }

    void PrintRoomSelectedAtGridPosition(string roomName, Vector2 gridPosition)
    {
        if (debuggingEnabled)
        {
            Debug.Log("Room Selected = " + roomName + " : Grid Position = " + gridPosition);
        }
    }

    void PrintExtractedExits()
    {
        if (true)
        {
            for (int k = 0; k < requiredRoomElementsForConnectingAdjoiningRoom.Count; k++)
            {
                Debug.Log("Extracted Required Exit: " + requiredRoomElementsForConnectingAdjoiningRoom[k]);
            }
        }
    }

    void PrintExitToGridPositionMapping(string exit, Vector2 gridPosition, string methodName)
    {
        if (true)
        {
            Debug.Log("In Method: " + methodName + "     |     Room Element: " + exit + "     |    Mapped To Grid Position " + gridPosition);
        }
    }

    void PrintDictionaryWithVector2KeyAndStringListValue(Dictionary<Vector2, List<string>> dictionary)
    {
        if (true)
        {
            //Debug.Log("=========================== Reading Grid Layout Dictionary =========================================");

            for (int i = 0; i < requiredRoomElementsForGridPositions.Count; i++)
            {
                dictionary.ElementAt(i).Value.ForEach(j => Debug.Log("Grid Position = " + dictionary.ElementAt(i).Key + " : Required Exit = " + j));
            }
        }
    }

    void PrintSuitableRoomsForSelection(List<string> suitableRooms, List<string> requiredExits, Vector2 currentGridPosition)
    {
        if (true)
        {
            Debug.Log("================== Exits Required For Grid Position " + currentGridPosition + " ==================");
            foreach (string exit in requiredExits)
            {
                Debug.Log("Exit this grid position must contain: " + exit);
            }

            Debug.Log("================== Rooms With Required Exits " + currentGridPosition + " ==================");
            foreach (string room in suitableRooms)
            {
                Debug.Log("Room which contains all exits required: " + room);
            }
        }
    }

    void PrintDeafultRoomCreated(Vector2 currentGridPosition, List<string> requiredRoomElements)
    {
        for (int i = 0; i < requiredRoomElements.Count; i++)
        {
            Debug.Log("Grid Position: " + currentGridPosition + "     |     Required Room Element: " + requiredRoomElements[i]);
        }
    }

    /*// Start Room Selector
        if (startAndEndRoomGridPositions[0] == currentGridPosition)
        {
            return startRooms[0];
        }

        // End Room Selector
        if (startAndEndRoomGridPositions[1] == currentGridPosition)
        {
            return endRooms[0];
        }*/


    // ----------------------------------------------------------------------------------
    // Debugging
    /* Debug.Log("Grid Position: " + currentGridPosition);
     Debug.Log("Room Name: " + roomName);
     string roomExitsExtracted = "";

     for (int j = 0; j < roomExits.Length; j++)
     {
         roomExitsExtracted += roomExits[j] + " ";
     }

     Debug.Log("Uncleaned Room Exits: " + roomExitsExtracted);*/
    // ----------------------------------------------------------------------------------
}
