using UnityEngine;
using VeinsOfMalice.Player;

public class DebugEssenceAdder : MonoBehaviour
{
    [SerializeField] private VeinsOfMalice.World.ItemData essenceItemData;
    private PlayerInventory inventory;

    private void Start()
    {
        inventory = GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            inventory = FindFirstObjectByType<PlayerInventory>();
        }
        
        Debug.Log("<color=yellow>[DebugEssenceAdder]</color> ACTIVADO: ¡Pulsa la tecla 'M' para recibir 1 de Esencia!");
    }

    private void Update()
    {
        // Usa el New Input System (Keyboard)
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.mKey.wasPressedThisFrame)
        {
            if (inventory != null)
            {
                inventory.AddEssence(1);
                if (essenceItemData != null)
                {
                    inventory.AddItem(essenceItemData, 1);
                }
                else
                {
                    Debug.LogWarning("[DebugEssenceAdder] No se ha asignado el essenceItemData en el Inspector, la esencia no aparecerá visualmente en el grid del inventario.");
                }
            }
            else
            {
                Debug.LogWarning("[DebugEssenceAdder] No se encontró el PlayerInventory");
            }
        }
    }
}
