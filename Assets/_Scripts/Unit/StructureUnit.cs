using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureUnit : Unit
{
    private Buildingprocess m_Buildingprocess;

    public bool IsUnderConstruction => m_Buildingprocess != null;

    void Update()
    {
        if (IsUnderConstruction)
        {
            m_Buildingprocess.Update();
            
        }
    }

    public void ResgisterProcess(Buildingprocess process)
    {
        m_Buildingprocess = process;
    }
}
