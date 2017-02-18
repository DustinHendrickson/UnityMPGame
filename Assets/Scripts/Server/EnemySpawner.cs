using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{
    public int enemyCount;
    public GameObject enemyPrefab;

    public int currentEnemyCount = 0;
    public int currentWave = 1;


    public override void OnStartServer()
    {
        Spawn();
        currentWave++;
    }

    private void Update()
    {
        currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (currentEnemyCount == 0)
        {
            Spawn();
            currentWave++;
        }
    }

    void Spawn()
    {
        for (int i = 0; i < (currentWave); i++)
        {
            currentEnemyCount++;
            var spawnPosition = new Vector3(
                transform.position.x + Random.Range(-20.0f, 20.0f),
                transform.position.y,
                transform.position.z + Random.Range(-20.0f, 20.0f));

            var spawnRotation = Quaternion.Euler(
                0.0f,
                Random.Range(0, 180),
                0.0f);

            var enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);

            NetworkServer.Spawn(enemy);
        }
    }
}