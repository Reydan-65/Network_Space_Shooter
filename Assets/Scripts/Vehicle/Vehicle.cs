using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody2D))]
public class Vehicle : NetworkBehaviour
{
    /// <summary>
    /// Масса для автоматической установки у ригида.
    /// Толкающая вперед сила.
    /// Вращающая сила.
    /// Максимальная линейная скорость.
    /// Максимальная вращательная скорость. В градусах/сек
    /// Сохраненная ссылка на ригид.
    /// </summary>
    [Header("Space ship")]
    [SerializeField] private float m_Mass;
    [SerializeField] private float m_Thrust;
    [SerializeField] private float m_Mobility;
    [SerializeField] private float m_MaxLinearVelocity;
    [SerializeField] private float m_MaxAngularVelocity;

    private Rigidbody2D m_Rigid;

    #region Public API

    /// <summary>
    /// Управление линейной тягой. -1.0 до +1.0
    /// Управление вращательной тягой. -1.0 до +1.0
    /// </summary>
    public float ThrustControl { get; set; }
    public float TorqueControl { get; set; }

    #endregion

    #region Unity Event
    private void Start()
    {
        m_Rigid = GetComponent<Rigidbody2D>();
        m_Rigid.mass = m_Mass;

        m_Rigid.inertia = 1;
    }

    private void FixedUpdate()
    {
        if (authority || netIdentity.connectionToClient == null)
        {
            UpdateRigidBody();
        }
    }

    #endregion

    /// <summary>
    /// Метод добавления сил кораблю для движения
    /// </summary>
    private void UpdateRigidBody()
    {
        m_Rigid.AddForce(ThrustControl * m_Thrust * transform.up * Time.fixedDeltaTime, ForceMode2D.Force);
        m_Rigid.AddForce(-m_Rigid.linearVelocity * (m_Thrust / m_MaxLinearVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);
        m_Rigid.AddTorque(TorqueControl * m_Mobility * Time.fixedDeltaTime, ForceMode2D.Force);
        m_Rigid.AddTorque(-m_Rigid.angularVelocity * (m_Mobility / m_MaxAngularVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);
    }
}
