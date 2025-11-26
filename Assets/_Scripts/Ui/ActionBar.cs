

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour
{
    [SerializeField] private Image m_BackgroundImage;
    [SerializeField] private ActionButton m_ActionButtonPrefab;

    private Color m_originalColor;
    private List<ActionButton> m_ActionButtons = new();

    private void Awake()
    {
        m_originalColor = m_BackgroundImage.color;
        Hiden();

    }

    public void RegisterAction()
    {
        var actionButton = Instantiate(m_ActionButtonPrefab, transform);
        actionButton.transform.SetParent(transform);
    }

    public void ClearActions()
    {
        for (int i = m_ActionButtons.Count - 1; i>= 0; i--)
        {
            Destroy(m_ActionButtons[i].gameObject);
            m_ActionButtons.RemoveAt(i);
        }
    }

    public void Hiden()
    {
        m_BackgroundImage.color = new Color(0, 0, 0, 0);
    }

    public void Show()
    {
        m_BackgroundImage.color = m_originalColor;
    }


}
