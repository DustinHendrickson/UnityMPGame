using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public GameObject healthcanvasPrefab;
    public float speed;
    private Rigidbody rb;
    private float distToGround;
    private Camera mainCamera;
    private Vector3 lastFaceDirection;
    private Animation anim;
    public Transform bulletSpawn;
    public float maxSpeed = 10.0f;
    public float jumpForce = 40.0f;

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

        anim = GetComponent<Animation>();

        anim["Walk"].speed = 2.0f;

    }

    public void FaceVector(Vector3 facePoint)
    {
        Vector3 faceDirection = Vector3.Normalize(facePoint);
        if (faceDirection != Vector3.zero)
        {
            transform.forward = faceDirection;
            lastFaceDirection = faceDirection;
        }
        else
        {
            if (lastFaceDirection != Vector3.zero)
            {
                transform.forward = lastFaceDirection;
            }
        }

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

        if (IsGrounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                upForce = jumpForce;
            }

            Vector3 movement = new Vector3(moveHorizontal, upForce, moveVertical);

            // Face the way we're moving.
            FaceVector(new Vector3(moveHorizontal, 0, moveVertical));

            // Add the force to move.
            rb.AddForce(movement * speed);

            if (movement != Vector3.zero)
            {
                // Set the animation speed to double the normal based on progress twoard max speed.
                anim["Walk"].speed = rb.velocity.magnitude / (maxSpeed / 2);
                if (!anim.IsPlaying("Walk"))
                {
                    anim.Play("Walk");
                }
            }
            else
            {
                if (!anim.IsPlaying("Idle"))
                {
                    anim.Play("Idle");
                }
            }
        } else
        {
            if (!anim.IsPlaying("Idle"))
            {
                anim.Play("Idle");
            }
        }


        // Restrict Max Speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }


    }

    void Update()
    {
        // We move these so everyone sees them.
        healthcanvasPrefab.transform.position = new Vector3(this.transform.position.x, (this.transform.position.y + 4), this.transform.position.z);

        if (isLocalPlayer)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 clickPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (transform.position - mainCamera.transform.position).magnitude));
                clickPos.y = this.transform.position.y;
   

                CmdFire(clickPos, owner, team);
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
        CmdChangeTeam(gameObject, 1);
        CmdChangeOwner(gameObject, (int)Random.Range(1, 99999999999));
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.2f);
    }

    [Command]
    void CmdFire(Vector3 clickPos, int owner, int team)
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Make sure the bullet doesnt collide with the shooter.
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());

        // Face the bullet at the clicked position.
        bullet.transform.LookAt(clickPos);

        // Add who owns the bullet.
        bullet.GetComponent<Bullet>().owner = owner;
        bullet.GetComponent<Bullet>().team = team;

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 30;

        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 1 seconds
        Destroy(bullet, 1.0f);
    }


}