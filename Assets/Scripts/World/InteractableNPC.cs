using UnityEngine;

namespace VeinsOfMalice.World
{
    public class InteractableNPC : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        [SerializeField] private string npcName = ""; // Si está vacío usa el nombre del objeto
        [SerializeField] [TextArea(3, 10)] private string dialogueText = "Hola, que tal?";
        
        [Header("Detection Settings")]
        [SerializeField] private float interactionRadius = 3f;
        [SerializeField] private LayerMask playerLayer;

        private bool playerInRange;

        private void Start()
        {
            // Fallback for player layer if not set
            if (playerLayer == 0)
                playerLayer = LayerMask.GetMask("Player");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }

        private void Update()
        {
            // Simple proximity check
            Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius, playerLayer);
            playerInRange = colliders.Length > 0;
            
            // In a more complex system, we might show an "E to interact" prompt here
        }

        public void Interact()
        {
            if (VeinsOfMalice.UI.DialogueUI.Instance != null)
            {
                if (VeinsOfMalice.UI.DialogueUI.Instance.IsDisplaying)
                {
                    VeinsOfMalice.UI.DialogueUI.Instance.HideDialogue();
                }
                else
                {
                    string displayName = string.IsNullOrEmpty(npcName) ? gameObject.name : npcName;
                    VeinsOfMalice.UI.DialogueUI.Instance.ShowDialogue(displayName, dialogueText, transform, interactionRadius);
                }
            }
        }

        public bool IsPlayerInRange() => playerInRange;
    }
}
