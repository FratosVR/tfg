using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class AnimImport : MonoBehaviour
{
    private Animator anim;

    [SerializeField]
    private List<AnimationClip> animaciones;
    protected AnimatorOverrideController animatorOverrideController;

    private int index;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animatorOverrideController;
        index = -1;
        timer = 0.0f;
        setClip();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 5)
        {
            timer = 0.0f;
            setClip();
        }
    }

    private void setClip()
    {
        anim.Play("Default", 0);
        index = (index + 1) % animaciones.Count;
        animatorOverrideController["mixamo.com"] = animaciones[index];
        anim.Play("Default", 0);
    }
}
