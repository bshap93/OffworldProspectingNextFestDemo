using System.Collections;
using Domains.Items.Inventory;
using Domains.Items.Scripts;
using Domains.Player.Events;
using Domains.Player.Scripts;
using Domains.Scene.Scripts;
using Gameplay.Events;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

namespace Domains.Gameplay.Mining.Scripts

{
    public class OreNode : MonoBehaviour, IMinable
    {
        [SerializeField] private GameObject pieces;
        [SerializeField] private BaseItem itemTypeMined;

        [Header("Mining Settings")] [SerializeField]
        private int dropOnHit;

        [SerializeField] private int hitsToDestroy;
        [SerializeField] private int dropOnDestroy;
        [SerializeField] private Vector3 knockAngle;
        [SerializeField] private AnimationCurve knockCurve;
        [SerializeField] private float knockDuration = 1;

        [Header("Feedbacks")] [SerializeField] private MMFeedbacks oreHitFeedback;


        [SerializeField] private MMFeedbacks oreDestroyFeedback;

        [SerializeField] private UnityEvent OnOreDestroyed;
        [SerializeField] private UnityEvent OnOreHit;

        public int OreHardness; // Hardness of the ore node.


        // Unique ID for the ore node.
        public string UniqueID;

        [SerializeField] private MMFeedbacks failHitFeedbacks;
        [SerializeField] private GameObject failHitParticles;
        [SerializeField] private GameObject oreHitParticles;
        [SerializeField] private GameObject oreDestroyParticles;
        private int dropIndex;
        private int hitIndex;

        private void Start()
        {
            StartCoroutine(InitializeAfterDestructableManager());
        }

        // Sets number of pickups to spawn.
        public void MinableMineHit()
        {
            var currentFuel = PlayerFuelManager.FuelPoints;
            var maxFuel = PlayerFuelManager.MaxFuelPoints;
            hitIndex++;
            if (hitIndex < hitsToDestroy)
                dropIndex = dropOnHit;
            else
                dropIndex = dropOnDestroy;

            // Gets node bounds for pickup spawn location.
            var renderer = GetComponent<Renderer>();
            var worldBounds = renderer.bounds;
            var minX = worldBounds.min.x;
            var maxX = worldBounds.max.x;
            var centerY = worldBounds.center.y;
            var minZ = worldBounds.min.z;
            var maxZ = worldBounds.max.z;

            // var inventoryManager = PlayerInventoryManager.Instance;
            if (PlayerInventoryManager.PlayerInventory != null)
                for (var i = 0; i < dropIndex; i++)
                {
                    var entry = new Inventory.InventoryEntry(UniqueID, itemTypeMined);
                    if (PlayerInventoryManager.AddItem(entry))
                        ItemEvent.Trigger(ItemEventType.Picked, entry, transform);
                    else
                        UnityEngine.Debug.LogWarning("Inventory full! Cannot pick up item.");
                }

            if (hitIndex < hitsToDestroy) //Controls when to shatter.
            {
                if (oreHitParticles != null)
                {
                    var fx = Instantiate(oreHitParticles, transform.position, Quaternion.identity);
                    Destroy(fx, 2f);
                }

                oreHitFeedback?.PlayFeedbacks();
                OnOreHit?.Invoke();


                // Knock animation.
                StartCoroutine(Animate());
            }
            else
            {
                // Spawn pieces and destroy.
                oreDestroyFeedback?.PlayFeedbacks();
                var position = transform.position;
                var rotation = transform.rotation;
                Instantiate(pieces, position, rotation);

                if (oreDestroyParticles != null)
                {
                    var fx = Instantiate(oreDestroyParticles, transform.position, Quaternion.identity);
                    Destroy(fx, 2f);
                }

                DestructableEvent.Trigger(DestructableEventType.Destroyed, UniqueID);
                OnOreDestroyed?.Invoke();
                Destroy(gameObject);
            }
        }

        public void MinableFailHit(Vector3 hitPoint)
        {
            failHitFeedbacks?.PlayFeedbacks();

            if (failHitParticles != null)
            {
                var fx = Instantiate(failHitParticles, hitPoint, Quaternion.identity);
                Destroy(fx, 2f);
            }
        }

        public int GetCurrentMinableHardness()
        {
            return OreHardness;
        }

        public void ShowMineablePrompt()
        { }


        private IEnumerator InitializeAfterDestructableManager()
        {
            // Wait a frame to ensure PickableManager has initialized
            yield return null;

            // Now check if this item should be destroyed
            if (DestructableManager.IsDestuctableDestroyed(UniqueID)) Destroy(gameObject);
        }


        private IEnumerator Animate()
        {
            var startRotation = transform.localRotation;
            var endRotation = startRotation * Quaternion.Euler(knockAngle);

            float t = 0;
            while (t < knockDuration)
            {
                var v = knockCurve.Evaluate(t / knockDuration);
                transform.localRotation = Quaternion.Lerp(startRotation, endRotation, v);
                t += Time.deltaTime;
                yield return null;
            }

            // Optional: restore to exact start if you want
            transform.localRotation = startRotation;
        }
    }
}