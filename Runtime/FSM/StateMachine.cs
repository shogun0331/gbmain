using System;
using QuickEye.Utility;
using UnityEngine;

namespace GB
{
    public class StateMachine<E> : MonoBehaviour,IStateMachineMachine where E : Enum
    {
        [SerializeField] protected UnityDictionary<string, Machine> mMacines;
        [SerializeField] E _State;
        protected FSM mFSM;
        bool _isInit = false;
        public E CurrentState {get{return _State;} }

        public void ClearMacine()
        {
            if(mMacines == null)mMacines = new UnityDictionary<string, Machine>();
            mMacines.Clear();

        }

        public Machine GetMacine()
        {
            if(mMacines.ContainsKey(_State.ToString()))
                return mMacines[_State.ToString()];

            return null;
        }
        
        public Machine GetMacine(E state)
        {
            if(mMacines.ContainsKey(state.ToString()))
                return mMacines[state.ToString()];

            return null;
        }

        protected void Init() 
        {
            CreateFSM<E>();
        }
        public void SetMacine(string key, Machine macine)
        {
            if(mMacines == null)mMacines = new UnityDictionary<string, Machine>();
            mMacines[key] = macine;
        }

        
        public void ChangeState(E state)
        {
            if(mFSM == null) return;
            _State = state;
            mFSM.SetState(state.ToString());
        }

        protected void SetEvent(string eventName)
        {
            if(mFSM  == null) return;
            if(mMacines == null) return;

            if(mMacines.ContainsKey(mFSM.State))
                mMacines[mFSM.State].OnEvent(eventName);
        }

        #region  FSM
        void CreateFSM<E>()
        {
            if(mMacines == null) return;
            if(_isInit) return;
            _isInit = true;

           string[] names  = Enum.GetNames(typeof(E));
           mFSM = FSM.Create(this.gameObject);

           for(int i = 0; i< names.Length; ++i)
           {
                string  key = names[i];

                mFSM.AddListener(key,
                (FSM.CallBack result)=>
                {
                    if(!mMacines.ContainsKey(key)) return;
                    
                    switch(result)
                    {
                        case FSM.CallBack.OnEnter:
                        mMacines[key].SetMachine(this);
                        mMacines[key].OnEnter();
                        break;
                        case FSM.CallBack.OnUpdate:
                        mMacines[key].OnUpdate();
                        break;
                        case FSM.CallBack.OnExit:
                        mMacines[key].OnExit();
                        break;
                    }
                });
           }
           
           mFSM.SetState(names[0]);            
        }

        #endregion
    
    }

    public interface IStateMachineMachine{}

}
