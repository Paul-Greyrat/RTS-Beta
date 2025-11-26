
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private Button m_Button;
    [SerializeField] private Image m_IconImage;

    void Oestroy()
    {
        m_Button.onClick.RemoveAllListeners();
    }
    public void Init(Sprite icon, UnityAction action)
    {
        m_IconImage.sprite = icon;
        m_Button.onClick.AddListener(action);
    }
    
}
