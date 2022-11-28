using TMPro;
using UnityEngine;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gameOverText;

    private LetterDisplayRow _answerDisplay;
    
    private void OnEnable()
    {
        _answerDisplay = GetComponentInChildren<LetterDisplayRow>();
        LeanTween.scale(gameObject, Vector3.one, .8f).setFrom(Vector3.zero).setEaseOutCubic();
        LeanTween.rotateAround(gameObject, Vector3.forward, 0, .8f).setFrom(-80).setEaseOutCubic();
    }

    public void SetGameOverText(bool won, int guesses)
    {
        if (won)
        {
            _gameOverText.text = "Congratulations you won in " + guesses + (guesses == 1 ? " guess" : " guesses");
            _answerDisplay.WinAnimation();
        }
        else
        {
            _gameOverText.text = "Too bad. Try again?";
        }
    }

    public void SetCorrectAnswerText(string answer)
    {
        _answerDisplay.SetWord(answer.ToUpper());
    }

    public void RestartGame()
    {
        _answerDisplay.ResetValues();
        GameManager.Instance.Restart();
        gameObject.SetActive(false);
    }
}
