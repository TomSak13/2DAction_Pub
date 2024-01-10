using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TitleData : MonoBehaviour
{
    public enum SettingContents
    {
        GameStart,
        Difficulty
    }

    public enum Difficulty
    {
        Easy,
        Normal,
        Difficult
    }

    public enum InputDirection
    {
        Upper,
        Down,
        Left,
        Right
    }

    public enum InputSelect
    {
        Enter,
        Back
    }

    public enum UiHierarchy
    {
        Route,
        Difficulty,
        StartGame /* �Q�[���X�^�[�g�J�ڗp */
    }

    public class ContentCombo
    {
        public Button ContentButton;
        public Dictionary<InputDirection, ContentCombo> Neighbor;
        public Dictionary<InputSelect ,UiHierarchy> NeighborHierarchy;
    }

    public event Action TitleDataChanged;
    [SerializeField] private CommonParam _commonParam;

    private bool _isGameStart;
    private UiHierarchy _currentHierarchy;

    private ContentCombo _selectedContentCombo;
    private ContentCombo _selectedDifficultyCombo;
    private Dictionary<ContentCombo, SettingContents> _titleContents;
    private Dictionary<ContentCombo, Difficulty> _difficultyContents;

    public bool IsGameStart { get => _isGameStart; set => _isGameStart = value; }
    
    public UiHierarchy CurrentHierarchy { get => _currentHierarchy; set => _currentHierarchy = value; }


    private void Start()
    {
        initButton();

        _currentHierarchy = UiHierarchy.Route;
        _commonParam.GameDifficulty = Difficulty.Normal;
    }

    private void Update()
    {

    }

    private void initButton()
    {
        Button startButton = GameObject.Find("GameStartButton").GetComponent<Button>();
        Button difficultyButton = GameObject.Find("DifficultySettingButton").GetComponent<Button>();

        Button easyButton = GameObject.Find("EasyButton").GetComponent<Button>();
        Button normalButton = GameObject.Find("NormalButton").GetComponent<Button>();
        Button difficultButton = GameObject.Find("DifficultButton").GetComponent<Button>();

        ContentCombo startCombo = CreateContentCombo(startButton, UiHierarchy.StartGame, UiHierarchy.Route);
        ContentCombo difficultyCombo = CreateContentCombo(difficultyButton, UiHierarchy.Difficulty, UiHierarchy.Route);
        ContentCombo easyCombo = CreateContentCombo(easyButton, UiHierarchy.Route, UiHierarchy.Route);
        ContentCombo normalCombo = CreateContentCombo(normalButton, UiHierarchy.Route, UiHierarchy.Route);
        ContentCombo difficultCombo = CreateContentCombo(difficultButton, UiHierarchy.Route, UiHierarchy.Route);

        SetContentComboNeghbor(startCombo, startCombo, difficultyCombo, startCombo, startCombo);
        SetContentComboNeghbor(difficultyCombo, startCombo, difficultyCombo, difficultyCombo, difficultyCombo);

        SetContentComboNeghbor(easyCombo, easyCombo, normalCombo, easyCombo, easyCombo);
        SetContentComboNeghbor(normalCombo, easyCombo, difficultCombo, normalCombo, normalCombo);
        SetContentComboNeghbor(difficultCombo, normalCombo, difficultCombo, difficultCombo, difficultCombo);

        _titleContents = new Dictionary<ContentCombo, SettingContents>()
        {
            {startCombo, SettingContents.GameStart},
            {difficultyCombo, SettingContents.Difficulty}
        };
        _difficultyContents = new Dictionary<ContentCombo, Difficulty>()
        {
            {easyCombo, Difficulty.Easy},
            {normalCombo, Difficulty.Normal},
            {difficultCombo, Difficulty.Difficult}
        };
        _selectedContentCombo = startCombo;
        _selectedDifficultyCombo = normalCombo;
    }

    private ContentCombo CreateContentCombo(Button target, UiHierarchy enter, UiHierarchy back)
    {
        ContentCombo retCombo = new ContentCombo
        {
            ContentButton = target,
            NeighborHierarchy = new Dictionary<InputSelect, UiHierarchy>() 
            {
                {InputSelect.Enter, enter},
                {InputSelect.Back, back}
            }
        };

        return retCombo;
    }

    private void SetContentComboNeghbor(ContentCombo target, ContentCombo upper, ContentCombo down, ContentCombo left, ContentCombo right)
    {
        target.Neighbor = new Dictionary<InputDirection, ContentCombo>()
        {
            {InputDirection.Upper, upper},
            {InputDirection.Down, down},
            {InputDirection.Left, left},
            {InputDirection.Right, right}
        };
    }

    public ContentCombo GetCurrentSelectedCombo()
    {
        ContentCombo ret;

        switch (_currentHierarchy)
        {
            case UiHierarchy.Route:
                ret = _selectedContentCombo;
                break;
            case UiHierarchy.Difficulty:
                ret = _selectedDifficultyCombo;
                break;
            default:
                ret = null;
                break;
        }

        return ret;
    }

    public void ReceiveInputHierarchy(InputSelect select)
    {
        switch (_currentHierarchy)
        {
            case UiHierarchy.Route:
                if (_selectedContentCombo != null)
                {
                    if (_selectedContentCombo.NeighborHierarchy.ContainsKey(select))
                    {
                        _currentHierarchy = _selectedContentCombo.NeighborHierarchy[select];
                    }
                }
                break;
            case UiHierarchy.Difficulty:
                if (_selectedDifficultyCombo != null)
                {
                    if (_selectedDifficultyCombo.NeighborHierarchy.ContainsKey(select))
                    {
                        _currentHierarchy = _selectedDifficultyCombo.NeighborHierarchy[select];
                    }
                }
                break;
            default:
                break;
        }
        UpdateData();
    }

    /// <summary>
    /// ���͂��ꂽ�����ɍ����A�I�𒆂̃R���e���c��ύX
    /// </summary>
    /// <param name="inputDirection"></param>
    public void ReceiveInputDirection(InputDirection inputDirection)
    {
        switch (_currentHierarchy)
        {
            case UiHierarchy.Route:
                if (_selectedContentCombo != null)
                {
                    if (_selectedContentCombo.Neighbor.ContainsKey(inputDirection))
                    {
                        _selectedContentCombo = _selectedContentCombo.Neighbor[inputDirection];
                    }
                }
                break;
            case UiHierarchy.Difficulty:
                if (_selectedDifficultyCombo != null)
                {
                    if (_selectedDifficultyCombo.Neighbor.ContainsKey(inputDirection))
                    {
                        _selectedDifficultyCombo = _selectedDifficultyCombo.Neighbor[inputDirection];
                        _commonParam.GameDifficulty = _difficultyContents[_selectedDifficultyCombo]; /* ���ݑI�𒆂̓�Փx��ݒ� */
                    }
                }
                break;
            default:
                break;
        }
        UpdateData();
    }

    public void UpdateData()
    {
        TitleDataChanged.Invoke();
    }
}
