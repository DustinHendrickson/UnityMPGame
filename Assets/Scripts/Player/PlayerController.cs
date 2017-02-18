using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public GameObject healthcanvasPrefab;
    public GameObject bulletspawnPrefab;
    public float speed;
    private Rigidbody rb;
    private float distToGround;
    private Camera mainCamera;

    [SyncVar(hook = "OnChangeOwner")]
    public int owner;
    [SyncVar(hook = "OnChangeTeam")]
    public int team;

    void OnChangeTeam(int newTeam)
    {
        team = newTeam;
    }

    void OnChangeOwner(int newOwner)
    {
        owner = newOwner;
    }

    [Command]
    void CmdChangeTeam(GameObject gameObject, int team)
    {
        gameObject.GetComponent<PlayerController>().team = team;
    }

    [Command]
    void CmdChangeOwner(GameObject gameObject, int owner)
    {
        gameObject.GetComponent<PlayerController>().owner = owner;
    }


    void Start()
    {

        rb = GetComponent<Rigidbody>();
        mainCamera = GameObject.FindWithTag("Main Camera").GetComponent<Camera>();
        distToGround = GetComponent<Collider>().bounds.extents.y;

        healthcanvasPrefab = (GameObject)Instantiate(
        healthcanvasPrefab,
        new Vector3(this.transform.position.x, 2, this.transform.position.z),
        transform.rotation);
        healthcanvasPrefab.transform.SetParent(this.transform, false);

        bulletspawnPrefab = (GameObject)Instantiate(
        bulletspawnPrefab,
        new Vector3(this.transform.position.x, 2, this.transform.position.z),
        transform.rotation);
        bulletspawnPrefab.transform.SetParent(this.transform, false);

    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        var upForce = 0.0f;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (Input.GetMouseButtonDown(1) && IsGrounded())
        {
            upForce = 25.0f;
        }

        Vector3 movement = new Vector3(moveHorizontal, upForce, moveVertical);

        rb.AddForce(movement * speed);

    }

    void Update()
    {
        // We move these so everyone sees them.
        healthcanvasPrefab.transform.position = new Vector3(this.transform.position.x, (this.transform.position.y + 2), this.transform.position.z);
        bulletspawnPrefab.transform.position = new Vector3(this.transform.position.x, (this.transform.position.y + 1.5f), this.transform.position.z);

        if (isLocalPlayer)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 clickPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (transform.position - mainCamera.transform.position).magnitude));
                clickPos.y = this.transform.position.y;

                CmdFire(clickPos, owner, team);
            }

            if (IsGrounded())
            {
                GetComponent<MeshRenderer>().material.color = Color.green;
            }
            else
            {
                //GetComponent<MeshRenderer>().material.color = Color.blue;
            }
        } else
        {
            if (gameObject.tag == "Enemy")
            {
                // Make AI Shoot
                Vector3 clickPos = mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(-720.0f, 720.0f), Random.Range(-720.0f, 720.0f), (transform.position - mainCamera.transform.position).magnitude));
                clickPos.y = this.transform.position.y;

                var random = Random.Range(1, 100);
                if (random <= 5)
                {
                    CmdFire(clickPos, 0, 0);
                }
            }
        }


    }


    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.green;

        CmdChangeTeam(gameObject, 1);
        CmdChangeOwner(gameObject, (int)Random.Range(1, 99999999999));
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 2.0f);
    }

    [Command]
    void CmdFire(Vector3 clickPos, int owner, int team)
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletspawnPrefab.transform.position,
            bulletspawnPrefab.transform.rotation);

        // Face the bullet at the clicked position.
        bullet.transform.LookAt(clickPos);

        // Add who owns the bullet.
        bullet.GetComponent<Bullet>().owner = owner;
        bullet.GetComponent<Bullet>().team = team;

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 60;

        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 1 seconds
        Destroy(bullet, 1.0f);
    }


}