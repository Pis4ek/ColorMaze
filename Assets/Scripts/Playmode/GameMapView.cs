using Configs;
using DG.Tweening;
using MainMenu;
using ObservableCollections;
using Playmode.Map;
using R3;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Utils;
using Zenject;
using static UnityEditor.Profiling.HierarchyFrameDataView;

namespace Playmode
{
    public class GameMapView : MonoBehaviour
    {
        [SerializeField] Transform _playerObject;
        [SerializeField] Transform _fillerPrefab;
        [SerializeField] Transform _fillersContainer;
        [SerializeField] Transform _obstaclePrefab;
        [SerializeField] Transform _obstaclesContainer;
        [SerializeField] Transform _coinContainer;
        [SerializeField] Transform _coinPrefab;


        [SerializeField] float _movingAnimationTime;
        [SerializeField] float _bounceAnimationTime;
        [SerializeField] Ease _movingAnimationEase;
        [SerializeField] Ease _bounceAnimationEase;
        [SerializeField] float _bounceMultiplier = 0.3f;

        private CompositeDisposable _disposable = new();
        private GameMapViewModel _viewModel;
        private LevelConfig _levelConfig;
        private PlaymodeInitialData _playmodeConfig;
        private SkinsConfig _skinsConfig;
        private AudioService _audioService;
        private TransformPool _pool;
        private Dictionary<Vector2Int, Transform> _fillers = new();
        private Dictionary<Vector2Int, Transform> _coins = new();
        private Queue<Sequence> _playerAnimation = new();
        private Sequence _currentAnimation;
        private bool _isAnimating = false;

        [Inject]
        public void Construct(GameMapViewModel viewModel, LevelConfig config, PlaymodeInitialData playmodeConfig,
            SkinsConfig skinsConfig, AudioService audioService)
        {
            _viewModel = viewModel;
            _levelConfig = config;
            _playmodeConfig = playmodeConfig;
            _skinsConfig = skinsConfig;
            _audioService = audioService;
            _pool = new(_fillerPrefab.gameObject, _fillersContainer, _levelConfig.Map.Length);

            SetSkinOnMaterials();

            _viewModel.OnExecute.Subscribe(AnimateExecuteCommand).AddTo(_disposable);
            _viewModel.OnUndo.Subscribe(AnimateUndoCommand).AddTo(_disposable);

            BuildMapObjects();
        }

        private void OnDestroy()
        {
            _currentAnimation?.Kill();
            _disposable.Dispose();
        }

        private void BuildMapObjects()
        {
            var mapSizeX = _levelConfig.Map.GetLength(0);
            var mapSizeY = _levelConfig.Map.GetLength(1);

            var mapObj = Instantiate(_obstaclePrefab, _obstaclesContainer);
            mapObj.position = new((mapSizeX - 1f) / 2f, -1f, (mapSizeY - 1f) / 2f);
            mapObj.localScale = new(mapSizeX + 2f, 1f, mapSizeY);
            mapObj.name = "Ground";

            mapObj = Instantiate(_obstaclePrefab, _obstaclesContainer);
            mapObj.position = new((mapSizeX - 1f) / 2f, 0f, mapSizeY);
            mapObj.localScale = new(mapSizeX + 2f, 1f, 1f);
            mapObj.name = "Border top";

            mapObj = Instantiate(_obstaclePrefab, _obstaclesContainer);
            mapObj.position = new(mapSizeX, 0f, (mapSizeY - 1f) / 2f);
            mapObj.localScale = new(1f, 1f, mapSizeX + 1f);
            mapObj.name = "Border right";

            mapObj = Instantiate(_obstaclePrefab, _obstaclesContainer);
            mapObj.position = new(-1f, 0f, (mapSizeY - 1f) / 2f);
            mapObj.localScale = new(1f, 1f, mapSizeX + 1f);
            mapObj.name = "Border left";

            foreach (var cellState in _viewModel.CellsStates)
            {
                if(cellState.Value == CellState.Obstacle)
                {
                    mapObj = Instantiate(_obstaclePrefab, _obstaclesContainer);
                    mapObj.position = MapToWorldPosition(cellState.Key);
                    mapObj.name = $"Obstacle {cellState.Key}";
                }
                else if (cellState.Value == CellState.Coin)
                {
                    mapObj = Instantiate(_coinPrefab, _coinContainer);
                    mapObj.position = MapToWorldPosition(cellState.Key);
                    _coins.Add(cellState.Key, mapObj);
                    mapObj.name = $"Coin {cellState.Key}";
                }
            }
        }

