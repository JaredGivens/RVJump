using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MMFeedbacks ForwardFeedback;
    public MMFeedbacks TurnFeedback;
    public float ForwardDuration = 1;
    public float TurnDuration = 1;
    public float Jump = 2;
    public MMTween.MMTweenCurve ForwardHorCurve;
    public MMTween.MMTweenCurve ForwardVertCurve;
    public MMTween.MMTweenCurve TurnHorCurve;
    public MMTween.MMTweenCurve TurnVertCurve;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Forward() {
        // transform.position += transform.forward * 2;
        ForwardFeedback?.PlayFeedbacks();
        MMTween.MoveTransform(this, transform, transform.position, transform.position + transform.forward * 2, null, 0, ForwardDuration, ForwardHorCurve);
        // MMTween.MoveTransform(this, transform, transform.position, transform.position + Vector3.up * Jump, null, 0, ForwardDuration, ForwardVertCurve);
		yield return MMCoroutine.WaitFor(ForwardDuration);
    }

    public IEnumerator Turn(int dir) {
        // transform.Rotate (new Vector3 (0, 0, 90));    
        TurnFeedback?.PlayFeedbacks();
        Transform rotateTransform = transform;
        rotateTransform.Rotate(0, 90, 0, Space.World);
        MMTween.MoveTransform(this, transform, transform, rotateTransform, null, 0, TurnDuration, TurnHorCurve);
        // Transform moveTransform = transform;
        // moveTransform.Translate(Vector3.up * Jump);
        // MMTween.MoveTransform(this, transform, transform.position, transform.position, null, 0, TurnDuration, TurnVertCurve);
		yield return MMCoroutine.WaitFor(TurnDuration);
    }
}