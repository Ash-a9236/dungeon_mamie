using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image itemImage;

    public bool IsEmpty()
    {
        return itemImage.sprite == null;
    }

    public void SetItem(Sprite newSprite)
    {
        itemImage.sprite = newSprite;
        itemImage.enabled = true;
    }
}