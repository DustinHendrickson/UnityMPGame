using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    public int owner;
    public int team;

    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        var health = hit.GetComponent<PlayerHealth>();
        var skipDestroy = false;


        if (health != null)
        {
            var hitOwner = hit.GetComponent<PlayerController>().owner;
            var hitTeam = hit.GetComponent<PlayerController>().team;

            if (hitOwner != owner && hitTeam != team)
            {
                health.TakeDamage(10);
            } else
            {
                skipDestroy = true;
            }
        }

        if (skipDestroy == false)
        {
            Destroy(gameObject);
        }
    }

}
