
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using UnityEngine;

public class CodeInputScript : MonoBehaviour
{
    string uncolored;
    public GameObject mainInputField;
    bool alreadyHighlighted = false;
    bool alreadyReinstated = true;

    // Use this for initialization
    void Start()
    {
        uncolored = "";
    }

    // Update is called once per frame

    void Update()
    {
        if (mainInputField.GetComponent<TMP_InputField>().isFocused == false)
        {
            if (!alreadyHighlighted)
                HighlightText();
            alreadyHighlighted = true;
            alreadyReinstated = false;
        }
        else
        {
            if (!alreadyReinstated)
                mainInputField.GetComponent<TMP_InputField>().text = uncolored;
            uncolored = mainInputField.GetComponent<TMP_InputField>().text;
            alreadyHighlighted = false;
            alreadyReinstated = true;

        }
    }

    /* 
     *   anything with a letter -> register,
     *   first word of each line -> instruction
     *   numbers -> their own color
    */
    void HighlightText()
    {
        string highlighted = uncolored;

        string[] lines = highlighted.Split(System.Environment.NewLine);

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

        mainInputField.GetComponent<TMP_InputField>().text = System.String.Join(System.Environment.NewLine, lines);
    }
}

