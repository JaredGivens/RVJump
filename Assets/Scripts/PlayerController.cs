using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class Move {
    public MoveCode Code;
    public int Param;
}

public class PlayerController : MonoBehaviour
{
    public float MoveDuration = 1;
    public float Distance = 6;
    private float _yVel = 0;
    private float _moveTime = 0;
    private int _turnDirection = 0;
    private Move _currentMove;
    public float JumpForce = 1;
    private Rigidbody _rigidbody;
    public Queue<Move> Moves;
    public MMFeedbacks IdleFeedbacks;
    public List<MMFeedbacks> FeedbackMat;
    private System.Action _onFinish;
    private int _level_number = 1;
    public bool Finished = false;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Moves != null) {
            switch(_currentMove.Code) {
                case MoveCode.Move:{
                    float dt = Math.Min(_moveTime, Time.deltaTime);
                    transform.position += transform.forward * 
                        Distance  / MoveDuration * dt;
                    break;
                }
                case MoveCode.Turn: {
                    float dt = Math.Min(_moveTime, Time.deltaTime);
                    transform.rotation = Quaternion.AngleAxis( dt * _turnDirection * 90 / MoveDuration, Vector3.up) * transform.rotation;
                    break;
                }
                default:
                    break;

            }
        }
        _moveTime -= Time.deltaTime;
        if (_moveTime < 0) {
            _moveTime += MoveDuration;
            if(Moves != null) {
                if (Moves.Count == 0) {
                    if (Finished) {
                         SceneManager.LoadScene("Level" + ++_level_number, LoadSceneMode.Additive);
                    }
                    return;
                }
                _currentMove = Moves.Dequeue();
                _rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
                if(FeedbackMat.Count > (int)_currentMove.Code) {
                    FeedbackMat[(int)_currentMove.Code]?.PlayFeedbacks();
                }
                if (_currentMove.Code == MoveCode.Reset) {
                    transform.position = Vector3.zero;
                    transform.rotation = Quaternion.identity;
                }
            } else {
                _currentMove = null;
                IdleFeedbacks?.PlayFeedbacks();
            }
        }

    }
    public void RunMoves(Queue<Move> moves, System.Action onFinish) {
        _onFinish = onFinish;
        Moves = moves;
    }
}