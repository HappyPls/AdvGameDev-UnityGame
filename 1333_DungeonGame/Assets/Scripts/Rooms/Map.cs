using System;

namespace Dungeon
{
    public class Map
    {
        private RoomType[,] _grid;
        private int _rows;
        private int _cols;
        private System.Random _rng;

        private int _row;
        private int _col;

        public int Rows { get { return _rows; } }
        public int Cols { get { return _cols; } }
        public int Row { get { return _row; } }
        public int Col { get { return _col; } }

        public Map(GridSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            _rows = Math.Max(3, settings.Rows);
            _cols = Math.Max(3, settings.Cols);

            if (settings.UseFixedSeed)
            {
                _rng = new System.Random(settings.Seed);
            }
            else
            {
                _rng = new System.Random();
            }

            _grid = new RoomType[_rows, _cols];

            //Center start position
            _row = _rows / 2;
            _col = _cols / 2;

            Generate(settings);
        }

        public RoomType Get(int r, int c)
        {
            return _grid[r, c];
        }

        public void Set(int r, int c, RoomType t)
        {
            _grid[r, c] = t;
        }

        public bool InBounds(int r, int c)
        {
            if (r < 0 || c < 0) return false;
            if (r >= _rows || c >= _cols) return false;
            return true;
        }

        private void Generate(GridSettings s)
        {
            int r = 0;
            while (r < _rows)
            {
                int c = 0;
                while (c < _cols)
                {
                    _grid[r, c] = RoomType.Empty;
                    c += 1;
                }
                r += 1;
            }

            int placedBoss = 0;
            while (placedBoss < s.MinBoss)
            {
                if (TryPlaceBoss(s))
                {
                    placedBoss += 1;
                }
                else
                {
                    PlaceRandomOfType(RoomType.Boss, avoidCenter: true);
                    placedBoss += 1;
                }
            }

            PlaceMinimum(RoomType.Treasure, s.MinTreasure);
            PlaceMinimum(RoomType.Encounter, s.MinEncounter);
            PlaceMinimum(RoomType.Trap, s.MinTrap);
            PlaceMinimum(RoomType.Safe, s.MinSafe);

            float wEmpty = 0.45f;
            float wEncounter = 0.25f;
            float wTrap = 0.15f;
            float wTreasure = 0.1f;
            float wSafe = 0.05f;

            r = 0;
            while (r < _rows)
            {
                int c = 0;
                while (c < _cols)
                {
                    if (_grid[r, c] == RoomType.Empty)
                    {
                        if (!(r == _row && c == _col))
                        {
                            RoomType pick = WeightedPick(wEmpty, wEncounter, wTrap, wTreasure, wSafe);
                            _grid[r, c] = pick;
                        }
                    }
                    c += 1;
                }
                r += 1;
            }

            _grid[_row, _col] = RoomType.Empty;
        }

        private RoomType WeightedPick(float wEmpty, float wEncounter, float wTrap, float wTreasure, float wSafe)
        {
            float total = wEmpty + wEncounter + wTrap + wTreasure + wSafe;
            if (total <= 0f) return RoomType.Empty;

            double r = _rng.NextDouble() * total;
            if (r < wEmpty) return RoomType.Empty;
            r -= wEmpty;
            if (r < wEncounter) return RoomType.Encounter;
            r -= wEncounter;
            if (r < wTrap) return RoomType.Trap;
            r -= wTrap;
            if (r < wTreasure) return RoomType.Treasure;
            return RoomType.Safe;
        }

        private void PlaceMinimum(RoomType t, int minCount)
        {
            if (minCount <= 0) return;
            int placed = CountType(t);
            while (placed < minCount)
            {
                if (PlaceRandomOfType(t, avoidCenter: true))
                {
                    placed += 1;
                }
                else
                {
                    break;
                }
            }
        }

        private int CountType(RoomType t)
        {
            int count = 0;
            int r = 0;
            while (r < _rows)
            {
                int c = 0;
                while (c < _cols)
                {
                    if (_grid[r, c] == t) count += 1;
                    c += 1;
                }
                r += 1;
            }
            return count;
        }

        private bool PlaceRandomOfType(RoomType t, bool avoidCenter)
        {
            int tries = 0;
            int maxTries = _rows * _cols * 2;

            while (tries < maxTries)
            {
                int rr = _rng.Next(0, _rows);
                int cc = _rng.Next(0, _cols);

                if (avoidCenter && rr == _row && cc == _col)
                {
                    tries += 1;
                    continue;
                }

                if (_grid[rr, cc] == RoomType.Empty)
                {
                    _grid[rr, cc] = t;
                    return true;
                }
                tries += 1;
            }
            return false;
        }

        private bool TryPlaceBoss(GridSettings s)
        {
            if (!s.KeepBossFarFromCenter) return PlaceRandomOfType(RoomType.Boss, true);

            int centerR = _rows / 2;
            int centerC = _cols / 2;
            int minDist = Math.Max(0, s.BossMinManhattanDistance);

            int bestR = -1;
            int bestC = -1;
            int bestDist = -1;

            int r = 0;
            while (r < _rows)
            {
                int c = 0;
                while (c < _cols)
                {
                    int dist = Math.Abs(r - centerR) + Math.Abs(c - centerC);
                    if (dist >= minDist && _grid[r, c] == RoomType.Empty)
                    {
                        if (dist > bestDist)
                        {
                            bestDist = dist;
                            bestR = r;
                            bestC = c;
                        }
                    }
                    c += 1;
                }
                r += 1;
            }

            if (bestR >= 0)
            {
                _grid[bestR, bestC] = RoomType.Boss;
                return true;
            }
            return false;
        }
        public bool TryMove(Direction dir)
        {
            int targetR = _row;
            int targetC = _col;

            if (dir == Direction.North) targetR -= 1;
            else if (dir == Direction.South) targetR += 1;
            else if (dir == Direction.East) targetC += 1;
            else if (dir == Direction.West) targetC -= 1;

            if (!InBounds(targetR, targetC)) return false;

            _row = targetR;
            _col = targetC;
            return true;
        }
    }
}
