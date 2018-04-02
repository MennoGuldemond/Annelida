using UnityEngine;
using UnityEngine.Networking;

public class Hero : NetworkBehaviour
{
    public HeroModel HeroStats;

    public GameObject CurrentProjectilePrefab;
    public Transform ProjectileSpawnpoint;
    public Transform ItemGraphics;

    private Vector3 _serverPositionSmoothVelocity;

    private float _baseAimSpeed = 140f; // Degrees per second.

    [SyncVar]
    private Vector3 _serverPosition;

    [SyncVar(hook = "OnAimAngleChange")]
    private float _aimAngle = 45f;

    void Start()
    {
        // Temporary test hero
        HeroStats = new HeroModel
        {
            Name = "Test Hero",
            HitPoints = 100,
            ActionPoints = 10,
            Speed = 5,
            AimPower = 10
        };
    }

    void Update()
    {
        if (isServer == true)
        {
            // Do things for the hero that only the server may do.
            // For example: take damage.
        }

        if (hasAuthority == true)
        {
            AuthorityMovement();
            AuthorityShooting();
        }
        else
        {
            // Is this hero in the correct position?
            transform.position = Vector3.SmoothDamp(transform.position, _serverPosition, ref _serverPositionSmoothVelocity, 0.25f);
        }

        // Generic updates for all clients/server


    }

    void AuthorityMovement()
    {
        float movement = Input.GetAxis("Horizontal") * HeroStats.Speed * Time.deltaTime;
        if (Input.GetButton("SlowInput"))
        {
            movement *= 0.2f;
        }

        // TODO: do something with moving up and down slopes.
        transform.Translate(movement, 0, 0);

        // Let the network know that we moved.
        CmdUpdatePosition(transform.position);
    }

    void AuthorityShooting()
    {
        float aimAngleChange = -Input.GetAxis("AimHorizontal") * _baseAimSpeed * Time.deltaTime;
        if (Input.GetButton("SlowInput"))
        {
            aimAngleChange *= 0.2f;
        }

        // TODO: Get angle limits from hero.
        _aimAngle = Mathf.Clamp(_aimAngle + aimAngleChange, 0, 180);
        ItemGraphics.localRotation = Quaternion.Euler(0, 0, _aimAngle);

        // Did we shoot?
        // TODO: Change inputs.
        if (Input.GetButtonUp("Fire"))
        {
            var shootVelocity = new Vector2(
                HeroStats.AimPower * Mathf.Cos(_aimAngle * Mathf.Deg2Rad),
                HeroStats.AimPower * Mathf.Sin(_aimAngle * Mathf.Deg2Rad)
                );
            CmdShoot(ProjectileSpawnpoint.position, shootVelocity);
        }
    }

    [Command]
    void CmdChangeItemAngle(float angle)
    {
        _aimAngle = angle;
    }

    [Command]
    void CmdShoot(Vector2 projectilePosition, Vector2 velocity)
    {
        // TODO: Make sure the position and velocity are legal on the server.

        // Create the projectile for the clients.
        var projectileGO = Instantiate(CurrentProjectilePrefab, projectilePosition, Quaternion.identity);
        var projectileRB = projectileGO.GetComponent<Rigidbody2D>();
        projectileRB.position = projectilePosition;
        projectileRB.velocity = velocity;
        projectileRB.rotation = Mathf.Atan2(velocity.y, velocity.x) * -Mathf.Rad2Deg;

        NetworkServer.Spawn(projectileGO);
    }

    [Command]
    void CmdUpdatePosition(Vector3 newPosition)
    {
        // TODO: check movement for legality.
        _serverPosition = newPosition;
    }


    // Syncvar hooks

    void OnAimAngleChange(float newAngle)
    {
        if (hasAuthority == true)
        {
            // This is my hero, I can ignore this.
            return;
        }

        // _aimAngle = newAngle;
    }
}