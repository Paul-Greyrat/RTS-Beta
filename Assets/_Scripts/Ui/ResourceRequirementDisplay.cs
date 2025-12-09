using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceRequirementDisplay : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI m_GoldText;
    [SerializeField] private TMPro.TextMeshProUGUI m_WoodText;

    public void Show( int reqGold, int reqWood)
    {
        m_GoldText.text = reqGold.ToString();
        m_WoodText.text = reqWood.ToString();
        UpdateColorRequirment( reqGold, reqWood);

    }

    void UpdateColorRequirment( int reqGold, int reqWood)
    {
        var gameManager = GameManager.Get();

        var greenColor = new Color(0, 0.8f, 0, 1f);
        m_GoldText.color = gameManager.Gold >= reqGold ? greenColor : Color.red;
        m_WoodText.color = gameManager.Wood >= reqWood ? greenColor : Color.red;
        
    }
}
