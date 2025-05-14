using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField]
    private Animator _controller;

    private static AnimationManager _instance;
    public static AnimationManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);
    }

    private enum GestureType { clap, fight, greeting, lookAt, run, sit, idle }

    public void SetAnimationType(string pred)
    {
        switch (pred.ToLower()) {
            case "dance":
                _controller.SetInteger("gesture", (int)GestureType.clap);
                break;
            case "fight":
                _controller.SetInteger("gesture", (int)GestureType.fight);
                break;
            case "greeting":
                _controller.SetInteger("gesture", (int)GestureType.greeting);
                break;
            case "point_out":
                _controller.SetInteger("gesture", (int)GestureType.lookAt);
                break;
            case "run":
                _controller.SetInteger("gesture", (int)GestureType.run);
                break;
            case "sit":
                _controller.SetInteger("gesture", (int)GestureType.sit);
                break;
            default:
                _controller.SetInteger("gesture", (int)GestureType.idle);
                break;
               
        }
    }
}
