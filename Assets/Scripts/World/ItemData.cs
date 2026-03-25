using UnityEngine;

namespace VeinsOfMalice.World
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Veins of Malice/Item")]
    public class ItemData : ScriptableObject
    {
        public string itemName = "New Item";
        public Sprite icon;
        [TextArea] public string description;
        public int value; // Can be cost or sell price
        public bool isStackable = true;
        public int maxStack = 99;
    }
}
