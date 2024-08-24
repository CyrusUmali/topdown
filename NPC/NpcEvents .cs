using UnityEngine;

public class NpcEvents : MonoBehaviour
{
    public event System.Action<Transform> OnNewNpcAdded;

    private void Awake()
    {
        if (OnNewNpcAdded != null)
        {
            OnNewNpcAdded(transform);
        }
    }
}
