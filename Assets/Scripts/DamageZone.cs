using Mirror;
using System.Collections;
using UnityEngine;

public class DamageZone : NetworkBehaviour
{
    [SerializeField] private float m_MovementSpeed = 1.0f;
    [SerializeField] private int m_Damage;
    [SerializeField] private float m_Rate;
    [SerializeField] private Vector3 m_MaxScale;
    [SerializeField] private float m_StepScale = 0.01f;

    [SyncVar]
    private Vector2 direction;

    [SyncVar]
    private Vector3 syncScale;

    [SyncVar]
    private Vector3 syncPosition;

    private float currentTime;

    public override void OnStartServer()
    {
        base.OnStartServer();
        syncScale = transform.GetChild(0).localScale;
        syncPosition = transform.position;
    }

    public void SetDirectionServer(Vector2 newDirection)
    {
        direction = newDirection;
    }

    private void Update()
    {
        if (isServer)
        {
            currentTime += Time.deltaTime;
            Move();
            UpdateScale();
            OnHit();
        }
        else
        {
            transform.position = syncPosition;
            transform.GetChild(0).localScale = syncScale;
        }
    }

    private void Move()
    {
        if (direction != Vector2.zero)
        {
            transform.position += (Vector3)direction * m_MovementSpeed * Time.deltaTime;
            syncPosition = transform.position;
        }
    }

    private void UpdateScale()
    {
        if (transform.GetChild(0).localScale.x < m_MaxScale.x)
        {
            syncScale = Vector3.Lerp(
                transform.GetChild(0).localScale,
                m_MaxScale,
                m_StepScale * Time.deltaTime * 10
            );
            transform.GetChild(0).localScale = syncScale;
        }
        else
        {
            StartCoroutine(OnMaxScale());
        }
    }

    private IEnumerator OnMaxScale()
    {
        yield return new WaitForSeconds(m_Rate);
        NetworkServer.Destroy(gameObject);
    }

    private void OnHit()
    {
        if (currentTime >= m_Rate)
        {
            float radius = transform.GetChild(0).localScale.x;
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

            foreach (Collider2D hit in hits)
            {
                if (hit.transform.root.TryGetComponent(out Destructible destructible))
                {
                    destructible.SvApplyDamage(m_Damage);
                }
            }
            currentTime = 0;
        }
    }
}