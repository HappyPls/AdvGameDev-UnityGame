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

        void Update()
        {
            if (!_initialized) return;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) TryMove("n");
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) TryMove("s");
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) TryMove("e");
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) TryMove("w");
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

                    Vector3 pos = GridToWorld(r,c);
                    RoomBase instance = Instantiate(prefab, pos, Quaternion.identity, RoomParent != null ? RoomParent : transform);
                    var view = instance.GetComponent<EncounterRoomView>(); if (view != null && room is EncounterRoom enc) view.SetCleared(enc.Cleared);

                    instance.name = $"Room_{r}_{c}";
                    instance.transform.localScale = Vector3.one * RoomSize;


                    _tiles[r, c] = instance.gameObject;
                }
            }

        }

        private RoomBase SelectPrefab(Room room)
        {
            if (room == null) return RoomPrefabs[0];

            if (room is BossEncounterRoom) return RoomPrefabs[5];
            if (room is EncounterRoom) return RoomPrefabs[1];
            if (room is TreasureRoom) return RoomPrefabs[2];
            if (room is TrapRoom) return RoomPrefabs[3];
            if (room is SafeRoom) return RoomPrefabs[4];
            if (room is EmptyRoom) return RoomPrefabs[0];

            return RoomPrefabs[0];
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
