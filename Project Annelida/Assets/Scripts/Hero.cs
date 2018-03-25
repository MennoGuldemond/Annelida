using UnityEngine;
using UnityEngine.Networking;

public class Hero : NetworkBehaviour
{
    public HeroModel HeroStats;

    [SyncVar]
    private Vector3 _serverPosition;

	private Vector3 _serverPositionSmoothVelocity;

    void Start()
    {
        // Temporary test hero
        HeroStats = new HeroModel
        {
            Name = "Test Hero",
            HitPoints = 100,
            ActionPoints = 10,
            Speed = 5
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
            AuthorityUpdate();
        }
        else
        {
			// Is this hero in the correct position?
			transform.position = Vector3.SmoothDamp(transform.position, _serverPosition, ref _serverPositionSmoothVelocity, 0.25f);			
        }

        // Generic updates for all clients/server

        

    }

    void AuthorityUpdate()
    {
        float movement = Input.GetAxis("Horizontal") * HeroStats.Speed * Time.deltaTime;

        // TODO: do something with moving up and down slopes.
        transform.Translate(movement, 0, 0);

        // Let the network know that we moved.
        CmdUpdatePosition(transform.position);
    }

    [Command]
    void CmdUpdatePosition(Vector3 newPosition)
    {
        // TODO: check movement for legality.
        _serverPosition = newPosition;
    }
}