using UnityEngine;
using Mirror;

public class VehicleTurret : NetworkBehaviour
{
    [SerializeField] private GameObject[] m_Projectiles;
    [SerializeField] private float[] m_FireRates;
    [SerializeField] private Transform m_ProjectileStartPosition;

    [SyncVar(hook = nameof(OnPrimaryReloadChanged))]
    private float m_PrimaryReload;
    [SyncVar(hook = nameof(OnSecondaryReloadChanged))]
    private float m_SecondaryReload;

    public float PrimaryReload => m_PrimaryReload;
    public float SecondaryReload => m_SecondaryReload;
    public float[] FireRates => m_FireRates;
    public Transform ProjectileStartPosition => m_ProjectileStartPosition;

    private void Start()
    {
        if (isServer)
        {
            m_PrimaryReload = m_FireRates[0];
            m_SecondaryReload = 0;
        }
    }

    private void Update()
    {
        if (!isServer) return;

        // Обновляем таймеры на сервере
        m_PrimaryReload = Mathf.Min(m_PrimaryReload + Time.deltaTime, m_FireRates[0]);
        m_SecondaryReload = Mathf.Min(m_SecondaryReload + Time.deltaTime, m_FireRates[1]);

        if (m_PrimaryReload >= m_FireRates[0]) m_PrimaryReload = m_FireRates[0];
        if (m_SecondaryReload >= m_FireRates[1]) m_SecondaryReload = m_FireRates[1];
    }

    [Command]
    public void CmdFire(int index)
    {
        if (!isActiveAndEnabled) return;
        SvFire(index);
    }

    [Server]
    private void SvFire(int index)
    {
        if (index == 0 && m_PrimaryReload < m_FireRates[0]) return;
        if (index == 1 && m_SecondaryReload < m_FireRates[1]) return;

        GameObject projectile = Instantiate(m_Projectiles[index],
                                         m_ProjectileStartPosition.position,
                                         m_ProjectileStartPosition.rotation);

        projectile.transform.up = m_ProjectileStartPosition.up;
        projectile.GetComponent<Projectile>().SetParent(transform.root);

        if (projectile.TryGetComponent(out NetworkIdentity identity))
        {
            NetworkServer.Spawn(projectile);
            if (index == 0) m_PrimaryReload = 0;
            if (index == 1) m_SecondaryReload = 0;
        }
        else
        {
            Destroy(projectile);
        }
    }

    private void OnPrimaryReloadChanged(float oldValue, float newValue)
    {
        m_PrimaryReload = newValue;
    }

    private void OnSecondaryReloadChanged(float oldValue, float newValue)
    {
        m_SecondaryReload = newValue;
    }
}
