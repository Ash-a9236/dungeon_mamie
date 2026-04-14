using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    // Drag your InventoryMenu (the whole UI) here in Inspector
    public GameObject inventoryUI;

    private bool isOpen = false;

    void Start()
    {
        // Start closed
        inventoryUI.SetActive(false);
    }

    void Update()
    {
        // When ESC is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryUI.SetActive(isOpen);

        // Optional: lock/unlock cursor
        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f; // pause game
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f; // resume game
        }
    }
}