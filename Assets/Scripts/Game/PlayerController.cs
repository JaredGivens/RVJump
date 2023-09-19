using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct Move {
    public MoveCode Code;
    public int Param;
}

public class PlayerController : MonoBehaviour
{
    [Header("Rigidbody Movement")]
    public float MoveDuration = 1;
    public float MoveDistance = 6;
    private float _moveTime = 0;
    public float JumpForce = 1;
    private Rigidbody _rigidbody;

    [Header("Instruction Movement")]
    public Queue<Move> MoveQueue;
    private Move _currentMove = new Move {
        Code = MoveCode.Idle,
        Param = 0,
    };
    private System.Action _onFinish;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (MoveQueue != null) {
            ProcessMove(_currentMove.Code);
        }

        _moveTime -= Time.deltaTime;
        if (_moveTime > 0) {
            return;
        }

        // wtf
        _moveTime += MoveDuration;
        if (MoveQueue != null) {
            if (MoveQueue.Count == 0) {
                _currentMove = IdleMove();
                return;
            }
            _currentMove = MoveQueue.Dequeue();
            Debug.Log(_currentMove.Code);
            _rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            if (_currentMove.Code == MoveCode.Reset) {
                // TODO: Set this to a spawn point (from game manager)
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
            }
        } else {
            _currentMove = IdleMove();
        }
    }

    public void Kill() {
        // Play explosion effect

        // Destroy game object after effect
    }

    public Move IdleMove() {
        return new Move {
            Code = MoveCode.Idle,
            Param = 0,
        };
    }

    public void RunMoves(Queue<Move> moveQueue, System.Action onFinish) {
        _onFinish = onFinish;
        MoveQueue = moveQueue;
    }

    private void ProcessMove(MoveCode moveCode) {
        float dt = Math.Min(_moveTime, Time.deltaTime);
        switch(moveCode) {
            case MoveCode.Move: {
                transform.position += transform.forward * MoveDistance  / MoveDuration * dt;
                break;
            }
            case MoveCode.Turn: {
                transform.rotation = Quaternion.AngleAxis( dt * _currentMove.Param * 90 / MoveDuration, Vector3.up) * transform.rotation;
                break;
            }
            default:
                break;
        }
    }
}