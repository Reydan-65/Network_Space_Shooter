using UnityEngine;
using Mirror;

public class TankTurret : NetworkBehaviour
{
    [SyncVar]
    private Quaternion syncRotation;

    private Transform turretTransform;

    private void Start()
    {
        turretTransform = transform;
    }

    private void Update()
    {
        if (isServer)
            syncRotation = turretTransform.rotation;
        else
            turretTransform.rotation = Quaternion.Lerp(turretTransform.rotation, syncRotation, Time.deltaTime * 10);
    }

    [Command]
    public void CmdRotateTurret(Quaternion rotation)
    {
        turretTransform.rotation = rotation;
    }
}
