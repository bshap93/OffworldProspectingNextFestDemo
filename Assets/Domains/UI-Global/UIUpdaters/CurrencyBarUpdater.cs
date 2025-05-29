using Domains.Player.Events;
using Domains.Player.Scripts;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace Domains.UI_Global.UIUpdaters
{
    public class CurrencyBarUpdater : MonoBehaviour, MMEventListener<CurrencyEvent>
    {
        public bool useTextPlaceholder = true;
        public TMP_Text textPlaceholderCurrency;
        public string currencySymbol = "$"; // Currency symbol, could be $, €, ¥, etc.

        private MMProgressBar _bar;
        private float _currentCurrency;

        private void Awake()
        {
            if (!useTextPlaceholder) _bar = GetComponent<MMProgressBar>();
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(CurrencyEvent eventType)
        {
            if (useTextPlaceholder)
                switch (eventType.EventType)
                {
                    case CurrencyEventType.AddCurrency:
                        _currentCurrency += eventType.Amount;
                        UpdateCurrencyText();
                        break;
                    case CurrencyEventType.RemoveCurrency:
                        _currentCurrency = Mathf.Max(0, _currentCurrency - eventType.Amount);
                        UpdateCurrencyText();
                        break;
                    case CurrencyEventType.SetCurrency:
                        _currentCurrency = eventType.Amount;
                        UpdateCurrencyText();
                        break;
                }
            else
                switch (eventType.EventType)
                {
                    case CurrencyEventType.AddCurrency:
                        _currentCurrency += eventType.Amount;
                        _bar.UpdateBar(_currentCurrency, 0, _currentCurrency * 2); // Dynamic max for visual effect
                        break;
                    case CurrencyEventType.RemoveCurrency:
                        _currentCurrency = Mathf.Max(0, _currentCurrency - eventType.Amount);
                        _bar.UpdateBar(_currentCurrency, 0, _currentCurrency * 2); // Dynamic max for visual effect
                        break;
                    case CurrencyEventType.SetCurrency:
                        _currentCurrency = eventType.Amount;
                        _bar.UpdateBar(_currentCurrency, 0, _currentCurrency * 2); // Dynamic max for visual effect
                        break;
                }
        }

        private void UpdateCurrencyText()
        {
            if (textPlaceholderCurrency != null) textPlaceholderCurrency.text = $"{_currentCurrency} {currencySymbol}";
        }

        public void Initialize()
        {
            _currentCurrency = PlayerCurrencyManager.CompanyCredits;

            if (useTextPlaceholder)
                UpdateCurrencyText();
            else
                _bar.UpdateBar(_currentCurrency, 0, _currentCurrency * 2); // Dynamic max for visual effect
        }
    }
}