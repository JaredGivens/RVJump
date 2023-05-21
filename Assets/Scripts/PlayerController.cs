using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public struct Move {
    public ActionCode Code;
    public int Param;
}

public class PlayerController : MonoBehaviour
{
    public float ActionDuration = 1;
    public float Distance = 6;
    private float _yVel = 0;
    private float _forwardTime = 0;
    private float _actionTime = 0;
    private int _turnDirection = 0;
    public float JumpForce = 1;
    private Rigidbody _rigidbody;
    public List<Move> Moves;
    public Move _currentAciton;
    public List<MMFeedback> feedbacks;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_currentAciton) {
            switch(_currentAction.Code) {
                case ActionCode.Move:
                    float dt = Math.Min(_actionTime, Time.deltaTime);
                    transform.position += transform.forward *
                        Distance  / AnimationDuration * dt;
                    break;
                case ActionCode.Turn:
                    float dt = Math.Min(_actionTime, Time.deltaTime);
                    transform.rotation = Quaternion.AngleAxis( dt * _turnDirection * 90 / AnimationDuration, Vector3.up) * transform.rotation;
                    break;
                default:
                    break;
            }
        }
        _actionTime -= dt;
        if (_actionTime < 0) {
            _actionTime += ActionDuration;
            if(_currentAciton) {
                switch (_currentAciton.Code) {
                    
                }
            }
        }
    }

    public void Forward() {
        _forwardTime = AnimationDuration;
        _rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }

    public void Turn(int dir) {
        _turnTime = AnimationDuration;
        _turnDirection = dir;
        _rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }
}