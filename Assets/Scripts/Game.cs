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
    public GameObject OOB;

    public Camera _camera;
    private GameState _state;
    private PlayerController _playerController;
    private TMP_InputField asmEditorText;


    // Start is called before the first frame update
    void Start()
    {
        asmEditorText = AsmEditor.GetComponent<TMP_InputField>();
        _playerController = player.GetComponent<PlayerController>();
        Debug.Log(asmEditorText);
        RunButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("hi");
            var asm_text = asmEditorText.text;
            Debug.Log(asm_text);
            ulong errorline = 0;
            var machine_code = RVAssembler.Assemble(asm_text, ref errorline);
            Debug.Log($"erorLine:{errorline}");
            Debug.Log("hi");
            
            var emulator = new RVEmulator();
            Debug.Log("hi");
            Debug.Log(BitConverter.ToUInt32(machine_code, 0));
            Debug.Log(BitConverter.ToUInt32(machine_code, 4));

            emulator.LoadProgram(machine_code);
            var instruction_count = machine_code.Length/4;
            Debug.Log("Instruction count: " + instruction_count);
            for(int i=0; i<instruction_count; i++)
            {
            Debug.Log("running new instr");
                var instr = emulator.RunOnce();
            Debug.Log("after runonce");
                Debug.Log($"instr: {instr}");
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
            }
        });
    }
}
