using UnityEngine;
using System.Collections;

public class DrawRays : MonoBehaviour
{
    public Transform Camera;
    public Transform Gun;
    public Transform Sight;
    public int Length = 100;
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Camera.position, Camera.TransformDirection(Vector3.forward) * Length);
        Gizmos.DrawRay(Gun.position, -Gun.up * Length);
        Gizmos.DrawRay(Camera.position, (Sight.position - Camera.position).normalized * Length);
    }
}