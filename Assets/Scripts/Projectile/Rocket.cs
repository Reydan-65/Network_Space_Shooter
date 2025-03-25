using UnityEngine;

public class Rocket : Projectile
{
    [SerializeField] private float m_minDistance;
    [SerializeField] private float m_RotateSpeed;
    [SerializeField] private float m_RaycastDistance;
    [SerializeField] private float m_ConeAngle;
    [SerializeField] private int m_NumberOfRays;

    private Transform m_Target;

    protected override void MoveProjectile()
    {
        m_Target = FindTarget();

        if (m_Target != null)
        {
            Vector2 direction = (Vector2)m_Target.position - (Vector2)transform.position;
            direction.Normalize();

            float angleToTarget = Vector2.SignedAngle(transform.up, direction);
            float maxRotation = m_RotateSpeed * Time.deltaTime;
            float clampedAngle = Mathf.Clamp(angleToTarget, -maxRotation, maxRotation);

            transform.Rotate(Vector3.forward, clampedAngle);
        }

        transform.Translate(transform.up * m_MovementSpeed * Time.deltaTime, Space.World);
    }

    private Transform FindTarget()
    {
        if (m_Target == null)
        {
            float angleStep = m_ConeAngle / (m_NumberOfRays - 1);
            
            for (int i = 0; i < m_NumberOfRays; i++)
            {
                float angle = -m_ConeAngle / 2 + i * angleStep;
                Vector2 direction = Quaternion.Euler(0, 0, angle) * transform.up;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, m_RaycastDistance);

                if (hit.collider != null)
                {
                    if (hit.collider.transform.root != m_Parent)
                    {
                        var distance = Vector3.Distance(transform.position, hit.point);

                        if (distance < m_minDistance && hit.collider.transform.root.TryGetComponent(out Vehicle vehicle) && vehicle.GetComponent<Destructible>().MaxHitPoint > 0)
                        {
                            return vehicle.transform;
                        }
                    }
                }
            }
        }
        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (m_minDistance <= 0) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_minDistance);

        float angleStep = m_ConeAngle / (m_NumberOfRays - 1);
        for (int i = 0; i < m_NumberOfRays; i++)
        {
            float angle = -m_ConeAngle / 2 + i * angleStep;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * transform.up;
            Gizmos.DrawRay(transform.position, direction * m_RaycastDistance);
        }
    }
#endif
}
