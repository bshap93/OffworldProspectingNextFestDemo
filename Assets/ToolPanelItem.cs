using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ToolPanelItem : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation scaleAnimation;

    [FormerlySerializedAs("SPR_Ring")] [SerializeField]
    private Image sprRing;

    [SerializeField] private Color unSelectedColor;
    [SerializeField] private Color selectedColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (scaleAnimation == null) scaleAnimation = GetComponent<DOTweenAnimation>();
    }


    public void Select()
    {
        sprRing.color = selectedColor;

        transform.DOScale(1.3f, 0.4f);
    }

    public void Deselect()
    {
        sprRing.color = unSelectedColor;
        transform.DOScale(1f, 0.4f);
    }
}