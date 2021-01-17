using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotGrid : MonoBehaviour
{
    public Inventory Inventory { get { return inventory; } }
    public IntVector2 GridSize { get { return gridSize; } }
    public SlotScript[,] Slots;

    [SerializeField] private Inventory inventory;
    [SerializeField] private IntVector2 gridSize;

    //Creates inventory slots
    private void CreateSlots()
    {
        Slots = new SlotScript[gridSize.x, gridSize.y];

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
                Slots[x,y].Setup(new IntVector2(x, y), null, IntVector2.Zero, false, this);
        }
    }
}
