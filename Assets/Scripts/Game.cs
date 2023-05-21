using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RVEmuSharp;
using UnityEditor;

enum GameState
{
    Typing,
    Running,
    Finished,
}

public enum ActionCode
{
    Move,
    Turn,
    Otherwise
}

public class Game : MonoBehaviour
{
    public Button RunButton;
    public GameObject player;
    public GameObject AsmEditor;

    public Camera camera;
    private GameState _state;
    private float _runTime;
    private int ActionDur = 1;
    private PlayerController _playerController;
    private TextMeshProUGUI asmEditorText;


    // Start is called before the first frame update
    void Start()
    {
        asmEditorText = AsmEditor.GetComponent<TextMeshProUGUI>();
        Debug.Log(asmEditorText);
        _playerController = player.GetComponent<PlayerController>();
        RunButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            var asm_text = asmEditorText.text;
            var machine_code = RVAssembler.Assemble(asm_text);
            var emulator = new RVEmulator();
            emulator.LoadProgram(machine_code);
            

            
            _playerController.Forward();
        });

    }

    void Run(int code = 0)
    {
        var regs = new int[32];
        regs[17] = code;
        _runTime += Time.deltaTime;
        if (_runTime > ActionDur)
        {
            _runTime -= ActionDur;
            var action = new Move();
            action.Code = regs[10] switch
            {
                0 => honk,
                1 => move forward
                2 => turn
            };
            action.Param = regs[11];
            _playerController.Actions.Add(action);
        }

    }
}
