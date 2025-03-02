using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameHotbar : MonoBehaviour
{
    public Image[] hotbarSlots; // Reference to in-game hotbar slots
    public TextMeshProUGUI[] hotbarCounts; // Reference to in-game hotbar count texts
    public ShopManager shopManager; // Reference to the ShopManager script

    void Start()
    {
        // If shopManager is not manually assigned, try to find it
        if (shopManager == null)
        {
            shopManager = FindFirstObjectByType<ShopManager>();
        }

        // Initial sync of hotbar
        SyncHotbar();
    }

    void Update()
    {
        // Optional: You can add logic here to continuously sync or only sync on changes
        // For now, we'll just sync every frame to ensure consistency
        SyncHotbar();
    }

    void SyncHotbar()
    {
        if (shopManager == null) return;

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            Sprite itemSprite = shopManager.GetHotbarItemSprite(i);
            int itemCount = shopManager.GetHotbarItemCount(i);

            if (itemSprite != null)
            {
                hotbarSlots[i].sprite = itemSprite;
                hotbarSlots[i].color = new Color(1, 1, 1, 1);
                hotbarCounts[i].text = itemCount > 1 ? itemCount.ToString() : "";
            }
            else
            {
                hotbarSlots[i].sprite = null;
                hotbarSlots[i].color = new Color(1, 1, 1, 0);
                hotbarCounts[i].text = "";
            }
        }
    }
}
