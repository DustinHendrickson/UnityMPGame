using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerHealth : NetworkBehaviour
{
    public const int maxHealth = 100;
    public bool destroyOnDeath;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;

    public RectTransform healthbar;
    private NetworkStartPosition[] spawnPoints;

    private void Start()
    {
        var go = getChildGameObject(gameObject, "Foreground");
        healthbar = (RectTransform)go.transform;

        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isServer)
        {
            return;
        }

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                currentHealth = maxHealth;
                // called on the Server, but invoked on the Clients
                RpcRespawn();
            }
            
        }

    }

    void OnChangeHealth(int health)
    {
        healthbar.sizeDelta = new Vector2(health, healthbar.sizeDelta.y);
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            // Set the spawn point to origin as a default value
            Vector3 spawnPoint = Vector3.zero;

            // If there is a spawn point array and the array is not empty, pick one at random
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            // Set the player’s position to the chosen spawn point
            transform.position = spawnPoint;
        }
    }

    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<RectTransform>(true);
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }
}