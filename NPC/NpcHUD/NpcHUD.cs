using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Make sure you include this namespace for TextMeshProUGUI

public class NpcHUD : MonoBehaviour
{
    public GameObject npcInfo;
    public GameObject control;

    public GameObject nameTextObject; // Expose GameObject instead of TextMeshPro
    public GameObject coinsTextObject;
    private GameObject expandBtn;
    private GameObject closeBtn;

    private void Start()
    {
        // Initial state: HUD is deactivated
        // npcInfo.SetActive(false);
        // control.SetActive(false);
    }

    public void InitializeButtons(GameObject npc)
    {
        if (npc == null)
        {
            Debug.LogError("NPC GameObject is null");
            return;
        }

        Transform hudTransform = npc.transform.Find("HUD");
        if (hudTransform == null)
        {
            Debug.LogWarning("HUD object not found under NPC");
            return;
        }

        closeBtn = FindButton(hudTransform, "CloseBtn");
        expandBtn = FindButton(hudTransform, "ExpandBtn");

        if (expandBtn != null)
        {
            SetupButtonCollider(expandBtn, ExpandHUD);
        }
        else
        {
            Debug.LogWarning("ExpandBtn not found under HUD");
        }

        if (closeBtn != null)
        {
            SetupButtonCollider(closeBtn, DeactivateHUD);
        }
        else
        {
            Debug.LogWarning("CloseBtn not found under HUD");
        }
    }

    private GameObject FindButton(Transform parent, string buttonName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == buttonName)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    private void SetupButtonCollider(GameObject button, System.Action onClickAction)
    {
        Collider2D collider = button.GetComponent<Collider2D>();
        if (collider == null)
        {
            // Debug.LogWarning($"No Collider2D found on {button.name}, adding one.");
            collider = button.AddComponent<BoxCollider2D>();
        }

        ButtonClickHandler handler = button.GetComponent<ButtonClickHandler>();
        if (handler == null)
        {
            // Debug.Log($"Adding ButtonClickHandler to {button.name}");
            handler = button.AddComponent<ButtonClickHandler>();
        }
        handler.onClickAction = onClickAction;
    }

    public void ExpandHUD()
{
    // Activate additional HUD elements
    npcInfo.SetActive(true);
    control.SetActive(true);

    // Log all components attached to the assigned GameObject
    if (nameTextObject != null)
    {
        // Debug.Log("Components attached to Name Text GameObject:");
 
        TextMeshProUGUI textMeshProUGUI = nameTextObject.GetComponent<TextMeshProUGUI>();
        if (textMeshProUGUI != null)
        {
            // Modify the text here
            string npcName = SelectedNpcManager.Instance.GetCurrentNpcName();
            if (!string.IsNullOrEmpty(npcName))
            {
                textMeshProUGUI.text = npcName;
                // Debug.Log($"NPC Name set to: {textMeshProUGUI.text}");
            }
            else
            {
                Debug.LogWarning("No NPC name available to set.");
            }
        }
        else
        {
            Debug.LogWarning("No TextMeshProUGUI component found on the assigned GameObject.");
        }





       TextMeshProUGUI coinsText = coinsTextObject.GetComponent<TextMeshProUGUI>();
        if (coinsText != null)
        {
    // Modify the text here
          int npcCoins = SelectedNpcManager.Instance.GetCurrentNpcCoins();
            coinsText.text = npcCoins.ToString(); // Convert int to string
        }
        else
        {
          Debug.LogWarning("No coinsText component found on the assigned GameObject.");
        }




    }
    else
    {
        Debug.LogWarning("Name Text GameObject is not assigned.");
    }

    // Debug.Log("ExpandHUD");
}
    public void DeactivateHUD()
    {
        // Deactivate all HUD elements
        npcInfo.SetActive(false);
        control.SetActive(false);

        // Clear the selected NPC and HUD from the manager
        SelectedNpcManager.Instance.ClearSelectedNpc();

        // Debug.Log("DeactivateHUD");
    }
}
