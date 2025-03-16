using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ItemType
    {
        LifeStealSword,
        HealthPotion,
        StaminaPotion,
        DamageReduction,
        DamageAmplifier,
        SpeedShoes,
    }
    public ItemType itemType;
    public int amount;

    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
                case ItemType.LifeStealSword: return ItemAssets.Instance.lifestealSwordSprite;
                case ItemType.HealthPotion: return ItemAssets.Instance.healthPotionSprite;
                case ItemType.StaminaPotion: return ItemAssets.Instance.staminaPotionSprite;
                case ItemType.DamageReduction: return ItemAssets.Instance.damageReductionSprite;
                case ItemType.DamageAmplifier: return ItemAssets.Instance.damageApplifierSprite;
                case ItemType.SpeedShoes: return ItemAssets.Instance.speedShoesSprite;
        }
    }

    public bool IsStackable()
    {
        switch (itemType)
        {
            default:
            case ItemType.HealthPotion:
            case ItemType.StaminaPotion:
            case ItemType.LifeStealSword:
            case ItemType.DamageReduction:
            case ItemType.DamageAmplifier:
            case ItemType.SpeedShoes:
                return true;
        }
    }

}
