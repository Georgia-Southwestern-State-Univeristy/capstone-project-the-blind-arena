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
    public PlayerController playerController;
    public Health playerHealth;
    public PlayerAttackManager playerAttackManager;
    public Projectile projectile;

    private int[] selectedIndexes;
    private static Dictionary<Sprite, int> purchasedItems = new Dictionary<Sprite, int>(); // Track item counts

    // List of items with their unique time values
    public List<ItemTime> itemTimeValues = new List<ItemTime>();

    void Start()
    {
        selectedIndexes = new int[boxes.Length];
        InitializeShop();

        // Debug logging for purchased items
        Debug.Log($"ShopManager Start - Purchased Items Count: {purchasedItems.Count}");
        foreach (var item in purchasedItems)
        {
            Debug.Log($"Purchased Item: Sprite = {item.Key.name}, Count = {item.Value}");
        }

        // Restore hotbar from static dictionary if it's not empty
        if (purchasedItems.Count > 0)
        {
            UpdateHotbar();
        }
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
            Debug.Log($"Adding {timeToAdd} seconds to the timer for item {purchasedSprite.name}");
            gameTimer.AddTime(timeToAdd);
            Debug.Log($"After Adding {timeToAdd} seconds to the timer for item {purchasedSprite.name}");
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

    // Method to get all purchased items
    public Dictionary<Sprite, int> GetPurchasedItems()
    {
        return new Dictionary<Sprite, int>(purchasedItems);
    }

    // Method to get the sprite for a specific hotbar slot
    public Sprite GetHotbarItemSprite(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < hotbarSlots.Length)
        {
            return hotbarSlots[slotIndex].sprite;
        }
        return null;
    }

    // Method to get the count for a specific hotbar slot
    public int GetHotbarItemCount(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < hotbarSlots.Length && hotbarSlots[slotIndex].sprite != null)
        {
            return purchasedItems.ContainsKey(hotbarSlots[slotIndex].sprite)
                ? purchasedItems[hotbarSlots[slotIndex].sprite]
                : 0;
        }
        return 0;
    }

    // New method to use an item from a specific hotbar slot
    public bool UseItemFromHotbar(int slotIndex)
    {
        // Validate slot index
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Length)
        {
            Debug.LogWarning($"Invalid hotbar slot index: {slotIndex}");
            return false;
        }

        // Get the sprite for the slot
        Sprite itemSprite = hotbarSlots[slotIndex].sprite;

        // Check if the slot is empty
        if (itemSprite == null)
        {
            Debug.Log($"Hotbar slot {slotIndex} is empty");
            return false;
        }

        ApplyItemEffect(itemSprite);

        // Reduce item count
        if (purchasedItems.ContainsKey(itemSprite))
        {
            purchasedItems[itemSprite]--;

            // Remove the item if count reaches zero
            if (purchasedItems[itemSprite] <= 0)
            {
                purchasedItems.Remove(itemSprite);
            }

            // Update the hotbar display
            UpdateHotbar();

            Debug.Log($"Used item from slot {slotIndex}: {itemSprite.name}");
            return true;
        }

        return false;
    }

    // Optional: Method to get the current count of an item in a specific slot
    public int GetItemCountInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Length)
        {
            return 0;
        }

        Sprite itemSprite = hotbarSlots[slotIndex].sprite;

        if (itemSprite == null)
        {
            return 0;
        }

        return purchasedItems.ContainsKey(itemSprite) ? purchasedItems[itemSprite] : 0;
    }
    public void ApplyItemEffect(Sprite itemSprite)
    {
        // Apply an effect based on the item
        if (itemSprite.name == "Shop_item_icons-removebg-preview_0") // Check for specific sprite (item)
        {
            // the Health script has a method to restore health
            playerHealth.Heal(50); 
            Debug.Log("Health restored by 50!");
        }

        if (itemSprite.name == "Shop_item_icons-removebg-preview_1") // Check for specific sprite (item)
        {
            playerHealth.stamina = playerHealth.MAX_STAMINA;
            playerHealth.UpdateStaminaBar(); // Update stamina bar UI
            Debug.Log("Stamina fully restored!");
        }

        if (itemSprite.name == "Shop_item_icons-removebg-preview_2") // Check for specific sprite (item)
        {
            // Apply speed boost
            playerController.AdjustSpeed(5f, 3f); // Increase speed by 5 for 3 seconds
        }

        if (itemSprite.name == "Shop_item_icons-removebg-preview_3") // Check for specific sprite (item)
        {
            // Apply speed boost
            playerAttackManager.AdjustDamage(5f, 5f); // Increase damage dealt by 10 for 3 seconds
        }
    }

}