using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class Box
    {
        public Image[] icons;
        public TextMeshProUGUI[] texts;
        public Button buyButton;
        public RectTransform parentBox;
    }

    [System.Serializable]
    public class ItemTime
    {
        public Sprite itemSprite; // The item's image
        public float timeToAdd; // Time added when purchased
    }

    public Box[] boxes;
    public Image[] hotbarSlots; // 10 slots for purchased items
    public TextMeshProUGUI[] hotbarCounts; // TextMeshPro elements for counts
    public GameTimer gameTimer; // Reference to GameTimer

    private int[] selectedIndexes;
    private Dictionary<Sprite, int> purchasedItems = new Dictionary<Sprite, int>(); // Track item counts

    // List of items with their unique time values
    public List<ItemTime> itemTimeValues = new List<ItemTime>();

    void Start()
    {
        selectedIndexes = new int[boxes.Length];
        InitializeShop();
    }

    void InitializeShop()
    {
        for (int boxIndex = 0; boxIndex < boxes.Length; boxIndex++)
        {
            Box box = boxes[boxIndex];

            if (box.icons.Length != 6 || box.texts.Length != 6)
            {
                Debug.LogError("Each box must have exactly 6 icons and texts.");
                continue;
            }

            selectedIndexes[boxIndex] = Random.Range(0, 6);
            for (int i = 0; i < 6; i++)
            {
                bool isSelected = (i == selectedIndexes[boxIndex]);
                box.icons[i].gameObject.SetActive(isSelected);
                box.texts[i].gameObject.SetActive(isSelected);

                if (isSelected)
                {
                    CenterIcon(box.icons[i].rectTransform, box.parentBox);
                }
            }

            if (box.buyButton != null)
            {
                box.buyButton.interactable = true;
                box.buyButton.onClick.RemoveAllListeners();
                int index = boxIndex;
                box.buyButton.onClick.AddListener(() => BuyItem(index));
            }
        }

        // Clear hotbar slots initially
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].sprite = null;
            hotbarSlots[i].color = new Color(1, 1, 1, 0);
            hotbarCounts[i].text = "";
        }
    }

    void BuyItem(int boxIndex)
    {
        Box box = boxes[boxIndex];
        int selectedIndex = selectedIndexes[boxIndex];

        if (box.buyButton != null)
        {
            box.buyButton.interactable = false;
        }

        Sprite purchasedSprite = box.icons[selectedIndex].sprite;

        // Increase count if item exists, otherwise add it
        if (purchasedItems.ContainsKey(purchasedSprite))
        {
            purchasedItems[purchasedSprite]++;
        }
        else
        {
            if (purchasedItems.Count >= hotbarSlots.Length)
            {
                purchasedItems.Remove(GetOldestItem()); // Remove oldest if full
            }
            purchasedItems[purchasedSprite] = 1;
        }

        // Add time based on the item's assigned time value
        float timeToAdd = GetItemTimeValue(purchasedSprite);
        if (timeToAdd > 0)
        {
            gameTimer.AddTime(timeToAdd);
        }

        UpdateHotbar();
    }

    void UpdateHotbar()
    {
        int i = 0;
        foreach (var item in purchasedItems)
        {
            hotbarSlots[i].sprite = item.Key;
            hotbarSlots[i].color = new Color(1, 1, 1, 1);
            hotbarCounts[i].text = item.Value > 1 ? item.Value.ToString() : ""; // Hide "1" for clarity
            i++;
        }

        // Hide unused slots
        for (; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].sprite = null;
            hotbarSlots[i].color = new Color(1, 1, 1, 0);
            hotbarCounts[i].text = "";
        }
    }

    Sprite GetOldestItem()
    {
        foreach (var item in purchasedItems)
        {
            return item.Key;
        }
        return null;
    }

    void CenterIcon(RectTransform icon, RectTransform parent)
    {
        icon.SetParent(parent);
        icon.anchoredPosition = Vector2.zero;
        icon.localPosition = Vector3.zero;
    }

    public void ResetShop()
    {
        purchasedItems.Clear();
        InitializeShop();
    }

    // Method to get the time value for a specific item
    float GetItemTimeValue(Sprite itemSprite)
    {
        foreach (var item in itemTimeValues)
        {
            if (item.itemSprite == itemSprite)
            {
                return item.timeToAdd;
            }
        }
        return 0f; // Default: no time added if item not found
    }
}
