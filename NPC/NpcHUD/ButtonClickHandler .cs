using UnityEngine;

public class ButtonClickHandler : MonoBehaviour
{
    public System.Action onClickAction;

    public void HandleClick()
    {
        // Debug.Log($"Button {gameObject.name} clicked!");
        if (onClickAction != null)
        {
            // Debug.Log($"Invoking action for {gameObject.name}");
            onClickAction.Invoke();
        }
        else
        {
            Debug.LogWarning($"No action assigned for {gameObject.name}");
        }
    }
}
