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

        [Header("Animation")]
        [SerializeField] private float upwardForce = 5f;
        [SerializeField] private float horizontalForceRange = 2f;
        [SerializeField] private float gravity = 15f;
        [SerializeField] private float scalePopDuration = 0.2f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void SpawnDamageNumber(Vector3 worldPosition, float amount, Color color)
        {
            if (damageTextPrefab == null) return;

            GameObject obj = Instantiate(damageTextPrefab, worldPosition + spawnOffset, Quaternion.identity);
            obj.SetActive(true); // Asegurarse de que el objeto esté activado al instanciarse
            TextMeshPro text = obj.GetComponentInChildren<TextMeshPro>(true);
            
            if (text != null)
            {
                text.text = Mathf.RoundToInt(amount).ToString();
                text.color = color;
            }

            // Undertale-style bouncy animation
            StartCoroutine(AnimateDamageNumber(obj, text));
        }

        private System.Collections.IEnumerator AnimateDamageNumber(GameObject obj, TextMeshPro text)
        {
            float elapsed = 0f;
            Vector3 velocity = new Vector3(Random.Range(-horizontalForceRange, horizontalForceRange), upwardForce, 0f);
            
            // Pop effect setup
            Vector3 originalScale = obj.transform.localScale;
            obj.transform.localScale = Vector3.zero;

            while (elapsed < floatingLifeTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / floatingLifeTime;

                // Physics movement
                velocity.y -= gravity * Time.deltaTime;
                obj.transform.position += velocity * Time.deltaTime;

                // Scale pop-in
                if (elapsed < scalePopDuration)
                {
                    float scaleT = elapsed / scalePopDuration;
                    float scaleCurve = Mathf.Sin(scaleT * Mathf.PI * 0.5f);
                    obj.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, scaleCurve);
                }
                else
                {
                    obj.transform.localScale = originalScale;
                }
                
                // Fade out in the last half of the lifetime
                if (text != null && t > 0.5f)
                {
                    Color c = text.color;
                    c.a = 1f - ((t - 0.5f) * 2f);
                    text.color = c;
                }

                yield return null;
            }

            Destroy(obj);
        }
    }
}
