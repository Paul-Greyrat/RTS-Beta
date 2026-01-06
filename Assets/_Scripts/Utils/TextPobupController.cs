using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPobupController : MonoBehaviour
{
    [SerializeField] private TextPobup m_TextPopupPrefab;

    public void Spawn(string text, Color color, Vector3 position)
    {
        var textPopup = Instantiate(m_TextPopupPrefab);
        textPopup.SetText(text, color);
        textPopup.transform.position = position;
    }
}
