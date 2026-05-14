using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI;

    private bool isOpen = false;

    void Start()
    {
        // Start closed
        inventoryUI.SetActive(false);
    }

    void Update()
    {
        // When I is pressed
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryUI.SetActive(isOpen);

        //lock/unlock cursor
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
