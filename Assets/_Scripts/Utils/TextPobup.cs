using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextPobup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    public void SetText(string text, Color color)
    {
        m_Text.text = text;
        m_Text.color = color;
    }
}
