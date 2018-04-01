using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour
{
    private Rigidbody2D _rigidbody;

    // TODO: these are temporary here..
    private float _radius = 2f;
    private float _damage = 10f;
    private bool _damageFallsOff = true;

    public GameObject Owner;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Let the projectile face towards it's direction.
        // TODO: We might not need this.
        float angle = Mathf.Atan2(_rigidbody.velocity.y, _rigidbody.velocity.x) * Mathf.Rad2Deg;
        _rigidbody.rotation = angle;


        if (isServer == true)
        {
            // TODO: Check ground collisions here, since that will not be handled by the phisics engine.
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // The damage/affect logic should only be handled by the server.
        if (isServer == false)
        {
            return;
        }

        // Check with who the collision was with.


    }
}
