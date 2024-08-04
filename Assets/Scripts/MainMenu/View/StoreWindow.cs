using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace MainMenu
{
    public class StoreWindow : MonoBehaviour
    {
        [SerializeField] Button _addCashButton;
        [SerializeField] ScalableButton _showLeftButton;
        [SerializeField] ScalableButton _showRightButton;
        [SerializeField] ScalableButton _backButton;
        [SerializeField] ScalableButton _equipButton;
        [SerializeField] Image _equipButtonCashIcon;
        [SerializeField] TextMeshProUGUI _equipButtonLabel;
        [SerializeField] TextMeshProUGUI _cashLabel;

        [Header("PlayerPreviewRendering")]
        [SerializeField] Camera _renderTextureCamera;
        [SerializeField] Material _playerPreviewObjectMaterial;


        private CompositeDisposable _disposables = new();
        private StoreWindowViewModel _viewModel;

        [Inject]
        private void Construct(StoreWindowViewModel viewModel)
        {
            _viewModel = viewModel;

            _showLeftButton.Init();
            _showRightButton.Init();
            _backButton.Init();
            _equipButton.Init();

            _viewModel.IsShown.Subscribe(OnShowUpdated).AddTo(_disposables);
            _viewModel.PlayerCash.Subscribe(UpdateCash).AddTo(_disposables);
            _viewModel.OnWindowStateChanged.Subscribe(UpdateState).AddTo(_disposables);

            _showLeftButton.OnClick += _viewModel.OnShowLeftClicked;
            _showRightButton.OnClick += _viewModel.OnShowRightClicked;
            _backButton.OnClick += _viewModel.OnBackClicked;
            _equipButton.OnClick += _viewModel.OnEquipClicked;
            _addCashButton.onClick.AddListener(_viewModel.OnAddCashClicked);


            UpdateState(new());
        }

        private void OnDestroy()
        {
            _disposables.Dispose(); 
            
            _showLeftButton.OnClick -= _viewModel.OnShowLeftClicked;
            _showRightButton.OnClick -= _viewModel.OnShowRightClicked;
            _backButton.OnClick -= _viewModel.OnBackClicked;
            _equipButton.OnClick -= _viewModel.OnEquipClicked;
            _addCashButton.onClick.RemoveAllListeners();
        }

        private void OnShowUpdated(bool isShown)
        {
            this.SetActive(isShown);
            _renderTextureCamera.SetActive(isShown);
        }

        private void UpdateState(Unit unit)
        {
            _equipButton.SetInteractable(_viewModel.IsEquipButtonInteractable);
            _equipButtonLabel.text = _viewModel.EquipButtonText;
            _equipButtonCashIcon.SetActive(_viewModel.IsEquipButtonCashIconActive);
            _playerPreviewObjectMaterial.color = _viewModel.CurrentShownSkin.Color;
        }

        private void UpdateCash(float cash)
        {
            _cashLabel.text = cash.ToString();
        }
    }
}