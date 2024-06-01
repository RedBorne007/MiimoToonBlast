using Coffee.UIExtensions;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _shuffleButton;

    [field: SerializeField] public Transform _particleParent { get; private set; }

    private float _previousScore;
    private Sequence _scoreTween;

    private BoardManager _boardManager;

    private void Start()
    {
        _boardManager = BoardManager.Instance;

        _resetButton.onClick.AddListener(OnResetButton);
        _shuffleButton.onClick.AddListener(OnShuffleButton);
    }

    private void OnResetButton()
    {
        _boardManager.Score = 0;
        _boardManager.GenerateBoard();
    }

    private void OnShuffleButton()
    {
        _boardManager.Shuffle();
    }

    public void UpdateScore()
    {
        if (_scoreTween != null && !_scoreTween.IsComplete())
        {
            _previousScore = _boardManager.Score;
            _scoreTween.Kill();
        }

        _scoreTween = DOTween.Sequence();

        float value = _previousScore;
        _scoreTween.Join(_scoreText.transform.DOPunchScale(Vector3.one * 0.25f, 0.25f)).Join(DOTween.To(() => value, x =>
        {
            value = x;
            _scoreText.text = x.ToString("F0");
        },
        _boardManager.Score, 0.5f).OnComplete(() =>
        {
            _previousScore = value;
        }));
    }
}
