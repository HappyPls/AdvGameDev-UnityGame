using UnityEngine;

namespace Dungeon
{
    public class DungeonController : MonoBehaviour
    {
        [Header("Dungeon Configuration")]
        public GridSettings Settings;
        public DungeonVisualizer Visualizer;

        [Header("Player")]
        public Player PlayerPrefab;
        public Transform PlayerMarker;
        private Player _playerInstance;

        [Header("Game Manager")]
        public GameManager GameManager;

        private Map _map;
        private bool _initialized;

        void Start()
        {
            if (!_initialized)
            {
                InitializeDungeon();
            }
        }
        public void InitializeDungeon()
        {
            if (Settings == null)
            {
                Debug.LogError("DungeonController: Missing GridSettings reference!");
                return;
            }

            if (Visualizer == null)
            {
                Debug.LogError("DungeonController: Missing DungeonVisualizer reference!");
                return;
            }

            //Generate new map logic from settings
            _map = new Map(Settings);

            //If a Player prefab is assigned but no instance exists, spawn it
            if (PlayerPrefab != null && _playerInstance == null)
            {
                _playerInstance = Instantiate(PlayerPrefab);
                _playerInstance.DisplayName = "Player";
                _playerInstance.name = "Player";
                PlayerMarker = _playerInstance.transform;
            }

            // Build dungeon visuals and position player
            Visualizer.Init(_map, Settings, PlayerMarker);

            _initialized = true;

            Debug.Log("Dungeon initialized with " + Settings.Rows + "x" + Settings.Cols + " grid.");
            NotifyGameManagerReady();
            TriggerCurrentRoom();
        }
        public void MovePlayerTo(int row, int col)
        {
            if (_map == null || Visualizer == null || PlayerMarker == null)
                return;

            if (row < 0 || col < 0 || row >= _map.Rows || col >= _map.Cols)
                return;

            Vector3 targetPos = new Vector3(
                row * Settings.CellSize,
                PlayerMarker.position.y,
                col * Settings.CellSize
            );

            PlayerMarker.position = targetPos;
        }
        public Map GetMap()
        {
            return _map;
        }
        public void RegenerateDungeon()
        {
            // Destroy existing room visuals
            if (Visualizer != null && Visualizer.transform.childCount > 0)
            {
                for (int i = Visualizer.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(Visualizer.transform.GetChild(i).gameObject);
                }
            }

            _initialized = false;
            InitializeDungeon();
        }
        public void NotifyGameManagerReady()
        {
            if (GameManager != null)
            {
                GameManager.OnDungeonReady(this);
            }
        }
        public bool TryMove(Direction dir)
        {
            if (_map == null) return false;

            bool moved = _map.TryMove(dir);
            if (!moved) return false;

            // Update player marker position
            if (Visualizer != null)
            {
                Visualizer.UpdatePlayerMarker(_map, PlayerMarker);
            }

            // Trigger room logic for the new cell
            TriggerCurrentRoom();
            return true;
        }

        private void TriggerCurrentRoom()
        {
            if (_map == null || Visualizer == null) return;

            RoomBase room = Visualizer.GetRoomInstanceAt(_map.Row, _map.Col);
            if (room != null)
            {
                Player p = _playerInstance != null ? _playerInstance : null;
                room.EnterRoom(p);
                room.TriggerRoomEvent(p);
            }
        }
    }
}
