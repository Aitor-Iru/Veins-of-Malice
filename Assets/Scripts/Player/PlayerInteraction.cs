using UnityEngine;
using VeinsOfMalice.World;

namespace VeinsOfMalice.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private float interactionRange = 3f;
        [SerializeField] private LayerMask interactableLayer;

        private void OnEnable()
        {
            if (interactableLayer.value == 0)
            {
                interactableLayer = LayerMask.GetMask("Gameplay");
            }

            if (inputReader != null)
            {
                inputReader.OnInteractStarted += HandleInteract;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.OnInteractStarted -= HandleInteract;
            }
        }

        private void HandleInteract()
        {
            Debug.Log($"<color=cyan>[PlayerInteraction]</color> Interact key pressed at position {transform.position}.");
            
            // Find the nearest NPC
            Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer, QueryTriggerInteraction.Collide);
            
            string layerNames = "";
            for (int i = 0; i < 32; i++)
            {
                if ((interactableLayer.value & (1 << i)) != 0)
                {
                    layerNames += LayerMask.LayerToName(i) + " ";
                }
            }

            Debug.Log($"<color=cyan>[PlayerInteraction]</color> Found {colliders.Length} colliders in range {interactionRange} on mask {interactableLayer.value} (Layers: {layerNames})");

            InteractableNPC nearestNPC = null;
            float shortestDistance = Mathf.Infinity;

            foreach (var col in colliders)
            {
                InteractableNPC npc = col.GetComponent<InteractableNPC>();
                if (npc != null)
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestNPC = npc;
                    }
                }
            }

            if (nearestNPC != null)
            {
                Debug.Log($"<color=green>[PlayerInteraction]</color> Interacting with {nearestNPC.name}.");
                nearestNPC.Interact();
            }
            else
            {
                Debug.LogWarning("<color=yellow>[PlayerInteraction]</color> No NPC found in range.");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}
