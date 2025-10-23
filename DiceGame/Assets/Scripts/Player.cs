using System;
using System.Collections.Generic;

namespace Dungeon
{
    /// <summary>
    /// Player that inherits core combat stats/behaviour from Combatant.
    /// Keeps session stats, dice inventory, and a simple item system.
    /// </summary>
    public class Player : Combatant
    {
        public bool IsComputer { get; protected set; }
        public int Score { get; protected set; }
        public int RollSum { get; protected set; }
        public int RollCount { get; protected set; }
        public int EvenCount { get; protected set; }
        public int OddCount { get; protected set; }

        // Dice inventory
        public List<int> InventorySides { get; protected set; } = new List<int>();

        // Item Inventory
        public List<Weapon> Weapons { get; protected set; } = new List<Weapon>();
        public List<ArmourItem> Armours { get; protected set; } = new List<ArmourItem>();
        public List<Consumable> Consumables { get; protected set; } = new List<Consumable>();

        // Equipped items
        public Weapon? EquippedWeapon { get; protected set; }
        public ArmourItem? EquippedArmour { get; protected set; }

        // Keep base stats so we can recalc after equip/unequip
        private int _baseDamageMin;
        private int _baseDamageMax;
        private int _baseArmour;

        public Player(
            string name,
            bool isComputer,
            int level = 1,
            int maxHp = 120,
            int armour = 2,
            int damageMin = 15,
            int damageMax = 20,
            int critChance = 15,
            int critMultiplier = 150,
            int[]? diceSides = null,
            Random? rng = null
        )
        : base(
            name: string.IsNullOrWhiteSpace(name) ? "Player" : name.Trim(),
            level: level,
            maxHp: maxHp,
            hP: maxHp,
            armour: armour,
            damageMin: damageMin,
            damageMax: damageMax,
            critChance: critChance,
            critMultiplier: critMultiplier,
            diceSides: diceSides ?? new int[] { 6, 6, 6, 6, 6 },
            rng: rng ?? new Random()
        )
        {
            IsComputer = isComputer;
            _baseDamageMin = DamageMin;
            _baseDamageMax = DamageMax;
            _baseArmour = Armour;
            ResetStats();

            if (diceSides != null && diceSides.Length > 0)
            {
                InventorySides.AddRange(diceSides);
            }
            else
            {
                InventorySides.AddRange(new int[] { 4, 6, 6, 8, 12 });
            }
        }

        // Session tracker
        public void ResetStats()
        {
            Score = 0;
            RollSum = 0;
            RollCount = 0;
            EvenCount = 0;
            OddCount = 0;
        }

        public void AddScore(int amount)
        {
            if (amount <= 0) return;
            Score += amount;
        }

        public void RecordRoll(int value)
        {
            if (value <= 0) return;

            RollSum += value;
            RollCount++;

            if ((value % 2) == 0) EvenCount += 1;
            else OddCount += 1;
        }

        // Dice inventory helpers
        public void AddDieToInventory(int sides)
        {
            if (sides < 2) sides = 2;
            InventorySides.Add(sides);
        }

        public bool RemoveDieFromInventoryAt(int index)
        {
            if (index < 0 || index >= InventorySides.Count) return false;
            InventorySides.RemoveAt(index);
            return true;
        }

