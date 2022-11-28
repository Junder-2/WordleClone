using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameConstants
{
    public enum LetterCloseness{Wrong, Close, Correct}
    
    public static Color PrimaryLightColor = new Color32(0x94,0xBF,0xD9, 0xFF);
    public static Color PrimaryDarkColor = Color.black;

    public static Color WrongColor = new Color32(0x3F,0x51,0x5C, 0xFF);
    
    public static Color CloseColor = new Color32(0xE5,0xD5,0x2B,0xFF);
    
    public static Color CorrectColor = new Color32(0x20,0xE7,0x54,0xFF);

    public static HashSet<string> GetWordList()
    {
        HashSet<string> wordList = new HashSet<string>();
        
        var textAsset = Resources.Load<TextAsset>("WordList").text;

        wordList = textAsset.Split("\r\n").ToHashSet();

        return wordList;
    }
}
