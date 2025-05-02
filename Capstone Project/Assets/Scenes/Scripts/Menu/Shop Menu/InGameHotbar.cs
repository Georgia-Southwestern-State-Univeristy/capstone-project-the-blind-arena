using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameHotbar : MonoBehaviour
{
    public Image[] hotbarSlots; // Reference to in-game hotbar slots
    public TextMeshProUGUI[] hotbarCounts; // Reference to in-game hotbar count texts
    public ShopManager shopManager; // Reference to the ShopManager script

    // Keybindings for hotbar slots (adjust as needed)
    public KeyCode[] hotbarKeys = new KeyCode[]
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Alpha0
    };

    void Start()
    {
        // If shopManager is not manually assigned, try to find it
        if (shopManager == null)
        {
            shopManager = FindFirstObjectByType<ShopManager>();
        }

        shopManager.OnHotbarUpdated += SyncHotbar;

        // Initial sync of hotbar
        SyncHotbar();
        shopManager.UpdateHotbar();
        shopManager.gameObject.SetActive(true);
        shopManager.gameObject.SetActive(false);
    }

    void Update()
    {
        // Check for hotbar key presses
        for (int i = 0; i < Mathf.Min(hotbarKeys.Length, hotbarSlots.Length); i++)
        {
            if (Input.GetKeyDown(hotbarKeys[i]))
            {
                UseItemInSlot(i);
                break; // Prevent using multiple items in the same frame
            }
        }

        // Sync hotbar to ensure it reflects the current state
        SyncHotbar();
    }

    void UseItemInSlot(int slotIndex)
    {
        // Validate slot index
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Length)
        {
            return;
        }

        // Try to use the item through ShopManager
        if (shopManager.UseItemFromHotbar(slotIndex))
        {
            // Optional: Add any additional effects when an item is used
            // For example, play a sound, show a visual effect, etc.
            Debug.Log($"Used item in slot {slotIndex}");
        }
    }

    void SyncHotbar()
    {
        // Use the GetPurchasedItems method to get a copy of the current items
        var purchasedItems = shopManager.GetPurchasedItems();

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
}
