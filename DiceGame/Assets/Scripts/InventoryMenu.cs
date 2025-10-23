using System;

namespace Dungeon
{
    public class InventoryMenu
    {
        // Opens the inventory menu
        public void Open(Player player)
        {
            string choice = "";

            while (choice != "0")
            {
                Console.WriteLine();
                Console.WriteLine("=== INVENTORY MENU ===");
                Console.WriteLine("1) Show Equipped Items");
                Console.WriteLine("2) Show Weapons");
                Console.WriteLine("3) Show Armour");
                Console.WriteLine("4) Show Consumables");
                Console.WriteLine("5) Equip Weapon");
                Console.WriteLine("6) Equip Armour");
                Console.WriteLine("7) Use Consumable");
                Console.WriteLine("8) Show Dice");
                Console.WriteLine("0) Exit");
                Console.Write("Enter a choice: ");
                choice = Console.ReadLine() ?? "";

                if (choice == "1")
                {
                    ShowEquipped(player);
                }
                else if (choice == "2")
                {
                    ShowWeapons(player);
                }
                else if (choice == "3")
                {
                    ShowArmour(player);
                }
                else if (choice == "4")
                {
                    ShowConsumables(player);
                }
                else if (choice == "5")
                {
                    EquipWeapon(player);
                }
                else if (choice == "6")
                {
                    EquipArmour(player);
                }
                else if (choice == "7")
                {
                    UseConsumable(player);
                }
                else if (choice == "8")
                {
                    ShowDice(player);
                }
                else if (choice == "0")
                {
                    Console.WriteLine("Exiting inventory...");
                }
                else
                {
                    Console.WriteLine("Invalid option. Try again.");
                }
            }
        }

        // Shows currently equipped gear and basic stats
        private void ShowEquipped(Player p)
        {
            Console.WriteLine();
            Console.WriteLine("Equipped Items:");

            if (p.EquippedWeapon != null)
                Console.WriteLine("Weapon: " + p.EquippedWeapon.Name + " (+" + p.EquippedWeapon.MinBonus + "-" + p.EquippedWeapon.MaxBonus + " dmg)");
            else
                Console.WriteLine("Weapon: none");

            if (p.EquippedArmour != null)
                Console.WriteLine("Armour: " + p.EquippedArmour.Name + " (+" + p.EquippedArmour.ArmourBonus + " armour)");
            else
                Console.WriteLine("Armour: none");

            Console.WriteLine("Stats:");
            Console.WriteLine("  Damage: " + p.DamageMin + " - " + p.DamageMax);
            Console.WriteLine("  Armour: " + p.Armour);
            Console.WriteLine("  HP: " + p.HP + "/" + p.MaxHp);
        }

        // Lists all weapons
        private void ShowWeapons(Player p)
        {
            Console.WriteLine();
            Console.WriteLine("Weapons:");
            if (p.Weapons.Count == 0)
            {
                Console.WriteLine("No weapons in inventory.");
                return;
            }

            for (int i = 0; i < p.Weapons.Count; i++)
            {
                Weapon w = p.Weapons[i];
                Console.WriteLine(i + ") " + w.Name + " (+" + w.MinBonus + "-" + w.MaxBonus + " dmg)");
            }
        }

        // Lists all armour
        private void ShowArmour(Player p)
        {
            Console.WriteLine();
            Console.WriteLine("Armour:");
            if (p.Armours.Count == 0)
            {
                Console.WriteLine("No armour in inventory.");
                return;
            }

            for (int i = 0; i < p.Armours.Count; i++)
            {
                ArmourItem a = p.Armours[i];
                Console.WriteLine(i + ") " + a.Name + " (+" + a.ArmourBonus + " armour)");
            }
        }

        // Lists all consumables
        private void ShowConsumables(Player p)
        {
            Console.WriteLine();
            Console.WriteLine("Consumables:");
            if (p.Consumables.Count == 0)
            {
                Console.WriteLine("No consumables in inventory.");
                return;
            }

            for (int i = 0; i < p.Consumables.Count; i++)
            {
                Consumable c = p.Consumables[i];
                Console.WriteLine(i + ") " + c.Name + " (heals " + c.HealAmount + ")");
            }
        }

        // Equip a weapon by index no.
        private void EquipWeapon(Player p)
        {
            ShowWeapons(p);
            if (p.Weapons.Count == 0) return;

            Console.Write("Enter weapon number to equip: ");
            int index;
            if (int.TryParse(Console.ReadLine(), out index))
            {
                if (!p.EquipWeaponByIndex(index))
                    Console.WriteLine("Invalid weapon number.");
                else
                    Console.WriteLine("Weapon equipped!");
            }
            else
            {
                Console.WriteLine("Please enter a valid number.");
            }
        }

        // Equip armour by index no.
        private void EquipArmour(Player p)
        {
            ShowArmour(p);
            if (p.Armours.Count == 0) return;

            Console.Write("Enter armour number to equip: ");
            int index;
            if (int.TryParse(Console.ReadLine(), out index))
            {
                if (!p.EquipArmourByIndex(index))
                    Console.WriteLine("Invalid armour number.");
                else
                    Console.WriteLine("Armour equipped!");
            }
            else
            {
                Console.WriteLine("Please enter a valid number.");
            }
        }

        // Use consumable by index no.
        private void UseConsumable(Player p)
        {
            ShowConsumables(p);
            if (p.Consumables.Count == 0) return;

            Console.Write("Enter consumable number to use: ");
            int index;
            if (int.TryParse(Console.ReadLine(), out index))
            {
                if (!p.UseConsumableByIndex(index))
                    Console.WriteLine("Invalid consumable number.");
                else
                    Console.WriteLine("You used the item.");
            }
            else
            {
                Console.WriteLine("Please enter a valid number.");
            }
        }

        // Shows dice the player has collected
        private void ShowDice(Player p)
        {
            Console.WriteLine();
            Console.WriteLine("Dice Inventory:");
            if (p.InventorySides.Count == 0)
            {
                Console.WriteLine("No dice collected.");
                return;
            }

            for (int i = 0; i < p.InventorySides.Count; i++)
            {
                Console.WriteLine("d" + p.InventorySides[i]);
            }
        }
    }
}
