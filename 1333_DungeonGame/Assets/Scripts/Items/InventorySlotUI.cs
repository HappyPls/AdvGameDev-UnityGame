using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class InventorySlotUI : MonoBehaviour
    {
        [Header("UI References")]
        public Image IconImage;
        public Text QuantityText;
        public Image RarityBorder;

        [Header("Rarity Colors")]
        public Color CommonColor = Color.white;
        public Color UncommonColor = Color.green;
        public Color RareColor = Color.blue;
        public Color EpicColor = new Color(0.6f, 0f, 0.8f);
        public Color LegendaryColor = new Color(1f, 0.6f, 0f);

        private int _slotIndex;
        private InventoryUI _inventoryUI;

        public void Init(InventoryUI inventoryUI, int slotIndex)
        {
            _inventoryUI = inventoryUI;
            _slotIndex = slotIndex;
        }

        public void Refresh(InventorySlot slot)
        {
            if (slot == null || slot.IsEmpty)
            {
                if (IconImage != null)
                {
                    IconImage.enabled = false;
                    IconImage.sprite = null;
                }

                if (QuantityText != null)
                {
                    QuantityText.text = "";
                }

                if (RarityBorder != null)
                {
                    RarityBorder.enabled = false;
                }

                return;
            }

            if (IconImage != null)
            {
                IconImage.enabled = true;
                IconImage.sprite = slot.Item != null ? slot.Item.Icon : null;
            }

            if (QuantityText != null)
            {
                if (slot.Item != null && slot.Item.IsStackable && slot.Quantity > 1)
                {
                    QuantityText.text = slot.Quantity.ToString();
                }
                else
                {
                    QuantityText.text = "";
                }
            }

            if (RarityBorder != null)
            {
                RarityBorder.enabled = true;
                RarityBorder.color = GetColorForRarity(slot.Rarity);
            }
        }

        public void OnClick()
        {
            if (_inventoryUI != null)
            {
                _inventoryUI.OnSlotClicked(_slotIndex);
            }
        }

        private Color GetColorForRarity(ItemRarity rarity)
        {
            if (rarity == ItemRarity.Uncommon) return UncommonColor;
            if (rarity == ItemRarity.Rare) return RareColor;
            if (rarity == ItemRarity.Epic) return EpicColor;
            if (rarity == ItemRarity.Legendary) return LegendaryColor;

            return CommonColor;
        }
    }
}
