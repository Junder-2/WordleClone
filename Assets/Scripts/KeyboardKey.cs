using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardKey : MonoBehaviour
{
    public KeyCode key;

    private TextMeshProUGUI _text;
    private Image _image;
    private Button _button;
    private InputManager _inputManager;

    private void Start()
    {
        _inputManager = InputManager.Instance;

        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
        
        _button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        _inputManager.PressKey(key);
    }

    private void OnValidate()
    {
        UpdateTextDisplay();
    }

    void UpdateTextDisplay()
    {
        if (_text == null)
            _text = GetComponentInChildren<TextMeshProUGUI>();

        switch (key)
        {
            case KeyCode.Backspace:
                _text.text = "<";
                break;
            case KeyCode.Return:
                _text.text = "=";
                break;
            default:
                _text.text = key.ToString();
                break;
        }
        
        
        _text.color = GameConstants.PrimaryDarkColor;
        
        if(_image == null)
            _image = GetComponent<Image>();

        _image.color = GameConstants.PrimaryLightColor;
    }

    private GameConstants.LetterCloseness _currentCloseness;
    private bool _deactivated = false; 
    public void SetCloseness(GameConstants.LetterCloseness closeness)
    {
        if(_currentCloseness == GameConstants.LetterCloseness.Correct) return;
        if(_currentCloseness == GameConstants.LetterCloseness.Close && closeness == GameConstants.LetterCloseness.Wrong) return;
        
        _image.color = closeness == GameConstants.LetterCloseness.Wrong ? GameConstants.WrongColor :
            closeness == GameConstants.LetterCloseness.Close ? GameConstants.CloseColor : GameConstants.CorrectColor;
        
        if(_currentCloseness == GameConstants.LetterCloseness.Close) return;

        if (closeness == GameConstants.LetterCloseness.Wrong && !_deactivated)
        {
            GetComponent<ButtonTween>().Deactivate();
            _deactivated = true;
        }
        
        _currentCloseness = closeness;
    }

    public void ResetCloseness()
    {
        _deactivated = false;
        _currentCloseness = GameConstants.LetterCloseness.Wrong;
        _image.color = GameConstants.PrimaryLightColor;
        GetComponent<ButtonTween>().ResetState();
    }
}
