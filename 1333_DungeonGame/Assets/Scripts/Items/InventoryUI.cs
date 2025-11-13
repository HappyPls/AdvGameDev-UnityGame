using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Dungeon
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI Reference")]
        public GameObject InventoryPanel;
        public Transform SlotsParent;
        public InventorySlotUI SlotPrefab;

        [Header("Gameplay References")]
        public Player Player;
        public Inventory Inventory;

        [Header("Behaviour")]
        public bool PauseOnOpen = true;

        private bool _isOpen = false;
        private List<InventorySlotUI> _slotUIs = new List<InventorySlotUI>();

        void Awake()
        {
            if (Player == null)
            {
                Player = FindFirstObjectByType<Player>();
            }

            if (Inventory == null && Player != null)
            {
                Inventory = Player.Inventory;
            }

            if (InventoryPanel != null)
            {
                InventoryPanel.SetActive(false);
            }

            BuildSlots();
        }

        void Update()
        {
            // New Input System polling
            Keyboard kb = Keyboard.current;
            if (kb != null && kb.bKey != null && kb.bKey.wasPressedThisFrame)
            {
                ToggleInventory();
            }

            if (_isOpen)
            {
                RefreshAllSlots();
            }
        }

        private void BuildSlots()
        {
            if (Inventory == null || SlotsParent == null || SlotPrefab == null)
            {
                Debug.LogWarning("InventoryUI: Missing references for building slots.");
                return;
            }

            int childCount = SlotsParent.childCount;
            int i = childCount - 1;
            while (i >= 0)
            {
                Destroy(SlotsParent.GetChild(i).gameObject);
                i -= 1;
            }

            _slotUIs.Clear();

            int capacity = Inventory.Capacity;
            int index = 0;
            while (index < capacity)
            {
                InventorySlotUI ui = Instantiate(SlotPrefab, SlotsParent);
                ui.Init(this, index);

                Button btn = ui.GetComponent<Button>();
                if (btn != null)
                {
                    int capturedIndex = index;
                    btn.onClick.AddListener(delegate { OnSlotClicked(capturedIndex); });
                }

                _slotUIs.Add(ui);
                index += 1;
            }
        }

        private void RefreshAllSlots()
        {
            if (Inventory == null) return;

            int count = _slotUIs.Count;
            int i = 0;
            while (i < count)
            {
                InventorySlotUI ui = _slotUIs[i];

                if (i < Inventory.Slots.Count)
                {
                    InventorySlot slot = Inventory.Slots[i];
                    ui.Refresh(slot);
                }
                else
                {
                    ui.Refresh(null);
                }

                i += 1;
            }
        }

        public void OnSlotClicked(int index)
        {
            if (Inventory == null || Player == null) return;
            if (index < 0 || index >= Inventory.Slots.Count) return;

            InventorySlot slot = Inventory.Slots[index];
            if (slot == null || slot.IsEmpty)
            {
                Debug.Log("InventoryUI: Empty slot clicked at index " + index);
                return;
            }

            ItemBase item = slot.Item;
            if (item == null)
            {
                Debug.Log("InventoryUI: Slot item is null at index " + index);
                return;
            }

            HealingItem healing = item as HealingItem;
            WeaponItem weapon = item as WeaponItem;
            ArmorItem armor = item as ArmorItem;

            if (healing != null)
            {
                healing.Use(Player);
                Inventory.RemoveItem(item, 1);
                RefreshAllSlots();
                return;
            }

            if (weapon != null)
            {
                Player.EquipWeapon(weapon, slot.Rarity);
                RefreshAllSlots();
                return;
            }

            if (armor != null)
            {
                Player.EquipArmor(armor, slot.Rarity);
                RefreshAllSlots();
                return;
            }

            item.Use(Player);
            Debug.Log("Used item: " + item.DisplayName + " from slot " + index);
        }

        private void ToggleInventory()
        {
            _isOpen = !_isOpen;

            if (InventoryPanel != null)
            {
                InventoryPanel.SetActive(_isOpen);
            }

            if (PauseOnOpen)
            {
                Time.timeScale = _isOpen ? 0f : 1f;
            }

            if (_isOpen)
            {
                RefreshAllSlots();
            }

            Debug.Log("Inventory " + (_isOpen ? "opened" : "closed"));
        }
    }
}
