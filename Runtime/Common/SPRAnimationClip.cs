using System;
using QuickEye.Utility;
using UnityEngine;

namespace GB
{
[Serializable]
public class SPRAnimationClip
{
    public enum State{Play = 0,Next,End,Trigger}
    [SerializeField] SPRAnimationData _Anim;

    SpriteRenderer _sprRender;

    public bool IsPlaying {get { return _isPlaying;}}
    bool _isPlaying;
    
    string _curID;
    
    public string AnimationName{get{return _curID;}}
    int _curIDX;
    public int AnimationIndex{get{return _curIDX;}}

    bool _isLoop;
    public bool IsLoop{get{return _isLoop;}}
    float _fixTimer;

    Action<State,int,TriggerData> _callBack;
    const float TIMER = 0.05f;

    

    

    public void Init(SpriteRenderer spriteRenderer,Action<State,int,TriggerData> callBack)
    {
        _sprRender = spriteRenderer;
        _callBack = callBack;
    }

    public void SetSPRAnimationData(string key,SPRAnimationData data)
    {
        _Anim = data;
    }

    public void Play(float speed = 1)
    {
        if(_sprRender == null) 
        {
            Debug.LogWarning("Animation Init(SpriteRenderer spriteRenderer,Action<State,int,TriggerData> callBack) : null");
            return;
        }
        if(_Anim == null ) return;
        
        
      
        if(_Anim.SpriteCount <= 0) return;
        _curIDX = 0;
        _sprRender.sprite = _Anim.GetSprite( _curIDX);
        _isPlaying = true;
        _isLoop = _Anim.IsLoop;
        if(_Anim.Speed <= 0) _Anim.Speed = 1;
        _fixTimer =  TIMER / (_Anim.Speed * speed);
        _callBack?.Invoke(State.Play,_curIDX,null);
    }


    public void Stop()
    {
        if(_sprRender == null) 
        {
              Debug.LogWarning("Animation Init(SpriteRenderer spriteRenderer,Action<State,int,TriggerData> callBack) : null");
            return;
        }

        _isPlaying = false;
        _callBack?.Invoke(State.End,_curIDX,null);
        
    }

    public void Resume()
    {
        if(_sprRender == null) 
        {
              Debug.LogWarning("Animation Init(SpriteRenderer spriteRenderer,Action<State,int,TriggerData> callBack) : null");
            return;
        }
        _isPlaying = true;
    }

    float _time;

    public void Update(float dt)
    {
        if(_isPlaying == false) return;
        if(_sprRender == null) 
        {
              Debug.LogWarning("Animation Init(SpriteRenderer spriteRenderer,Action<State,int,TriggerData> callBack) : null");
            return;
        }

        if(_Anim == null) return;

        
        _time += dt;

        if(_time > _fixTimer)
        {
            _time = 0;
            _curIDX ++;

            if(_curIDX >= _Anim.SpriteCount )
            {
                if(_isLoop)
                {
                    _curIDX = 0;
                    _sprRender.sprite = _Anim.GetSprite(_curIDX);
                    //_callBack?.Invoke(State.Next,_curIDX,string.Empty);
                }
                else
                {
                    Stop();
                    return;
                }
            }

            _sprRender.sprite = _Anim.GetSprite(_curIDX);
            
            if(_Anim.Triggers.ContainsKey(_curIDX))
            {
                if(_Anim.Triggers != null)
                {
                    for(int i = 0; i< _Anim.Triggers[_curIDX].Count;++i)
                        _callBack?.Invoke(State.Trigger,_curIDX,_Anim.Triggers[_curIDX][i]);
                }
            }
        }
    }
}

}