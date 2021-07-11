//========= Copyright 2016-2019, HTC Corporation. All rights reserved. ===========
//ReticlePoser slightly modified to mark objects of determined Layers

using HTC.UnityPlugin.Pointer3D;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;

public class CapReticlePoser : MonoBehaviour
{
    public interface IMaterialChanger
    {
        Material reticleMaterial { get; }
    }
    #region Public variables
    #region Raycaster variables
    public Pointer3DRaycaster raycaster;
    [FormerlySerializedAs("Target")]
    public Transform reticleForDefaultRay;
    public Transform reticleForCurvedRay;
    public bool showOnHitOnly = true;
    #endregion
    #region Hit variables
    public GameObject hitTarget;
    public float hitDistance;
    public Material defaultReticleMaterial;
    public MeshRenderer[] reticleRenderer;

    public int[] validLayers;
    #endregion
    #endregion

    #region Private variables
    private Material m_matFromChanger;
    #endregion
#if UNITY_EDITOR
    protected virtual void Reset()
    {
        for (var tr = transform; raycaster == null && tr != null; tr = tr.parent)
        {
            raycaster = tr.GetComponentInChildren<Pointer3DRaycaster>(true);
        }

        reticleRenderer = GetComponentsInChildren<MeshRenderer>(true);
    }
#endif
    protected virtual void LateUpdate()
    {
        var points = raycaster.BreakPoints;
        var pointCount = points.Count;
        var result = raycaster.FirstRaycastResult();

        //Debug.Log(result.gameObject.tag);

        if ((showOnHitOnly && !result.isValid) || pointCount <= 1 || !validLayers.Contains(result.gameObject.layer))
        {
            reticleForDefaultRay.gameObject.SetActive(false);
            reticleForCurvedRay.gameObject.SetActive(false);
            return;
        }

        var isCurvedRay = raycaster.CurrentSegmentGenerator() != null;

        if (reticleForDefaultRay != null) { reticleForDefaultRay.gameObject.SetActive(!isCurvedRay); }
        if (reticleForCurvedRay != null) { reticleForCurvedRay.gameObject.SetActive(isCurvedRay); }

        var targetReticle = isCurvedRay ? reticleForCurvedRay : reticleForDefaultRay;
        if (result.isValid && validLayers.Contains(result.gameObject.layer))
        {
            if (targetReticle != null)
            {
                targetReticle.position = result.worldPosition;
                targetReticle.rotation = Quaternion.LookRotation(result.worldNormal, raycaster.transform.forward);
            }

            hitTarget = result.gameObject;
            hitDistance = result.distance;
        }
        else
        {
            if (targetReticle != null)
            {
                targetReticle.position = points[pointCount - 1];
                targetReticle.rotation = Quaternion.LookRotation(points[pointCount - 1] - points[pointCount - 2], raycaster.transform.forward);
            }

            hitTarget = null;
            hitDistance = 0f;
        }

        // Change reticle material according to IReticleMaterialChanger
        var matChanger = hitTarget == null ? null : hitTarget.GetComponentInParent<IMaterialChanger>();
        var newMat = matChanger == null ? null : matChanger.reticleMaterial;
        if (m_matFromChanger != newMat)
        {
            m_matFromChanger = newMat;

            if (newMat != null)
            {
                SetReticleMaterial(newMat);
            }
            else if (defaultReticleMaterial != null)
            {
                SetReticleMaterial(defaultReticleMaterial);
            }
        }
    }

    private void SetReticleMaterial(Material mat)
    {
        if (reticleRenderer == null || reticleRenderer.Length == 0) { return; }

        foreach (MeshRenderer mr in reticleRenderer)
        {
            mr.material = mat;
        }
    }

    protected virtual void OnDisable()
    {
        reticleForDefaultRay.gameObject.SetActive(false);
        reticleForCurvedRay.gameObject.SetActive(false);
    }
}
