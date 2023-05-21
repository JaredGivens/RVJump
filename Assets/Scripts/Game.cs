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

public enum MoveCode
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
    private PlayerController _playerController;
    private TMP_InputField asmEditorText;


    // Start is called before the first frame update
    void Start()
    {
        asmEditorText = AsmEditor.GetComponent<TMP_InputField>();
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

                    var move = new Move
                    {
                        Code = emulator.GetRegister(10) switch
                        {
                            1 => MoveCode.Move,
                            2 => MoveCode.Turn,
                            _ => MoveCode.Honk
                        },
                        Param = (int)emulator.GetRegister(11),
                    };
                    _playerController.Moves.Enqueue(move);
                }
                break;
            }
        });
    }
}
