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

        protected void Init() 
        {
            CreateFSM<E>();
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
