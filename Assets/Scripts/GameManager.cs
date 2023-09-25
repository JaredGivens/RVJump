using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

using RVEmuSharp;


public enum GameState
{
    Typing,
    Paused,
    Running,
}

public enum MoveCode
{
    Honk,
    Move,
    Turn,
    Idle,
    Reset,
}

public class GameManager : MonoBehaviour
{
    [Header("Game")]
    public GameState CurrentGameState = GameState.Typing;
    public GameObject PlayerPrefab;

    [Header("Level data")]
    public string CurrentLevelName = "";
    public string NextLevelName = "";
    public Transform SpawnPoint;
    public GameObject FinishLineObject;
    public GameObject OutOfBoundsObject;
    private GameObject _activePlayerObject;

    // Editor buttons
    [Header("Main GUI")]
    public Button RunButton;
    public Button PauseButton;
    public Button RewindButton;
    public GameObject AsmEditorObject;

    // Finish prompt and buttons
    [Header("Finish GUI")]
    public GameObject FinishedPrompt;
    public Button ContinueButton;
    public Button StayButton;
    public GameObject ContinuePrompt;
    public Button ContunueButton2; // bad name

    // Core componenets
    private PlayerController _playerController;
    private AsmEditor _asmEditor;

    private void OnEnable() {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;

        // Get editor components
        _asmEditor = AsmEditorObject.GetComponent<AsmEditor>();
        FinishLineObject.GetComponent<FinishLine>().OnPlayer = LevelFinishedEvent;
        OutOfBoundsObject.GetComponent<OutOfBounds>().OnPlayer = ResetPlayer; // TODO: TEMP

        // Add event to GUI buttons
        RunButton.onClick.AddListener(() => {
            RunEmulation();
        });

        PauseButton.onClick.AddListener(() => {
            PauseGame();
        });

        RewindButton.onClick.AddListener(() => {
            RewindToInitialState();
        });

        StayButton.onClick.AddListener(() => {
            StayOnLevel();
        });

        ContinueButton.onClick.AddListener(() => {
            ContinueToNextLevel();
        });

        ContunueButton2.onClick.AddListener(() => {
            ContinueToNextLevel();
        });

        SpawnPlayer();
    }

    private void OnDisable() {
        // Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled.
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        // Display level names
        Debug.Log(scene.name);
        CurrentLevelName = scene.name;
        // Debug.Log(mode);
    }

    public void LevelFinishedEvent() {
        FinishedPrompt.SetActive(true);
    }

    public void StayOnLevel() {
        FinishedPrompt.SetActive(false);
        ContinuePrompt.SetActive(true);
    }

    public void ContinueToNextLevel() {
        SceneManager.LoadScene(NextLevelName);
    }

    // Creates the player at the levels spawn point
    public void SpawnPlayer() {
        _activePlayerObject = Instantiate(PlayerPrefab, SpawnPoint.position, SpawnPoint.rotation);
        _playerController = _activePlayerObject.GetComponent<PlayerController>();
    }

    public void ResetPlayer() {
        // confused on when we would ever need this
        Queue<Move> moveQueue = new Queue<Move>();
        moveQueue.Enqueue(new Move {
            Code = MoveCode.Reset,
            Param = 0,
        });

        _playerController.RunMoves(moveQueue, () => {
            _playerController.MoveQueue = null;
        });
        CurrentGameState = GameState.Typing;
    }


    private void RunEmulation() {
        if (CurrentGameState == GameState.Running) {
            return;
        }

        // No code in the editor, prevent crashing
        if (string.IsNullOrEmpty(_asmEditor.AsmText)) {
            return;
        }

        // Convert to machine code
        ulong errorline = 0;
        byte[] machineCode = RVAssembler.Assemble(_asmEditor.AsmText, ref errorline);
        if (errorline > 0) {
            // TODO: Highlight the error line (preferably the entire row)
            Debug.Log($"erorLine:{errorline}");
            return;
        }

        // Run emulator
        int instructionCount = machineCode.Length / 4;
        RVEmulator emulator = new RVEmulator();
        emulator.LoadProgram(machineCode);
        Queue<Move> moveQueue = new Queue<Move>();
        for (int i = 0; i < instructionCount; i++) {
            uint instruction = emulator.RunOnce();
            if (instruction == 0x00000073) {
                Move moveInstruction = new Move {
                    Code = emulator.GetRegister(10) switch {
                        1 => MoveCode.Move,
                        2 => MoveCode.Turn,
                        _ => MoveCode.Honk
                    },
                    Param = (int)emulator.GetRegister(11),
                };
                if (moveInstruction.Code == MoveCode.Turn) {
                    Debug.Log("im turning");
                }
                moveQueue.Enqueue(moveInstruction);
            }
        }

        CurrentGameState = GameState.Running;
        _playerController.RunMoves(moveQueue, ResetPlayer);
    }

    private void PauseGame() {
        // Nothing to pause
        if (CurrentGameState == GameState.Running) {
            return;
        }
    }

    private void RewindToInitialState() {
        if (CurrentGameState == GameState.Typing) {
            return;
        }

        // Play visual screen effect

        // Delete current player
        Destroy(_activePlayerObject);

        // Rewind all objects to their initial states (they should prob subscribe to this event)

        SpawnPlayer();
        CurrentGameState = GameState.Typing;
    }
}