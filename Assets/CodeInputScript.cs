using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

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
            mainInputField.GetComponent<Image>().color = Color.green;
        }
        else {
           if (!alreadyReinstated)
                mainInputField.GetComponent<TMP_InputField>().text = uncolored;
            uncolored = mainInputField.GetComponent<TMP_InputField>().text;
            mainInputField.GetComponent<Image>().color = Color.red;
            alreadyHighlighted = false;
            alreadyReinstated = true;

        }
        Debug.Log(mainInputField.GetComponent<TMP_InputField>().text);
    }

    void HighlightText() {
        string highlighted = uncolored;
        highlighted = highlighted.Replace("add", "<color #880808> add </color>");
        mainInputField.GetComponent<TMP_InputField>().text = highlighted;
    }
}

