using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineReload : MonoBehaviour
{
    public Vector3 StartPosition, EndPosition;
    private float Length;

    public void Setup(float length)
    {
        transform.localPosition = StartPosition;
        Length = length;
    }

    void Update()
    {
        if (StartPosition != EndPosition)
            transform.localPosition = Vector3.Lerp(transform.localPosition, EndPosition, Time.deltaTime / Length);
        else
            Destroy(this);
    }
}
