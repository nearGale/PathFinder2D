using System;
using Application;
using UnityEngine;

public class AnimComponent : MonoBehaviour
{
    private Animator m_Animator;
    private string m_CurrentState;
    private Vector3 m_LocalScale = Vector3.zero;
    private float m_AnimatorSpeed;
    public IEntity Owner
    {
        get; private set;
    }

    private void Update()
    {
        if (Owner != null)
        {
            var v = Owner.GetVelocity();
            m_Animator.speed = m_AnimatorSpeed * v.magnitude / Owner.GetMaxVelocity();
            if(Math.Abs(v.y) >Math.Abs(v.x))
            {
                 if(v.y > 0)
                 {
                     PlayWalkUp();
                 }
                 else
                 {
                     PlayWalkDown();
                 }
            }
            else
            {
                if (v.x >= 0)
                {
                    PlayWalkRight();
                }
                else
                {
                    PlayWalkLeft();
                }
            }
        }
    }

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_CurrentState = string.Empty;
        m_LocalScale = gameObject.transform.localScale;
        m_AnimatorSpeed = m_Animator.speed;
    }

    public void SetOwner(IEntity owner)
    {
        this.Owner = owner;
    }
    public void PlayWalkLeft()
    {
        PlayAnim("walk_side");
        gameObject.transform.localScale = new Vector3(-m_LocalScale.x, m_LocalScale.y, m_LocalScale.z);
    }
    public void PlayWalkRight()
    {
        PlayAnim("walk_side");
        gameObject.transform.localScale = new Vector3(m_LocalScale.x, m_LocalScale.y, m_LocalScale.z);
    }
    public void PlayWalkUp()
    {
        PlayAnim("walk_up");
    }
    public void PlayWalkDown()
    {
        PlayAnim("walk_down");
    }
    void PlayAnim(string state)
    {
        if (!string.Equals(m_CurrentState, state))
        {
            m_CurrentState = state;
            m_Animator.CrossFade(state, 0.15f);
        }
    }
}