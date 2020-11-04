using UnityEngine;
using UnityEngine.UIElements;

[HelpURL("https://www.youtube.com/watch?v=6zR3uvLVzc4")]
public class StoneManager : MonoBehaviour
{
    public static int stoneCounter;
    public static string stoneType { get; set; }

    // Must get reference to the UI Document as it contains the all info regarding the custom UI
    public UIDocument uiDocument;

    Label stoneCounterLabel;

    void OnEnable()
    {
       
        // Get the VisualElement which contains the label
        VisualElement rootVisualElement = uiDocument.rootVisualElement;

        // Query for the label by the label name
        stoneCounterLabel = rootVisualElement.Q<Label>("Stone-Counter-Label");

    }

    void Update()
    {
        // Update the GUI with the lastest value of the stoneCounter variable
        stoneCounterLabel.text = $"{stoneType}: {stoneCounter}";
    }

    public static void IncrementStoneCounter()
    {
        stoneCounter++;
    }

    public static void DecrementStoneCounter()
    {
        if (stoneCounter > 0)
        {
            stoneCounter--;
        }
    }
}
