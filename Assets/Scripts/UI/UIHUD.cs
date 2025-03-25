using UnityEngine;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    public static UIHUD Instance;

    [SerializeField] private GameObject m_UIPanel;
    [SerializeField] private Button m_UILogoutButton;
    [SerializeField] private Image m_RocketFillImage;

    private Transform m_Target;
    private VehicleTurret m_Turret;
    private Player m_Player;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (m_RocketFillImage != null)
            m_RocketFillImage.fillAmount = 0f;

        SetGameplayUI();
    }

    private void Update()
    {
        CheckTargetVehicle();
        UpdateRocketFillImage();
    }

    private void UpdateRocketFillImage()
    {
        if (m_RocketFillImage == null) return;

        if (m_Target != null && (m_Player == null || m_Turret == null))
        {
            m_Player = m_Target.GetComponent<Player>();
            if (m_Player != null && m_Player.ActiveVehicle != null)
                m_Turret = m_Player.ActiveVehicle.GetComponent<VehicleTurret>();
        }

        if (m_Turret == null)
        {
            m_RocketFillImage.fillAmount = 0f;
            return;
        }

        float reloadProgress = Mathf.Clamp01(m_Turret.SecondaryReload / m_Turret.FireRates[1]);
        m_RocketFillImage.fillAmount = 0f + reloadProgress;

        m_RocketFillImage.color = m_RocketFillImage.fillAmount >= 0.99f ? Color.green : Color.yellow;
    }

    private void CheckTargetVehicle()
    {
        if (m_Target == null) return;

        if (m_Player == null)
            m_Player = m_Target.GetComponent<Player>();

        if (m_Player == null || m_Player.ActiveVehicle == null)
        {
            SetOnMenuPanel();
            m_Turret = null; // —брасываем ссылку на турель
        }
        else
            SetGameplayUI();
    }

    public void Respawn()
    {
        if (m_Player != null) m_Player.CmdSpawnClientVehicle();
    }

    public void SetGameplayUI()
    {
        if (m_UIPanel != null) m_UIPanel.SetActive(false);
        if (m_UILogoutButton != null) m_UILogoutButton.gameObject.SetActive(true);
    }

    public void SetOnMenuPanel()
    {
        if (m_UIPanel != null) m_UIPanel.SetActive(true);
        if (m_UILogoutButton != null) m_UILogoutButton.gameObject.SetActive(false);
    }

    public void SetTarget(Transform target)
    {
        m_Target = target;
        m_Player = target?.GetComponent<Player>();
        m_Turret = null;
    }
}