using UnityEngine;
using UnityEngine.UI;
using VeinsOfMalice.World;

namespace VeinsOfMalice.UI
{
    /// <summary>
    /// InventorySlotUI — Representa una casilla individual en el inventario.
    /// </summary>
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject emptyGraphic;
        [SerializeField] private TMPro.TextMeshProUGUI itemNameText;
        [SerializeField] private TMPro.TextMeshProUGUI itemAmountText;
        
        public void SetItem(ItemData item, int amount = 1)
        {
            if (item != null)
            {
                if (iconImage != null)
                {
                    iconImage.sprite = item.icon;
                    iconImage.enabled = item.icon != null;
                }

                if (itemNameText != null) itemNameText.text = item.itemName;
                if (itemAmountText != null) itemAmountText.text = amount > 1 ? "x" + amount : "";

                if (emptyGraphic != null) emptyGraphic.SetActive(false);
            }
            else
            {
                ClearSlot();
            }
        }

        public void ClearSlot()
        {
            if (iconImage != null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }

            if (itemNameText != null) itemNameText.text = "";
            if (itemAmountText != null) itemAmountText.text = "";

            if (emptyGraphic != null) emptyGraphic.SetActive(true);
        }

        // Se pueden añadir callbacks para OnClick, etc.
    }
}
