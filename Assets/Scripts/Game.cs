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
    Honk
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
            while (true)
            {
                var instr = emulator.RunOnce();
                if (instr == 0x0)
                {
                    Debug.Log("Ran last instruction");
                    break;
                }
                else if (instr == 0x00000073)
                {
                    Debug.Log("Ran ecall");
                    var regs = new int[32];

                    _runTime -= ActionDur;
                    var action = new Move
                    {
                        Code = regs[10] switch
                        {
                            1 => ActionCode.Move,
                            2 => ActionCode.Turn,
                            _ => ActionCode.Honk
                        },
                        Param = regs[11]
                    };
                    _playerController.Moves.Add(action);
                }
            }
        });
    }
}
