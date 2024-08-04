using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utils
{
    [RequireComponent(typeof(Image))]
    public class ScalableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event Action OnClick;

        [Header("Scales")]
        [SerializeField] float _enterScale = 1.1f;
        [SerializeField] float _downScale = 0.8f;

        [Header("AnimationTimes")]
        [SerializeField] float _enterAnimTime = 0.17f;
        [SerializeField] float _exitAnimTime = 0.3f;
        [SerializeField] float _downAnimTime = 0.2f;
        [SerializeField] float _upAnimTime = 0.3f;

        [Header("AnimationTypes")]
        [SerializeField] Ease _enterEasing = Ease.OutCubic;
        [SerializeField] Ease _exitEasing = Ease.OutCubic;
        [SerializeField] Ease _downEasing = Ease.OutCubic;
        [SerializeField] Ease _upEasing = Ease.OutCubic;

        [Header("InteractionOptions")]
        [SerializeField] float _colorMultiplier = 0.9f;
        [SerializeField] float _alphaMultiplier = 0.6f;

        [field: SerializeField] public bool IsInteractable { get; private set; } = true;

        private Sequence _currentAnim;
        private Image _image;
        private Color _originalColor;
        private Vector3 _defaultScale = Vector3.one;
        private bool _isPointerEnter = false;

        private void Awake()
        {
            Init();
        }

        private void OnDestroy()
        {
            _currentAnim?.Kill();
        }

        public void Init()
        {
            _defaultScale = transform.localScale;
            _image = GetComponent<Image>();
            _originalColor = _image.color;

            SetInteractable(IsInteractable);
        }

        public void SetDefaultState()
        {
            _currentAnim?.Kill();
            transform.localScale = _defaultScale;
        }

        public void SetAnimation(Sequence animation)
        {
            _currentAnim?.Kill();
            _currentAnim = animation;
            _currentAnim.Play();
        }

        public void SetInteractable(bool isInteractable)
        {
            if (isInteractable)
            {
                IsInteractable = true;

                _image.color = _originalColor;
            }
            else
            {
                IsInteractable = false;

                var color = new Color(
                    _originalColor.r * _colorMultiplier,
                    _originalColor.g * _colorMultiplier,
                    _originalColor.b * _colorMultiplier,
                    _originalColor.a * _alphaMultiplier
                    );
                _image.color = color;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (IsInteractable == false) return;

            OnClick?.Invoke();
        }

        #region Up/Down       
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (IsInteractable == false) return;

            var tween = transform.DOScale(_defaultScale * _downScale, _downAnimTime).SetEase(_downEasing);
            SetAnimation(DOTween.Sequence().Append(tween).AppendCallback(CheckActivity));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (IsInteractable == false) return;

            if (_isPointerEnter)
            {
                var tween = transform.DOScale(_defaultScale * _enterScale, _upAnimTime).SetEase(_upEasing);
                SetAnimation(DOTween.Sequence().Append(tween).AppendCallback(CheckActivity));
            }
            else
            {
                var tween = transform.DOScale(_defaultScale, _upAnimTime).SetEase(_upEasing);
                SetAnimation(DOTween.Sequence().Append(tween).AppendCallback(CheckActivity));
            }
        }
        #endregion

        #region Enter/Exit
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsInteractable == false) return;

            _isPointerEnter = true;
            var tween = transform.DOScale(_defaultScale * _enterScale, _enterAnimTime).SetEase(_enterEasing);
            SetAnimation(DOTween.Sequence().Append(tween).AppendCallback(CheckActivity));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsInteractable == false) return;

            _isPointerEnter = false;
            var tween = transform.DOScale(_defaultScale, _exitAnimTime).SetEase(_exitEasing);
            SetAnimation(DOTween.Sequence().Append(tween).AppendCallback(CheckActivity));
        }
        #endregion

        private void CheckActivity()
        {
            if (isActiveAndEnabled == false)
            {
                transform.localScale = _defaultScale;
            }
        }


    }
}