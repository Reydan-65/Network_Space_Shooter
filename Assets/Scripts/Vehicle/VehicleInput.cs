using UnityEngine;

[RequireComponent(typeof(Player))]
public class VehicleInput : MonoBehaviour
{
    private Player m_Player;

    private void Awake()
    {
        m_Player = GetComponent<Player>();
    }

    private void Update()
    {
        if (m_Player.isLocalPlayer)
        {
            UpdateControlKeyboard();
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

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            torque = 1.0f;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            torque = -1.0f;

        m_Player.ActiveVehicle.ThrustControl = thrust;
        m_Player.ActiveVehicle.TorqueControl = torque;
    }
}
