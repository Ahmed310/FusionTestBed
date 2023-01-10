using Fusion;
using UnityEngine;

public class NetworkSpawner : NetworkBehaviour
{
    public GameObject playerPref;
    public override void Spawned()
    {
        Runner.Spawn(playerPref, Vector3.zero, Quaternion.identity, Runner.LocalPlayer);
    }
}
