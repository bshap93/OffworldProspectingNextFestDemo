using MoreMountains.Tools;

namespace Domains.Player.Events
{
    public enum CurrencyEventType
    {
        AddCurrency,
        RemoveCurrency,
        SetCurrency
    }

    public struct CurrencyEvent
    {
        public CurrencyEventType EventType;
        public float Amount;

        public static void Trigger(CurrencyEventType eventType, float amount)
        {
            var currencyEvent = new CurrencyEvent
            {
                EventType = eventType,
                Amount = amount
            };

            MMEventManager.TriggerEvent(currencyEvent);
        }
    }
}