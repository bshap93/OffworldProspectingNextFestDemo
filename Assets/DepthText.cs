using System.Collections;
using Domains.Player.Scripts;
using TMPro;
using UnityEngine;

public class DepthText : MonoBehaviour
{
    [SerializeField] private TMP_Text depthText;
    private PlayerInteraction _playerInteraction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _playerInteraction = FindFirstObjectByType<PlayerInteraction>();

        StartCoroutine(UpdateDepthText());
    }


    private IEnumerator UpdateDepthText()
    {
        while (true)
        {
            // Every second, update the text with the current depth
            var currentDepth = _playerInteraction.currentDigDepth;
            // Round to 2 significant figures
            currentDepth = Mathf.Round(currentDepth * 100f) / 100f; // Round to 2 decimal places
            depthText.text = $"{currentDepth}";

            yield return new WaitForSeconds(1f); // Update every second
        }
    }
}