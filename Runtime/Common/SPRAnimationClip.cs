using System;
using QuickEye.Utility;
using UnityEngine;

namespace GB
{
[Serializable]
public class SPRAnimationClip
{
    public enum State{Play = 0,Next,End,Trigger}
    [SerializeField] UnityDictionary<string,SPRAnimationData> _dictAnim;

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

    public void AddSPRAnimationData(string key,SPRAnimationData data)
    {
        if(_dictAnim == null)
        _dictAnim = new UnityDictionary<string, SPRAnimationData>();

        _dictAnim[key] = data;
    }

    public void Play(float speed = 1)
    {
        if(_sprRender == null) 
        {
              Debug.LogWarning("Animation Init(SpriteRenderer spriteRenderer,Action<State,int,TriggerData> callBack) : null");
            return;
        }
        if(_dictAnim.Count <= 0) return;
        if(string.IsNullOrEmpty(_curID))
        {
            foreach(var v in _dictAnim) 
            {
                _curID = v.Key;
                _curIDX = 0;
                break;
            }
        }
      
        if(_dictAnim[_curID].SpriteCount <= 0) return;
        _curIDX = 0;
        _sprRender.sprite = _dictAnim[_curID].GetSprite( _curIDX);
        _isPlaying = true;
        _isLoop = _dictAnim[_curID].IsLoop;
        if(_dictAnim[_curID].Speed <= 0) _dictAnim[_curID].Speed = 1;
        _fixTimer =  TIMER / (_dictAnim[_curID].Speed * speed);
        _callBack?.Invoke(State.Play,_curIDX,null);
    }

    public void Play(string id,float speed = 1)
    {
        if(_sprRender == null) 
        {
              Debug.LogWarning("Animation Init(SpriteRenderer spriteRenderer,Action<State,int,TriggerData> callBack) : null");
            return;
        }

        if(_dictAnim.Count <= 0) return;
        if(!_dictAnim.ContainsKey(id)) return;
        if(_dictAnim[id].SpriteCount <= 0) return;

        _curID = id;
        _curIDX = 0;
        _sprRender.sprite = _dictAnim[_curID].GetSprite(0);
        
        _isPlaying = true;
        _isLoop = _dictAnim[_curID].IsLoop;
        if(_dictAnim[_curID].Speed <= 0) _dictAnim[_curID].Speed = 1;
        _fixTimer =  TIMER / (_dictAnim[_curID].Speed * speed);
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

        
        _time += dt;

        if(_time > _fixTimer)
        {
            _time = 0;
            _curIDX ++;

            if(_curIDX >= _dictAnim[_curID].SpriteCount )
            {
                if(_isLoop)
                {
                    _curIDX = 0;
                    _sprRender.sprite = _dictAnim[_curID].GetSprite(_curIDX);
                    //_callBack?.Invoke(State.Next,_curIDX,string.Empty);
                }
                else
                {
                    Stop();
                    return;
                }
            }

            _sprRender.sprite = _dictAnim[_curID].GetSprite(_curIDX);
            
            if(_dictAnim[_curID].Triggers.ContainsKey(_curIDX))
            {
                if(_dictAnim[_curID].Triggers != null)
                {
                    for(int i = 0; i< _dictAnim[_curID].Triggers[_curIDX].Count;++i)
                        _callBack?.Invoke(State.Trigger,_curIDX,_dictAnim[_curID].Triggers[_curIDX][i]);
                }
            }
        }
    }
}

}