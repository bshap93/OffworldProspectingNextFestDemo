using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Domains.Gameplay.Mining.Scripts
{
    public class FadeAndDestroy : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 3f;

        private void Start()
        {
            StartCoroutine(FadeOutAndDestroy());
        }

        private IEnumerator FadeOutAndDestroy()
        {
            var timer = 0f;

            // Get all renderers in this object and its children
            var renderers = GetComponentsInChildren<Renderer>();

            // Cache original materials to avoid modifying shared ones
            var materials = new Material[renderers.Length];
            for (var i = 0; i < renderers.Length; i++)
            {
                materials[i] = renderers[i].material;
                materials[i].SetFloat("_Mode", 2); // Set to Fade
                materials[i].SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                materials[i].SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                materials[i].SetInt("_ZWrite", 0);
                materials[i].DisableKeyword("_ALPHATEST_ON");
                materials[i].EnableKeyword("_ALPHABLEND_ON");
                materials[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                materials[i].renderQueue = 3000;
            }

            while (timer < fadeDuration)
            {
                var alpha = 1f - timer / fadeDuration;

                foreach (var mat in materials)
                    if (mat.HasProperty("_Color"))
                    {
                        var color = mat.color;
                        color.a = alpha;
                        mat.color = color;
                    }

                timer += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}