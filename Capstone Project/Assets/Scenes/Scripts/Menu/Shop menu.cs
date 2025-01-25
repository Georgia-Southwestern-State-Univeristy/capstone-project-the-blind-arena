using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomImageSelectorMultiSlot : MonoBehaviour
{
    // Assign these in the Inspector
    public Image[] slot1Images; // Images for Slot 1
    public TextMeshProUGUI[] slot1Texts; // Texts for Slot 1
    public Button slot1Button; // Buy button for Slot 1

    public Image[] slot2Images; // Images for Slot 2
    public TextMeshProUGUI[] slot2Texts; // Texts for Slot 2
    public Button slot2Button; // Buy button for Slot 2

    public Image[] slot3Images; // Images for Slot 3
    public TextMeshProUGUI[] slot3Texts; // Texts for Slot 3
    public Button slot3Button; // Buy button for Slot 3

    void Start()
    {
        // Validate input
        ValidateSetup(slot1Images, slot1Texts, "Slot 1");
        ValidateSetup(slot2Images, slot2Texts, "Slot 2");
        ValidateSetup(slot3Images, slot3Texts, "Slot 3");

        // Attach button click events
        if (slot1Button != null) slot1Button.onClick.AddListener(() => RandomizeSlot(slot1Images, slot1Texts));
        if (slot2Button != null) slot2Button.onClick.AddListener(() => RandomizeSlot(slot2Images, slot2Texts));
        if (slot3Button != null) slot3Button.onClick.AddListener(() => RandomizeSlot(slot3Images, slot3Texts));

        // Hide all images and texts initially
        HideAll(slot1Images, slot1Texts);
        HideAll(slot2Images, slot2Texts);
        HideAll(slot3Images, slot3Texts);
    }

    void RandomizeSlot(Image[] images, TextMeshProUGUI[] texts)
    {
        // Randomly select an index
        int selectedIndex = Random.Range(0, images.Length);

        // Loop through images and texts in the slot
        for (int i = 0; i < images.Length; i++)
        {
            if (i == selectedIndex)
            {
                // Show the selected image and text
                images[i].gameObject.SetActive(true);
                texts[i].gameObject.SetActive(true);
            }
            else
            {
                // Hide the other images and text
                images[i].gameObject.SetActive(false);
                texts[i].gameObject.SetActive(false);
            }
        }
    }

    void HideAll(Image[] images, TextMeshProUGUI[] texts)
    {
        foreach (var image in images) image.gameObject.SetActive(false);
        foreach (var text in texts) text.gameObject.SetActive(false);
    }

    void ValidateSetup(Image[] images, TextMeshProUGUI[] texts, string slotName)
    {
        if (images.Length != 6 || texts.Length != 6)
        {
            Debug.LogError($"Slot '{slotName}' must have exactly 6 images and 6 TextMeshPro objects assigned.");
        }
    }
}