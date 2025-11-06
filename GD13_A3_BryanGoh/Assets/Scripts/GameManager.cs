#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

namespace Dungeon
{
    public class GameManager : MonoBehaviour
    {
        public System.Random Rng = new System.Random();

        [Header("Scene References")]
        [SerializeField] private DungeonVisualizer visualizer;
        [SerializeField] private GameObject playerPrefab;
        private Transform playerMarker;

        [Header("Grid Settings")]
        [SerializeField] private GridSettings _gridSettings = null;

        private Map _map = null!;
        private Player _player = null!;

        public bool HasWon { get; private set; }
        public bool InCombat { get; private set; }
        private bool _encounterCheckedThisCell = false;
        private int _lastRow = int.MinValue;
        private int _lastCol = int.MinValue;

        void Start()
        {
            _map = new Map(_gridSettings.Rows, _gridSettings.Cols);
            _player = new Player(name: "Hero", isComputer: false);

            if (playerMarker == null)
            {
                if (playerPrefab != null)
                {
                    var go = Instantiate(playerPrefab);
                    go.name = "PlayerMarker";
                    playerMarker = go.transform;
                }
                else
                {
                    // create a sphere if no prefab provided
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.name = "PlayerMarker";
                    playerMarker = go.transform;
                }
            }

            // Init visualizer
            if (visualizer != null)
                visualizer.Init(_map, playerMarker);

            var startRoom = _map.CurrentRoom();
            startRoom?.OnRoomEntered(this, _player);

            Debug.Log("Welcome to Dicey Dungeon");
            PrintGameIntro();
            PrintPokerRules();
        }

        public void StartEncounter()
        {
            if (InCombat) return;
            InCombat = true;

            int prevRow = _map.Row;
            int prevCol = _map.Col;

            Room room = _map.CurrentRoom();
            Enemy enemy = EnemySpawner.SpawnForRoom(room, Rng);

            CombatMenu cm = new CombatMenu();
            cm.StartCombat(_player, enemy, _map, prevRow, prevCol);

            if (enemy.HP <= 0)
            {
                EncounterRoom enc = _map.CurrentRoom() as EncounterRoom;
                if (enc != null) enc.Cleared = true;

                var boss = _map.CurrentRoom() as BossEncounterRoom;
                if (boss != null) boss.Cleared = true;

                if (enemy is IronGolem)
                {
                    HasWon = true;
                    Debug.Log("You have slain the Iron Golem! You defeated the Iron Golem and escape the dungeon!");
                    InCombat = false;
                    return;
                }

                Debug.Log("You search the foe's remains.");
                GrantRandomLoot(_player, 1, 2, LootBias.ConsumablesLean);
            }

            InCombat = false;
        }

        private void Update()
        {
            if (InCombat) return;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) MoveCommand("north");
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) MoveCommand("south");
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MoveCommand("east");
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) MoveCommand("west");

            if (Input.GetKeyDown(KeyCode.E))
            {
                var room = _map.CurrentRoom();
                room?.OnRoomSearched(this, _player);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                FleeToPrevious();
            }
        }

        private void MoveCommand(string direction)
        {
            var oldRoom = _map.CurrentRoom();
            oldRoom?.OnRoomExit(this, _player);

            if(_map.TryMove(direction))
            {
                var newRoom = _map.CurrentRoom();
                newRoom?.OnRoomEntered(this, _player);
                
                visualizer.UpdatePlayerMarker(_map, playerMarker);
                _encounterCheckedThisCell = false;
                MaybeStartEncounter();
            }
            else
            {
                Debug.Log("You can't move that way.");
            }
        }

        private void FleeToPrevious()
        {
            var oldRoom = _map.CurrentRoom();
            oldRoom?.OnRoomExit(this, _player);

            if(_map.FleeToPreviousRoom())
            {
                var newRoom = _map.CurrentRoom();
                newRoom?.OnRoomEntered(this, _player);

                visualizer.UpdatePlayerMarker(_map, playerMarker);

                _encounterCheckedThisCell = false;
                MaybeStartEncounter();
            }
            else
            {
                Debug.Log("You cannot flee");
            }
        }

        public void GiveItem(Player player, Item item)
        {
            if (item == null) return;

            if (item.Type == ItemType.Weapon)
            {
                player.AddWeapon((Weapon)item);
                Debug.Log("You found a weapon: " + item.Name);
            }
            else if (item.Type == ItemType.Armour)
            {
                player.AddArmour((ArmourItem)item);
                Debug.Log("You found armour: " + item.Name);
            }
            else
            {
                player.AddConsumable((Consumable)item);
                Debug.Log("You found a consumable: " + item.Name);
            }
        }

        public void GrantRandomLoot(Player player, int minCount, int maxCount, LootBias bias)
        {
            if (minCount < 1) minCount = 1;
            if (maxCount < minCount) maxCount = minCount;

            int count = Rng.Next(minCount, maxCount + 1);
            for (int i = 0; i < count; i++)
            {
                Item item = LootSystem.RandomItem(Rng, bias);
                GiveItem(player, item);
            }
        }
        private void MaybeStartEncounter()
        {
            if (InCombat) return;

            if (_encounterCheckedThisCell && _lastRow == _map.Row && _lastCol == _map.Col) return;

            Room room = _map.CurrentRoom();
            _encounterCheckedThisCell = true;
            _lastRow = _map.Row;
            _lastCol = _map.Col;

            var enc = room as EncounterRoom;
            if (enc != null && !enc.Cleared)
            {
                StartEncounter();
            }
        }
        private void PrintGameIntro()
        {
            Debug.Log("You wake up in a dungeon with no memory of how you got here.");
            Debug.Log("You know that you have to kill an Iron Golem to get an exit stone to leave the place.");
            Debug.Log("What do you do?");
        }

        private void PrintPokerRules()
        {
            Debug.Log("== POKER HAND RANKINGS ==");
            Debug.Log("Combat in this game depends on the dice you roll!");
            Debug.Log("Your attack power depends on your dice combination. Here’s the ranking from strongest to weakest:");
            Debug.Log("   Five of a Kind   - All dice show the same number. (Highest damage multiplier)");
            Debug.Log("   Four of a Kind   - Four dice of the same number.");
            Debug.Log("   Full House       - Three of one number and two of another.");
            Debug.Log("   Straight         - Five dice in sequence (e.g., 2-3-4-5-6).");
            Debug.Log("   Three of a Kind  - Three dice showing the same number.");
            Debug.Log("   Two Pair         - Two different pairs (e.g., 3-3 and 5-5).");
            Debug.Log("   One Pair         - Two dice showing the same number.");
            Debug.Log("   High Card        - None of the above; the highest die decides base damage.");
            Debug.Log("Higher hands give stronger multipliers — aim for combinations!");
        }

        public void PrintDiceBundle(int[] sidesBundle)
        {
            if (sidesBundle == null || sidesBundle.Length == 0)
            {
                Debug.Log("(no dice)");
                return;
            }

            string diceLine = "Dice: ";
            for (int i = 0; i < sidesBundle.Length; i++)
            {
                diceLine += "d" + sidesBundle[i];
                if (i < sidesBundle.Length - 1) diceLine += " ";
            }
            Debug.Log(diceLine);
        }
    }
}
