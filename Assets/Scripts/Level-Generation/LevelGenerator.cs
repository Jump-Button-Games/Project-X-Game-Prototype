using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] rooms;

    // ************************************
    //          GENERIC EXIT CODES
    // ************************************

    List<string> exitCodes = new List<string> { "n", "no", "nne", "ne", "ene", "e", "eo", "ese", "se", "sse", "s", "so", "ssw", "sw", "wsw", "w", "wo", "wnw", "nw", "nnw" };
    List<string> exitCodesOpposites = new List<string> { "s", "so", "sse", "se", "wnw", "w", "wo", "wsw", "ne", "nne", "n", "no", "nnw", "nw", "ese", "e", "eo", "ene", "sw", "ssw" };

    // *********************************
    //          EXIT MAPPER
    // *********************************
    Dictionary<string, string> roomElementsMapper = new Dictionary<string, string>();

    // *******************************************
    //          FILTERED REQUIRED EXITS
    // *******************************************
    List<string> roomElementsRequiredForAdjoiningRoom = new List<string>();

    // *********************************
    //            ROOM CODES
    // *********************************

    // NW Room Codes
    List<string> nwCornerRooms = new List<string> { "cr-nc-eo-sc-wc" }; // , "cr-nc-ese-s-wc"

    // NE Room Codes
    List<string> neCornerRooms = new List<string> { "cr-nc-ec-sc-wo" };

    // SW Room Codes
    List<string> swCornerRooms = new List<string> { "cr-nc-eo-sc-wc" };

    // SE Room Codes
    List<string> seCornerRooms = new List<string> { "cr-nc-ec-sc-wo" };

    // Start and End Room Codes
    List<string> startRooms = new List<string> { "s-1" };
    List<string> endRooms = new List<string> { "e-1" };

    // Link Room Codes
    //List<string> linkRooms = new List<string> { "lk-nc-eo-sc-wo", "lk-nc-eo-s-wo", "lk-n-eo-sc-wo", "lk-nc-eo-sc-wc", "lk-nc-eo-sc-ec" }; // Add an wc to get next part of algo working
    List<string> linkRooms = new List<string> { "lk-nc-eo-sc-wo", "lk-n-eo-s-wo", "lk-nc-eo-sc-wc", "lk-n-eo-sc-wc", "lk-nc-eo-s-wo" };
    //List<string> linkRooms = new List<string> { "l-1", "l-2", "l-3", "l-4", "l-5", "l-6", "l-7", "l-8", "l-9", "l-10", "l-11", "l-12", "l-13", "l-14", "l-15", "l-16" };


    // Contains The World Position and Room Name To Place In That World Position
    Dictionary<Vector2, string> worldPositionAndRoomNameForLoadingLevel = new Dictionary<Vector2, string>();

    // This will be used to remember what type of rooms will be needed in the next position in the grid or down further levels in the grid 
    Dictionary<Vector2, List<string>> roomGridPositionsWithRequiredExits = new Dictionary<Vector2, List<string>>();

    Dictionary<Vector2, string> roomNameToGridPosition = new Dictionary<Vector2, string>();

    // Starting position in world
    // The NW corner room will have the coordinate (0, -0.5f)
    float yStartingPoint = -0.5f;

    const float xRoomLength = 16f;
    const float yRoomHeight = 9f;

    // Keep as instance variable as it could be used throught the whole class
    const int gridLength = 5;

    // *******************
    // Variables For Rules
    // *******************

    Vector2[] startAndEndRoomGridPositions = new Vector2[2];
    Vector2[] cornerRoomGridPositions = new Vector2[4];

    Vector2 gridStartingPosition;
    string roomName;

    public bool levelGeneratorSetupDebuggingEnabled = false;
    public bool gridGeneratorDebuggingEnabled = false;
    public bool removingUnwantedRoomNameElementsDebuggingEnabled = false;
    public bool mapCurrentRoomElementsToAdjoiningRoomElementsDebuggingEnabled = false;
    public bool mapRoomElementsToAdjoiningRoomGridPositionDebuggingEnabled = false;
    public bool debuggingEnabled = false;
    public bool lowLevelDebuggingEnabled = false; 

    // Used To Break Room Names Into Indiviual Strings
    string[] roomNameSpearator = { "-" };


    // DO WE ALWAYS REQUIRE AN EO EXIT UNLESS WE'RE AT THE RIGHT HAND SIDE OF GRID?

    // When the algorithm has a room with more than one exit it finds the opposite required exits for the next room E.g Room A has eo, Room B needs wo, Room B however has 2 exits (eo and wo)
    // Room C needs a wo to connect ot Room B's eo but it does not NEED an eo, currently the algorithm thinks Room C needs an eo as well as an so


    // ALGORITHM SEEMS TO WORK WITH BASIC ROOM WHICH HAS A WO AND EO
    // NEED TO ADD IN A ROOM WHICH ALSO CONTAINS AN 'S' AND A CONNECTING ROOM WHICH CONTAINS AN 'N'
    // SOME METHODS NEED TO BE FILLED OUT TO HANDLE THIS
    // THE BOTTOM CORNER ROOMS ARE NOT BEING FILLED OUT
    // THE START AND END ROOM ARE NOT BEING ADDED

    void Awake()
    {
        PrintLevelGeneratorSetup();

        ExitMapper();

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

                // Determine Grid and World Positions
                Vector2 currentGridPosition = new Vector2(row, column);
                Vector2 worldPosition = CalculateWorldPositionBasedOnGridPosition(row, column);

                // Select A Hand Built Room For Current Grid Position
                if (currentGridPosition == gridStartingPosition)
                {
                    roomName = FirstRoomSelector();
                }
                else
                {
                    // This selects a room for the current grid position based on exits required (exitsRequiredForConectingThisRoom)
                    // Exits required for grid Positions are generated from MapRequiredExitsToCorrectGridPosition
                    roomName = RoomSelector(currentGridPosition);

                }

                PrintCurrentRoomAndGridPositionInformation(currentGridPosition, roomName);

                // exitsRequiredForConectingThisRoom is Extremely Important as the correct Room will be selected on its contents
                // If those exits are incorrect then the room slected has a high chance or being in correct and producing unplayable paths in the grid
                // Not a perfeclty accurate name "roomElementsRequiredForAdjoiningRoom" need to check how it lines up with the code down the line
                roomElementsRequiredForAdjoiningRoom = ExtractRequiredRoomNameElements(currentGridPosition, roomName);

                PrintExtractedExits();

                // Predicts which exits will be required for future grid positions
                MapRequiredExitsToCorrectGridPosition(currentGridPosition, roomElementsRequiredForAdjoiningRoom);

                // Think this will be passed into SaveRoomDetailsForLoading method
                roomNameToGridPosition.Add(currentGridPosition, roomName);

                SaveRoomDetailsForLoading(roomName, worldPosition);
/*
                if (column == 1)
                {
                    break;
                }*/

            }

            /*if (row == 1)
            {
                break;
            }*/
            
        }

        PrintRoomNameToGridPosition();

        //PrintDictionaryWithVector2KeyAndStringList();

        // THIS MIGHT BE ABLE TO BE PUT INTO A SINGLE METHOD TO CLEAN IT UP
        // THIS IS WORKING CODE FROM THE FIRST VERSION OF THE ALGORITHM
        // By this stage, we have a dictionary with the worldPoint / worldPosition as the key
        // And the roomName / Room Code Name as the value
        // This dictionary holds a theoretical "Area" but needs to be passed to a function to 
        // make it 'real' and print to screen

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

    // ***************************************************************************
    //                              EXIT MAPPER
    // ***************************************************************************

    void ExitMapper()
    {
        for (int i = 0; i < exitCodes.Count; i++)
        {
            roomElementsMapper.Add(exitCodes[i], exitCodesOpposites[i]);
        }

        PrintExitMapperCompletion();
    }

    // ***************************************************************************
    //                                  RULES
    // ***************************************************************************

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
        // These are set up in the order that will be iterated over in the loop

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


    // ***************************************************************************
