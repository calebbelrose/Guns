using UnityEngine;
using System.Collections;

public class DrawRays : MonoBehaviour
{
    public Transform Camera;
    public Transform Gun;
    public Transform Sight;
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Camera.position, Camera.TransformDirection(Vector3.forward) * 100);
        Gizmos.DrawRay(Gun.position, -Gun.up * 100);
        Gizmos.DrawRay(Camera.position, (Sight.position - Camera.position).normalized * 100);
    }
}