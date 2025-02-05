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
        public Image hotbarSlot;  // Hotbar slot where purchased icon appears
        public RectTransform parentBox; // Parent RectTransform for centering
    }

    public Box[] boxes; // Array of 3 boxes
    public int hiddenSortingOrder = 0;
    public int visibleSortingOrder = 10;

    private int[] selectedIndexes; // Stores the randomly selected icon index for each box

    void Start()
    {
        selectedIndexes = new int[boxes.Length];
        InitializeShop();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ApplySortingOrder();
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

            // Randomly select one icon and text per box
            selectedIndexes[boxIndex] = Random.Range(0, 6);
            for (int i = 0; i < 6; i++)
            {
                bool isSelected = (i == selectedIndexes[boxIndex]);
                box.icons[i].gameObject.SetActive(isSelected);
                box.texts[i].gameObject.SetActive(isSelected);

                SetSortingOrder(box.icons[i], isSelected ? visibleSortingOrder : hiddenSortingOrder);
                SetSortingOrder(box.texts[i], isSelected ? visibleSortingOrder : hiddenSortingOrder);

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

            // Clear hotbar slot initially
            if (box.hotbarSlot != null)
            {
                box.hotbarSlot.sprite = null;
                box.hotbarSlot.color = new Color(1, 1, 1, 0); // Hide hotbar slot
            }
        }
    }

    void ApplySortingOrder()
    {
        foreach (Box box in boxes)
        {
            for (int i = 0; i < 6; i++)
            {
                SetSortingOrder(box.icons[i], hiddenSortingOrder);
                SetSortingOrder(box.texts[i], hiddenSortingOrder);
            }

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

    void BuyItem(int boxIndex)
    {
        Box box = boxes[boxIndex];
        int selectedIndex = selectedIndexes[boxIndex];

        if (box.buyButton != null)
        {
            box.buyButton.interactable = false; // Disable buy button after purchase
        }

        if (box.hotbarSlot != null)
        {
            // Set the hotbar slot to the purchased icon
            box.hotbarSlot.sprite = box.icons[selectedIndex].sprite;
            box.hotbarSlot.color = new Color(1, 1, 1, 1); // Make hotbar icon visible
        }
    }

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

    void CenterIcon(RectTransform icon, RectTransform parent)
    {
        icon.SetParent(parent);

        // Center the icon in the box
        icon.anchoredPosition = Vector2.zero; // Center position
        icon.localPosition = Vector3.zero; // Ensure no offsets
    }

    public void ResetShop()
    {
        InitializeShop();
    }
}

