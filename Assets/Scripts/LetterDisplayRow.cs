using System.Collections;
using UnityEngine;

public class LetterDisplayRow : MonoBehaviour
{
    private LetterDisplay[] _displayLetters;

    private int _displayIndex;

    private void Awake()
    {
        _displayLetters = GetComponentsInChildren<LetterDisplay>();

        foreach (var display in _displayLetters)
        {
            display.SetLetter('0');
        }

        _displayIndex = 0;
    }

    public void ResetValues()
    {
        foreach (var display in _displayLetters)
        {
            display.SetLetter('0');
            LeanTween.cancel(display.gameObject);
            LeanTween.scale(display.gameObject, Vector3.one, 0.1f);
            LeanTween.rotate(display.gameObject, Vector3.zero, .1f);
        }

        _displayIndex = 0;
    }

    public void SetWord(string word)
    {
        char[] wordLetters = word.ToCharArray();

        for (int i = 0; i < _displayLetters.Length; i++)
        {
            _displayLetters[i].SetLetter(wordLetters[i]);
        }
    }

    public void SetLetter(char letter)
    {
        if(_displayLetters[^1].GetLetter() != '0') return;
        
        _displayLetters[_displayIndex].SetLetter(letter);
        _displayIndex++;
        _displayIndex = Mathf.Clamp(_displayIndex, 0, _displayLetters.Length-1);
    }

    public void RemoveLetter()
    {
        if (_displayLetters[_displayIndex].GetLetter() == '0')
            _displayIndex--;
        
        _displayIndex = Mathf.Clamp(_displayIndex, 0, _displayLetters.Length-1);
        
        _displayLetters[_displayIndex].SetLetter('0');
    }

    public bool CanCheck()
    {
        return _displayLetters[^1].GetLetter() != '0';
    }

    public string GetWord()
    {
        string inputWord = "";

        foreach (var letterDisplay in _displayLetters)
        {
            inputWord += letterDisplay.GetLetter();
        }

        return inputWord;
    }

    public void NotWordAnimation()
    {
        if (LeanTween.isTweening(_displayLetters[0].gameObject)) return;
        foreach (var letterDisplay in _displayLetters)
        {
            letterDisplay.transform.eulerAngles = Vector3.zero;
            LeanTween.cancel(letterDisplay.gameObject);
            LeanTween.rotateZ(letterDisplay.gameObject, 15f, .1f).setEaseInQuad();
            LeanTween.rotateZ(letterDisplay.gameObject, -15f, .1f).setDelay(.1f).setEaseInQuad().setLoopPingPong(3);
            LeanTween.rotateZ(letterDisplay.gameObject, 0, .1f).setDelay(.1f * 7f).setEaseOutQuad();
        }
    }

    public void WinAnimation()
    {
        foreach (var letterDisplay in _displayLetters)
        {
            letterDisplay.SetCloseness(GameConstants.LetterCloseness.Correct);
            LeanTween.cancel(letterDisplay.gameObject);

            LeanTween.scale(letterDisplay.gameObject, Vector3.one * 1.1f, .5f).setEaseInOutQuad().setLoopPingPong();
            LeanTween.rotateAroundLocal(letterDisplay.gameObject, Vector3.forward, 360, 1f).setFrom(0).setEaseOutQuart().setLoopCount(-1);
        }
    }

    public void GuessCloseness(GameConstants.LetterCloseness[] closeness, bool lastRow = false)
    {
        StartCoroutine(GuessAnimation(closeness, lastRow));
    }

    IEnumerator GuessAnimation(GameConstants.LetterCloseness[] closeness, bool lastRow)
    {
        const float transitionsTime = .25f;
        bool winCheck = true;
        for (int i = 0; i < _displayLetters.Length; i++)
        {
            if (winCheck && closeness[i] != GameConstants.LetterCloseness.Correct) winCheck = false;
            float finalScale = closeness[i] == GameConstants.LetterCloseness.Wrong ? .9f : 1f;
            LeanTween.scaleX(_displayLetters[i].gameObject, 0, transitionsTime).setEaseInCubic();
            LeanTween.scaleY(_displayLetters[i].gameObject, 1.2f, transitionsTime).setEaseOutQuad();
            yield return new WaitForSeconds(transitionsTime);
            LeanTween.scaleX(_displayLetters[i].gameObject, finalScale, transitionsTime).setEaseOutCubic();
            LeanTween.scaleY(_displayLetters[i].gameObject, finalScale, transitionsTime).setEaseInQuad();
            _displayLetters[i].SetCloseness(closeness[i]);
        }

        yield return new WaitForSeconds(.5f);
        yield return new WaitForEndOfFrame();
        
        if(winCheck)
            GameManager.Instance.GameOver(true);

        else if(lastRow)
            GameManager.Instance.GameOver(false);
    }
}
