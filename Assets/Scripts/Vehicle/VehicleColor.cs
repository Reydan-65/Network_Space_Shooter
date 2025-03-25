using UnityEngine;
using Mirror;

public class VehicleColor : NetworkBehaviour
{
    [SerializeField] private Vehicle m_Vehicle;
    [SerializeField] private SpriteRenderer[] m_SpriteRenderer;

    private void Start()
    {
        if (m_Vehicle != null && m_SpriteRenderer.Length > 0 && m_Vehicle.Owner != null)
            m_SpriteRenderer[0].color = m_Vehicle.Owner.GetComponent<Player>().PlayerColor;
    }
}
