using System;
using System.Collections.Generic;
using Aya.Tween;
using QuickEye.Utility;
using UnityEngine;
using NaughtyAttributes;
using System.Collections;

namespace GB
{
    public class Flow : MonoBehaviour
    {
        [ReorderableList]
        [SerializeField] List<FlowData> _flowDatas;
        

        void OnDisable()
        {
            Clear();
        }

        public void Clear()
        {

            for (int i = 0; i < _flowDatas.Count; ++i)
            {
                if (_flowDatas[i].TweenAnim != null)
                {
                    _flowDatas[i].TweenAnim.Stop();
                    _flowDatas[i].TweenAnim.SetStart();
                }
                if(_flowDatas[i].Particle != null)
                _flowDatas[i].Particle.Stop();

            }



            StopAllCoroutines();

        }



        [Button]
        public void Play()
        {


#if UNITY_EDITOR

            if (Application.isPlaying)
            {
                StartCoroutine(Coroutine_Play());
            }
            else
            {
                GB.Edit.EditorCoroutines.StartCoroutine(Coroutine_Play(), this);
            }

#else

            StartCoroutine(Coroutine_Play());

#endif
        }

        IEnumerator Coroutine_Play()
        {
            List<float> timeList = new List<float>();
            float prevTime = 0;
            int curIDX = 0;

            for (int i = 0; i < _flowDatas.Count; ++i)
            {

                if (i == 0)
                    prevTime = 0;
                else
                    prevTime = _flowDatas[i - 1].Time;

                float time = _flowDatas[i].Time - prevTime;

                timeList.Add(time);
            }

            
            for (int i = 0; i < _flowDatas.Count; ++i)
            {
                if (_flowDatas[i].TweenAnim != null)
                {
                    _flowDatas[i].TweenAnim.Stop();
                    _flowDatas[i].TweenAnim.SetStart();
                   
                }

            }


            for (int i = 0; i < _flowDatas.Count; ++i)
            {
                if (_flowDatas[i] == null) continue;

                yield return new WaitForSeconds(timeList[i]);
                curIDX = i;
                DataPlay(_flowDatas[i]);
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                while (true)
                {
                    yield return new WaitForSeconds(0.2f);

                    var data = _flowDatas[_flowDatas.Count - 1];

                    if (curIDX == _flowDatas.Count - 1 && data.IsPlaying == false)
                    {
                        for (int i = 0; i < _flowDatas.Count; ++i)
                        {
                            if (_flowDatas[i].TweenAnim != null)
                            {
                               _flowDatas[i].TweenAnim.Stop();
                               _flowDatas[i].TweenAnim.SetStart();
                            }

                        }

                        yield break;
                    }
                }
            }
#endif

        }

        void DataPlay(FlowData data)
        {
            
            if (data == null) return;

            if (data.TweenAnim != null)
            {
                if (Application.isPlaying)
                {
                    data.TweenAnim.Play();

                }
                else
                {
                    data.TweenAnim.SyncTweenerParams();


                    #if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        TweenManager.Ins.PreviewTweenerList.Add(data.TweenAnim.Tweener);
                    }
                    #endif
                    data.TweenAnim.Tweener.Play();

                }
            }

            if (data.Particle != null)
            {
                data.Particle.Play();
            }
            

        }
    }

    [Serializable]
    public class FlowData
    {
        public bool IsPlaying
        {
            get
            {
                bool isPlaying = false;
                if (TweenAnim != null)
                {
                    if (TweenAnim.Tweener != null)
                        isPlaying = TweenAnim.Tweener.IsPlaying;
                }

                if (!isPlaying && Particle != null)
                {
                    isPlaying = Particle.isPlaying;
                }

                return isPlaying;

            }

        }
        public float Time;
        public TweenAnimation TweenAnim;
        public ParticleSystem Particle;

    }
}
