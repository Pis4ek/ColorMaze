using DG.Tweening;
using Playmode.Map;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playmode
{
    public class PlaymodePlayerBankView : MonoBehaviour
    {
        [SerializeField] Image _coinImage;
        [SerializeField] TextMeshProUGUI _coinsCountLabel;
        [SerializeField] TextMeshProUGUI _coinsAddLabel;

        private readonly CompositeDisposable _disposables = new();
        private PlaymodePlayerBankViewModel _bank;
        private GameMapViewModel _gameMap;
        private Sequence _coinAnimation;
        private Sequence _labelAnimation;

        [Inject]
        public void Construct(PlaymodePlayerBankViewModel bank, GameMapViewModel gameMap)
        {
            _bank = bank;
            _gameMap = gameMap;

            _bank.CashEarnedForGame.Subscribe(OnCashChanged).AddTo(_disposables);
            _bank.LastEarnedCash.Skip(1).Subscribe(OnLastEarnedCashChanged).AddTo(_disposables);
            _gameMap.IsDrawableCellsEnded.Subscribe(OnLevelCompleted).AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void OnCashChanged(string newValue)
        {
            _coinsCountLabel.text = newValue;
            _coinAnimation?.Kill();

            _coinAnimation = DOTween.Sequence()
                .Append(_coinImage.transform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutCubic))
                .Append(_coinImage.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutCubic));

            _coinAnimation.Play();
        }

        private void OnLastEarnedCashChanged(string newValue)
        {
            var textTransform = _coinsAddLabel.transform as RectTransform;
            _coinsAddLabel.text = newValue;
            _labelAnimation?.Kill();
            _coinsAddLabel.alpha = 1f;
            textTransform.anchoredPosition3D = new(40, -20);

            var startPosition = textTransform.anchoredPosition3D;

            _labelAnimation = DOTween.Sequence()
                .Append(textTransform.DOAnchorPos3D(startPosition + Vector3.up * 40f, 0.4f).SetEase(Ease.Linear))
                .Append(textTransform.DOAnchorPos3D(startPosition + Vector3.up * 80f, 0.4f).SetEase(Ease.Linear))
                .Join(_coinsAddLabel.DOFade(0f, 0.4f)
                .OnKill(() => { textTransform.anchoredPosition3D = startPosition; }));


            _labelAnimation.Play();
        }


        private void OnLevelCompleted(bool value)
        {
            this.SetActive(!value);
        }

    }
}