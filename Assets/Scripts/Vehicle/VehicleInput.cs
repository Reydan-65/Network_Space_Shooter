using UnityEngine;

[RequireComponent(typeof(Player))]
public class VehicleInput : MonoBehaviour
{
    private Player m_Player;

    [SerializeField] private float m_MouseSensivity;

    private void Awake()
    {
        m_Player = GetComponent<Player>();
    }

    private void Update()
    {
        if (m_Player.isLocalPlayer)
        {
            UpdateControlKeyboard();
            UpdateControlMouse();
        }
    }

    private void UpdateControlKeyboard()
    {
        if (m_Player.ActiveVehicle == null) return;

        float thrust = 0;
        float torque = 0;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            thrust = 1.0f;

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            thrust = -1.0f;

        if (m_Player.ActiveVehicle.Type == VehicleType.Tank)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                torque = 1.0f;

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                torque = -1.0f;
        }

        if (Input.GetKey(KeyCode.Space))
            m_Player.ActiveVehicle.Fire(0);

        if (Input.GetKey(KeyCode.LeftControl))
            m_Player.ActiveVehicle.Fire(1);

        m_Player.ActiveVehicle.ThrustControl = thrust;
        m_Player.ActiveVehicle.TorqueControl = torque;
    }

    private void UpdateControlMouse()
    {
        if (m_Player.ActiveVehicle == null) return;

        Vector3 mousePosition = Input.mousePosition;
        Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -10));

        if (m_Player.ActiveVehicle.Type == VehicleType.Ship)
        {
            Vector3 direction = position - m_Player.ActiveVehicle.transform.position;
            m_Player.ActiveVehicle.transform.up = new Vector3(direction.x, direction.y, 0);
        }

        if (m_Player.ActiveVehicle.Type == VehicleType.Tank)
        {
            Transform turret = m_Player.ActiveVehicle.transform.GetChild(0).GetChild(1).GetComponent<Transform>();
            Vector3 direction = position - turret.position;

            if (turret != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
                turret.GetComponent<TankTurret>().CmdRotateTurret(targetRotation);
            }
        }
    }
}
