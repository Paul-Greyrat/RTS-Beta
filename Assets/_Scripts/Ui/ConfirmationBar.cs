using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationBar : MonoBehaviour
{

    [SerializeField] private ResourceRequirementDisplay m_ResourceDisplay;
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private Button m_CancelButton;

    void OnDisable()
    {
        m_ConfirmButton.onClick.RemoveAllListeners();
        m_CancelButton.onClick.RemoveAllListeners();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show( int reqGold, int reqWood)
    {
        gameObject.SetActive(true);
        m_ResourceDisplay.Show(reqGold, reqWood);
    }

    public void SetupHooks(System.Action onConfirm, System.Action onCancel)
    {
        m_ConfirmButton.onClick.RemoveAllListeners();
        m_ConfirmButton.onClick.AddListener(() => onConfirm?.Invoke());

        m_CancelButton.onClick.RemoveAllListeners();
        m_CancelButton.onClick.AddListener(() => onCancel?.Invoke());
    }

}
