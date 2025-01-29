using UnityEngine;
using UnityEngine.UI;

public class ShopIconManager : MonoBehaviour
{
    [System.Serializable]
    public class Box
    {
        public Image[] icons;  // The six icons in this box
        public Button buyButton; // The buy button for this box
    }

    public Box[] boxes; // Array containing 3 boxes
    public int hiddenSortingOrder = 0;  // Sorting order for hidden icons
    public int visibleSortingOrder = 10; // Sorting order for the selected icon

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

    // Initializes the shop by selecting random icons per box
    void InitializeShop()
    {
        foreach (Box box in boxes)
        {
            if (box.icons.Length != 6)
            {
                Debug.LogError("Each box must have exactly 6 icons assigned.");
                continue;
            }

            // Hide all icons and pick one at random to show
            int selectedIndex = Random.Range(0, 6);
            for (int i = 0; i < box.icons.Length; i++)
            {
                bool isSelected = (i == selectedIndex);
                box.icons[i].gameObject.SetActive(isSelected);
                SetSortingOrder(box.icons[i], isSelected ? visibleSortingOrder : hiddenSortingOrder);
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
            foreach (Image icon in box.icons)
            {
                SetSortingOrder(icon, hiddenSortingOrder);
            }

            // Ensure selected icon remains visible
            foreach (Image icon in box.icons)
            {
                if (icon.gameObject.activeSelf)
                {
                    SetSortingOrder(icon, visibleSortingOrder);
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

    // Sets the sorting order of an image
    void SetSortingOrder(Image image, int order)
    {
        Canvas canvas = image.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = image.gameObject.AddComponent<Canvas>();
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