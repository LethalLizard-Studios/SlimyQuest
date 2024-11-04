using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotView : MonoBehaviour
{
    [SerializeField] private RawImage blockMask;
    [SerializeField] private GameObject blockOverlay;
    [SerializeField] private RawImage icon;

    [SerializeField] private TextMeshProUGUI quantityText;

    public void SetData(bool hasBlockMask, bool hasBlockOverlay, Texture2D iconTexture, string quantity)
    {
        blockMask.enabled = hasBlockMask;
        blockOverlay.SetActive(hasBlockOverlay);
        icon.texture = iconTexture;
        quantityText.text = quantity;
    }
}
