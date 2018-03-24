using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    public float TimeLeft = 120;

    void Update()
    {
        if (isServer == false)
        {
			// We assume for now that we don't want to do anything here if we are not the server.
			return;
        }


        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0)
        {
            // Game is over
        }
    }
}
