#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Dungeon
{
    public class DungeonVisualizer : MonoBehaviour
    {
        [Header("Rooms")]
        [SerializeField] private RoomBase[] RoomPrefabs;
        [SerializeField] private float RoomSize = 1;
        [SerializeField] private Transform RoomParent;

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


        private void BuildGrid()
        {
            if (RoomPrefabs == null || RoomPrefabs.Length == 0)
            {
                Debug.LogError("Room Prefabs is Empty!");
                return;
            }

            _tiles = new GameObject[_map.Rows, _map.Cols];

            for (int r = 0; r < _map.Rows; r++)
            {
                for (int c = 0; c < _map.Cols; c++)
                {
                    Room room = _map.RoomAt(r, c);
                    RoomBase prefab = SelectPrefab(room);

                    Vector3 pos = GridToWorld(r, c);
                    RoomBase instance = Instantiate(prefab, pos, Quaternion.identity,
                        RoomParent != null ? RoomParent : transform);

                    instance.name = $"Room_{r}_{c}";
                    instance.transform.localScale = Vector3.one * RoomSize;
                    _tiles[r, c] = instance.gameObject;
                }
            }
        }

        private RoomBase SelectPrefab(Room room)
        {
            if (room == null) return RoomPrefabs[0];

            if (room is BossEncounterRoom) return RoomPrefabs[9];
            if (room is EncounterRoom)
            {
                int[] options = { 1, 2, 3 };
                int index = options[UnityEngine.Random.Range(0, options.Length)];
                return RoomPrefabs[index];
            }
            if (room is TreasureRoom)
            {
                int[] options = { 4, 5, 6 };
                int index = options[UnityEngine.Random.Range(0, options.Length)];
                return RoomPrefabs[index];
            }
            if (room is SafeRoom) return RoomPrefabs[7];
            if (room is TrapRoom) return RoomPrefabs[8];
            if (room is EmptyRoom) return RoomPrefabs[0];

            return RoomPrefabs[0];
        }

        private void PositionPlayerMarker()
        {
            if (_playerMarker == null) return;
            _playerMarker.position = GridToWorld(_map.Row, _map.Col) +
                                     new Vector3(0f, playerYOffset, 0f);
        }

        private Vector3 GridToWorld(int r, int c)
        {
            return new Vector3(r * cellSize, 0f, c * cellSize);
        }

        public void UpdatePlayerMarker(Map map, Transform marker)
        {
            _map = map;
            _playerMarker = marker;
            PositionPlayerMarker();
        }
    }
}
