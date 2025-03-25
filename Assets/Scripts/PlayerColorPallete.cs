using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class PlayerColorPallete : NetworkBehaviour
{
    public static PlayerColorPallete Instance;

    [SerializeField] private List<Color> m_AllColors;

    private List<Color> m_AvailableColors;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        m_AvailableColors = new List<Color>();
        m_AllColors.CopyTo(m_AvailableColors);
    }

    public Color TakeRandomColor()
    {
        int index = Random.Range(0, m_AvailableColors.Count);
        Color color = m_AvailableColors[index];

        m_AvailableColors.RemoveAt(index);

        return color;
    }

    public void PutColor(Color color)
    {
        if (m_AllColors.Contains(color))
        {
            if (!m_AvailableColors.Contains(color))
            {
                m_AvailableColors.Add(color);
            }
        }
    }
}
