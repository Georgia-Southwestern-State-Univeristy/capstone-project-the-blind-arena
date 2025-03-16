using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using static Item;

public class Inventory
{

    public event EventHandler OnItemListChanged;

    private List<Item> itemList;
    public Inventory()
    {
        itemList = new List<Item>();

        AddItem(new Item { itemType = Item.ItemType.LifeStealSword, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.HealthPotion, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.StaminaPotion, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.DamageReduction, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.DamageAmplifier, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.SpeedShoes, amount = 1 });
        Debug.Log(itemList.Count);
    }

public void AddItem(Item item)
    {
        if (item.IsStackable())
        {
            bool itemAlreadyInInventory = false;
            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount += item.amount;
                    itemAlreadyInInventory = true;
                }
            }

            if (itemAlreadyInInventory)
            {
                itemList.Add(item);
            }
        }
        else
        {
            itemList.Add(item);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }
}
