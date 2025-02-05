using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct GBCoroutine
{
    List<IEnumerator> _coroutineList;

    MonoBehaviour _mono;

    Coroutine  _coroutine;

    Action _result;

    public GBCoroutine SetMonoBehaviour(MonoBehaviour monoBehaviour)
    {
        _mono = monoBehaviour;
        return this;
    }

    public GBCoroutine AddIEnumerator(IEnumerator coroutine)
    {
        if(_coroutineList == null)_coroutineList = new List<IEnumerator>();
        _coroutineList.Add(coroutine);
        return this;
    }
    public GBCoroutine OnComplete(Action result)
    {
        _result = result;
        return this;
    }

    public GBCoroutine Play()
    {
        _coroutine = _mono.StartCoroutine(RunCoroutines());
        return this;
    }

    IEnumerator RunCoroutines()
    {
        for (int i = 0; i < _coroutineList.Count; i++)
        {
            yield return _mono.StartCoroutine(_coroutineList[i]);
        }

        _result?.Invoke();
    }

    public void Stop()
    {
        if(_mono != null &&_coroutine != null)
        _mono.StopCoroutine(_coroutine);
    }


}

