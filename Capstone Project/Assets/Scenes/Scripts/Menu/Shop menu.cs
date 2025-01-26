using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomImageSelectorWithSortingOrder : MonoBehaviour
{
    // Assign these in the Inspector
    public Image[] slot1Images; // Images for Slot 1
    public TextMeshProUGUI[] slot1Texts; // Texts for Slot 1
    public Button slot1Button; // Button to open Slot 1

    public Image[] slot2Images; // Images for Slot 2
    public TextMeshProUGUI[] slot2Texts; // Texts for Slot 2
    public Button slot2Button; // Button to open Slot 2

    public Image[] slot3Images; // Images for Slot 3
    public TextMeshProUGUI[] slot3Texts; // Texts for Slot 3
    public Button slot3Button; // Button to open Slot 3

    private int baseSortingOrder = 100; // Base sorting order value
    private int highlightSortingOrder = 200; // Sorting order for highlighted elements

    void Start()
    {
        // Validate input
        ValidateSetup(slot1Images, slot1Texts, "Slot 1");
        ValidateSetup(slot2Images, slot2Texts, "Slot 2");
        ValidateSetup(slot3Images, slot3Texts, "Slot 3");

        // Attach button click events
        if (slot1Button != null) slot1Button.onClick.AddListener(() => OpenSlot(slot1Images, slot1Texts));
        if (slot2Button != null) slot2Button.onClick.AddListener(() => OpenSlot(slot2Images, slot2Texts));
        if (slot3Button != null) slot3Button.onClick.AddListener(() => OpenSlot(slot3Images, slot3Texts));

        // Hide all images and texts initially
        HideAll(slot1Images, slot1Texts);
        HideAll(slot2Images, slot2Texts);
        HideAll(slot3Images, slot3Texts);
    }

    void OpenSlot(Image[] images, TextMeshProUGUI[] texts)
    {
        // Check if arrays are valid
        if (images == null || texts == null || images.Length == 0 || texts.Length == 0 || images.Length != texts.Length)
        {
            Debug.LogError("Images or Texts array is invalid. Ensure all arrays are assigned and have matching lengths.");
            return;
        }

        // Randomly select an index
        int selectedIndex = Random.Range(0, images.Length);

        // Loop through images and texts
        for (int i = 0; i < images.Length; i++)
        {
            if (i == selectedIndex)
            {
                // Show the selected image and text
                images[i].gameObject.SetActive(true);
                texts[i].gameObject.SetActive(true);

                // Increase their sorting order to highlight them
                SetSortingOrder(images[i], highlightSortingOrder);
                SetSortingOrder(texts[i], highlightSortingOrder);
            }
            else
            {
                // Hide the other images and text
                images[i].gameObject.SetActive(false);
                texts[i].gameObject.SetActive(false);

                // Reset sorting order
                SetSortingOrder(images[i], baseSortingOrder);
                SetSortingOrder(texts[i], baseSortingOrder);
            }
        }
    }

    void HideAll(Image[] images, TextMeshProUGUI[] texts)
    {
        foreach (var image in images)
        {
            image.gameObject.SetActive(false);
            SetSortingOrder(image, baseSortingOrder);
        }

        foreach (var text in texts)
        {
            text.gameObject.SetActive(false);
            SetSortingOrder(text, baseSortingOrder);
        }
    }

    void SetSortingOrder(Graphic graphic, int order)
    {
        var canvas = graphic.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = graphic.gameObject.AddComponent<Canvas>();
        }
        canvas.overrideSorting = true;
        canvas.sortingOrder = order;
    }

    void ValidateSetup(Image[] images, TextMeshProUGUI[] texts, string slotName)
    {
        if (images.Length != texts.Length)
        {
            Debug.LogError($"Slot '{slotName}' must have exactly the same number of images and TextMeshPro objects assigned.");
        }
    }
}