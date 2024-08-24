using UnityEngine;
using UnityEngine.UI;

public class CloseButtonHandler : MonoBehaviour
{
    private Button closeButton;

    void Start()
    {
        
            closeButton = GetComponent<Button>();
            if (closeButton == null)
            {
                Debug.LogError("CloseBtn does not have a Button component attached.");
                return;
            }

            closeButton.onClick.AddListener(OnCloseButtonClick);
        
      
    }

    void OnCloseButtonClick()
    {
        
            // Debug.Log("Close button clicked!");

           // Find the GameObject named "NpcHud"
            GameObject npcHud = GameObject.Find("InfoPanel");
            if (npcHud != null)
            {
                // Set the NpcHud GameObject inactive
                npcHud.SetActive(false);
            }
            else
            {
                Debug.LogError("InfoPanel GameObject not found.");
            }
       
    }
}
