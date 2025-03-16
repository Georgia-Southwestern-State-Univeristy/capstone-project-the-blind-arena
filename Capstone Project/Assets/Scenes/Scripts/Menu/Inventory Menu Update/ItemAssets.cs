using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    public Sprite lifestealSwordSprite;
    public Sprite healthPotionSprite;
    public Sprite staminaPotionSprite;
    public Sprite damageReductionSprite;
    public Sprite damageApplifierSprite;
    public Sprite speedShoesSprite;


}