        private void SetSkinOnMaterials()
        {
            var selectedSkinColor = _skinsConfig.GetNodeBySkinID(_playmodeConfig.SelectedSkinID).Color;
            _playerObject.GetComponent<MeshRenderer>().sharedMaterial.color = selectedSkinColor;
            var filler = _pool.Get();
            filler.GetComponent<MeshRenderer>().sharedMaterial.color = selectedSkinColor;
            _pool.Add(filler);
        }

        private void AnimateExecuteCommand(CollectionAddEvent<MoveCommand> ev)
        {
            var cmd = ev.Value;

            var animation = AnimatePlayerToPosition(MapToWorldPosition(cmd.PlayerPositionAfterExecute), cmd.Direction);
            animation.PrependCallback(() =>
            {
                foreach (var cell in cmd.CellsStateBeforeExecute)
                {
                    var fillerObj = _pool.Get();
                    fillerObj.name = $"Filler x{cell.Key.x}, y{cell.Key.y}";
                    fillerObj.position = MapToWorldPosition(cell.Key, -0.4999f);
                    _fillers.Add(cell.Key, fillerObj);
                    fillerObj.Activate();

                    if(_coins.TryGetValue(cell.Key, out var coinObj))
                    {
                        coinObj.Disactivate();
                    }
                }
            });
            CheckAnimationPossibility();
        }

        private void AnimateUndoCommand(CollectionRemoveEvent<MoveCommand> ev)
        {
            var cmd = ev.Value;

            var animation = AnimatePlayerToPosition(MapToWorldPosition(cmd.PlayerPositionBeforeExecute), -cmd.Direction);
            animation.PrependCallback(() =>
            {
                foreach (var cell in cmd.CellsStateBeforeExecute)
                {
                    var fillerObj = _fillers[cell.Key];
                    _fillers.Remove(cell.Key);
                    fillerObj.Disactivate();
                    _pool.Add(fillerObj);

                    if (_coins.TryGetValue(cell.Key, out var coinObj))
                    {
                        coinObj.Activate();
                    }
                }
            });
            CheckAnimationPossibility();
        }

        private Sequence AnimatePlayerToPosition(Vector3 targetPosition, Vector2Int direction)
        {
            var bouncePosition = targetPosition + MapToWorldPosition(-direction) * _bounceMultiplier;

            var movingTween = _playerObject.DOMove(targetPosition, _movingAnimationTime).SetEase(_movingAnimationEase);
            var bounceTween = _playerObject.DOMove(bouncePosition, _bounceAnimationTime).SetEase(_bounceAnimationEase);
            var endingTween = _playerObject.DOMove(targetPosition, _bounceAnimationTime).SetEase(_bounceAnimationEase);

            var animation = DOTween.Sequence()
                .Append(movingTween)
                .AppendCallback(PlayHitSound)
                .Append(bounceTween)
                .Append(endingTween);

            animation.OnKill(() => {
                _isAnimating = false;
                CheckAnimationPossibility();
            });

            _playerAnimation.Enqueue(animation);

            return animation;
        }

        private void PlayHitSound()
        {
            _audioService.PlaySound("Hit");
        }

        private void CheckAnimationPossibility()
        {
            if (_isAnimating || _playerAnimation.Count == 0) return; 

            _isAnimating = true;
            _currentAnimation = _playerAnimation.Dequeue().Play();
        }

        private Vector3 MapToWorldPosition(Vector2Int mapPosition, float height = 0f)
        {
            return new Vector3(mapPosition.x, height, mapPosition.y);
        }
    }
}