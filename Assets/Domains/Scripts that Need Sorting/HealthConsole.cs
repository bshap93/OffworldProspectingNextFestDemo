using Domains.Player.Scripts;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class HealthConsole : MonoBehaviour
    {
        public int healthPricePerUnit = 10;
        [SerializeField] private MMFeedbacks buyHealthFeedbacks;
        private HealthUIController healthUIController;

        private float playerCurrencyAmount;

        private float playerHealthRemaining;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            playerHealthRemaining = PlayerHealthManager.HealthPoints;
            playerCurrencyAmount = PlayerCurrencyManager.CompanyCredits;
            healthUIController = FindFirstObjectByType<HealthUIController>();
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}