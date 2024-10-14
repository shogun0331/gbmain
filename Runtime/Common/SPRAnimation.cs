using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using QuickEye.Utility;
using UnityEngine;
using UnityEngine.Events;
namespace GB
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SPRAnimation : MonoBehaviour
    {
        [SerializeField] string _animationName;
        [SerializeField] UnityDictionary<string,SPRAnimationData> _animations = new UnityDictionary<string, SPRAnimationData>();

        SPRAnimationClip _sprAnimations;

        public bool PlayAutomatically;
        
        Action<SPRAnimationClip.State,int,TriggerData> _callBack;

        public UnityEvent onStartEvent;
        public UnityEvent onEndEvent;

        void Awake()
        {
            _sprAnimations = new SPRAnimationClip();
            _sprAnimations.Init(GetComponent<SpriteRenderer>(),OnCallBack);
            foreach(var v in _animations)
            {
                _sprAnimations.AddSPRAnimationData(v.Key,v.Value);
            }
        }

        void OnEnable()
        {

            if(PlayAutomatically)
            {
                Play(_animationName);
            }
        
            
        }
        public void AddListener(Action<SPRAnimationClip.State,int,TriggerData> callback)
        {
            _callBack = callback;
        }

        public virtual void OnCallBack(SPRAnimationClip.State state,int index,TriggerData trigger)
        {
            if(state == SPRAnimationClip.State.Play)
            {
                onStartEvent?.Invoke();
            }
            else if(state == SPRAnimationClip.State.End)
            {
                onEndEvent?.Invoke();
            }
            
            _callBack?.Invoke(state,index,trigger);
        }
        

        public void Play(string animationName)
        {
            if(!_animations.ContainsKey(animationName)) return;
            _animationName = animationName;

            _sprAnimations.Play(_animationName);
        }


        // Update is called once per frame
        void Update()
        {
            if(_sprAnimations != null)
                _sprAnimations.Update(Time.deltaTime);
            

        }
    }
}
