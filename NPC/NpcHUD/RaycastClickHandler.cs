using UnityEngine;

public class RaycastClickHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject; 
                // Debug.Log("Clicked on: " + clickedObject.name);

                // Handle NPC clicks
                NpcUtil npcUtil = clickedObject.GetComponent<NpcUtil>();
                if (npcUtil != null)
                {
                    npcUtil.HandleClick();
                }
               

                // Handle Button clicks
                ButtonClickHandler buttonClickHandler = clickedObject.GetComponent<ButtonClickHandler>();
                if (buttonClickHandler != null)
                {
                    buttonClickHandler.HandleClick();
                }
            }
        }
    }
}
