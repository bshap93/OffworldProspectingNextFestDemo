using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Gameplay.Mining.Scripts
{
    public class DecimatedStonePieces : MonoBehaviour
    {
        public float disintegrationDelay = 1.5f;
        public float fadeDuration = 0.75f;
        public float maxStagger = 0.4f;

        [SerializeField] private MMFeedbacks disintegrationFeedback;
        [SerializeField] private GameObject disintegrationParticles;
        [SerializeField] private GameObject[] pieces;

        private void Start()
        {
            StartCoroutine(Disintegrate());
        }

        private IEnumerator Disintegrate()
        {
            yield return new WaitForSeconds(disintegrationDelay);
            disintegrationFeedback?.PlayFeedbacks();

            foreach (var piece in pieces)
                if (piece != null)
                {
                    StartCoroutine(FadeAndDestroyPiece(piece));
                    yield return new WaitForSeconds(Random.Range(0f, maxStagger));
                }

            // Destroy the root after all pieces are done
            Destroy(gameObject, 3f);
        }

        private IEnumerator FadeAndDestroyPiece(GameObject piece)
        {
            var renderers = piece.GetComponentsInChildren<Renderer>();
            var propertyBlock = new MaterialPropertyBlock();

            var elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                var alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                foreach (var renderer in renderers)
                {
                    renderer.GetPropertyBlock(propertyBlock);
                    propertyBlock.SetColor("_Color", new Color(1f, 1f, 1f, alpha));

                    renderer.SetPropertyBlock(propertyBlock);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Final alpha = 0
            foreach (var renderer in renderers)
            {
                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
                renderer.SetPropertyBlock(propertyBlock);
            }

            // Spawn disintegration particle
            if (disintegrationParticles != null)
            {
                var fx = Instantiate(disintegrationParticles, piece.transform.position, Quaternion.identity);
                Destroy(fx, 2f);
            }

            Destroy(piece);
        }
    }
}