using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected float m_MovementSpeed;
    [SerializeField] protected int m_Damage;
    [SerializeField] protected float m_LifeTime;
    [SerializeField] protected GameObject m_DestroySFX;

    protected Transform m_Parent;

    protected virtual void Start()
    {
        Destroy(gameObject, m_LifeTime);
        m_Parent = transform.root;
    }

    protected virtual void Update()
    {
        MoveProjectile();
        OnHit();
    }

    protected virtual void OnHit()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, Time.deltaTime * m_MovementSpeed);

        if (hit == true)
        {
            if (hit.collider.transform.root != m_Parent && hit.collider.transform.root.GetComponent<DamageZone>() == false)
            {
                if (NetworkSessionManager.Instance != null)
                {
                    if (NetworkSessionManager.Instance.IsServer)
                    {
                        Destructible dest = hit.collider.transform.root.GetComponent<Destructible>();

                        if (dest != null)
                        {
                            dest.SvApplyDamage(m_Damage);
                        }
                    }

                    if (NetworkSessionManager.Instance.IsClient)
                    {
                        if (m_DestroySFX != null)
                        {
                            Instantiate(m_DestroySFX, transform.position, Quaternion.identity);
                        }
                        else
                            Debug.LogWarning("DestroySFX is null!");
                    }
                }

                Destroy(gameObject);
            }
        }
    }

    protected virtual void MoveProjectile()
    {
        float stepLength = Time.deltaTime * m_MovementSpeed;
        Vector2 step = transform.up * stepLength;
        transform.position += new Vector3(step.x, step.y, 0);
    }

    // Public Methods
    public void SetParent(Transform parent)
    {
        m_Parent = parent;
    }
}
