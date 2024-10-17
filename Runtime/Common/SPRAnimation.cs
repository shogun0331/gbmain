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
        [SerializeField] SPRAnimationClip _animation;
        [SerializeField] UnityDictionary<string,SPRAnimationData> _animations = new UnityDictionary<string, SPRAnimationData>();

        

        public bool PlayAutomatically;
        
        Action<SPRAnimationClip.State,int,TriggerData> _callBack;

        public UnityEvent onStartEvent;
        public UnityEvent onEndEvent;

        public int IndexTrigger;
        public UnityEvent onTriggerEvent;

        void Awake()
        {
            if(_animation == null) _animation = new SPRAnimationClip();
            _animation.Init(GetComponent<SpriteRenderer>(),OnCallBack);
            
        }

        void OnEnable()
        {

            if(PlayAutomatically)
            {
                Play();
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

            if(state == SPRAnimationClip.State.Trigger)
            {
                if(index == IndexTrigger)
                {
                    onTriggerEvent?.Invoke();
                }

            }
            
            _callBack?.Invoke(state,index,trigger);
        }

        public void Play()
        {
            _animation.Play();
        }
        

        public void Play(string animationName)
        {
            if(!_animations.ContainsKey(animationName)) return;
            _animation.SetSPRAnimationData(animationName,_animations[animationName]);
            _animation.Play();
        }


        // Update is called once per frame
        void Update()
        {
            if(_animation != null)
                _animation.Update(Time.deltaTime);
            

        }
    }
}
