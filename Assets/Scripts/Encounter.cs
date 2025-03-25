using Mirror;
using UnityEngine;

public class Encounter : NetworkBehaviour
{
    [SerializeField] private GameObject[] m_EncountersPrefab;
    [SerializeField] private float m_RateTime;

    [SyncVar]
    private float currentTime;

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentTime = 0;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    private void Update()
    {
        if (!isServer) return;

        currentTime += Time.deltaTime;

        if (currentTime >= m_RateTime)
        {
            Vector2 direction = GetRandomDirection();
            SpawnEncounter(direction);
            currentTime = 0;
        }
    }

    private void SpawnEncounter(Vector2 direction)
    {
        GameObject encounter = Instantiate(m_EncountersPrefab[0], transform.position, transform.rotation);

        DamageZone damageZone = encounter.GetComponent<DamageZone>();
        damageZone.SetDirectionServer(direction);

        NetworkServer.Spawn(encounter);
    }

    private Vector2 GetRandomDirection()
    {
        Vector2 dir;
        do
        {
            dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        } while (dir == Vector2.zero);

        return dir;
    }
}
