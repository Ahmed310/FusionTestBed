using Fusion;
using System;
using UnityEngine;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner _runner;

    public static Action<NetworkObject, bool> OnVisibilityChange = delegate { };

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();


    private void UpdateIntresetManagment(NetworkObject networkObject, bool isVisible) 
    {
        return;
        Debug.Log($"Visibility of {networkObject.name} is set to {isVisible} isMine:: {myPlayer.PlayerId}={networkObject.Id.Raw}");
        if(_runner == null) return;
        _runner.SetPlayerAlwaysInterested(myPlayer, networkObject, isVisible);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        UnityEngine.Debug.LogError($"OnConnected...");
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        UnityEngine.Debug.LogError($"OnDC...");
    }

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        UnityEngine.Debug.LogError($"OnHostMigration... {hostMigrationToken.GameMode}");
       
    }


    public static PlayerRef myPlayer;
    NetworkObject playerObject;
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        UnityEngine.Debug.LogError($"OnPlayerJoinned... {player.PlayerId} | {runner.ActivePlayers?.Count()}" );


        if (player == runner.LocalPlayer)
        {
            myPlayer = player;
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            playerObject = networkPlayerObject;
          
            //foreach (var pl in runner.ActivePlayers)
            //{
            //    networkPlayerObject.SetPlayerAlwaysInterested(pl, true);
            //}

        }
        

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        UnityEngine.Debug.LogError($"OnPlayerLeft...");
        // Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (_mouseButton1)
            data.buttons |= NetworkInputData.MOUSEBUTTON2;
        _mouseButton1 = false;

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }


    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        UnityEngine.Debug.LogError($"OnSessionList... {sessionList.Count}");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }


    private bool _mouseButton0;
    private bool _mouseButton1;
    private void Update()
    {
        _mouseButton0 = _mouseButton0 || Input.GetMouseButton(0);
        _mouseButton1 = _mouseButton1 || Input.GetMouseButton(1);
    }

    void Start() 
    {
        OnVisibilityChange = UpdateIntresetManagment;

        StartGame(GameMode.Shared);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("")) { }
        if (GUILayout.Button("")) { }
        if (GUILayout.Button("rejoin")) 
        {
            var closeTask = ShutDown();
            closeTask.ContinueWith(t => reJoin());
        }

        if (GUILayout.Button("ShutDown"))
        {
            ShutDown();
        }

        if (GUILayout.Button("Join"))
        {
            Join();
        }
    }

    public void Join()
    {
        StartGame(GameMode.Shared);
    }


    public void reJoin()
    {
        var shutDown = ShutDown();
        shutDown.ContinueWith(t => StartGame(GameMode.Shared));
    }

    async Task ShutDown()
    {
         await _runner.Shutdown(false);
    }

    async Task StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        if(_runner == null) _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

       

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom"
           
        });
    }

}
