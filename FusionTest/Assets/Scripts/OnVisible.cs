using Fusion;
using UnityEngine;

public class OnVisible : MonoBehaviour
{
    public NetworkObject networkObject; 

    private void OnBecameVisible()
    {
        Debug.Log($"OnBecome Visible {this.name}");
        BasicSpawner.OnVisibilityChange?.Invoke(networkObject, true);
    }

    private void OnBecameInvisible()
    {
        Debug.Log($"OnBecome InVisible {this.name}");
        BasicSpawner.OnVisibilityChange?.Invoke(networkObject, false);
    }
}
