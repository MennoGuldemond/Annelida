using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public GameObject HeroPrefab;

    void Start()
    {
        if (isServer == true)
        {
            SpawnHero();
        }
    }

    void Update()
    {

    }

    public void SpawnHero()
    {
        if (isServer == false)
        {
            Debug.LogError("SpawnHero(): Can only do what it needs to do on the server.");
            return;
        }

        // TODO: We might want to let the player choose where to spawn his/her hero.
        // At least in certain game modes.
        GameObject heroGameObject = Instantiate(HeroPrefab);
        NetworkServer.SpawnWithClientAuthority(heroGameObject, connectionToClient);
    }

}
