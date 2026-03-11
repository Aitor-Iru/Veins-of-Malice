using UnityEngine;
using VeinsOfMalice.Player;
using VeinsOfMalice.World;

namespace VeinsOfMalice.DebugUtils
{
    public class InventoryTester : MonoBehaviour
    {
        public ItemData testItem;
        public PlayerInventory inventory;

        [ContextMenu("Add Test Item")]
        public void AddItem()
        {
            if (inventory == null) inventory = GetComponent<PlayerInventory>();
            if (testItem != null && inventory != null)
            {
                inventory.AddItem(testItem);
            }
        }
    }
}
