using HighlightPlus;
using TMPro;
using UnityEngine;

namespace Domains.Items.Scripts
{
    public class ItemInfoDisplay : MonoBehaviour
    {
        [Header("Item Reference")] [SerializeField]
        private BaseItem itemData;

        [Header("Info Display Settings")] [SerializeField]
        private TMP_FontAsset font;

        [SerializeField] private float iconScale = 1f;
        [SerializeField] private Vector3 iconOffset = new(0, 1, 0);
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private Color backgroundColor = new(0, 0, 0, 0.7f);
        [SerializeField] private float hoverTimeBeforeInfo = 0.3f; // Time before info appears

        private HighlightEffect highlightEffect;
        private Mesh infoMesh;
        private float lookTimer;
        private bool playerIsLooking;
        private GameObject tmpTextObject;

        private void Awake()
        {
            // Get reference to the existing HighlightEffect component
            highlightEffect = GetComponent<HighlightEffect>();
            if (highlightEffect == null)
            {
                UnityEngine.Debug.LogWarning("HighlightEffect component not found on " + gameObject.name);
                return;
            }

            // Configure icon settings
            ConfigureIconFX();

            // Start with icon disabled
            highlightEffect.iconFX = false;
        }

        private void Update()
        {
            if (highlightEffect == null) return;

            if (playerIsLooking)
            {
                // Show info after delay
                lookTimer += Time.deltaTime;
                if (lookTimer >= hoverTimeBeforeInfo && !highlightEffect.iconFX)
                {
                    if (infoMesh == null) GenerateInfoMesh();
                    highlightEffect.iconFX = true;
                }
            }
            else
            {
                // Hide info when not looking
                lookTimer = 0f;
                highlightEffect.iconFX = false;
            }
        }

        private void OnEnable()
        {
            // Make sure everything resets when object is enabled
            playerIsLooking = false;
            lookTimer = 0f;
            highlightEffect.iconFX = false;
        }

        private void ConfigureIconFX()
        {
            highlightEffect.iconFXScale = iconScale;
            highlightEffect.iconFXOffset = iconOffset;
            highlightEffect.iconFXLightColor = textColor;
            highlightEffect.iconFXDarkColor = backgroundColor;
            highlightEffect.iconFXAnimationOption = IconAnimationOption.VerticalBounce;
            highlightEffect.iconFXAnimationSpeed = 2f;
            highlightEffect.iconFXAnimationAmount = 0.05f;
            highlightEffect.iconFXStayDuration = 0f; // Stay as long as player is looking
        }

        private void GenerateInfoMesh()
        {
            // Create temporary GameObject with TextMeshPro to generate our info mesh
            if (tmpTextObject != null) Destroy(tmpTextObject);

            // Format item information
            var itemInfo = FormatItemInfo();

            // Create text object
            tmpTextObject = new GameObject("InfoMeshGenerator");
            tmpTextObject.transform.SetParent(transform);
            tmpTextObject.transform.localPosition = Vector3.zero;

            var textMesh = tmpTextObject.AddComponent<TextMeshPro>();
            textMesh.font = font;
            textMesh.fontSize = 3f;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.text = itemInfo;
            textMesh.color = textColor;
            textMesh.rectTransform.sizeDelta = new Vector2(1f, 0.5f);

            // Add a background quad
            var background = new GameObject("Background");
            background.transform.SetParent(tmpTextObject.transform);
            background.transform.localPosition = new Vector3(0, 0, 0.01f);
            background.transform.localScale = new Vector3(1.1f, 1.1f, 1);

            var bgRenderer = background.AddComponent<MeshRenderer>();
            var bgFilter = background.AddComponent<MeshFilter>();
            bgFilter.mesh = CreateQuadMesh();

            var bgMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            bgMat.color = backgroundColor;
            bgRenderer.material = bgMat;

            // Force update and get the mesh
            textMesh.ForceMeshUpdate();

            // Create a combined mesh from text and background
            var combine = new CombineInstance[2];

            // Add background mesh
            combine[0].mesh = bgFilter.sharedMesh;
            combine[0].transform = background.transform.localToWorldMatrix;

            // Add text mesh
            combine[1].mesh = textMesh.mesh;
            combine[1].transform = textMesh.transform.localToWorldMatrix;

            // Create the final mesh
            infoMesh = new Mesh();
            infoMesh.CombineMeshes(combine, true, false);

            // Assign to highlight effect
            highlightEffect.iconFXMesh = infoMesh;

            // Hide the temporary object
            tmpTextObject.SetActive(false);
        }

        private string FormatItemInfo()
        {
            if (BaseItem.IsNull(itemData)) return "Unknown Item";

            var itemType = itemData.ItemType.ToString();
            var valueText = itemData.ItemValue > 0 ? $"Value: {itemData.ItemValue}" : "";
            var weightText = itemData.ItemWeight > 0 ? $"Weight: {itemData.ItemWeight:0.0}" : "";

            return $"{itemData.ItemName}\n<size=70%>{itemType}</size>\n<size=60%>{valueText} {weightText}</size>";
        }

        private Mesh CreateQuadMesh()
        {
            var mesh = new Mesh();

            // Vertices - flat quad
            var vertices = new Vector3[4]
            {
                new(-0.5f, -0.5f, 0),
                new(0.5f, -0.5f, 0),
                new(-0.5f, 0.5f, 0),
                new(0.5f, 0.5f, 0)
            };

            // UV coordinates
            var uv = new Vector2[4]
            {
                new(0, 0),
                new(1, 0),
                new(0, 1),
                new(1, 1)
            };

            // Triangles
            var triangles = new int[6]
            {
                0, 2, 1, // first triangle
                2, 3, 1 // second triangle
            };

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            return mesh;
        }

        // Public method to be called from your existing interaction system
        // when player looks at this item
        public void OnPlayerLook(bool isLooking)
        {
            playerIsLooking = isLooking;

            // If player stops looking, hide icon immediately
            if (!isLooking && highlightEffect != null) highlightEffect.iconFX = false;
        }

        // Method to set the item data at runtime (useful if you're reusing objects)
        public void SetItemData(BaseItem newItemData)
        {
            itemData = newItemData;
            infoMesh = null; // Clear the cached mesh so it regenerates with new data
        }
    }
}