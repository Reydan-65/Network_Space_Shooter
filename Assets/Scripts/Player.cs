using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject m_VehiclePrefab;

    public Vehicle ActiveVehicle { get; set; }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
        {
            Debug.Log("Local player started.");
            CmdSpawnClientVehicle();
        }
    }

    [Command]
    private void CmdSpawnClientVehicle()
    {
        Debug.Log("CmdSpawnClientVehicle called.");
        SvSpawnClientVehicle();
    }

    [Server]
    private void SvSpawnClientVehicle()
    {
        if (ActiveVehicle != null) return;

        GameObject playerVehicle = Instantiate(m_VehiclePrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);

        ActiveVehicle = playerVehicle.GetComponent<Vehicle>();
        RpcSetClientActiveVehicle(ActiveVehicle.netIdentity);
        Debug.Log("Server spawned vehicle.");
    }

    [ClientRpc]
    private void RpcSetClientActiveVehicle(NetworkIdentity vehicle)
    {
        ActiveVehicle = vehicle.GetComponent<Vehicle>();

        if (ActiveVehicle != null && isLocalPlayer && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(ActiveVehicle.transform);
            Debug.Log("Client set active vehicle.");
        }
    }
}
