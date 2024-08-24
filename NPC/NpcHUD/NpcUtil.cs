using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcUtil : MonoBehaviour
{
    public void HandleClick()
    {
        // Debug.Log(gameObject.name);

        // Find the child GameObject named "HUD"
        Transform hudTransform = transform.Find("HUD");
        if (hudTransform != null)
        {
            // Select this NPC and its HUD
            SelectedNpcManager.Instance.SelectNpc(gameObject, hudTransform.gameObject);

            // Find the NpcHUD script and initialize buttons
            NpcHUD npcHUD = hudTransform.GetComponent<NpcHUD>();
            if (npcHUD != null)
            {
                npcHUD.InitializeButtons(gameObject);
                // npcHUD.ExpandHUD();
            }
            else
            {
                Debug.LogWarning("NpcHUD script not found on HUD");
            }
        }
        else
        {
            Debug.LogWarning("HUD child not found");
        }

        // Find the NpcHud > control GameObject and set it active
        GameObject npcHud = GameObject.Find("NpcHud");
        if (npcHud != null)
        {
            Transform controlTransform = npcHud.transform.Find("ComtrolPanel");
            if (controlTransform != null)
            {
                controlTransform.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("ComtrolPanel child not found in NpcHud");
            }
        }
        else
        {
            Debug.LogWarning("NpcHud not found");
        }
    }
}
