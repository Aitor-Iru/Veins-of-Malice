using UnityEngine;
using TMPro;

namespace VeinsOfMalice.UI
{
    /// <summary>
    /// Spawns floating damage numbers in the world.
    /// </summary>
    public class DamageNumberManager : MonoBehaviour
    {
        public static DamageNumberManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private float floatingLifeTime = 1f;
        [SerializeField] private Vector3 spawnOffset = new Vector3(0, 1.5f, 0);

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void SpawnDamageNumber(Vector3 worldPosition, float amount, Color color)
        {
            if (damageTextPrefab == null) return;

            GameObject obj = Instantiate(damageTextPrefab, worldPosition + spawnOffset, Quaternion.identity);
            TextMeshPro text = obj.GetComponentInChildren<TextMeshPro>();
            
            if (text != null)
            {
                text.text = Mathf.RoundToInt(amount).ToString();
                text.color = color;
            }

            // Simple move up and fade out effect
            StartCoroutine(AnimateDamageNumber(obj, text));
        }

        private System.Collections.IEnumerator AnimateDamageNumber(GameObject obj, TextMeshPro text)
        {
            float elapsed = 0f;
            Vector3 startPos = obj.transform.position;
            Vector3 targetPos = startPos + Vector3.up * 1f;

            while (elapsed < floatingLifeTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / floatingLifeTime;

                obj.transform.position = Vector3.Lerp(startPos, targetPos, t);
                
                if (text != null)
                {
                    Color c = text.color;
                    c.a = 1f - t;
                    text.color = c;
                }

                yield return null;
            }

            Destroy(obj);
        }
    }
}
