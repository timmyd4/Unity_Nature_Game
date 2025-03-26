using UnityEngine;
using TMPro;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public TextMeshProUGUI roundText;
    public Transform[] spawnPoints;
    public ParticleSystem spawnEffect;

    private int currentRound = 1;
    private int activeEnemies = 0;

    void Start()
    {
        AiBehavior.OnEnemyDied += HandleEnemyDied;
        StartCoroutine(StartRound());
    }

    private void OnDestroy()
    {
        AiBehavior.OnEnemyDied -= HandleEnemyDied;
    }

    private IEnumerator StartRound()
    {
        while (true)
        {
            // Update UI with the current round number
            roundText.text = "Round: " + currentRound;

            // Calculate number of enemies to spawn for the current round
            int enemiesToSpawn = Random.Range(10, 15) + (currentRound - 1);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(4f); // Delay between spawns
            }

            // Wait until all enemies are killed
            while (activeEnemies > 0)
            {
                yield return null;
            }

            // Move to the next round
            currentRound++;
        }
    }

    private void SpawnEnemy()
    {
        // Choose a random spawn point
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[spawnIndex].position;

        // Play particle effect at the spawn point
        if (spawnEffect != null)
        {
            ParticleSystem effect = Instantiate(spawnEffect, spawnPosition, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration + 1f);
        }

        // Spawn the enemy and increment active enemy count
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies++;
    }

    private void HandleEnemyDied(AiBehavior enemy)
    {
        // Decrement active enemy count
        activeEnemies--;

        // Ensure the count doesn’t go negative
        if (activeEnemies < 0)
            activeEnemies = 0;
    }
}
