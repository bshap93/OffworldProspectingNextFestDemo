﻿using System.Collections;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Domains.Input.Scripts
{
    public class ButtonPromptWithAction : MonoBehaviour
    {
        [Header("Bindings")]
        /// the image to use as the prompt's border
        [Tooltip("the image to use as the prompt's border")]
        public Image Border;

        /// the image to use as background
        [Tooltip("the image to use as background")]
        public Image Background;

        /// the canvas group of the prompt's container
        [Tooltip("the canvas group of the prompt's container")]
        public CanvasGroup ContainerCanvasGroup;

        /// the Text component of the prompt
        [Tooltip("the Text component of the prompt")]
        public TMP_Text PromptText;

        [Tooltip("the action")] public TMP_Text ActionText;

        [Header("Durations")]
        /// the duration of the fade in, in seconds
        [Tooltip("the duration of the fade in, in seconds")]
        public float FadeInDuration = 0.2f;

        /// the duration of the fade out, in seconds
        [Tooltip("the duration of the fade out, in seconds")]
        public float FadeOutDuration = 0.2f;

        protected Color _alphaOne = new(1f, 1f, 1f, 1f);

        protected Color _alphaZero = new(1f, 1f, 1f, 0f);
        protected Coroutine _hideCoroutine;

        protected Color _tempColor;

        public virtual void Initialization()
        {
            ContainerCanvasGroup.alpha = 0f;
        }

        public virtual void SetText(string newText)
        {
            PromptText.text = newText;
        }

        public virtual void SetBackgroundColor(Color newColor)
        {
            Background.color = newColor;
        }

        public virtual void SetTextColor(Color newColor)
        {
            PromptText.color = newColor;
            ActionText.color = newColor;
            _tempColor = newColor;
        }

        public virtual void Show(string key, string action)
        {
            gameObject.SetActive(true);
            PromptText.text = key;
            ActionText.text = action;
            if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);
            ContainerCanvasGroup.alpha = 0f;
            StartCoroutine(MMFade.FadeCanvasGroup(ContainerCanvasGroup, FadeInDuration, 1f));
        }

        public virtual void Hide()
        {
            if (!gameObject.activeInHierarchy) return;
            _hideCoroutine = StartCoroutine(HideCo());
        }

        protected virtual IEnumerator HideCo()
        {
            ContainerCanvasGroup.alpha = 1f;
            StartCoroutine(MMFade.FadeCanvasGroup(ContainerCanvasGroup, FadeOutDuration, 0f));
            yield return new WaitForSeconds(FadeOutDuration);
            gameObject.SetActive(false);
        }
    }
}