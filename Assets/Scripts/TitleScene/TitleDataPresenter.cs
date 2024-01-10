using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleDataPresenter : MonoBehaviour
{
    

    /* model */
    [SerializeField] private TitleData titleData;

    /* view */
    [SerializeField] private Button _gameStartButton;
    [SerializeField] private GameObject _gameStartSelectedMarker;
    [SerializeField] private Button _difficultySettingButton;
    [SerializeField] private GameObject _difficultySettingSelectedMarker;

    [SerializeField] private Button _difficultyEasyButton;
    [SerializeField] private GameObject _difficultyEasySelectedMarker;
    [SerializeField] private Button _difficultyNormalButton;
    [SerializeField] private GameObject _difficultyNormalSelectedMarker;
    [SerializeField] private Button _difficultyDifficultButton;
    [SerializeField] private GameObject _difficultyDifficultSelectedMarker;

    [SerializeField] private GameObject _difficultyPanel;
    private Dictionary<Button, GameObject> _menu;

    [SerializeField] private SceneChanger _sceneChanger;

    // Start is called before the first frame update
    private void Start()
    {
        if (titleData != null)
        {
            titleData.TitleDataChanged += OnTitleDatahChanged;
        }

        _menu = new Dictionary<Button, GameObject>
        { 
            {_gameStartButton, _gameStartSelectedMarker},
            {_difficultySettingButton, _difficultySettingSelectedMarker},
            {_difficultyEasyButton, _difficultyEasySelectedMarker},
            {_difficultyNormalButton, _difficultyNormalSelectedMarker},
            {_difficultyDifficultButton, _difficultyDifficultSelectedMarker}
        };

        if (_difficultyPanel != null)
        {
            _difficultyPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (titleData != null)
        {
            titleData.TitleDataChanged -= OnTitleDatahChanged;
        }
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void StartGame()
    {
        if (_sceneChanger != null)
        {
            _sceneChanger.LoadGameScene();
        }
    }

    public void InputKey(KeyCode keyCode)
    {
        if (titleData == null)
        {
            return;
        }

        switch (keyCode)
        {
            case KeyCode.UpArrow:
                titleData.ReceiveInputDirection(TitleData.InputDirection.Upper);
                break;
            case KeyCode.DownArrow:
                titleData.ReceiveInputDirection(TitleData.InputDirection.Down);
                break;
            case KeyCode.LeftArrow:
                titleData.ReceiveInputDirection(TitleData.InputDirection.Left);
                break;
            case KeyCode.RightArrow:
                titleData.ReceiveInputDirection(TitleData.InputDirection.Right);
                break;
            case KeyCode.Return:
                titleData.ReceiveInputHierarchy(TitleData.InputSelect.Enter);
                break;
            case KeyCode.Escape:
                titleData.ReceiveInputHierarchy(TitleData.InputSelect.Back);
                break;
            default:
                break;
        }
    }

    public void UpdateView()
    {
        if (titleData == null)
        {
            return;
        }

        /* view�X�V(���ݑI�𒆂̐ݒ�X�V) */

        /* UI�\�����Ă���K�w�̍X�V */
        TitleData.UiHierarchy currentHierarchy = titleData.CurrentHierarchy;
        /* TODO: ������view�̃q�G�����L�[�X�V */
        if (currentHierarchy == TitleData.UiHierarchy.StartGame)
        {
            StartGame();
            return; /* Game Start */
        }
        else if(currentHierarchy == TitleData.UiHierarchy.Difficulty)
        {
            _difficultyPanel.SetActive(true);
        }
        else 
        {
            _difficultyPanel.SetActive(false);
        }

        /* �I�𒆂̃{�^���������}�[�J�[�̕\�� */
        TitleData.ContentCombo selectCombo = titleData.GetCurrentSelectedCombo();
        if (_menu.ContainsKey(selectCombo.ContentButton))
        {
            /* �������񂷂ׂĔ�\�� */
            foreach(var selectObject in _menu.Values)
            {
                if (selectObject != null)
                {
                    selectObject.SetActive(false);
                }
            }
            /* ���ݑI�𒆂̂��̂�\�� */
            _menu[selectCombo.ContentButton].SetActive(true);
        }
    }

    public void OnTitleDatahChanged()
    {
        UpdateView();
    }
}
