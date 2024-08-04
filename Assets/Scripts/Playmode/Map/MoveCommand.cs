using System.Collections.Generic;
using UnityEngine;

namespace Playmode.Map
{
    public class MoveCommand : ICommand
    {
        private readonly Dictionary<Vector2Int, CellState> _cellsStateBeforeExecute = new();
        private readonly Dictionary<Vector2Int, CellState> _cellsStateAfterExecute = new();
        private readonly GameMap _map;
        private readonly PlaymodePlayerBank _bank;

        public readonly Vector2Int Direction;
        public IReadOnlyDictionary<Vector2Int, CellState> CellsStateBeforeExecute => _cellsStateBeforeExecute;
        public Vector2Int PlayerPositionBeforeExecute { get; private set; }
        public Vector2Int PlayerPositionAfterExecute { get; private set; }
        public float EarnedCoinsByMove { get; private set; } = 0f;

        public MoveCommand(GameMap map, PlaymodePlayerBank bank, Vector2Int direction)
        {
            _map = map;
            Direction = direction;
            _bank = bank;

            PlayerPositionBeforeExecute = _map.PlayerPosition;
            CaclulateAffectedCells();
        }

        public void Execute()
        {
            _map.PlayerPosition = PlayerPositionAfterExecute;
            foreach (var cellPosition in _cellsStateBeforeExecute.Keys)
            {
                _map.ChangeCellState(cellPosition, CellState.Drawed);
            }
            _bank.ChangeCash(EarnedCoinsByMove);
        }

        public void Undo()
        {
            _map.PlayerPosition = PlayerPositionBeforeExecute;
            foreach (var cell in _cellsStateBeforeExecute)
            {
                _map.ChangeCellState(cell.Key, cell.Value);
            }
            _bank.ChangeCash(-EarnedCoinsByMove);
        }

        private void CaclulateAffectedCells()
        {
            for (var pos = PlayerPositionBeforeExecute; ; pos += Direction)
            {
                if(_map.TryGetState(pos, out var state))
                {
                    if(state == CellState.Obstacle)
                    {
                        PlayerPositionAfterExecute = pos - Direction;
                        break;
                    }
                    else if(state == CellState.Clear)
                    {
                        _cellsStateBeforeExecute.Add(pos, state);
                    }
                    else if(state == CellState.Coin)
                    {
                        _cellsStateBeforeExecute.Add(pos, state);
                        EarnedCoinsByMove++;
                    }
                }
                else
                {
                    PlayerPositionAfterExecute = pos - Direction;
                    break;
                }
            }
        }
    }
}