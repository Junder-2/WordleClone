using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private KeyCode[] _detectableKeys;
    private GameManager _gameManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
            
        Instance = this;

        List<KeyCode> tempKeyList = ((KeyCode[])System.Enum.GetValues(typeof(KeyCode))).Where(key => key.ToString().Length == 1).ToList();

        tempKeyList.Add(KeyCode.Return);
        tempKeyList.Add(KeyCode.Backspace);

        _detectableKeys = tempKeyList.ToArray();
    }

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (!Input.anyKeyDown) return;
        
        foreach (var keyCode in _detectableKeys)
        {
            if (!Input.GetKeyDown(keyCode)) continue;
            PressKey(keyCode);
            break;
        }
    }

    public void PressKey(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Return:
                _gameManager.CheckRow();
                break;
            case KeyCode.Backspace:
                _gameManager.RemoveLetter();
                break;
            default:
                _gameManager.AddLetter(keyCode.ToString()[0]);
                break;
        }
    }
}
