using DG.Tweening;
using UnityEngine;

namespace Playmode
{
    public class CoinView : MonoBehaviour
    {
        private Vector3 _startRotation;
        private Vector3 _startPosition;
        private Sequence _rotationAnimation;
        private Sequence _movingAnimation;

        private void Start()
        {
            _startRotation = transform.rotation.eulerAngles;
            _startPosition = transform.position;

            _rotationAnimation = DOTween.Sequence()
                .Append(transform.DORotate(_startRotation + Vector3.up * 30f, 1f).SetEase(Ease.Linear))
                .Append(transform.DORotate(_startRotation + Vector3.up * -30f, 2f).SetEase(Ease.Linear))
                .Append(transform.DORotate(_startRotation, 1f).SetEase(Ease.Linear));

            _rotationAnimation.SetLoops(-1);
            _rotationAnimation.Play();

            _movingAnimation = DOTween.Sequence()
                .Append(transform.DOMove(_startPosition + Vector3.up * -0.2f, 1f).SetEase(Ease.Linear))
                .Append(transform.DOMove(_startPosition, 1f).SetEase(Ease.Linear));

            _movingAnimation.SetLoops(-1);
            _movingAnimation.Play();
        }

        private void OnDestroy()
        {
            _rotationAnimation.Kill();
            _movingAnimation.Kill();
        }
    }
}