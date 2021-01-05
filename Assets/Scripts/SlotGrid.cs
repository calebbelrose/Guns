using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotGrid : MonoBehaviour
{
    public int Width { get { return Width; } }
    public int Height { get { return Height; } }

    [SerializeField] private int width;
    [SerializeField] private int height;

    private SlotScript[,] Slots;
}
