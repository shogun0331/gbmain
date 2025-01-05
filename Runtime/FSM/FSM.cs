using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GB
{

    public class FSM : MonoBehaviour
    {
         public enum CallBack { OnEnter, OnUpdate, OnExit }
        protected FsmListener mCurrent;
        [SerializeField] string _curState;

        public string State{get{return _curState;}}

        protected Dictionary<string, FsmListener> mDictMachine;

        public static FSM Create(GameObject obj)
        {
            FSM machine = obj.AddComponent<FSM>();
            
            machine.mDictMachine = new Dictionary<string, FsmListener>();
            return machine;
        }

        public FSM AddListener(Enum state, Action<CallBack> callback)
        {
            string key = state.ToString();

            FsmListener listener = new FsmListener()
            {
                OnEnter = () => { callback?.Invoke(CallBack.OnEnter); },
                OnUpdate = () =>{callback?.Invoke(CallBack.OnUpdate);},
                OnExit = () => { callback?.Invoke(CallBack.OnExit); },
            };

            mDictMachine[key] = listener;
            return this;
        }

        public FSM AddListener(string state, Action<CallBack> callback)
        {
            string key = state;

            FsmListener listener = new FsmListener()
            {
                OnEnter = () => { callback?.Invoke(CallBack.OnEnter); },
                OnUpdate = () =>{callback?.Invoke(CallBack.OnUpdate);},
                OnExit = () => { callback?.Invoke(CallBack.OnExit); },
            };

            mDictMachine[key] = listener;
            return this;
        }

        public void AddListener(Enum state, FsmListener callback)
        {
            string key = state.ToString();
            if (mDictMachine.ContainsKey(key))
                mDictMachine[key] = callback;
        }


  

        public void SetState(Enum state)
        {
            string key = state.ToString();

            if (mDictMachine.ContainsKey(key))
            {
                if (key != _curState)
                {
                    mCurrent.OnExit?.Invoke();
                    _curState = key;
                    mCurrent = mDictMachine[key];
                    mCurrent.OnEnter?.Invoke();
                }
            }
        }

        public void SetState(string state)
        {
            string key = state;

            if (mDictMachine.ContainsKey(key))
            {
                if (key != _curState)
                {
                    mCurrent.OnExit?.Invoke();
                    _curState = key;
                    mCurrent = mDictMachine[key];
                    mCurrent.OnEnter?.Invoke();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            mCurrent.OnUpdate?.Invoke();
        }
    
    }

    public struct FsmListener
    {
        public Action OnEnter;
        public Action OnUpdate;
        public Action OnExit;
    }

}