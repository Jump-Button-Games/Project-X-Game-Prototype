using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Grid Generation Variables
    Dictionary<string, string> openElementsMapper;
    Rooms rooms;
    RoomElements roomElements;
    List<string> requiredRoomElementsForConnectingAdjoiningRoom = new List<string>();

    // Stores the required room elements for upcoming grid positions
    Dictionary<Vector2, List<string>> requiredRoomElementsForGridPositions = new Dictionary<Vector2, List<string>>();

    // Area Loading Variables
    Area area;

    // Grid Configuration
    GridConfiguration gridConfiguration;

    // Variables For Rules
    Vector2[] startAndEndRoomGridPositions = new Vector2[2];
    Vector2[] cornerRoomGridPositions = new Vector2[4];

    string currentRoom;

    // Debugging Purposes
    Dictionary<Vector2, string> gridPositionsWithCorrespondingRoomNames = new Dictionary<Vector2, string>();

    // Loading
    // Store prefabs for loading rooms
    readonly GameObject[] roomsGO;

    void Awake()
    {
        area = new Area();
        rooms = new Rooms();
        roomElements = new RoomElements();
        
        gridConfiguration = new GridConfiguration();

        populateOpenElementsMapper(roomElements.openElements, roomElements.adjoiningOpenElements);

        // Rules Setup
        StartAndEndRoomGridPositionRule();
        CornerRoomGridPositionRule();
    }

    // CODE CLEANED UP A GOOD BIT
    // NORTH SOUTH EXITS WORKING
    // ISSUE AGAIN WITH THE WEST AND EAST OF GRID NOT HAVING THE CLOSED EXITS THEY REQUIRE

    void Start()
    {
        for (int row = 0; row < GridConfiguration.size; row++)
        {
            for (int column = 0; column < GridConfiguration.size; column++)
            {
                Vector2 currentGridPosition = new Vector2(row, column);
                Vector2 worldPoint = CalculateWorldPositionBasedOnGridPosition(currentGridPosition);

                SelectRoomForCurrentGridPosition(currentGridPosition);

                Debug.Log("Current Grid Position: " + currentGridPosition + "   |   Current Room Selected: " + currentRoom);

                CollectRequiredRoomElementsForAdjoiningRooms(currentGridPosition, currentRoom);

                // Injecting Required Elements Missing From CollectRequiredRoomElementsForAdjoiningRooms
                // wc for (0,0) is being passed in here through the required.....Room list
                MapRequiredElementsForAjoiningRoomsToCorrectGridPositions(currentGridPosition, requiredRoomElementsForConnectingAdjoiningRoom);

                // Debugging Purposes
                gridPositionsWithCorrespondingRoomNames.Add(currentGridPosition, currentRoom);

                SaveRoomDetailsForLoading(worldPoint, currentRoom);
            }
        }

        // PrintContentsOfVector2AndStringDictionary(gridPositionsWithCorrespondingRoomNames);

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
    void populateOpenElementsMapper(List<string> openElements, List<string> adjoiningOpenElements)
    {
        openElementsMapper = new Dictionary<string, string>();

        for (int i = 0; i < openElements.Count; i++)
        {
            openElementsMapper.Add(openElements[i], adjoiningOpenElements[i]);
        }
    }

    // Step 1b: Rule Setup
    void StartAndEndRoomGridPositionRule()
    {
        int randomStartPosition = UnityEngine.Random.Range(1, (GridConfiguration.size - 1));
        Vector2 start = new Vector2(randomStartPosition, 0);

        int randomEndPosition = UnityEngine.Random.Range(1, (GridConfiguration.size - 1));
        Vector2 end = new Vector2(randomEndPosition, (GridConfiguration.size - 1));

        startAndEndRoomGridPositions[0] = start;
        startAndEndRoomGridPositions[1] = end;
    }

    void CornerRoomGridPositionRule()
    {
        // In the order the room will be iterated over in the loop

        // nw
        cornerRoomGridPositions[0] = new Vector2(0, 0);

        // ne
        cornerRoomGridPositions[1] = new Vector2(0, GridConfiguration.size - 1);

        // sw
        cornerRoomGridPositions[2] = new Vector2(GridConfiguration.size - 1, 0);

        // se
        cornerRoomGridPositions[3] = new Vector2(GridConfiguration.size - 1, GridConfiguration.size - 1);
    }

    /*
     *      STEP 2: GRID GENERATION
     */

    // Step 2a: Calculate World Positions For Rooms
    Vector2 CalculateWorldPositionBasedOnGridPosition(Vector2 currentGridPosition)
    {
        float wolrdPositionX = currentGridPosition.y * Area.roomLengthOnXAxis;
        float worldPositionY = Area.worldStartingPointOnYAxis - (Area.roomLengthOnYAxis * currentGridPosition.x);

        return new Vector2(wolrdPositionX, worldPositionY);
    }

    // Step 2b: Select a room for the current grid position
    void SelectRoomForCurrentGridPosition(Vector2 currentGridPosition)
    {
        if (currentGridPosition == gridConfiguration.startingPosition)
        {
            currentRoom = FirstRoomSelector();
        }
        else
        {
            currentRoom = AllOtherRoomsSelector(currentGridPosition);
        }
    }

    string FirstRoomSelector()
    {
        return RandomRoomSelection(rooms.nwCornerRooms);
    }

    // TODO: THIS ENTIRE METHOD NEEDS TO BE LOOK AT MORE CLOSELY AND CLEANED UP
    string AllOtherRoomsSelector(Vector2 currentGridPosition)
    {
        // North-East Corner Room
        if (cornerRoomGridPositions[1] == currentGridPosition)
        {
            return RandomRoomSelection(rooms.neCornerRooms);
        }

        // South-West Corner Room
        if (cornerRoomGridPositions[2] == currentGridPosition)
        {
            return RandomRoomSelection(rooms.swCornerRooms);
        }

        // South-East Corner Room
        if (cornerRoomGridPositions[3] == currentGridPosition)
        {
            return RandomRoomSelection(rooms.seCornerRooms);
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
            int tempRandomRoom = UnityEngine.Random.Range(0, rooms.linkRooms.Count);
            return rooms.linkRooms[tempRandomRoom];
        }
        else 
        {
            // Working on LinkedRooms for now but it will need to be able to handle any type of room it is provided
            foreach (string roomName in rooms.linkRooms)
            {
                bool doesRoomMeetExitRequirements = false;

                // loop though exitsRequiredForCurrentGridPosition and compare each to the room name
                for (int i = 0; i < exitsRequiredForCurrentGridPosition.Count; i++)
                {

                    // Changed single character elements to be double character elements 
                    // Should provide better visibility for the Contains method
                    // E.G n is now nm
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

        return SelectRandomRoomWhichMeetsElementsRequirements(roomsWhichMeetTheExitRequirements, currentGridPosition);
    }

    string RandomRoomSelection(List<string> listOfRooms)
    {
        int selectRandomNwCornerRoom = UnityEngine.Random.Range(0, listOfRooms.Count);
        return listOfRooms[selectRandomNwCornerRoom];
    }

    // TODO: AVOID HARD CODING THE DEFAULT ROOM
    List<string> CreateDefaultRoomElementsForCurrentGridPosition(Vector2 currentGridPosition)
    {
        List<string> requiredRoomElements = new List<string>();

        if (currentGridPosition.y == 0) // First Column
        {
            // Enter Into The Grid at the current Grid Position A Default Requirement Of A wc
            requiredRoomElements.Add("wc");
            requiredRoomElements.Add("nc");
            requiredRoomElements.Add("sc");
            requiredRoomElements.Add("eo");
            requiredRoomElementsForGridPositions.Add(currentGridPosition, requiredRoomElements);
        }

        if (currentGridPosition.y > 0 && currentGridPosition.y < (GridConfiguration.size - 1)) // In-between end columns
        {
            requiredRoomElements.Add("wo");
            requiredRoomElements.Add("nc");
            requiredRoomElements.Add("sc");
            requiredRoomElements.Add("eo");
            requiredRoomElementsForGridPositions.Add(currentGridPosition, requiredRoomElements);
        }

        if (currentGridPosition.y == (GridConfiguration.size - 1)) // In-between end columns
        {
            requiredRoomElements.Add("wo");
            requiredRoomElements.Add("nc");
            requiredRoomElements.Add("sc");
            requiredRoomElements.Add("ec");
            requiredRoomElementsForGridPositions.Add(currentGridPosition, requiredRoomElements);
        }

        return requiredRoomElements;
    }

    string SelectRandomRoomWhichMeetsElementsRequirements(List<string> roomsWhichMeetTheExitRequirements, Vector2 currentGridPosition)
    {
        int selectRandomRoom = UnityEngine.Random.Range(0, roomsWhichMeetTheExitRequirements.Count);

        return roomsWhichMeetTheExitRequirements[selectRandomRoom];
    }


    // Step 2c: Collect the required exits for the adjoining rooms
    void CollectRequiredRoomElementsForAdjoiningRooms(Vector2 currentGridPosition, string currentRoom)
    {
        // New Iteration Of Loop Requires Empty List
        ClearRequiredRoomElementsForConnectingAdjoiningRoomList();
    
        string[] separatedElements = currentRoom.Split(roomElements.separator, StringSplitOptions.RemoveEmptyEntries);

        List<string> separatedElementsFiltered = FilterUnneededElementsForAdjoiningRooms(currentGridPosition, separatedElements);

        // Inject Required Elements
        List<string> separatedElementsFilteredAndInjected = InjectRequiredElementsForAdjoiningRooms(currentGridPosition, separatedElementsFiltered);

        DetermineRequiredAdjoiningRoomElements(separatedElementsFilteredAndInjected);
    }

    void ClearRequiredRoomElementsForConnectingAdjoiningRoomList()
    {
        requiredRoomElementsForConnectingAdjoiningRoom.Clear();
    }

    // This method is the first step of filtering
    // It's removing / keeping elements essential to building the outer grid correctly
    /*List<string> RemoveUnneededRoomElementsForAdjoiningRoom(Vector2 currentGridPosition, string[] separatedElements)
    {
        List<string> separatedElementsList = separatedElements.ToList();

        RemoveRoomTypeElement(separatedElementsList);

        // Loop through list of Elements
        // Should Always Be 4
        for (int i = 0; i < separatedElementsList.Count; i++)
        {
            Debug.Log("Interation Count: " + (i + 1) + "    |   List Count: " + separatedElementsList.Count);

            string elements = "";
            foreach (string element in separatedElementsList)
            {
                elements += element + " ";
            }

            Debug.Log("Elements Still In separatedElementsList: " + elements);
            // Rules For Building The Grid With Outer Walls

            RemoveNorthClosedElementUnlessInTheTopRowOfGrid(i, currentGridPosition.x, separatedElementsList);

            RemoveWestClosedElementUnlessInTheFirstColumnOfGrid(i, currentGridPosition.y, separatedElementsList);

            // TESTING THIS CODE BEING COMMENTED OUT
            //RemoveSouthClosedElementUnlessInTheLastRowOfGrid(i, currentGridPosition.x, listOfAllRoomNameElements);

            RemoveEastClosedElementUnlessInTheLastColumnOfGrid(i, currentGridPosition.y, separatedElementsList);
        }

        // Advanced Rules For Removing Exits Which Are Not Required

        RemoveEastExitRequirementExceptForLastColumn(currentGridPosition.y, separatedElementsList);

        return separatedElementsList;
    }*/

    List<string> FilterUnneededElementsForAdjoiningRooms(Vector2 currentGridPosition, string[] separatedElements)
    {
        List<string> separatedElementsList = separatedElements.ToList();

        RemoveRoomTypeElement(separatedElementsList);

        // Keep Elements Required For The Outside Of Grid
        RemoveNorthClosedElementUnlessInTheTopRowOfGrid(currentGridPosition.x, separatedElementsList);

        // UNCLEAR WHETHER THIS METHOD WILL BE NEEDED OR NOT
        RemoveWestClosedElementUnlessInTheFirstColumnOfGrid(currentGridPosition.y, separatedElementsList);

        // UNCLEAR WHETHER THIS METHOD WILL BE NEEDED OR NOT
        RemoveEastClosedElementUnlessInTheLastColumnOfGrid(currentGridPosition.y, separatedElementsList);

        // Advanced Rules For Removing Exits Which Are Not Required

        RemoveEastExitRequirementExceptForLastColumn(currentGridPosition.y, separatedElementsList);

        return separatedElementsList;
    }

    void RemoveRoomTypeElement(List<string> separatedElementsList)
    {
        separatedElementsList.Remove(separatedElementsList[0]);
    }

    void RemoveNorthClosedElementUnlessInTheTopRowOfGrid(float currentGridRow, List<string> separatedElementsList)
    {
        if (separatedElementsList.Contains(RoomElements.nc) && currentGridRow != 0)
        {
            separatedElementsList.Remove(RoomElements.nc);
        }
    }

    void RemoveWestClosedElementUnlessInTheFirstColumnOfGrid(float currentGridColumn, List<string> separatedElementsList)
    {
        if (separatedElementsList.Contains(RoomElements.wc) && currentGridColumn != 0)
        {
            separatedElementsList.Remove(RoomElements.wc);
        }
    }


    // Algorithm might need to keep all sc's which are picked as this means an nc will be required
    /*void RemoveSouthClosedElementUnlessInTheLastRowOfGrid(int i, float currentGridRow, List<string> listOfAllRoomNameElements)
    {
        if (currentGridRow != (GridConfiguration.size - 1)) // If Not In The Last Row Of Grid 
        {
            if (listOfAllRoomNameElements[i].Contains("sc")) // If Room Has A Closed South Exit Then Remove It (Not Required)
            {
                listOfAllRoomNameElements.Remove(listOfAllRoomNameElements[i]);
            }
        }
    }*/

    void RemoveEastClosedElementUnlessInTheLastColumnOfGrid(float currentGridColumn, List<string> separatedElementsList)
    {
        if (separatedElementsList.Contains(RoomElements.ec) && currentGridColumn != (GridConfiguration.size - 1))
        {
            separatedElementsList.Remove(RoomElements.ec);
        }
    }

    void RemoveEastExitRequirementExceptForLastColumn(float currentGridColumn, List<string> separatedElementsList)
    {
        if (separatedElementsList.Contains(RoomElements.wo) && separatedElementsList.Count >= 2 && currentGridColumn != (GridConfiguration.size - 1)) // Possible this needs to be (- 2) not (- 1)
        {
            separatedElementsList.Remove(RoomElements.wo);
        }
    }

    List<string> InjectRequiredElementsForAdjoiningRooms(Vector2 currentGridPosition, List<string> separatedElementsFiltered)
    {
        // Inject West Wall
        if (currentGridPosition.y == 0 && (currentGridPosition.x > 0 && currentGridPosition.x < (GridConfiguration.size - 1)))
        {
            separatedElementsFiltered.Add(RoomElements.wc);
        }

        return separatedElementsFiltered;
    }

    void DetermineRequiredAdjoiningRoomElements(List<string> roomElementsAfterFiltering)
    {
        for (int i = 0; i < roomElementsAfterFiltering.Count; i++)
        {
            if (roomElementsAfterFiltering[i].Equals("nc") || roomElementsAfterFiltering[i].Equals("ec") || roomElementsAfterFiltering[i].Equals("sc") || roomElementsAfterFiltering[i].Equals("wc"))
            {
                requiredRoomElementsForConnectingAdjoiningRoom.Add(roomElementsAfterFiltering[i]);
            }

            if (!roomElementsAfterFiltering[i].Equals("nc") && !roomElementsAfterFiltering[i].Equals("wc") && !roomElementsAfterFiltering[i].Equals("ec") && !roomElementsAfterFiltering[i].Equals("sc"))
            {
                requiredRoomElementsForConnectingAdjoiningRoom.Add(openElementsMapper[roomElementsAfterFiltering[i]]);
            }            
        }
     }

    // Possible To Reduce Number Of Iterations Of Loop
    // First Check Which Part Of Grid We're In and Then Called Required Method E.G x == 0 MapTopRowRoomsToHaveNorthClosedElement
    // Then iterate within the method, can stop unncessary looping
    void MapRequiredElementsForAjoiningRoomsToCorrectGridPositions(Vector2 currentGridPosition, List<string> roomElementsRequiredForAdjoiningRoom)
     {
        for (int i = 0; i < roomElementsRequiredForAdjoiningRoom.Count; i++)
        {
            // Reset To Current Grid Position On Each Iteration
            Vector2 calculatedGridPosition = currentGridPosition;

            // Grid Position Movement
            Vector2 gridPositionForNorthElements = new Vector2(1, 0);
            Vector2 gridPositionForWestElements = new Vector2(0, 1);

            // Possible if statement could reduce how often this code is checked
            // E.G Only is i == 0
            // CAN RENAME LIKE EAST METHOD WAS

            // Can futher add to this method for room which have sc the room below must have an nc
            MapTopRowRoomsToHaveNorthClosedElement(i, calculatedGridPosition, gridPositionForWestElements, roomElementsRequiredForAdjoiningRoom);

            if (calculatedGridPosition.y == (GridConfiguration.size - 1))
            {
                MapEastClosedElementToGridPositionsWhichRequiresIt(i, calculatedGridPosition, gridPositionForWestElements, roomElementsRequiredForAdjoiningRoom);
            }

            // CAN RENAME LIKE EAST METHOD WAS
            MapBottomRowRoomsToHaveSouthClosedElement(i, calculatedGridPosition, gridPositionForWestElements, roomElementsRequiredForAdjoiningRoom);

            // West Elements 
            MapRequiredWestOpenElementsToGridPosition(i, calculatedGridPosition, gridPositionForWestElements, roomElementsRequiredForAdjoiningRoom);
            MapRequiredWestClosedElementsToGridPosition(i, calculatedGridPosition, gridPositionForNorthElements, roomElementsRequiredForAdjoiningRoom);

            // North Elements 
            MapRoomsRequiredNorthClosedElementsToGridPosition(i, calculatedGridPosition, gridPositionForNorthElements, roomElementsRequiredForAdjoiningRoom);
            MapRequireNorthOpenElemetsToGridPosition(i, calculatedGridPosition, gridPositionForNorthElements, roomElementsRequiredForAdjoiningRoom);
        }
    }

    void MapTopRowRoomsToHaveNorthClosedElement(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForWestExit, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        if (calculatedGridPosition.x == 0)
        {
            if (roomElementsRequiredForAdjoiningRoom[i].Equals("nc"))
            {
                // The room to the right must also have a closed north exit 'nc'
                calculatedGridPosition += gridPositionForWestExit;

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

    void MapEastClosedElementToGridPositionsWhichRequiresIt(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForWestExit, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        // Forcefully Add 'nc' if list doesn't already contain it
        if (!roomElementsRequiredForAdjoiningRoom.Contains("ec"))
        {
            calculatedGridPosition += gridPositionForWestExit;

            if (!requiredRoomElementsForGridPositions.ContainsKey(calculatedGridPosition))
            {
                List<string> gridPositionExitList = new List<string>();
                gridPositionExitList.Add("ec");
                requiredRoomElementsForGridPositions.Add(calculatedGridPosition, gridPositionExitList);
            }
            
        }
        else
        {
            if (!requiredRoomElementsForGridPositions.ContainsKey(calculatedGridPosition))
            {
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

    void MapBottomRowRoomsToHaveSouthClosedElement(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForWestExit, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        if (calculatedGridPosition.x == (GridConfiguration.size - 1))
        {
            if (roomElementsRequiredForAdjoiningRoom[i].Equals("sc"))
            {
                // The room to the right must also have a closed north exit 'sc'
                calculatedGridPosition += gridPositionForWestExit;

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

    void MapRequiredWestOpenElementsToGridPosition(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForWestExit, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        if (roomElementsRequiredForAdjoiningRoom[i].StartsWith("w") && !roomElementsRequiredForAdjoiningRoom[i].Equals("wc"))
        {
            // The room to the right must also have a closed north exit 'nc'
            calculatedGridPosition += gridPositionForWestExit;

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

    void MapRequiredWestClosedElementsToGridPosition(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForNorthElements, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        if (calculatedGridPosition.y == 0 && roomElementsRequiredForAdjoiningRoom[i].Equals(RoomElements.wc))
        {
            calculatedGridPosition += gridPositionForNorthElements; // Below Room Must Have West Exit

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

    void MapRoomsRequiredNorthClosedElementsToGridPosition(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForNorthClosedElement, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        if (roomElementsRequiredForAdjoiningRoom[i].Equals("sc"))
        {
            calculatedGridPosition += gridPositionForNorthClosedElement;

            if (!requiredRoomElementsForGridPositions.ContainsKey(calculatedGridPosition))
            {
                // List of exits for specific grid positions
                List<string> gridPositionExitList = new List<string>();
                gridPositionExitList.Add("nc");
                requiredRoomElementsForGridPositions.Add(calculatedGridPosition, gridPositionExitList);
            }
            else
            {
                requiredRoomElementsForGridPositions[calculatedGridPosition].Add("nc");
            }
        }
    }

    void MapRequireNorthOpenElemetsToGridPosition(int i, Vector2 calculatedGridPosition, Vector2 gridPositionForNorthExit, List<string> roomElementsRequiredForAdjoiningRoom)
    {
        if (roomElementsRequiredForAdjoiningRoom[i].Equals("nm"))
        {
            calculatedGridPosition += gridPositionForNorthExit;

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

    void SaveRoomDetailsForLoading(Vector2 worldPoint, string roomName)
    {
        area.pointAndRoomName.Add(worldPoint, roomName);
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

}
