using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
}

public class Game : MonoBehaviour
{
    public Button RunButton;
    public GameObject player;
    public Camera camera;
    private GameState _state;
    private float _runTime;
    private int ActionDur = 1;
    private PlayerController _playerController;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = player.GetComponent<PlayerController>();
        RunButton.GetComponent<Button>().onClick.AddListener(() =>
        {
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
            var action = new Action();
            action.Code = regs[10];
            action.Param = regs[11];
            _playerController.Actions.Add(action);
        }

    }
}
