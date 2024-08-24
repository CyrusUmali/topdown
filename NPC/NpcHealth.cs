using System.Collections;
using UnityEngine;

public class NpcHealth : MonoBehaviour
{
    public int startingHealth = 3;
    public int currentHealth;
     public GameObject lastAttacker; // Reference to the last attacker
    private Flash flash;
    private NpcAI npcAI;
    public static event System.Action<Transform> OnNpcDied;

    // References to the health bar components
    public Transform healthBarFill;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        npcAI = GetComponent<NpcAI>();
        currentHealth = startingHealth;
        SetHealthBar((float)currentHealth / startingHealth);
    }

    public void TakeDamage(int damage , GameObject attacker)
    {
        if (npcAI.CurrentState == NpcAI.State.Dead) return; // Check if the NPC is already dead

        currentHealth -= damage;
          lastAttacker = attacker; 
        StartCoroutine(flash.FlashRoutine());
        StartCoroutine(CheckDetectDeathRoutine());
        SetHealthBar((float)currentHealth / startingHealth);
    }

    private IEnumerator CheckDetectDeathRoutine()
    {
        yield return new WaitForSeconds(flash.GetRestoredMatTime());
        DetectDeath();
    }

    private void DetectDeath()
    {
        if (currentHealth <= 0 && npcAI.CurrentState != NpcAI.State.Dead)
        {
            npcAI.Dead(lastAttacker);
            OnNpcDied?.Invoke(transform);
        }
    }

    // Method to update the health bar fill
    private void SetHealthBar(float healthPercent)
    {
        healthPercent = Mathf.Clamp01(healthPercent); // Ensure healthPercent is between 0 and 1

       
            float newScaleX = healthPercent; // HealthPercent directly maps to the scale along X-axis
            float newScaleY = 1f; // Keep Y-scale as it is
            float newScaleZ = 1f; // Keep Z-scale as it is

            // Set the new local scale
            healthBarFill.localScale = new Vector3(newScaleX, newScaleY, newScaleZ);

            // Adjust the local position to anchor the fill from the left
            float offsetX = (1f - healthPercent) / 2f; // Offset to left anchor point
            healthBarFill.localPosition = new Vector3(-offsetX, 0f, 0f); // Adjust X position for left anchor
       
    }
}
