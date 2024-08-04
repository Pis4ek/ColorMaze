using Configs;
using System.Collections.Generic;
using UnityEngine;

namespace Playmode.Map
{
    public class GameMap
    {
        private readonly Dictionary<Vector2Int, CellState> _cellsMap = new();
        private readonly HashSet<Vector2Int> _drawableCells = new();
        private readonly HashSet<Vector2Int> _coinsCells = new();
        private readonly LevelConfig _config;

        public Vector2Int PlayerPosition;

        public int DrawableCellsCount => _drawableCells.Count;
        public IReadOnlyDictionary<Vector2Int, CellState> CellsStates => _cellsMap;

        public GameMap(LevelConfig config)
        {
            _config = config;
            PlayerPosition = config.StartPoint;

            for(int x = 0;  x < config.Map.GetLength(0); x++)
            {
                for (int y = 0; y < config.Map.GetLength(1); y++)
                {
                    var cellState = (CellState)config.Map[x, y];
                    _cellsMap.Add(new(x, y), cellState);

                    if(cellState == CellState.Clear) _drawableCells.Add(new(x, y));
                }
            }

            if (IsInMap(PlayerPosition) == false)
            {
                throw new System.Exception("Invalid start position in level config.");
            }
            SetCoins();
        }

        public bool IsInMap(Vector2Int position)
        {
            return _cellsMap.ContainsKey(position);
        }

        public bool IsCellMovable(Vector2Int position)
        {
            return TryGetState(position, out var state) && state != CellState.Obstacle;
        }

        public bool TryGetState(Vector2Int position, out CellState state)
        {
            return _cellsMap.TryGetValue(position, out state);
        }

        public void ChangeCellState(Vector2Int position, CellState state)
        {
            if(IsInMap(position))
            {
                _cellsMap[position] = state;

                if (state == CellState.Clear || state == CellState.Coin)
                    _drawableCells.Add(position);
                else
                    _drawableCells.Remove(position);
            }
        }

        private void SetCoins()
        {
            while (true)
            {
                var randX = Random.Range(0, _config.Map.GetLength(1));
                var randY = Random.Range(0, _config.Map.GetLength(0));
                var position = new Vector2Int(randX, randY);

                if (position != PlayerPosition && _drawableCells.Contains(position) 
                    && _coinsCells.Contains(position) == false)
                {
                    _coinsCells.Add(position);
                    _cellsMap[position] = CellState.Coin;
                }

                if (_coinsCells.Count == _config.CoinsCount) return;
            }

        }
    }
}