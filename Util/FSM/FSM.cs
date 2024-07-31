using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GB
{

    public class FSM : MonoBehaviour
    {
        public enum FSM_CallBack
        {
            OnEnable = 0,
            OnDisable,
            OnUpdate,
            //OnLateUpdate,
            //OnFixedUpdate
        }

        public static FSM Create(GameObject gameObject, List<Fsm_Func> fsm_Funcs)
        {
            
            FSM fsm = gameObject.AddComponent<FSM>();
            fsm._currentState = -1;
            fsm._dictActions = new Dictionary<int, Fsm_Func>();
            fsm._isPlaying = false;
            
            for (int i = 0; i < fsm_Funcs.Count; ++i)
                fsm._dictActions[fsm_Funcs[i].ID] = fsm_Funcs[i];

            return fsm;
        }

        int _currentState;
        public int CurrentState { get { return _currentState; } }
        [SerializeField] Dictionary<int, Fsm_Func> _dictActions;

        [SerializeField] bool _isPlaying;

        private  void OkEnable(FSM_CallBack call)
        {
            if (_isPlaying == false) return;
            if (_dictActions.ContainsKey(_currentState) == false) return;

            _dictActions[_currentState].onEnable?.Invoke();
        }

        private void OkDisable(FSM_CallBack call)
        {
            if (_isPlaying == false) return;
            if (_dictActions.ContainsKey(_currentState) == false) return;
            _dictActions[_currentState].onDisable?.Invoke();
        }

        private void OkUpdate(FSM_CallBack call)
        {
            if (_isPlaying == false) return;
            if (_dictActions.ContainsKey(_currentState) == false) return;

            _dictActions[_currentState].onUpdate?.Invoke();
        }

        private void Update()
        {
            OkUpdate(FSM_CallBack.OnUpdate);
        }

        public void Play(int state = 0)
        {
            _isPlaying = true;
            ChangeState(state);
        }

        public void Stop()
        {
            _currentState = -1;
            _isPlaying = false;
        }


        //private void OkFixedUpdate(FSM_CallBack call)
        //{
        //    if (_isPlaying == false) return;
        //    if (_dictActions.ContainsKey(_currentState) == false) return;

        //    _dictActions[_currentState].onFixedUpdate?.Invoke();
        //}

        //private void OkLateUpdate(FSM_CallBack call)
        //{
        //if (_isPlaying == false) return;
        //    if (_dictActions.ContainsKey(_currentState) == false) return;

        //    _dictActions[_currentState].onLateUpdate?.Invoke();
        //}

        //private void FixedUpdate()
        //{
        //    OkFixedUpdate(FSM_CallBack.OnFixedUpdate);
        //}

        //private void LateUpdate()
        //{
        //    OkLateUpdate(FSM_CallBack.OnLateUpdate);
        //}


        public void ChangeState(int state)
        {
            bool isEnable = false;
            
            if (state != _currentState)
            {
                OkDisable(FSM_CallBack.OnDisable);
                isEnable = true;
            }

            _currentState = state;

            if (isEnable)
            {
                OkEnable(FSM_CallBack.OnEnable);
            }
        }
    
    }

    public struct Fsm_Func
    {
        public int ID;
        public Action onEnable;
        public Action onDisable;
        public Action onUpdate;
        //public Action onFixedUpdate;
        //public Action onLateUpdate;
    }

}