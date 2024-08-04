using ObservableCollections;
using Playmode.Map;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playmode
{
    public class GameMapViewModel : IDisposable
    {
        private readonly CompositeDisposable _disposable = new();
        private readonly ObservableStack<MoveCommand> _commands = new();
        private readonly GameMap _map;
        private readonly PlaymodePlayerBank _bank;

        public readonly ReactiveProperty<Vector3> WorldPosition = new();
        public readonly ReactiveProperty<bool> IsDrawableCellsEnded = new();

        public Observable<CollectionAddEvent<MoveCommand>> OnExecute => _commands.ObserveAdd();
        public Observable<CollectionRemoveEvent<MoveCommand>> OnUndo => _commands.ObserveRemove();
        public IReadOnlyDictionary<Vector2Int, CellState> CellsStates => _map.CellsStates;


        public GameMapViewModel(GameMap map, PlaymodePlayerBank bank)
        {
            _map = map;
            _bank = bank;
        }

        public void MoveByDirection(Vector2Int direction)
        {
            if(IsDrawableCellsEnded.Value == false)
            {
                if (_map.IsCellMovable(_map.PlayerPosition + direction))
                {
                    var cmd = new MoveCommand(_map, _bank, direction);
                    _commands.Push(cmd);
                    cmd.Execute();

                    if (_map.DrawableCellsCount == 0)
                    {
                        _bank.ApplyCash();
                        IsDrawableCellsEnded.Value = true;
                    }
                }
            }

        }

        public void UndoLastCommand()
        {
            if (_commands.Count == 0) return;

            _commands.Pop().Undo();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

    }
}