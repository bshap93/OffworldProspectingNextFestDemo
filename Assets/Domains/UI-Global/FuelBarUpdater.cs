using Domains.Player.Events;
using Domains.Player.Scripts;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Domains.UI_Global
{
    public class FuelBarUpdater : MonoBehaviour, MMEventListener<FuelEvent>

    {
        [SerializeField] private Slider fuelBarSlider;
        [SerializeField] private TMP_Text fuelPercentageText;


        private float _currentFuel;

        private float _maxFuel;


        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(FuelEvent eventType)
        {
            _currentFuel = PlayerFuelManager.FuelPoints;
            _maxFuel = PlayerFuelManager.MaxFuelPoints;

            UpdateBar();
        }

        private void UpdateBar()
        {
            fuelBarSlider.value = _currentFuel / _maxFuel;
            fuelPercentageText.text = $"{_currentFuel / _maxFuel * 100:0}%";
        }


        public void Initialize()
        {
            _maxFuel = PlayerFuelManager.MaxFuelPoints;
            _currentFuel = PlayerFuelManager.FuelPoints;
            UpdateBar();
        }
    }
}