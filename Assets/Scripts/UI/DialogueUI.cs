using UnityEngine;
using TMPro;
using System.Collections;

namespace VeinsOfMalice.UI
{
    public class DialogueUI : MonoBehaviour
    {
        public static DialogueUI Instance { get; private set; }

        [Header("UI Elements")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [Header("Settings")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private float typeSpeed = 0.05f;

        private Coroutine typeCoroutine;
        private bool isTyping;
        private string fullText;
        private Transform currentTalker;
        private float maxDistance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            if (inputReader != null)
                inputReader.OnJumpStarted += HandleSkip;
        }

        private void OnDisable()
        {
            if (inputReader != null)
                inputReader.OnJumpStarted -= HandleSkip;
        }

        private void Start()
        {
            if (dialoguePanel) dialoguePanel.SetActive(false);
        }

        private void HandleSkip()
        {
            if (isTyping)
            {
                if (typeCoroutine != null)
                    StopCoroutine(typeCoroutine);
                
                dialogueText.text = fullText;
                isTyping = false;
            }
        }

        private void Update()
        {
            if (dialoguePanel != null && dialoguePanel.activeSelf && currentTalker != null)
            {
                // Find player
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    float distance = Vector3.Distance(player.transform.position, currentTalker.position);
                    if (distance > maxDistance + 1f) // Give a little buffer
                    {
                        HideDialogue();
                    }
                }
            }
        }

        public void ShowDialogue(string name, string text, Transform talker = null, float range = 5f)
        {
            if (dialoguePanel == null) return;

            dialoguePanel.SetActive(true);
            currentTalker = talker;
            maxDistance = range;
            fullText = text;
            
            if (nameText != null)
                nameText.text = name.ToUpper();
            
            if (dialogueText == null) return;

            if (typeCoroutine != null)
                StopCoroutine(typeCoroutine);
            
            typeCoroutine = StartCoroutine(TypeText(text));
        }

        public void HideDialogue()
        {
            if (dialoguePanel) dialoguePanel.SetActive(false);
            isTyping = false;
        }

        private IEnumerator TypeText(string text)
        {
            isTyping = true;
            dialogueText.text = "";
            foreach (char c in text.ToCharArray())
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }
            isTyping = false;
        }

        public bool IsDisplaying => isTyping || (dialoguePanel != null && dialoguePanel.activeSelf);
    }
}