//                     GRID POSITION TO WORLD POSITION CALCULATOR
    // ***************************************************************************

    Vector2 CalculateWorldPositionBasedOnGridPosition(float i, float j)
    {
        float wolrdPositionX = j * xRoomLength;
        float worldPositionY = yStartingPoint - (yRoomHeight * i);

        return new Vector2(wolrdPositionX, worldPositionY);
    }

    // ***************************************************************************
    //                              ROOM SELECTORS
    // ***************************************************************************

    string FirstRoomSelector()
    {
        int selectRandomNwCornerRoom = UnityEngine.Random.Range(0, nwCornerRooms.Count);
        PrintInitialRoomSelectedToStartLevelGenerator(selectRandomNwCornerRoom);
        return nwCornerRooms[selectRandomNwCornerRoom];
    }

    // TODO: THIS ENTIRE METHOD NEEDS TO BE LOOK AT MORE CLOSELY AND CLEANED UP
    string RoomSelector(Vector2 currentGridPosition)
    {
        // North-East Corner Room
        if (cornerRoomGridPositions[1] == currentGridPosition)
        {
            int selectRandomNeCornerRoom = UnityEngine.Random.Range(0, neCornerRooms.Count);
            PrintRoomSelectedAtGridPosition(neCornerRooms[selectRandomNeCornerRoom], currentGridPosition);
            return neCornerRooms[selectRandomNeCornerRoom];
        }

        // South-West Corner Room
        if (cornerRoomGridPositions[2] == currentGridPosition)
        {
            int selectRandomSwCornerRoom = UnityEngine.Random.Range(0, swCornerRooms.Count);
            PrintRoomSelectedAtGridPosition(neCornerRooms[selectRandomSwCornerRoom], currentGridPosition);
            return swCornerRooms[selectRandomSwCornerRoom];
        }

        // South-East Corner Room
        if (cornerRoomGridPositions[3] == currentGridPosition)
        {
            int selectRandomSeCornerRoom = UnityEngine.Random.Range(0, seCornerRooms.Count);
            PrintRoomSelectedAtGridPosition(neCornerRooms[selectRandomSeCornerRoom], currentGridPosition);
            return seCornerRooms[selectRandomSeCornerRoom];
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

        List<string> roomsWhichMeetTheExitRequirements = new List<string>();
        List<string> exitsRequiredForCurrentGridPosition;

        // This Dictionary contains the mapping of the grid position to the list of exits it may require
        if (roomGridPositionsWithRequiredExits.ContainsKey(currentGridPosition))
        {
            //Debug.Log("Key Already Exists For Grid Position: " + currentGridPosition);
            exitsRequiredForCurrentGridPosition = roomGridPositionsWithRequiredExits[currentGridPosition];
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

                    Debug.Log("====== LOOKING FOR SC: Grid Position: " + currentGridPosition + "     |     Exit Required: " + exitsRequiredForCurrentGridPosition[i]);

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

        return SelectRandomRoomWhichMeetsRequirements(roomsWhichMeetTheExitRequirements, currentGridPosition);
    }

    // TODO: AVOID HARD CODING THE DEFAULT ROOM
    List<string> CreateDefaultRoomElementsForCurrentGridPosition(Vector2 currentGridPosition)
    {

        Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Creating Default Room For Grid Position: " + currentGridPosition + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

        List<string> requiredRoomElements = new List<string>();

        if (currentGridPosition.y == 0) // First Column
        {
            // Enter Into The Grid at the current Grid Position A Default Requirement Of A wc
            requiredRoomElements.Add("wc");
            requiredRoomElements.Add("nc");
            roomGridPositionsWithRequiredExits.Add(currentGridPosition, requiredRoomElements);

           /* if (currentGridPosition.x == 0)
            {
                requiredRoomElements.Add("sc");
                roomGridPositionsWithRequiredExits.Add(currentGridPosition, requiredRoomElements);
            }*/
        }

        PrintDeafultRoomCreated(currentGridPosition, requiredRoomElements);

        return requiredRoomElements;
    }

    string SelectRandomRoomWhichMeetsRequirements(List<string> roomsWhichMeetTheExitRequirements, Vector2 currentGridPosition)
    {
        int selectRandomRoom = UnityEngine.Random.Range(0, roomsWhichMeetTheExitRequirements.Count);
        PrintRoomSelectedAtGridPosition(roomsWhichMeetTheExitRequirements[selectRandomRoom], currentGridPosition);
        return roomsWhichMeetTheExitRequirements[selectRandomRoom];
    }

    // ***************************************************************************
    //                              EXIT PREDICTOR
    // ***************************************************************************

    List<string> ExtractRequiredRoomNameElements(Vector2 currentGridPosition, string roomName)
    {
        string[] allRoomNameElementsSeparated = roomName.Split(roomNameSpearator, StringSplitOptions.RemoveEmptyEntries);

        List<string> roomElementsAfterFiltering = RemoveUnwantedRoomNameElements(currentGridPosition, allRoomNameElementsSeparated);

        PrintRoomElementsAfterFiltering(roomElementsAfterFiltering);

        List<string> requiredConnectingExits = new List<string>();

        MapCurrentRoomElementsToAdjoiningRoomElements(roomElementsAfterFiltering, requiredConnectingExits);

        return requiredConnectingExits;
    }

    List<string> RemoveUnwantedRoomNameElements(Vector2 currentGridPosition, string[] allRoomNameElements)
    {
        List<string> listOfAllRoomNameElements = allRoomNameElements.ToList();

        //Debug.Log("Checking If The Removal Of Non Needed sc Occurs At Grid Position: " + currentGridPosition);

        //Debug.Log("Before Loop Triggers, List Count Is: " + listOfAllRoomNameElements.Count);

        // Is there a bug here
        for (int i = 0; i < listOfAllRoomNameElements.Count; i++)
        {
            RemoveRoomTypeElement(i, listOfAllRoomNameElements);

            // Rules For Building The Grid With Outer Walls

            RemoveNorthClosedElementUnlessInTheTopRowOfGrid(i, currentGridPosition.x, listOfAllRoomNameElements);

            RemoveWestClosedElementUnlessInTheFirstColumnOfGrid(i, currentGridPosition.y, listOfAllRoomNameElements);

            RemoveSouthClosedElementUnlessInTheLastRowOfGrid(i, currentGridPosition.x, listOfAllRoomNameElements);

            RemoveEastClosedElementUnlessInTheLastColumnOfGrid(i, currentGridPosition.y, listOfAllRoomNameElements);

            //Debug.Log("End Of Loop Iteration, List Count Is: " + listOfAllRoomNameElements.Count);
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
                // East Exits Are Not Required For Any Room Except For Rooms In The First Column
                if (listOfAllRoomNameElements[i].Contains("wo")) 
                {
                    PrintRemovingRoomExitElement(listOfAllRoomNameElements[i]);
                    listOfAllRoomNameElements.Remove(listOfAllRoomNameElements[i]);
                }
            }
        }
    }

    void MapCurrentRoomElementsToAdjoiningRoomElements(List<string> roomElementsAfterFiltering, List<string> requiredConnectingExits)
    {
        PrintMapCurrentExitsToAdjoiningExits();

        for (int i = 0; i < roomElementsAfterFiltering.Count; i++)
        {
            if (roomElementsAfterFiltering[i].Equals("nc") || roomElementsAfterFiltering[i].Equals("ec") || roomElementsAfterFiltering[i].Equals("sc") || roomElementsAfterFiltering[i].Equals("wc"))
            {
                PrintRoomElementAcceptedIntoRequiredElementsList(roomElementsAfterFiltering[i]);
                requiredConnectingExits.Add(roomElementsAfterFiltering[i]);
            }

            if (!roomElementsAfterFiltering[i].Equals("nc") && !roomElementsAfterFiltering[i].Equals("wc") && !roomElementsAfterFiltering[i].Equals("ec") && !roomElementsAfterFiltering[i].Equals("sc"))
            {
                PrintRoomElementWithTheMappedElement(roomElementsAfterFiltering[i], roomElementsMapper[roomElementsAfterFiltering[i]]);
                requiredConnectingExits.Add(roomElementsMapper[roomElementsAfterFiltering[i]]);
            }            
        }
     }

     void MapRequiredExitsToCorrectGridPosition(Vector2 currentGridPosition, List<string> roomElementsRequiredForAdjoiningRoom)
     {
        /*There are only 4 directions a position can go. A position can only be 1 away from the source. N,E,S,W

        Use current grid position to guide where the required exit should go. E.G currentGridPosition = (0,0) and requires a room which has a West Exit so they can connect to each other
        Therefore, a West Exit means the room to the right of (0,0) needs to have a West exit, (0,1) needs to have a West Exit To Connect To (0,0)
        Therefore, a South Exit means the room below needs to have a North exit, If (0,1) has a south exit then room (1,1) must have a north exit to connect to it

        Check the dictionary for previous entry, if no entry add one, if an entry get list of exits and add to list

        Loop through the list of required exits and calculate which grid position needs that exit, this example is using grid position (0,0) and room is lk-nc-eo-s-wo
        After exit extraction and mapping then the exits required for room lk-nc-eo-s-wo are 'wo' and 'n'
        This means, room (0,1) needs to have 'wo'
        and room (1,0) needs to have 'n'*/

        // Possibility For This Part Of The Code
        // Recognise What Grid Position It Is
        // If It Needs An Outer Wall 'nc' Directly Insert Into The List First

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

            //MapFirstColumnRoomsToHaveWestClosedElement(i, calculatedGridPosition, gridPositionForWestExit, roomElementsRequiredForAdjoiningRoom);

            MapBottomRowRoomsToHaveSouthClosedElement(i, calculatedGridPosition, gridPositionForWestExit, roomElementsRequiredForAdjoiningRoom);

            MapRoomsWhichRequireWestElementToGridPosition(i, calculatedGridPosition, gridPositionForWestExit, roomElementsRequiredForAdjoiningRoom);

           

            

            

            // West Closed Exit Rule
            /*if (currentGridPosition.y == 0) // (1,0)
            {
                if (exitsRequiredForConnectingRooms[i].Equals("wc"))
                {

                }
            }*/


            // If a north exit is required to connect to (0,0)
            // May Need Editing For Future New North Exits
            /*if (roomElementsRequiredForAdjoiningRoom[i].Equals("n")) // && currentGridPosition.x > 0
            {
                // Then the 'n' must be passed into the requiredExits List for Grid Position (1,0)
                calculatedGridPosition += gridPositionForNorthExit;

                PrintExitToGridPositionMapping(roomElementsRequiredForAdjoiningRoom[i], calculatedGridPosition);

                if (!roomGridPositionsWithRequiredExits.ContainsKey(calculatedGridPosition))
                {
                    List<string> gridPositionExitList = new List<string>();
                    gridPositionExitList.Add(roomElementsRequiredForAdjoiningRoom[i]);
                    roomGridPositionsWithRequiredExits.Add(calculatedGridPosition, gridPositionExitList);
                }
            }

            // If a west exit is required to connect to (0,0)
            if (roomElementsRequiredForAdjoiningRoom[i].StartsWith("w")) // && currentGridPosition.y > 0
            {
                // Then the 'w' must be passed into the requiredExits List for Grid Position (0,1)
                calculatedGridPosition += gridPositionForWestExit;

                PrintExitToGridPositionMapping(roomElementsRequiredForAdjoiningRoom[i], calculatedGridPosition);

                if (!roomGridPositionsWithRequiredExits.ContainsKey(calculatedGridPosition))
                {
                    // List of exits for specific grid positions
                    List<string> gridPositionExitList = new List<string>();
                    gridPositionExitList.Add(roomElementsRequiredForAdjoiningRoom[i]);
                    roomGridPositionsWithRequiredExits.Add(calculatedGridPosition, gridPositionExitList);
                }
            }*/
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

                if (!roomGridPositionsWithRequiredExits.ContainsKey(calculatedGridPosition))
                {
                    // List of exits for specific grid positions
                    List<string> gridPositionExitList = new List<string>();
                    gridPositionExitList.Add(roomElementsRequiredForAdjoiningRoom[i]);
                    roomGridPositionsWithRequiredExits.Add(calculatedGridPosition, gridPositionExitList);
                }
            }
        }
    }

    void MapBottomRowRoomsToHaveSouthClosedElement(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForWestExit, List<string> roomElementsRequiredForAdjoiningRoom)
    {

        //PrintExitToGridPositionMapping(roomElementsRequiredForAdjoiningRoom[i], calculatedGridPosition, "MapBottomRowRoomsToHaveSouthClosedElement");
        Debug.Log("Before Assigning Exit To Grid Position, Grid Pos: " + calculatedGridPosition + "     |     ");

        if (calculatedGridPosition.x == (gridLength - 1))
        {
            if (roomElementsRequiredForAdjoiningRoom[i].Equals("sc"))
            {
                // The room to the right must also have a closed north exit 'sc'
                calculatedGridPosition += gridPositionForWestExit;
                PrintExitToGridPositionMapping(roomElementsRequiredForAdjoiningRoom[i], calculatedGridPosition, "MapBottomRowRoomsToHaveSouthClosedElement");

                if (!roomGridPositionsWithRequiredExits.ContainsKey(calculatedGridPosition))
                {
                    // List of exits for specific grid positions
                    List<string> gridPositionExitList = new List<string>();
                    gridPositionExitList.Add(roomElementsRequiredForAdjoiningRoom[i]);
                    roomGridPositionsWithRequiredExits.Add(calculatedGridPosition, gridPositionExitList);
                }
                else // Need an else here in case an entry to the dictionary already exists?
                {
                    roomGridPositionsWithRequiredExits[calculatedGridPosition].Add(roomElementsRequiredForAdjoiningRoom[i]);
                }
            }
        } 

        // Print Out Dictionary
        if (calculatedGridPosition.x == 4 && calculatedGridPosition.y == 0)
        {
            PrintDictionaryWithVector2KeyAndStringListValue(roomGridPositionsWithRequiredExits);
        }
    }

    // DOESN'T LOOK LIKE THE PLACE FOR GUARNTEEING THAT THE FIRST COLUMN HAS A WEST EXIT 
    // IT'S NOT A DEFAULT ROOM NEEDED TO BE CREATED
    /*void MapFirstColumnRoomsToHaveWestClosedElement(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForWestExit, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        if (calculatedGridPosition.y == 0)
        {
            if (roomElementsRequiredForAdjoiningRoom[i].Equals("wc"))
            { 
            
            }
        }
    }*/

    void MapRoomsWhichRequireWestElementToGridPosition(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForWestExit, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        if (roomElementsRequiredForAdjoiningRoom[i].StartsWith("w") && !roomElementsRequiredForAdjoiningRoom[i].Equals("wc"))
        {
            // The room to the right must also have a closed north exit 'nc'
            calculatedGridPosition += gridPositionForWestExit;
            PrintExitToGridPositionMapping(roomElementsRequiredForAdjoiningRoom[i], calculatedGridPosition, "MapRoomsWhichRequireWestElementToGridPosition");

            if (!roomGridPositionsWithRequiredExits.ContainsKey(calculatedGridPosition))
            {
                // List of exits for specific grid positions
                List<string> gridPositionExitList = new List<string>();
                gridPositionExitList.Add(roomElementsRequiredForAdjoiningRoom[i]);
                roomGridPositionsWithRequiredExits.Add(calculatedGridPosition, gridPositionExitList);
            }
            else
            {
                roomGridPositionsWithRequiredExits[calculatedGridPosition].Add(roomElementsRequiredForAdjoiningRoom[i]);
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
            for (int k = 0; k < roomElementsRequiredForAdjoiningRoom.Count; k++)
            {
                Debug.Log("Extracted Required Exit: " + roomElementsRequiredForAdjoiningRoom[k]);
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

            for (int i = 0; i < roomGridPositionsWithRequiredExits.Count; i++)
            {
                dictionary.ElementAt(i).Value.ForEach(j => Debug.Log("Grid Position = " + dictionary.ElementAt(i).Key + " : Required Exit = " + j));
            }
        }
    }

    void PrintRoomNameToGridPosition()
    {
        if (true)
        {
            for (int i = 0; i < roomNameToGridPosition.Count; i++)
            {
                Debug.Log("Grid Position = " + roomNameToGridPosition.ElementAt(i).Key + " : Room Selected = " + roomNameToGridPosition.ElementAt(i).Value);
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
