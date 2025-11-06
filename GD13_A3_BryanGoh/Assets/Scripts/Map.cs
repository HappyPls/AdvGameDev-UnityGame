using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Dungeon
{
    public class Map
    {
        private Room[,] _grid;
        private int _rows = 0;
        private int _cols = 0;
        private System.Random _rng;

        private int _prevRow;
        private int _prevCol;

        public int Row { get; private set; }
        public int Col { get; private set; }
        public int Rows => _rows;
        public int Cols => _cols;
        public Room RoomAt(int r, int c) => _grid[r, c];

        public Map() : this(5, 5) { }

        public Map(int rows, int cols)
        {
            _rows = Math.Max(5, rows);
            _cols = Math.Max(5, cols);

            _grid = new Room[_rows, _cols];

            GenerateRooms();

            Row = _rows / 2;
            Col = _cols / 2;
            _grid[Row, Col].VisitCount++;

            _prevRow = Row;
            _prevCol = Col;

            WireNeighbours();
        }

        public Room CurrentRoom()
        {
            if (Row < 0) Row = 0;
            if (Row >= _rows) Row = _rows - 1;
            if (Col < 0) Col = 0;
            if (Col >= _cols) Col = _cols - 1;
            return _grid[Row, Col];
        }

        public bool TryMove(string direction)
        {
            int newRow = Row;
            int newCol = Col;

            switch (direction.ToLower())
            {
                case "north":
                case "n": newRow -= 1; break;
                case "south":
                case "s": newRow += 1; break;
                case "east":
                case "e": newCol += 1; break;
                case "west":
                case "w": newCol -= 1; break;
                default: return false;
            }

            if (newRow < 0 || newRow >= _rows || newCol < 0 || newCol >= _cols)
                return false;

            _prevRow = Row;
            _prevCol = Col;

            Row = newRow;
            Col = newCol;

            Room newRoom = _grid[Row, Col];
            newRoom.VisitCount++;
            return true;
        }

        public bool FleeToPreviousRoom()
        {
            if (_prevRow < 0 || _prevRow >= _rows || _prevCol < 0 || _prevCol >= _cols)
                return false;

            int curRow = Row;
            int curCol = Col;

            Row = _prevRow;
            Col = _prevCol;

            _prevRow = curRow;
            _prevCol = curCol;

            _grid[Row, Col].VisitCount++;
            return true;
        }

        private void GenerateRooms()
        {
            _rng = new System.Random();
            bool haveTreasure = false;
            bool haveEncounter = false;

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    Room room = null;
                    int maxTries = 8;

                    for (int attempt = 0; attempt < maxTries; attempt++)
                    {
                        room = RollRoom(_rng, ref haveTreasure, ref haveEncounter);

                        if (!SameTypeAs(r - 1, c, room) && !SameTypeAs(r, c - 1, room))
                            break;
                    }
                    if (SameTypeAs(r - 1, c, room) || SameTypeAs(r, c - 1, room))
                        room = new EmptyRoom();

                    _grid[r, c] = room;
                }
            }

            if (!haveTreasure) PlaceGuaranteed<TreasureRoom>(ref haveTreasure);
            if (!haveEncounter) PlaceGuaranteed<EncounterRoom>(ref haveEncounter);

            int spawnRow = _rows / 2;
            int spawnCol = _cols / 2;
            _grid[spawnRow, spawnCol] = new EmptyRoom();
            PlaceBossFarFromCenter();
        }

        private void WireNeighbours()
        {
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    Room room = _grid[r, c];
                    room.North = (r > 0) ? _grid[r - 1, c] : null;
                    room.South = (r < _rows - 1) ? _grid[r + 1, c] : null;
                    room.East = (c < _cols - 1) ? _grid[r, c + 1] : null;
                    room.West = (c > 0) ? _grid[r, c - 1] : null;
                }
            }
        }

        public void SetPosition(int row, int col)
        {
            Row = row;
            Col = col;
            _grid[Row, Col].VisitCount++;
        }

        private Room RollRoom(System.Random rng, ref bool haveTreasure, ref bool haveEncounter)
        {
            int roll = rng.Next(0, 100);

            if (roll < 20)
            {
                haveTreasure = true;
                return new TreasureRoom();
            }
            else if (roll < 40)
            {
                return new SafeRoom();
            }
            else if (roll < 50)
            {
                return new TrapRoom();
            }
            else if (roll < 60)
            {
                haveEncounter = true;
                return new EncounterRoom();
            }
            else
            {
                return new EmptyRoom();
            }
        }

        private bool SameTypeAs(int r, int c, Room candidate)
        {
            if (r < 0 || c < 0 || r >= _rows || c >= _cols) return false;
            var other = _grid[r, c];
            return other != null && other.GetType() == candidate.GetType();
        }
        private void PlaceGuaranteed<T>(ref bool flag) where T : Room, new()
        {
            for (int tries = 0; tries < 200; tries++)
            {
                int r = _rng.Next(0, _rows);
                int c = _rng.Next(0, _cols);
                var candidate = new T();

                if (!SameTypeAs(r - 1, c, candidate) &&
                    !SameTypeAs(r + 1, c, candidate) &&
                    !SameTypeAs(r, c - 1, candidate) &&
                    !SameTypeAs(r, c + 1, candidate))
                {
                    _grid[r, c] = candidate;
                    flag = true;
                    return;
                }
            }

            // fallback: if no good spot found
            _grid[_rows - 1, 0] = new T();
            flag = true;
        }

        private void PlaceBossFarFromCenter()
        {
            int startR = _rows / 2;
            int startC = _cols / 2;
            var candidates = new List<(int r, int c)>();

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    int dist = Math.Abs(r - startR) + Math.Abs(c - startC);
                    if (dist < 3) continue; // keep boss away from start

                    var boss = new BossEncounterRoom();
                    if (!SameTypeAs(r - 1, c, boss) &&
                        !SameTypeAs(r + 1, c, boss) &&
                        !SameTypeAs(r, c - 1, boss) &&
                        !SameTypeAs(r, c + 1, boss))
                    {
                        candidates.Add((r, c));
                    }
                }
            }

            if (candidates.Count > 0)
            {
                var pick = candidates[_rng.Next(candidates.Count)];
                _grid[pick.r, pick.c] = new BossEncounterRoom();
            }
            else
            {
                _grid[0, _cols - 1] = new BossEncounterRoom();
            }
        }

    }
}
