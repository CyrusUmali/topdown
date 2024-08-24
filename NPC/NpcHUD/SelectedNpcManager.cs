using UnityEngine;

public class SelectedNpcManager : MonoBehaviour
{
    public static SelectedNpcManager Instance { get; private set; }
    private GameObject currentSelectedNpc;
    private GameObject currentHud;
    private Inventory currentNpcInventory; // Reference to the selected NPC's Inventory
    private string currentNpcName; // Variable to store the name of the selected NPC

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ClearSelectedNpc()
    {
        if (currentHud != null)
        {
            currentHud.SetActive(false);
        }
        currentSelectedNpc = null;
        currentHud = null;
        currentNpcName = null; // Clear the stored name when NPC is deselected
        currentNpcInventory = null; // Clear the reference to the Inventory

        NewCameraController.Instance.SetCameraFollowTarget(null); // Stop following the NPC
        NewCameraController.Instance.stateDrivenCameraObject.SetActive(false); // Deactivate StateDrivenCamera
    }

    public void SelectNpc(GameObject npc, GameObject hud)
    {
        // Deactivate the current HUD if there is one
        if (currentHud != null)
        {
            currentHud.SetActive(false);
        }

        // Set the new selected NPC and its HUD
        currentSelectedNpc = npc;
        currentHud = hud;
        currentNpcName = npc != null ? npc.name : null; // Store the name of the selected NPC
        currentNpcInventory = npc != null ? npc.GetComponent<Inventory>() : null; // Get the Inventory of the selected NPC

        // Activate the new HUD
        if (currentHud != null)
        {
            currentHud.SetActive(true);
        }

        // Activate StateDrivenCamera and set the camera to follow the new selected NPC
        NewCameraController.Instance.stateDrivenCameraObject.SetActive(true);
        NewCameraController.Instance.SetCameraFollowTarget(currentSelectedNpc);
    }

    // Method to get the name of the currently selected NPC
    public string GetCurrentNpcName()
    {
        return currentNpcName;
    }

    // Method to get the current amount of coins in the selected NPC's inventory
    public int GetCurrentNpcCoins()
    {
        return currentNpcInventory != null ? currentNpcInventory.GetCoins() : 0;
    }

    // Method to add coins to the selected NPC's inventory
    public void AddCoinsToCurrentNpc(int amount)
    {
        if (currentNpcInventory != null)
        {
            currentNpcInventory.AddCoins(amount);
        }
    }

    // Method to remove coins from the selected NPC's inventory
    public bool RemoveCoinsFromCurrentNpc(int amount)
    {
        return currentNpcInventory != null && currentNpcInventory.RemoveCoins(amount);
    }
}
