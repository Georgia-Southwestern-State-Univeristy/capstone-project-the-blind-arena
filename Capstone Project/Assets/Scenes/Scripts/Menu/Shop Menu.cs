using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class Box
    {
        public Image[] icons;  // The six icons in this box
        public Text[] texts;   // Corresponding text for each icon
        public Button buyButton; // The buy button for this box
    }

    public Box[] boxes; // Array containing 3 boxes
    public int hiddenSortingOrder = 0;  // Sorting order for hidden icons & text
    public int visibleSortingOrder = 10; // Sorting order for the selected icon & text

    void Start()
    {
        InitializeShop();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ApplySortingOrder();
        }
    }

    // Initializes the shop by selecting random icons and texts per box
    void InitializeShop()
    {
        foreach (Box box in boxes)
        {
            if (box.icons.Length != 6 || box.texts.Length != 6)
            {
                Debug.LogError("Each box must have exactly 6 icons and 6 texts assigned.");
                continue;
            }

            // Hide all icons and texts, then randomly pick one to show
            int selectedIndex = Random.Range(0, 6);
            for (int i = 0; i < 6; i++)
            {
                bool isSelected = (i == selectedIndex);
                box.icons[i].gameObject.SetActive(isSelected);
                box.texts[i].gameObject.SetActive(isSelected);

                SetSortingOrder(box.icons[i], isSelected ? visibleSortingOrder : hiddenSortingOrder);
                SetSortingOrder(box.texts[i], isSelected ? visibleSortingOrder : hiddenSortingOrder);
            }

            // Enable the buy button at the start
            if (box.buyButton != null)
            {
                box.buyButton.interactable = true;
                ColorBlock colors = box.buyButton.colors;
                colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gray out when disabled
                box.buyButton.colors = colors;

                // Add button listener
                box.buyButton.onClick.RemoveAllListeners();
                box.buyButton.onClick.AddListener(() => BuyItem(box));
            }
        }
    }

    // Adjusts sorting order when "I" is pressed
    void ApplySortingOrder()
    {
        foreach (Box box in boxes)
        {
            for (int i = 0; i < 6; i++)
            {
                SetSortingOrder(box.icons[i], hiddenSortingOrder);
                SetSortingOrder(box.texts[i], hiddenSortingOrder);
            }

            // Ensure selected icon & text remain visible
            for (int i = 0; i < 6; i++)
            {
                if (box.icons[i].gameObject.activeSelf)
                {
                    SetSortingOrder(box.icons[i], visibleSortingOrder);
                    SetSortingOrder(box.texts[i], visibleSortingOrder);
                    break;
                }
            }
        }
    }

    // Handles buying an item from a box
    void BuyItem(Box box)
    {
        if (box.buyButton != null)
        {
            box.buyButton.interactable = false; // Disable button after purchase
        }
    }

    // Sets the sorting order of an image or text
    void SetSortingOrder(Graphic graphic, int order)
    {
        Canvas canvas = graphic.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = graphic.gameObject.AddComponent<Canvas>();
        }
        canvas.overrideSorting = true;
        canvas.sortingOrder = order;
    }

    // Resets the shop for a new round
    public void ResetShop()
    {
        InitializeShop();
    }
}