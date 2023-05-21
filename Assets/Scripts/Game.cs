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
    Honk,
    Move,
    Turn,
    Idle,
    Reset,
}

public class Game : MonoBehaviour
{
    public Button RunButton;
    public GameObject player;
    public GameObject AsmEditor;
    public GameObject OOB;

    private GameState _state = GameState.Typing;
    private PlayerController _playerController;
    private TMP_InputField asmEditorText;

    // Start is called before the first frame update
    void Start()
    {
        OOB.GetComponent<OOB>().OnPlayer = Reset;

        asmEditorText = AsmEditor.GetComponent<TMP_InputField>();
        _playerController = player.GetComponent<PlayerController>();
        Debug.Log(asmEditorText);
        RunButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if(_state == GameState.Running) {
                return;
            }
            _state = GameState.Running;
            var asm_text = asmEditorText.text;
            Debug.Log(asm_text);
            ulong errorline = 0;
            var machine_code = RVAssembler.Assemble(asm_text, ref errorline);
            Debug.Log($"erorLine:{errorline}");
            Debug.Log("hi");
            
            var emulator = new RVEmulator();
            Debug.Log("hi");

            emulator.LoadProgram(machine_code);
            var instruction_count = machine_code.Length/4;
            Debug.Log("en");
            Debug.Log(BitConverter.ToUInt32(machine_code, 0));
            Debug.Log(BitConverter.ToUInt32(machine_code, 4));
            Debug.Log(BitConverter.ToUInt32(machine_code, 8));
            Debug.Log(BitConverter.ToUInt32(machine_code, 12));
            Debug.Log(BitConverter.ToUInt32(machine_code, 16));
            var moves = new Queue<Move>();
            for(int i=0; i<instruction_count; i++)
            {
                var instr = emulator.RunOnce();
                if (instr == 0x00000073)
                {
                    Debug.Log("x10 " + emulator.GetRegister(10));
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
                    moves.Enqueue(move);
                }
            }
            _playerController.RunMoves(moves, Reset);
        });
    }

    void Reset() {
        var moves = new Queue<Move>();
        moves.Enqueue(new Move {
            Code = MoveCode.Reset,
            Param = 0,
        });
        _playerController.RunMoves(moves, () => {
            _playerController.Moves = null;
        });
        _state = GameState.Typing;
    }
}
