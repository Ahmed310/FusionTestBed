using Fusion;
using System.Linq;
using UnityEngine;

public class Player : NetworkBehaviour, IPlayerJoined
{
    [SerializeField] private PhysxBall _prefabPhysxBall;
    [SerializeField] private NetworkPrefabRef _playerPrefab;

    private NetworkCharacterControllerPrototype _cc;
    private Vector3 _forward;
    private TickTimer delay;


    [Networked(OnChanged = nameof(OnBallSpawned))]
    public NetworkBool spawned { get; set; }

    public static void OnBallSpawned(Changed<Player> changed)
    {
        changed.Behaviour.material.color = Color.white;
    }

    [ContextMenu("Cull")]
    public void NetworkCull() 
    {
        Runner.SetPlayerAlwaysInterested(BasicSpawner.myPlayer, Object, false);
    }

    [ContextMenu("UnCull")]
    public void UnNetworkCull()
    {
        Runner.SetPlayerAlwaysInterested(BasicSpawner.myPlayer, Object, true);
    }

    public override void Spawned()
    {
        base.Spawned();
        //var netObj = GetComponent<NetworkObject>();
        var count = Runner.ActivePlayers.Count();


        //foreach (var player in Runner.ActivePlayers)
        //{
        //    Object.SetPlayerAlwaysInterested(player, true);
        //}

        Debug.LogError($"players count:: {count}");
    }

    private Material _material;
    Material material
    {
        get
        {
            if (_material == null)
                _material = GetComponentInChildren<MeshRenderer>().material;
            return _material;
        }
    }

    public override void Render()
    {
        material.color = Color.Lerp(material.color, Color.blue, Time.deltaTime);
    }

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
    }



    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            transform.Translate(5 * data.direction * Runner.DeltaTime);
            //_cc.Move(5 * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0)
                _forward = data.direction;

            if (delay.ExpiredOrNotRunning(Runner))
            {
                if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabPhysxBall,
                      transform.position + _forward,
                      Quaternion.LookRotation(_forward),
                      Object.InputAuthority,
                      (runner, o) =>
                      {
                          // Initialize the Ball before synchronizing it
                          o.GetComponent<PhysxBall>().Init(_forward);
                      });
                    spawned = !spawned;
                }
                else if ((data.buttons & NetworkInputData.MOUSEBUTTON2) != 0)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabPhysxBall,
                      transform.position + _forward,
                      Quaternion.LookRotation(_forward),
                      Object.InputAuthority,
                      (runner, o) =>
                      {
                          o.GetComponent<PhysxBall>().Init(10 * _forward);
                      });
                    spawned = !spawned;
                }
            }
        }
        return;
        if (this.HasInputAuthority)
        {
            BasicSpawner._runner.AddPlayerAreaOfInterest(BasicSpawner.myPlayer, transform.position, 10);
        }
    }

    void IPlayerJoined.PlayerJoined(PlayerRef player)
    {
        //if (Runner.IsServer)
        //{
        //    Object.SetPlayerAlwaysInterested(player, true);
        //}

        //Object.SetPlayerAlwaysInterested(player, true);
    }
}