using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MainMenu
{
    public class LevelButton : MonoBehaviour, IPointerClickHandler
    {
        public event Action OnClick;

        [SerializeField] TextMeshProUGUI _levelNumberLabel;
        [SerializeField] Image _levelDoneIcon;

        public void Initialize(int levelNuumber, bool isDone)
        {
            _levelNumberLabel.text = levelNuumber.ToString();
            _levelDoneIcon.SetActive(isDone);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }
    }
}