using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGeneratorDebugger
{
    // Variables For Debugging
    public bool levelGeneratorSetupDebuggingEnabled = false;
    public bool gridGeneratorDebuggingEnabled = false;
    public bool removingUnwantedRoomNameElementsDebuggingEnabled = false;
    public bool mapCurrentRoomElementsToAdjoiningRoomElementsDebuggingEnabled = false;
    public bool mapRoomElementsToAdjoiningRoomGridPositionDebuggingEnabled = false;
    public bool debuggingEnabled = false;
    public bool lowLevelDebuggingEnabled = false;

    // ***************************************************************************
    //                              Debugging Methods
    // ***************************************************************************

    public void PrintDictionaryWithVector2KeyAndStringListValue(Dictionary<Vector2, List<string>> dictionary)
    {
        if (true)
        {
            //Debug.Log("=========================== Reading Grid Layout Dictionary =========================================");

            for (int i = 0; i < dictionary.Count; i++)
            {
                dictionary.ElementAt(i).Value.ForEach(j => Debug.Log("Grid Position = " + dictionary.ElementAt(i).Key + " : Required Exit = " + j));
            }
        }
    }

    public void PrintContentsOfVector2AndStringDictionary(Dictionary<Vector2, string> dictionary)
    {
        for (int i = 0; i < dictionary.Count; i++)
        {
            Vector2 gridPos = dictionary.ElementAt(i).Key;
            string roomName = dictionary.ElementAt(i).Value;
            Debug.Log("     Grid Position: " + gridPos + "     |     Room Name: " + roomName);
        }
    }

    /*void PrintLevelGeneratorSetup()
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

    *//*void PrintInitialRoomSelectedToStartLevelGenerator(int selectRandomNwCornerRoom)
    {
        if (gridGeneratorDebuggingEnabled && lowLevelDebuggingEnabled)
        {
            Debug.Log("Initial Corner Room Selected: " + nwCornerRooms[selectRandomNwCornerRoom]);
        }  
    }*//*

    void PrintCurrentRoomAndGridPositionInformation(Vector2 currentGridPosition, string roomName)
    {
        if (false)
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
        if (false)
        {
            Debug.Log("Room Exit Element Removed:  " + roomExitElement);
        }
    }

    void PrintRoomElementsRequiredForAdjoiningRoomsAfterFiltering(Vector2 currentGridPosition, List<string> filteredRoomElements)
    {
        if (false)
        {
            string elements = "";

            foreach (string element in filteredRoomElements)
            {
                elements += element + " ";
            }

            Debug.Log("Grid Position: " + currentGridPosition + "     |     " + "  Room Elements After Filtering: " + elements);
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
        if (false)
        {
            for (int k = 0; k < requiredRoomElementsForConnectingAdjoiningRoom.Count; k++)
            {
                Debug.Log("Extracted Required Exit: " + requiredRoomElementsForConnectingAdjoiningRoom[k]);
            }
        }
    }

    void PrintExitToGridPositionMapping(string exit, Vector2 gridPosition, string methodName)
    {
        if (false)
        {
            Debug.Log("In Method: " + methodName + "     |     Room Element: " + exit + "     |    Mapped To Grid Position " + gridPosition);
        }
    }

    

    void PrintSuitableRoomsForSelection(List<string> suitableRooms, List<string> requiredExits, Vector2 currentGridPosition)
    {
        if (false)
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

    *//*void PrintDeafultRoomCreated(Vector2 currentGridPosition, List<string> requiredRoomElements)
    {
        for (int i = 0; i < requiredRoomElements.Count; i++)
        {
            Debug.Log("Grid Position: " + currentGridPosition + "     |     Required Room Element: " + requiredRoomElements[i]);
        }
    }*//*

    */
}
