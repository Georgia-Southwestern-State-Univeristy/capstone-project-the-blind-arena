using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class Box
    {
        public Image[] icons;  // The six shop icons
        public TextMeshProUGUI[] texts;   // Corresponding TextMeshPro text
        public Button buyButton; // Buy button for this box
        public RectTransform parentBox; // Parent RectTransform for centering
    }

    public Box[] boxes; // Array of shop boxes
    public Image[] hotbarSlots; // 10 slots for purchased items

    private int[] selectedIndexes; // Stores randomly selected icons
    private Queue<Sprite> purchasedItems = new Queue<Sprite>(); // Track purchased items

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

            // Randomly select one icon and text per box
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

            // Enable buy button
            if (box.buyButton != null)
            {
                box.buyButton.interactable = true;
                box.buyButton.onClick.RemoveAllListeners();
                int index = boxIndex; // Capture index for closure
                box.buyButton.onClick.AddListener(() => BuyItem(index));
            }
        }

        // Clear hotbar slots initially
        foreach (var slot in hotbarSlots)
        {
            slot.sprite = null;
            slot.color = new Color(1, 1, 1, 0); // Hide empty slots
        }
    }

    void BuyItem(int boxIndex)
    {
        Box box = boxes[boxIndex];
        int selectedIndex = selectedIndexes[boxIndex];

        if (box.buyButton != null)
        {
            box.buyButton.interactable = false; // Disable button after purchase
        }

        Sprite purchasedSprite = box.icons[selectedIndex].sprite;

        if (purchasedItems.Count >= hotbarSlots.Length)
        {
            purchasedItems.Dequeue(); // Remove oldest item if full
        }
        purchasedItems.Enqueue(purchasedSprite);

        UpdateHotbar();
    }

    void UpdateHotbar()
    {
        int i = 0;
        foreach (var slot in hotbarSlots)
        {
            if (i < purchasedItems.Count)
            {
                slot.sprite = purchasedItems.ToArray()[i];
                slot.color = new Color(1, 1, 1, 1); // Show icon
            }
            else
            {
                slot.sprite = null;
                slot.color = new Color(1, 1, 1, 0); // Hide if empty
            }
            i++;
        }
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
}