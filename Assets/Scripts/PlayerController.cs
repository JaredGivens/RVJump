using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public float AnimationDuration = 1;
    public float Distance = 6;
    private float _yVel = 0;
    private float _forwardTime = 0;
    private float _turnTime = 0;
    private int _turnDirection = 0;
    public float JumpForce = 1;
    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_forwardTime != 0) {
            float dt = Math.Min(_forwardTime, Time.deltaTime);
            transform.position += transform.forward *
                Distance  / AnimationDuration * dt;
            _forwardTime -= dt;
        }
        if (_turnTime > 0) {
            float dt = Math.Min(_turnTime, Time.deltaTime);
            transform.rotation = Quaternion.AngleAxis( dt * _turnDirection * 90 / AnimationDuration, Vector3.up) * transform.rotation;
            _turnTime -= dt;
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