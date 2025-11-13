using UnityEngine;

namespace Dungeon
{
    public class DungeonVisualizer : MonoBehaviour
    {
        [Header("Prefabs by Type")]
        public RoomBase[] EmptyPrefabs;
        public RoomBase[] EncounterPrefabs;
        public RoomBase[] BossPrefabs;
        public RoomBase[] TrapPrefabs;
        public RoomBase[] TreasurePrefabs;
        public RoomBase[] SafePrefabs;

        [Header("Parents & Visual")]
        public Transform RoomParent;
        public float PlayerYOffset = 0.75f;

        private bool _initialized;
        private Map _map;
        private GridSettings _settings;
        private Transform _playerMarker;
        private RoomBase[,] _instances;

        public void Init(Map map, GridSettings settings, Transform playerMarker)
        {
            if (_initialized) return;
            _map = map;
            _settings = settings;
            _playerMarker = playerMarker;

            BuildGrid();
            PositionPlayerMarker();
            _initialized = true;
        }


        private void BuildGrid()
        {
            int rows = _map.Rows;
            int cols = _map.Cols;
            _instances = new RoomBase[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    RoomBase prefab = PickPrefab(_map.Get(r, c));
                    Vector3 pos = new Vector3(r * _settings.CellSize, 0f, c * _settings.CellSize);
                    Transform parent = RoomParent != null ? RoomParent : transform;

                    if (prefab != null)
                    {
                        RoomBase inst = Instantiate(prefab, pos, Quaternion.identity, parent);
                        inst.transform.localScale = Vector3.one * _settings.RoomScale;
                        inst.Init(new Vector2Int(r, c));
                        _instances[r, c] = inst;
                    }
                }
            }
        }
        private RoomBase PickPrefab(RoomType type)
        {
            RoomBase[] pool = null;
            switch (type)
            {
                case RoomType.Empty: pool = EmptyPrefabs; break;
                case RoomType.Encounter: pool = EncounterPrefabs; break;
                case RoomType.Boss: pool = BossPrefabs; break;
                case RoomType.Trap: pool = TrapPrefabs; break;
                case RoomType.Treasure: pool = TreasurePrefabs; break;
                case RoomType.Safe: pool = SafePrefabs; break;
            }

            if (pool == null || pool.Length == 0) return null;
            int idx = Random.Range(0, pool.Length);
            return pool[idx];
        }

        private void PositionPlayerMarker()
        {
            if (_playerMarker == null || _map == null || _settings == null) return;
            Vector3 pos = new Vector3(_map.Row * _settings.CellSize, 0f, _map.Col * _settings.CellSize);
            _playerMarker.position = pos + new Vector3(0f, PlayerYOffset, 0f);
        }

        public RoomBase GetRoomInstanceAt(int r, int c)
        {
            if (_instances == null) return null;
            if (r < 0 || c < 0 || r >= _instances.GetLength(0) || c >= _instances.GetLength(1)) return null;
            return _instances[r, c];
        }

        public void UpdatePlayerMarker(Map map, Transform marker)
        {
            if (marker == null || map == null || _settings == null) return;
            Vector3 pos = new Vector3(map.Row * _settings.CellSize, 0f, map.Col * _settings.CellSize);
            marker.position = pos + new Vector3(0f, PlayerYOffset, 0f);
        }
    }
}
