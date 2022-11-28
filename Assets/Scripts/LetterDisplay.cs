using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterDisplay : MonoBehaviour
{
    private char _letter;

    private Image _image;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _image = GetComponent<Image>();

        _letter = '0';
    }

    public void SetLetter(char letter)
    {
        _letter = letter;

        _text.text = _letter == '0' ? "" : _letter.ToString();

        _image.color = GameConstants.PrimaryLightColor;
    }

    public char GetLetter() => _letter;

    public void SetCloseness(GameConstants.LetterCloseness closeness)
    {
        _image.color = closeness == GameConstants.LetterCloseness.Correct ? GameConstants.CorrectColor :
            closeness == GameConstants.LetterCloseness.Close ? GameConstants.CloseColor : GameConstants.WrongColor;
    }
}
