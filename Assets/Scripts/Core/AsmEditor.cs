
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using UnityEngine;

public class AsmEditor : MonoBehaviour
{
    [System.NonSerialized]
    public string AsmText;
    private TMP_InputField _editorInputField;
    private bool alreadyHighlighted = false;
    private bool alreadyReinstated = true;

    private void Start()
    {
        AsmText = "";
        _editorInputField = GetComponent<TMP_InputField>();
    }

    private void Update()
    {
        if (_editorInputField.isFocused == false)
        {
            if (!alreadyHighlighted) {
                HighlightText();
            }
            alreadyHighlighted = true;
            alreadyReinstated = false;
        }
        else
        {
            if (!alreadyReinstated) {
                _editorInputField.text = AsmText;
            }
            AsmText = _editorInputField.text;
            alreadyHighlighted = false;
            alreadyReinstated = true;
        }
    }

    /*
     *   anything with a letter -> register,
     *   first word of each line -> instruction
     *   numbers -> their own color
    */
    private void HighlightText()
    {
        string highlightedText = AsmText;
        string[] lines = highlightedText.Split(System.Environment.NewLine);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(" ");
            line[0] = "<color #880808>" + line[0] + "</color>";
            for (int word_index = 1; word_index < line.Length; word_index++)
            {
                string word = line[word_index];
                if (Regex.Matches(line[word_index], @"[a-zA-Z]").Count != 0)
                {
                    // Debug.Log(word[word.Length - 1].CompareTo(','));
                    if (word[word.Length - 1].CompareTo(',') == 0)
                    {
                        line[word_index] = "<color #00FF00>" + word.Substring(0, word.Length - 1) + "</color>" + ",";
                    }
                    else
                    {
                        line[word_index] = "<color #00FF00>" + line[word_index] + "</color>";

                    }
                }
            }
            lines[i] = System.String.Join(' ', line);
        }

        _editorInputField.text = System.String.Join(System.Environment.NewLine, lines);
    }
}

