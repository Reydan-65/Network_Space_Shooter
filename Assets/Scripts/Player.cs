using UnityEngine;
using Mirror;
using System.Linq;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject[] m_VehiclePrefab;
    [SerializeField] private float m_SpawnSpace;

    private Transform[] m_SpawnPoints;

    public Vehicle ActiveVehicle { get; set; }

    [SyncVar]
    private Color m_PlayerColor;
    public Color PlayerColor => m_PlayerColor;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
        {
            if (UIHUD.Instance != null)
                UIHUD.Instance.SetTarget(transform);

            CmdSpawnClientVehicle();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("Spawn Point");

        if (spawnPointObjects.Length > 0)
        {
            m_SpawnPoints = new Transform[spawnPointObjects.Length];

            for (int i = 0; i < spawnPointObjects.Length; i++)
            {
                m_SpawnPoints[i] = spawnPointObjects[i].transform;
            }
        }

        m_PlayerColor = PlayerColorPallete.Instance.TakeRandomColor();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        PlayerColorPallete.Instance.PutColor(m_PlayerColor);
    }

    [Command]
    public void CmdSpawnClientVehicle()
    {
        if (this == null || !isActiveAndEnabled) return;
        SvSpawnClientVehicle();
    }

    [Server]
    private void SvSpawnClientVehicle()
    {
        if (ActiveVehicle != null) return;
        if (m_SpawnPoints == null || m_SpawnPoints.Length == 0) return;

        Vector2 spawnPosition = Vector2.zero;
        bool foundFreePosition = false;

        var shuffledSpawnPoints = m_SpawnPoints.OrderBy(x => Random.value).ToArray();

        foreach (var spawnPoint in shuffledSpawnPoints)
        {
            if (spawnPoint == null) continue;

            if (Physics2D.OverlapCircle(spawnPoint.position, m_SpawnSpace) == null)
            {
                spawnPosition = spawnPoint.position;
                foundFreePosition = true;
                break;
            }
        }

        if (!foundFreePosition) return;

        GameObject playerVehicle = Instantiate(m_VehiclePrefab[Random.Range(0, m_VehiclePrefab.Length)], spawnPosition, Quaternion.identity);
        if (playerVehicle == null) return;

        NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);

        ActiveVehicle = playerVehicle.GetComponent<Vehicle>();
        if (ActiveVehicle == null) return;

        ActiveVehicle.Owner = netIdentity;

        // Для хоста не подписываемся на сервере
        if (!isLocalPlayer)
        {
            var destructible = playerVehicle.GetComponent<Destructible>();
            if (destructible != null)
            {
                destructible.OnDeath += HandleVehicleDeath;
            }
        }

        RpcSetClientActiveVehicle(ActiveVehicle.netIdentity);
    }

    [ClientRpc]
    private void RpcSetClientActiveVehicle(NetworkIdentity vehicle)
    {
        ActiveVehicle = vehicle.GetComponent<Vehicle>();

        if (ActiveVehicle != null && isLocalPlayer)
        {
            if (VehicleCamera.Instance != null)
            {
                VehicleCamera.Instance.SetTarget(ActiveVehicle.transform);
            }

            var destructible = ActiveVehicle.GetComponent<Destructible>();
            if (destructible != null)
            {
                destructible.OnDeath += HandleVehicleDeath;

                if (destructible.HitPoint <= 0)
                {
                    HandleVehicleDeath();
                }
            }
        }
    }

    public void HandleVehicleDeath()
    {
        if (!isLocalPlayer) return;

        if (VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(null);
        }

        if (UIHUD.Instance != null)
        {
            UIHUD.Instance.SetOnMenuPanel();
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isLocalPlayer && ActiveVehicle != null)
        {
            var destructible = ActiveVehicle.GetComponent<Destructible>();
            if (destructible != null)
            {
                destructible.OnDeath -= HandleVehicleDeath;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (m_SpawnSpace <= 0) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_SpawnSpace);
    }
#endif
}