using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<InventoryItem> items = new List<InventoryItem>();
    
    public Image[] slotImages;
    public TextMeshProUGUI[] countTexts;

    [System.Serializable]
    public class InventoryItem {
        public Sprite sprite;
        public int count;
    }

    private void Awake() {
        instance = this;
        Debug.Log("Inventory Script is AWAKE");
    }

    public void AddItem(Sprite itemSprite) {
        if (itemSprite == null) {
            Debug.LogError("The coin you picked up has NO SPRITE assigned!");
            return;
        }

        InventoryItem existingItem = items.Find(i => i.sprite == itemSprite);
        if (existingItem != null) {
            existingItem.count++;
        } else {
            items.Add(new InventoryItem { sprite = itemSprite, count = 1 });
        }

        UpdateUI();
    }

    void UpdateUI() {
        // This loop is "Safe" - it won't crash if arrays are empty
        for (int i = 0; i < slotImages.Length; i++) {
            if (slotImages[i] == null) continue; 

            if (i < items.Count) {
                slotImages[i].sprite = items[i].sprite;
                slotImages[i].enabled = true;
                slotImages[i].color = Color.white;

                if (i < countTexts.Length && countTexts[i] != null) {
                    countTexts[i].text = items[i].count.ToString();
                    countTexts[i].enabled = items[i].count > 1;
                }
            } else {
                slotImages[i].enabled = false;
                if (i < countTexts.Length && countTexts[i] != null) {
                    countTexts[i].enabled = false;
                }
            }
        }
    }
}