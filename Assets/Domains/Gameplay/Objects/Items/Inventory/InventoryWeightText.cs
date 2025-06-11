using System;
using System.Globalization;
using Domains.Items.Events;
using Domains.Scene.Scripts;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Domains.Items.Inventory
{
    public class InventoryWeightText : MonoBehaviour, MMEventListener<InventoryEvent>
    {
        [SerializeField] private Image weightIcon;
        [SerializeField] private Color notFullColor;
        [SerializeField] private Color fullColor;
        private TMP_Text _text;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _text = GetComponent<TMP_Text>();
            UpdateWeightText();
        }


        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(InventoryEvent eventType)
        {
            if (_text == null)
                return;

            if (eventType.EventType == InventoryEventType.ContentChanged)
                UpdateWeightText();
            else if (eventType.EventType == InventoryEventType.UpgradedWeightLimit)
                try
                {
                    var maxWeight = PlayerInventoryManager.GetMaxWeight();
                    var currentWeight = PlayerInventoryManager.GetCurrentWeight();
                    _text.text =
                        $"{currentWeight.ToString(CultureInfo.InvariantCulture)} / {maxWeight.ToString(CultureInfo.InvariantCulture)}";
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Error updating inventory weight text: {ex.Message}");
                }
        }


        private void UpdateWeightText()
        {
            if (_text == null)
                return;

            try
            {
                var currentWeight = PlayerInventoryManager.GetCurrentWeight();
                var maxWeight = PlayerInventoryManager.GetMaxWeight();
                _text.text =
                    $"{currentWeight.ToString(CultureInfo.InvariantCulture)} / {maxWeight.ToString(CultureInfo.InvariantCulture)}";
                
                if (currentWeight >= maxWeight)
                {
                    weightIcon.color = fullColor;
                    _text.color = fullColor;
                }
                else
                {
                    weightIcon.color = notFullColor;
                    _text.color = notFullColor;
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error updating inventory weight text: {ex.Message}");
            }
        }
    }
}