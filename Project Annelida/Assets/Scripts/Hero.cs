using UnityEngine;
using UnityEngine.Networking;

public class Hero : NetworkBehaviour
{
    public HeroModel HeroStats;

    public GameObject CurrentProjectilePrefab;
    public Transform ProjectileSpawnpoint;
    public Transform ItemGraphics;

    public bool UseSmoothMovement = true;
    public float SmoothTime = 0.2f;
    private Vector3 _serverPositionSmoothVelocity;

    public float CurrentShootPower = 0f;
    public float MaxShootPower = 2f;

    private float _baseAimSpeed = 140f; // Degrees per second.
    private float _powerupSpeed = 2f;

    [SyncVar]
    private Vector3 _serverPosition;

    [SyncVar]
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
            if (UseSmoothMovement == true)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _serverPosition, ref _serverPositionSmoothVelocity, SmoothTime);
                ItemGraphics.localRotation = Quaternion.Slerp(ItemGraphics.localRotation, Quaternion.Euler(0, 0, _aimAngle), SmoothTime);
            }
            else
            {
                transform.position = _serverPosition;
                ItemGraphics.localRotation = Quaternion.Euler(0, 0, _aimAngle);
            }
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

        // TODO: do something with moving up and down slopes etc.
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
        CmdChangeAimAngle(_aimAngle);

        // TODO: Change inputs.
        if (Input.GetButtonDown("Fire"))
        {
            // We started to power up an attack, reset power.
            CurrentShootPower = 0;
        }
        if (Input.GetButton("Fire"))
        {
            // We are powering up an attack.
            CurrentShootPower += Time.deltaTime * _powerupSpeed;
        }
        if (Input.GetButtonUp("Fire") || CurrentShootPower >= MaxShootPower)
        {
            var shootVelocity = new Vector2(
                HeroStats.AimPower * Mathf.Cos(_aimAngle * Mathf.Deg2Rad),
                HeroStats.AimPower * Mathf.Sin(_aimAngle * Mathf.Deg2Rad)
                );

            // Ajust speed of the attack.
            shootVelocity *= CurrentShootPower;
            CmdShoot(ProjectileSpawnpoint.position, shootVelocity);

            // Reset power value and end turn.
            // TODO: End turn.
            CurrentShootPower = 0;
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

    [Command]
    void CmdChangeAimAngle(float newAngle)
    {
        _aimAngle = newAngle;
    }
}