        public (int a, int b, int c, int d, int e) ChooseFiveDice(List<int> availableSides)
        {
            if (availableSides == null || availableSides.Count == 0)
                availableSides = new List<int> { 4, 6, 8, 10, 12, 20, 24 };

            if (!IsComputer)
            {
                while (true)
                {
                    Console.WriteLine(Name + ", choose FIVE dice (duplicates allowed) from: " + string.Join(", ", availableSides));
                    Console.Write("Enter like: 6 6 6 6 6 or d6 d8 d12 d10 d24: ");

                    string line = (Console.ReadLine() ?? "").Trim().ToLower();
                    string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length != 5)
                    {
                        Console.WriteLine("Please enter exactly five choices.\n");
                        continue;
                    }

                    int a = ParseSides(parts[0]);
                    int b = ParseSides(parts[1]);
                    int c = ParseSides(parts[2]);
                    int d = ParseSides(parts[3]);
                    int e = ParseSides(parts[4]);

                    if (!availableSides.Contains(a) ||
                        !availableSides.Contains(b) ||
                        !availableSides.Contains(c) ||
                        !availableSides.Contains(d) ||
                        !availableSides.Contains(e))
                    {
                        Console.WriteLine("One or more choices are not in the allowed list. Try again.\n");
                        continue;
                    }

                    return (a, b, c, d, e);
                }
            }
            else
            {
                int a = availableSides[Rng.Next(availableSides.Count)];
                int b = availableSides[Rng.Next(availableSides.Count)];
                int c = availableSides[Rng.Next(availableSides.Count)];
                int d = availableSides[Rng.Next(availableSides.Count)];
                int e = availableSides[Rng.Next(availableSides.Count)];
                return (a, b, c, d, e);
            }
        }

        private int ParseSides(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return 6;
            token = token.Trim().ToLower();
            if (token.StartsWith("d") && token.Length > 1) token = token.Substring(1);

            int v;
            if (!int.TryParse(token, out v)) v = 6;
            if (v < 2) v = 2;
            return v;
        }

        // Item inventory helpers

        // Add items
        public void AddWeapon(Weapon w)
        {
            if (w == null) return;
            Weapons.Add(w);
        }

        public void AddArmour(ArmourItem a)
        {
            if (a == null) return;
            Armours.Add(a);
        }

        public void AddConsumable(Consumable c)
        {
            if (c == null) return;
            Consumables.Add(c);
        }

        // Equip by index
        public bool EquipWeaponByIndex(int index)
        {
            if (index < 0 || index >= Weapons.Count) return false;
            EquippedWeapon = Weapons[index];
            RecalculateStatsFromEquipment();
            Console.WriteLine(Name + " equips " + EquippedWeapon.Name + ".");
            return true;
        }

        public bool EquipArmourByIndex(int index)
        {
            if (index < 0 || index >= Armours.Count) return false;
            EquippedArmour = Armours[index];
            RecalculateStatsFromEquipment();
            Console.WriteLine(Name + " equips " + EquippedArmour.Name + ".");
            return true;
        }

        public void UnequipWeapon()
        {
            EquippedWeapon = null;
            RecalculateStatsFromEquipment();
        }

        public void UnequipArmour()
        {
            EquippedArmour = null;
            RecalculateStatsFromEquipment();
        }

        // Use consumable by index
        public bool UseConsumableByIndex(int index)
        {
            if (index < 0 || index >= Consumables.Count) return false;

            Consumable c = Consumables[index];
            if (c.HealAmount > 0 && HP > 0)
            {
                int before = HP;
                Heal(c.HealAmount);
                int healed = HP - before;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Name + " uses " + c.Name + " and heals " + healed + " HP!");
                Console.ResetColor();
            }
            // You can add other effects later (buffs, etc.)

            Consumables.RemoveAt(index);
            return true;
        }

        // Recalculate combat stats from base + equipment
        private void RecalculateStatsFromEquipment()
        {
            // reset to base
            DamageMin = _baseDamageMin;
            DamageMax = _baseDamageMax;
            Armour = _baseArmour;

            // apply weapon
            if (EquippedWeapon != null)
            {
                DamageMin += EquippedWeapon.MinBonus;
                DamageMax += EquippedWeapon.MaxBonus;
                if (DamageMax < DamageMin) DamageMax = DamageMin;
            }

            // apply armour
            if (EquippedArmour != null)
            {
                Armour += EquippedArmour.ArmourBonus;
                if (Armour < 0) Armour = 0;
            }
        }
    }
}
