using UnityEngine;

namespace GB
{
    public abstract class Machine : MonoBehaviour
    {
        public abstract void SetMachine(IStateMachineMachine Data);
        public abstract void OnEnter();
        public abstract void OnUpdate();
        public abstract void OnExit();
        public abstract void OnEvent(string eventName);

    }
}
