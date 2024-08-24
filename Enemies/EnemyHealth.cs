using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockBackThrust = 0f;

    // Set the spawner reference
    public void SetSpawner(MobSpawner spawner)
    {
        this.spawner = spawner;
    }

     private MobSpawner spawner;

    private int currentHealth;
    private Knockback knockback;
    private Flash flash;

    private void Awake (){
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    private void Start() {      
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        knockback.GetKnockedBack(PlayerController.Instance.transform ,  knockBackThrust);
        StartCoroutine(flash.FlashRoutine());
        StartCoroutine(CheckDetectDeathRoutine());
    }

    private IEnumerator CheckDetectDeathRoutine(){
        yield return new WaitForSeconds(flash.GetRestoredMatTime());
        DetectDeath();
    }

    public void DetectDeath() {
        if (currentHealth <= 0) {
            Instantiate(deathVFXPrefab , transform.position , Quaternion.identity);
            Destroy(gameObject);
     
      if (spawner != null)
        {
            spawner.NotifyMobDestroyed(this.gameObject);
        }


        }
    }
}
