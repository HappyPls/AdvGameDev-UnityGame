using UnityEngine;

namespace Dungeon
{
    public class DungeonVisualizer : MonoBehaviour
    {
        [Header("Visuals")]
        public float cellSize = 1f;
        public float playerYOffset = 0.75f;

        private Map _map;
        private Transform _playerMarker;
        private GameObject[,] _tiles;
        private bool _initialized;

        // Initializes with both the map and the player marker Transform
        public void Init(Map map, Transform playerMarker)
        {
            if (_initialized) return;
            _map = map;
            _playerMarker = playerMarker;
            BuildGrid();
            PositionPlayerMarker();
            _initialized = true;
        }

        void Update()
        {
            if (!_initialized) return;

            if (Input.GetKeyDown(KeyCode.Alpha1)) TryMove("n");
            if (Input.GetKeyDown(KeyCode.Alpha2)) TryMove("s");
            if (Input.GetKeyDown(KeyCode.Alpha3)) TryMove("e");
            if (Input.GetKeyDown(KeyCode.Alpha4)) TryMove("w");
        }

        private void BuildGrid()
        {
            _tiles = new GameObject[_map.Rows, _map.Cols];

            for (int r = 0; r < _map.Rows; r++)
            {
                for (int c = 0; c < _map.Cols; c++)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.name = $"Tile_{r}_{c}";
                    go.transform.position = GridToWorld(r, c);
                    go.transform.localScale = Vector3.one * cellSize;

                    var rend = go.GetComponent<Renderer>();
                    if (rend != null)
                        rend.material.color = ColorFor(_map.RoomAt(r, c));

                    _tiles[r, c] = go;
                }
            }
        }

        private void PositionPlayerMarker()
        {
            if (_playerMarker == null) return;
            _playerMarker.position = GridToWorld(_map.Row, _map.Col) + new Vector3(0f, playerYOffset, 0f);
        }

        private Vector3 GridToWorld(int r, int c)
        {
            return new Vector3(r * cellSize, 0f, c * cellSize);
        }

        private static Color ColorFor(Room room)
        {
            if (room is BossEncounterRoom) return Color.red;
            if (room is EncounterRoom) return Color.yellow;
            if (room is TreasureRoom) return Color.green;
            if (room is TrapRoom) return new Color(1f, 0.5f, 0f);
            if (room is SafeRoom) return Color.cyan;
            return Color.gray;
        }

        private void TryMove(string dir)
        {
            if (_map.TryMove(dir))
            {
                PositionPlayerMarker();
                Debug.Log($"Moved {dir.ToUpper()} → ({_map.Row},{_map.Col}) [{_map.CurrentRoom().GetType().Name}] visits: {_map.CurrentRoom().VisitCount}");
            }
            else
            {
                Debug.Log("Blocked.");
            }
        }
    }
}
