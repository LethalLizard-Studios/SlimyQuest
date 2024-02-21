/* Rights Reserved to Leland TL Carter of LethalLizard Studios ©2024
-- Last Change: 10/30/2023
*/

using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Hotbar))]
public class TextPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private RectTransform background;

    private Hotbar hotbar;

    private void Start()
    {
        hotbar = GetComponent<Hotbar>();
    }

    public void Display()
    {
        if (hotbar.GetItemInSlot(hotbar.CurrentSlotAdjusted()) <= 0)
        {
            background.gameObject.SetActive(false);
            return;
        }
        StopAllCoroutines();
        StartCoroutine(DisplayPopup());
    }

    private IEnumerator DisplayPopup()
    {
        background.gameObject.SetActive(true);

        Block block = Registry.AtIndex(hotbar.GetItemInSlot(hotbar.CurrentSlotAdjusted()) - 1);
        nameText.text = block.m_properties.m_hasOtherDrop ? block.m_properties.m_otherdrop.m_name : block.m_name;

        int energy = block.m_properties.m_energyValue;

        if (energy > 0)
        {
            energyText.text = $"{energy:N0} EMC";
            energyText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            energyText.transform.parent.gameObject.SetActive(false);
        }

        background.sizeDelta = new Vector2((nameText.text.Length * 16) + 16, 26);
        yield return new WaitForSeconds(3);
        background.gameObject.SetActive(false);
    }
}
