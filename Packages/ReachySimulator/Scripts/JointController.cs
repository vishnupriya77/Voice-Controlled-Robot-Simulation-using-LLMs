using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArticulationBody))]
public class JointController : MonoBehaviour
{
    private float targetPosition;
    private bool changed;
    private ArticulationBody articulation;
    private bool compliant = false;

    void Awake()
    {
        articulation = GetComponent<ArticulationBody>();
        if (articulation == null)
        {
            Debug.LogError($"❌ Missing ArticulationBody on {gameObject.name}");
        }
    }

    void FixedUpdate()
    {
        if (changed && articulation != null)
        {
            var drive = articulation.xDrive;
            drive.target = targetPosition;

            // Set compliance via stiffness
            drive.stiffness = compliant ? 0f : 1000f;
            drive.damping = 100f; // Helps smooth movement
            drive.forceLimit = 1000f;

            articulation.xDrive = drive;


            changed = false;
        }
    }

    public void RotateTo(float newTargetPosition)
    {
        targetPosition = newTargetPosition;
        changed = true;
    }

    public void IsCompliant(bool comp)
    {
        compliant = comp;
        changed = true;
    }

    public float GetPresentPosition()
    {
        return articulation != null ? Mathf.Rad2Deg * articulation.jointPosition[0] : 0f;
    }
}
