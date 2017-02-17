using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealthUpdate : NetworkBehaviour
{

    private RectTransform healthbar;
	
	// Update is called once per frame
	void Update () {

        var bg = transform.parent.gameObject;
        var hc = bg.transform.parent.gameObject;
        var go = hc.transform.parent.gameObject;

        var health = go.GetComponent<PlayerHealth>();
        var healthbar = this.gameObject.GetComponent<RectTransform>();       

        healthbar.sizeDelta = new Vector2(health.currentHealth, healthbar.sizeDelta.y);
    }
}