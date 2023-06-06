using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool IsTarget
    {
        get { return isTarget; }
        set
        {
            isTarget = value;
        }
    }

    [SerializeField] private Material defaultMat;
    [SerializeField] private Material hoverMat;
    [SerializeField] private Material targetMat;
    [SerializeField] private Color color;

    private bool isTarget;

    private void Awake()
    {
        ResetMaterial();
    }

    private bool ParentHasMaterial()
    {
        return this.GetComponent<MeshRenderer>() != null;
    }

    private void CleanMaterials(Material[] matArray, Material defMat)
    {
        if (matArray.Length <= 1) return;

        Material[] cleanMaterialList = { defMat };
        matArray = cleanMaterialList;
    }

    public void AssignRandomColor()
    {
        if (!ParentHasMaterial())
        {
            color = new Color(Random.value, Random.value, Random.value);
            this.GetComponentInChildren<MeshRenderer>().material.color = color;
        }
    }

    public virtual void SetMaterial(Material mat)
    {
        Material[] cleanMaterialList = { mat };

        if (!ParentHasMaterial())
        {
            this.GetComponentInChildren<MeshRenderer>().materials = cleanMaterialList;
            return;
        }
        
        this.GetComponent<MeshRenderer>().materials = cleanMaterialList;
    }

    public virtual void SetMaterials(Material[] mats)
    {
        if (!ParentHasMaterial())
        {
            this.GetComponentInChildren<MeshRenderer>().materials = mats;
            return;
        }

        this.GetComponent<MeshRenderer>().materials = mats;
    }

    public void UseHovermat()
    {
        if (isTarget)
        {
            Material[] hoverTargetSet = { targetMat, hoverMat };
            SetMaterials(hoverTargetSet);
            return;
        }

        Material[] hoverDefaultSet = { defaultMat, hoverMat };
        SetMaterials(hoverDefaultSet);
    }

    public void ResetMaterial()
    {
        if (isTarget)
        {
            SetMaterial(targetMat);
            return;
        }

        SetMaterial(defaultMat);
    }
}
