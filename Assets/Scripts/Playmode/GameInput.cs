using R3;
using System.Collections;
using UnityEngine;

namespace Playmode
{
    public class GameInput
    {
        private GameMapViewModel _viewModel;

        public GameInput(GameMapViewModel viewModel)
        {
            _viewModel = viewModel;
            Observable.EveryUpdate().Subscribe(_ => Update());
        }

        private void Update()
        {
            if(_viewModel.IsDrawableCellsEnded.CurrentValue == false)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    _viewModel.UndoLastCommand();
                }
                else if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    _viewModel.MoveByDirection(Vector2Int.up);
                }
                else if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    _viewModel.MoveByDirection(Vector2Int.down);
                }
                else if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    _viewModel.MoveByDirection(Vector2Int.left);
                }
                else if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    _viewModel.MoveByDirection(Vector2Int.right);
                }
            }

        }
    }
}