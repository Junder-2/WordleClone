using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private WinPanel _winPanel;

    public static GameManager Instance;
    
    private LetterDisplayRow[] _letterDisplayRows;
    private int _rowIndex;

    public string Answer;

    private Dictionary<char, KeyboardKey> _keyboardKeys = new Dictionary<char, KeyboardKey>();

    private HashSet<string> _availableWords = new HashSet<string>();
    private HashSet<string> _alreadyGuessed = new HashSet<string>();

    private bool _blockInput;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        _availableWords = GameConstants.GetWordList();

        Answer = _availableWords.ElementAt(Random.Range(0, _availableWords.Count));

        _rowIndex = 0;

        _letterDisplayRows = FindObjectsOfType<LetterDisplayRow>().OrderBy(row => row.transform.GetSiblingIndex()).ToArray();

        KeyboardKey[] tempKeyboardKeys = FindObjectsOfType<KeyboardKey>();

        foreach (var keyDisplay in tempKeyboardKeys)
        {
            string keyName = keyDisplay.key.ToString().ToLower();
            if(keyName.Length != 1) continue;
            
            _keyboardKeys.Add(keyName[0], keyDisplay);
        }
    }

    public void AddLetter(char letter)
    {
        if(_blockInput) return;
        _letterDisplayRows[_rowIndex].SetLetter(letter);        
    }

    public void RemoveLetter()
    {
        if(_blockInput) return;
        _letterDisplayRows[_rowIndex].RemoveLetter();
    }

    public void CheckRow()
    {
        if(_blockInput) return;
        
        if(!_letterDisplayRows[_rowIndex].CanCheck())
        {
            _letterDisplayRows[_rowIndex].NotWordAnimation();
            return;
        }

        string guess = _letterDisplayRows[_rowIndex].GetWord().ToLower();

        if(!_availableWords.Contains(guess) || _alreadyGuessed.Contains(guess))
        {
            _letterDisplayRows[_rowIndex].NotWordAnimation();
            return;
        }
        _alreadyGuessed.Add(guess);
        
        CheckWord(guess);
        _rowIndex++;

        if (_rowIndex == _letterDisplayRows.Length)
        {
            _blockInput = true;
        }
    }

    void CheckWord(string guess)
    {
        char[] answerChars = Answer.ToCharArray();
        char[] guessChars = guess.ToCharArray();
        
        GameConstants.LetterCloseness[] closeness = new GameConstants.LetterCloseness[answerChars.Length];
        HashSet<int> usedIndex = new HashSet<int>();

        for (int guessIndex = 0; guessIndex < guessChars.Length; guessIndex++)
        {
            KeyboardKey key;
            _keyboardKeys.TryGetValue(guessChars[guessIndex], out key);
            
            if (guessChars[guessIndex] == answerChars[guessIndex])
            {
                closeness[guessIndex] = GameConstants.LetterCloseness.Correct;
                usedIndex.Add(guessIndex);
            }
        }

        for (int guessIndex = 0; guessIndex < guessChars.Length; guessIndex++)
        {
            KeyboardKey key;
            _keyboardKeys.TryGetValue(guessChars[guessIndex], out key);
            
            for (var answerIndex = 0; answerIndex < answerChars.Length; answerIndex++)
            {
                var answerChar = answerChars[answerIndex];
                if (guessChars[guessIndex] != answerChar || usedIndex.Contains(answerIndex) ||
                    closeness[guessIndex] == GameConstants.LetterCloseness.Correct) continue;

                usedIndex.Add(answerIndex);
                closeness[guessIndex] = GameConstants.LetterCloseness.Close;
                break;
            }

            if (key != null) key.SetCloseness(closeness[guessIndex]);
        }
        
        _letterDisplayRows[_rowIndex].GuessCloseness(closeness, _rowIndex == _letterDisplayRows.Length-1);
    }
    
    public void GameOver(bool won)
    {
        _blockInput = true;
        _winPanel.gameObject.SetActive(true);
        _winPanel.SetCorrectAnswerText(Answer);
        _winPanel.SetGameOverText(won, _rowIndex);
    }

    public void Restart()
    {
        Answer = _availableWords.ElementAt(Random.Range(0, _availableWords.Count));

        _rowIndex = 0;

        foreach (var row in _letterDisplayRows)
        {
            row.ResetValues();
        }

        foreach (var key in _keyboardKeys.Values)
        {
            key.ResetCloseness();
        }

        _blockInput = false;
    }
}
