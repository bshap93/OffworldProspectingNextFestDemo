using DG.Tweening;
using UnityEngine;

namespace Domains.Effects.Scripts
{
    public static class FadeUtils
    {
        public static void FadeOut(GameObject go, float duration = 0.2f)
        {
            var renderers = go.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            foreach (var mat in r.materials)
                if (mat.HasProperty("_Color"))
                {
                    var color = mat.color;
                    mat.DOFade(0f, duration).SetEase(Ease.OutQuad);
                }
        }

        public static void FadeIn(GameObject go, float duration = 0.2f)
        {
            var renderers = go.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            foreach (var mat in r.materials)
                if (mat.HasProperty("_Color"))
                {
                    var color = mat.color;
                    color.a = 0f;
                    mat.color = color;
                    mat.DOFade(1f, duration).SetEase(Ease.OutQuad);
                }
        }
    }
}