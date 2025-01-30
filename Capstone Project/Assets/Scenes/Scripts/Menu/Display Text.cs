using UnityEngine;
using TMPro;

public class DisplayInputTextMultipleTMP : MonoBehaviour
{
    // Reference to the TMP Input Field
    public TMP_InputField inputField;

    // References to the TMP Text objects where the input will be displayed
    public TMP_Text displayText1;
    public TMP_Text displayText2;

    // Method to update the display texts
    public void UpdateDisplayText()
    {
        // Get the text from the input field
        string userInput = inputField.text;

        // Set the text of both display texts
        displayText1.text = userInput;
        displayText2.text = userInput;

        // Optional: Log for debugging
        Debug.Log("Text updated in both fields: " + userInput);
    }
}