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

enum ActionCode
{
    Move,
    Turn,
}

public class Game : MonoBehaviour
{
    public Button RunButton;
    public GameObject player;
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
            StartCoroutine(_playerController.Forward());
        });

    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case GameState.Typing:
                break;
            case GameState.Running:
                Run();
                break;
        }
    }

    void Run(int code = 0)
    {
        var regs = new int[32];
        regs[17] = code;
        _runTime += Time.deltaTime;
        if (_runTime > ActionDur)
        {
            _runTime -= ActionDur;
            switch ((ActionCode)regs[17])
            {
                case ActionCode.Move:
                    // _playerController.Forward();
                    break;
                case ActionCode.Turn:
                    // _playerController.Turn(regs[10]);
                    break;
            }
        }

    }
}
