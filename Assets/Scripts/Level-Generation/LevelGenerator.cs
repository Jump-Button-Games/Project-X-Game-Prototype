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
    List<string> elementsForAdjoiningGridPositions = new List<string>();

    // Stores the required room elements for upcoming grid positions
    Dictionary<Vector2, List<string>> mappedElementsToGridPositions = new Dictionary<Vector2, List<string>>();

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

    // REFACTORED A LOT OF CODE
    // TWO COMMON ISSUES WHICH NEED TO BE SORTED
    // 1. SOME ROWS OF ROOM HAVE NO SOUTH EXIT AND THEREFORE INACCESSIBLE TO THE PLAYER
    // 2. 'ec' ELEMENTS ARE BEING PICKED FOR MIDDLE GRID POSITIONS, CURRENTLY NOT HANDLED IN THE CODE, THIS CUTS OFF ROOMS FROM PLAYER 
    //     AND HAS EXITS LEADING TO NO WHERE 'wo' -> 'ec' (NOT POSSIBLE TO DO THIS IN GAME)

    // ONE BUG TO BE SORTED
    // SOMETIMES AN INDEX OUT OF BOUNDS EXCEPTION IS THROWN, SEEMS AS IF IT CAN'T FIND A ROOM TO SELECT
    void Start()
    {
        for (int row = 0; row < GridConfiguration.size; row++)
        {
            for (int column = 0; column < GridConfiguration.size; column++)
            {
                Vector2 currentGridPosition = new Vector2(row, column);
                Vector2 worldPoint = CalculateWorldPositionBasedOnGridPosition(currentGridPosition);

                // Room Selected Here Will Fill Current Grid Position
                // The Elements Of This Room Will Be Used To Determine What Elements Are Required For Its Adjoining Rooms
                SelectRoomForCurrentGridPosition(currentGridPosition);

                Debug.Log("Current Grid Position: " + currentGridPosition + "   |   Current Room Selected: " + currentRoom);

                // These Methods Are To Collect The Correct Elements Required And Map Them To The Correct Grid Positions
                // On The Next Iteration The Elements Will Be Checked And Will Tell The Room Selector What Elements Are Absolutely Required For The Current Grid Position
                // When This Methods Are Called, The Current Room Has ALREADY Been Chosen
                // They Function To Determine The Required Elements For The NEXT Room
                CollectRequiredElementsForAdjoiningRooms(currentGridPosition, currentRoom); // WE HAVE THE REQUIRED ELEMENTS
                //MapElementsForAdjoiningRoomsToGridPositions(currentGridPosition, requiredElementsForAdjoiningGridPositions);
                MapRequiredElementsForAdjoiningRoomsToGridPositions(currentGridPosition); // NOW MAP THE ELEMENTS , requiredElementsForAdjoiningGridPositions

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
        List<string> elementsRequiredForCurrentGridPosition;

        // This Dictionary contains the mapping of the grid position to the list of exits it may require
        if (mappedElementsToGridPositions.ContainsKey(currentGridPosition))
        {
            elementsRequiredForCurrentGridPosition = mappedElementsToGridPositions[currentGridPosition];
        }
        else
        {
            // No exits to check
            // This happens when on (1,0) where (0,0) has no south exit, need a default for these Rooms
            //// TODO: STRAIGHT UP HARD CODING THE DEFAULT ROOM CURRENTLY, FIND BETTER SOLUTION
            elementsRequiredForCurrentGridPosition = CreateDefaultRoomElementsForCurrentGridPosition(currentGridPosition);
        }

        // -------------------------------------------
        // Need A Better Way To Handle This NULL Check
        // -------------------------------------------
        if (elementsRequiredForCurrentGridPosition == null)
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
                for (int i = 0; i < elementsRequiredForCurrentGridPosition.Count; i++)
                {

                    // Changed single character elements to be double character elements 
                    // Should provide better visibility for the Contains method
                    // E.G n is now nm
                    if (roomName.Contains(elementsRequiredForCurrentGridPosition[i]))
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
            mappedElementsToGridPositions.Add(currentGridPosition, requiredRoomElements);
        }

        if (currentGridPosition.y > 0 && currentGridPosition.y < (GridConfiguration.size - 1)) // In-between end columns
        {
            requiredRoomElements.Add("wo");
            requiredRoomElements.Add("nc");
            requiredRoomElements.Add("sc");
            requiredRoomElements.Add("eo");
            mappedElementsToGridPositions.Add(currentGridPosition, requiredRoomElements);
        }

        if (currentGridPosition.y == (GridConfiguration.size - 1)) // In-between end columns
        {
            requiredRoomElements.Add("wo");
            requiredRoomElements.Add("nc");
            requiredRoomElements.Add("sc");
            requiredRoomElements.Add("ec");
            mappedElementsToGridPositions.Add(currentGridPosition, requiredRoomElements);
        }

        return requiredRoomElements;
    }

    string SelectRandomRoomWhichMeetsElementsRequirements(List<string> roomsWhichMeetTheExitRequirements, Vector2 currentGridPosition)
    {
        int selectRandomRoom = UnityEngine.Random.Range(0, roomsWhichMeetTheExitRequirements.Count);

        return roomsWhichMeetTheExitRequirements[selectRandomRoom];
    }


    // Step 2c: Collect the required exits for the adjoining rooms
    void CollectRequiredElementsForAdjoiningRooms(Vector2 currentGridPosition, string currentRoom)
    {
        ClearRequiredRoomElementsForConnectingAdjoiningRoomList();
        string[] separatedElements = currentRoom.Split(roomElements.separator, StringSplitOptions.RemoveEmptyEntries);

        // Elements Which Connect To Adjoining Rooms
        List<string> openElements = ExtractOpenElements(currentGridPosition, separatedElements);

        // Inject Closed Elements For Outer Walls
        List<string> openAndClosedElements = InjectClosedElementsForOuterWalls(currentGridPosition, openElements);

        DetermineRequiredAdjoiningRoomElements(openAndClosedElements);
    }

    void ClearRequiredRoomElementsForConnectingAdjoiningRoomList()
    {
        // New Iteration Of Loop Requires Empty List
        elementsForAdjoiningGridPositions.Clear();
    }

    List<string> ExtractOpenElements(Vector2 currentGridPosition, string[] separatedElements)
    {
        List<string> separatedElementsList = separatedElements.ToList();

        RemoveRoomTypeElement(separatedElementsList);

        // Remove Closed Elements First 
        RemoveNorthClosedElement(separatedElementsList);
        RemoveEastClosedElement(separatedElementsList);
        RemoveWestClosedElement(separatedElementsList);

        // MAY ALWAYS NEED AN 'sc'. CODE LATER DETERMINES WHETHER IT CAN HAVE AN OPEN ELEMENT OR NOT

        // CHECK ON THIS CODE LATER
        // Advanced Rules For Removing Exits Which Are Not Required
        RemoveEastExitRequirementExceptForLastColumn(currentGridPosition.y, separatedElementsList);

        return separatedElementsList;
    }

    void RemoveRoomTypeElement(List<string> separatedElementsList)
    {
        separatedElementsList.Remove(separatedElementsList[0]);
    }

    void RemoveNorthClosedElement(List<string> separatedElementsList)
    {
        if (separatedElementsList.Contains(RoomElements.nc))
        {
            separatedElementsList.Remove(RoomElements.nc);
        }  
    }

    void RemoveEastClosedElement(List<string> separatedElementsList)
    {
        if (separatedElementsList.Contains(RoomElements.ec))
        {
            separatedElementsList.Remove(RoomElements.ec);
        }
    }

    void RemoveWestClosedElement(List<string> separatedElementsList)
    {
        if (separatedElementsList.Contains(RoomElements.wc))
        {
            separatedElementsList.Remove(RoomElements.wc);
        }
    }

    // MAYBE SHOULD BE PULLED FROM THE ExtractOpenElements METHOD
    void RemoveEastExitRequirementExceptForLastColumn(float currentGridColumn, List<string> separatedElementsList)
    {
        if (separatedElementsList.Contains(RoomElements.wo) && separatedElementsList.Count >= 2 && currentGridColumn != (GridConfiguration.size - 1))
        {
            separatedElementsList.Remove(RoomElements.wo);
        }
    }

    List<string> InjectClosedElementsForOuterWalls(Vector2 currentGridPosition, List<string> separatedElementsFiltered)
    {
        // Build Grid Outer Walls
        InjectNorthClosedElementForTopWall(currentGridPosition, separatedElementsFiltered);

        InjectEastClosedElementForRightWall(currentGridPosition, separatedElementsFiltered);

        InjectWestClosedElementForLeftWall(currentGridPosition, separatedElementsFiltered);

        return separatedElementsFiltered;
    }

    void InjectNorthClosedElementForTopWall(Vector2 currentGridPosition, List<string> separatedElementsFiltered)
    {
        if (currentGridPosition.x == 0)
        {
            separatedElementsFiltered.Add(RoomElements.nc);
        }
    }

    void InjectEastClosedElementForRightWall(Vector2 currentGridPosition, List<string> separatedElementsFiltered)
    {
        if (currentGridPosition.y == (GridConfiguration.size - 1))
        {
            separatedElementsFiltered.Add(RoomElements.ec);
        }
    }

    void InjectWestClosedElementForLeftWall(Vector2 currentGridPosition, List<string> separatedElementsFiltered)
    {
        if (currentGridPosition.y == 0)
        {
            separatedElementsFiltered.Add(RoomElements.wc);
        }
    }

    void DetermineRequiredAdjoiningRoomElements(List<string> roomElementsAfterFiltering)
    {
        for (int i = 0; i < roomElementsAfterFiltering.Count; i++)
        {
            if (roomElementsAfterFiltering[i].Equals(RoomElements.nc) || roomElementsAfterFiltering[i].Equals(RoomElements.ec) 
                || roomElementsAfterFiltering[i].Equals(RoomElements.sc) || roomElementsAfterFiltering[i].Equals(RoomElements.wc))
            {
                elementsForAdjoiningGridPositions.Add(roomElementsAfterFiltering[i]);
            }

            if (!roomElementsAfterFiltering[i].Equals(RoomElements.nc) && !roomElementsAfterFiltering[i].Equals(RoomElements.wc) 
                && !roomElementsAfterFiltering[i].Equals(RoomElements.ec) && !roomElementsAfterFiltering[i].Equals(RoomElements.sc))
            {
                elementsForAdjoiningGridPositions.Add(openElementsMapper[roomElementsAfterFiltering[i]]);
            }            
        }
    }

    // SEEMS TO BE WORKING
    // THERE IS A BIT OF AMIBUITY AS TO WHICH METHODS MUST CHECK WHETHER THE elementsForAdjoiningGridPositions LIST CONTAINS OR DOESN'T CONTAIN THE ELEMENT
    // SOME MUST CHECK THEY DO CONTAIN AND OTHER MUST CHECK THEY DON'T CONTAIN
    // SOME CONSISTENCY HERE WOULD BE WELCOMED TO AVOID FUTURE CONFUSION
    void MapRequiredElementsForAdjoiningRoomsToGridPositions(Vector2 currentGridPosition)
    {
        Vector2 southAdjoiningGridPosition = new Vector2(1, 0);
        Vector2 eastAdjoiningGridPosition = new Vector2(0, 1);

        // Map Closed Elements To Dictionary
        MapNorthClosedElements(currentGridPosition, eastAdjoiningGridPosition, southAdjoiningGridPosition);
        MapEastClosedElements(currentGridPosition, eastAdjoiningGridPosition);
        MapSouthClosedElements(currentGridPosition, southAdjoiningGridPosition);
        MapWestClosedElements(currentGridPosition, southAdjoiningGridPosition);

        // Map Open Elements To Dictionary
        MapNorthOpenElements(currentGridPosition, southAdjoiningGridPosition);
        MapWestOpenElements(currentGridPosition, eastAdjoiningGridPosition);
    }

    void MapNorthClosedElements(Vector2 currentGridPosition, Vector2 eastAdjoiningGridPosition, Vector2 southAdjoiningGridPosition)
    {
        Vector2 calculatedAdjoiningGridPosition;

        // If Room Is Located In Top Row Of Grid Then Next Room To The Right Must Contain A North Closed Exit
        if (currentGridPosition.x == 0)
        {
            calculatedAdjoiningGridPosition = (currentGridPosition += eastAdjoiningGridPosition); // Take Current Grid Position And Move It To The Right Of The Grid 1 Place

            if (elementsForAdjoiningGridPositions.Contains(RoomElements.nc))
            {
                if (!mappedElementsToGridPositions.ContainsKey(calculatedAdjoiningGridPosition)) // If Position Has Not Previously Been Entered Into Dictionary
                {
                    List<string> elementsForCalculatedGridPosition = new List<string>() { RoomElements.nc }; // Create New List With 'nc' Element
                    mappedElementsToGridPositions.Add(calculatedAdjoiningGridPosition, elementsForCalculatedGridPosition); // Add Position And List To Dictionary
                }
                else
                {
                    mappedElementsToGridPositions[calculatedAdjoiningGridPosition].Add(RoomElements.nc); // Grid Position Already Exists So Add 'nc' Element To Its List
                }
            }
        }

        // If Room Contains A South Closed Element Then Below Room Must Contain A North Closed Exit
        if (elementsForAdjoiningGridPositions.Contains(RoomElements.sc))
        {
            calculatedAdjoiningGridPosition = (currentGridPosition += southAdjoiningGridPosition);

            if (!mappedElementsToGridPositions.ContainsKey(calculatedAdjoiningGridPosition)) // If Position Has Not Previously Been Entered Into Dictionary
            {
                List<string> elementsForCalculatedGridPosition = new List<string>() { RoomElements.nc }; // Create New List With 'nc' Element
                mappedElementsToGridPositions.Add(calculatedAdjoiningGridPosition, elementsForCalculatedGridPosition); // Add Position And List To Dictionary
            }
            else
            {
                mappedElementsToGridPositions[calculatedAdjoiningGridPosition].Add(RoomElements.nc); // Grid Position Already Exists So Add 'nc' Element To Its List
            }
        }
    }

    void MapEastClosedElements(Vector2 currentGridPosition, Vector2 eastAdjoiningGridPosition)
    {
        Vector2 calculatedAdjoiningGridPosition;

        if (currentGridPosition.y == (GridConfiguration.size - 2)) // If In Second Last Column Of Grid
        {
            calculatedAdjoiningGridPosition = (currentGridPosition += eastAdjoiningGridPosition); // Take Current Grid Position And Move It To The Right Of The Grid 1 Place

            if (!elementsForAdjoiningGridPositions.Contains(RoomElements.ec))
            {
                if (!mappedElementsToGridPositions.ContainsKey(calculatedAdjoiningGridPosition)) // If Position Has Not Previously Been Entered Into Dictionary
                {
                    List<string> elementsForCalculatedGridPosition = new List<string>() { RoomElements.ec }; // Create New List With 'ec' Element
                    mappedElementsToGridPositions.Add(calculatedAdjoiningGridPosition, elementsForCalculatedGridPosition); // Add Position And List To Dictionary
                }
                else
                {
                    mappedElementsToGridPositions[calculatedAdjoiningGridPosition].Add(RoomElements.ec); // Grid Position Already Exists So Add 'ec' Element To Its List
                }
            }
        }
    }

    void MapSouthClosedElements(Vector2 currentGridPosition, Vector2 southAdjoiningGridPosition)
    {
        Vector2 calculatedAdjoiningGridPosition;

        if (currentGridPosition.x == (GridConfiguration.size - 1))
        {
            calculatedAdjoiningGridPosition = (currentGridPosition += southAdjoiningGridPosition); // Take Current Grid Position And Move It Down The Grid 1 Place

            if (elementsForAdjoiningGridPositions.Contains(RoomElements.sc))
            {
                if (!mappedElementsToGridPositions.ContainsKey(calculatedAdjoiningGridPosition)) // If Position Has Not Previously Been Entered Into Dictionary
                {
                    List<string> elementsForCalculatedGridPosition = new List<string>() { RoomElements.sc }; // Create New List With 'sc' Element
                    mappedElementsToGridPositions.Add(calculatedAdjoiningGridPosition, elementsForCalculatedGridPosition); // Add Position And List To Dictionary
                }
                else
                {
                    mappedElementsToGridPositions[calculatedAdjoiningGridPosition].Add(RoomElements.sc); // Grid Position Already Exists So Add 'sc' Element To Its List
                }
            }
        }
    }

    void MapWestClosedElements(Vector2 currentGridPosition, Vector2 southAdjoiningGridPosition)
    {
        Vector2 calculatedAdjoiningGridPosition;

        if (currentGridPosition.y == 0)
        {
            calculatedAdjoiningGridPosition = (currentGridPosition += southAdjoiningGridPosition); // Take Current Grid Position And Move It Down The Grid 1 Place

            if (elementsForAdjoiningGridPositions.Contains(RoomElements.wc))
            {
                if (!mappedElementsToGridPositions.ContainsKey(calculatedAdjoiningGridPosition)) // If Position Has Not Previously Been Entered Into Dictionary
                {
                    List<string> elementsForCalculatedGridPosition = new List<string>() { RoomElements.wc }; // Create New List With 'wc' Element
                    mappedElementsToGridPositions.Add(calculatedAdjoiningGridPosition, elementsForCalculatedGridPosition); // Add Position And List To Dictionary
                }
                else
                {
                    mappedElementsToGridPositions[calculatedAdjoiningGridPosition].Add(RoomElements.wc); // Grid Position Already Exists So Add 'wc' Element To Its List
                }
            }
        }
    }

    void MapNorthOpenElements(Vector2 currentGridPosition, Vector2 southAdjoiningGridPosition)
    {
        Vector2 calculatedAdjoiningGridPosition;

        if (elementsForAdjoiningGridPositions.Contains(RoomElements.nm))
        {
            calculatedAdjoiningGridPosition = (currentGridPosition += southAdjoiningGridPosition); // Take Current Grid Position And Move It Down The Grid 1 Place

            if (!mappedElementsToGridPositions.ContainsKey(calculatedAdjoiningGridPosition))
            {
                List<string> elementsForCalculatedGridPosition = new List<string>() { RoomElements.nm }; // Create New List With 'nm' Element
                mappedElementsToGridPositions.Add(calculatedAdjoiningGridPosition, elementsForCalculatedGridPosition); // Add Position And List To Dictionary
            }
            else
            {
                mappedElementsToGridPositions[calculatedAdjoiningGridPosition].Add(RoomElements.nm); // Grid Position Already Exists So Add 'nm' Element To Its List
            }
        }
    }

    void MapWestOpenElements(Vector2 currentGridPosition, Vector2 eastAdjoiningGridPosition)
    {
        Vector2 calculatedAdjoiningGridPosition;

        if (elementsForAdjoiningGridPositions.Contains(RoomElements.wo))
        {
            calculatedAdjoiningGridPosition = (currentGridPosition += eastAdjoiningGridPosition); // Take Current Grid Position And Move It To The Right Of The Grid 1 Place

            if (!mappedElementsToGridPositions.ContainsKey(calculatedAdjoiningGridPosition))
            {
                List<string> elementsForCalculatedGridPosition = new List<string>() { RoomElements.wo }; // Create New List With 'wo' Element
                mappedElementsToGridPositions.Add(calculatedAdjoiningGridPosition, elementsForCalculatedGridPosition); // Add Position And List To Dictionary
            }
            else
            {
                mappedElementsToGridPositions[calculatedAdjoiningGridPosition].Add(RoomElements.wo); // Grid Position Already Exists So Add 'wo' Element To Its List
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
