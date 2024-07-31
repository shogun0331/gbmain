using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GamePad : MonoBehaviour
{
    [SerializeField] Image _imgBg;
    [SerializeField] Image _imgCtr;

    Vector2 _Direction;
    float _Percent;
    [SerializeField] float _Distance;

    public bool IsOnPad
    {
        get
        {
            return _IsOnPad;
        }
        set
        {
            _IsOnPad = value;
            _imgBg.gameObject.SetActive(_IsOnPad);
            _imgCtr.gameObject.SetActive(_IsOnPad);
        }
    }

    bool _IsOnPad;
    bool _IsTouch;

    private void Awake()
    {
        _imgBg.raycastTarget = false;
        _imgCtr.raycastTarget = false;
        IsOnPad = false;
    }

    public void SetBg(Sprite sprite, bool isNativeSize = false)
    {
        if (sprite == null) return;
        if (_imgBg == null) return;

        _imgBg.sprite = sprite;
        if(isNativeSize)
        _imgBg.SetNativeSize();
    }

    public void SetCtr(Sprite sprite, bool isNativeSize = false)
    {
        if (sprite == null) return;
        if (_imgCtr == null) return;
        _imgCtr.sprite = sprite;
        if(isNativeSize)
        _imgCtr.SetNativeSize();
    }

    public Vector2 GetDirection()
    {
        return _Direction;
    }

    public float GetPercent()
    {
        return _Percent;
    }

    public void Process()
    {

        int touchCount = Input.touchCount;

        if (touchCount != 0)
        {
            for (int i = 0; i < touchCount; ++i)
            {
          
                Touch touch = Input.GetTouch(i);

                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }

                switch (touch.phase)
                {
                    case TouchPhase.Began:                        
                        TouchBegan(touch.position);
                        break;
                    case TouchPhase.Moved:
                        if (_IsTouch == false) return;
                        TouchMoved(touch.position);
                        break;
                    case TouchPhase.Ended:
                        TouchEnded();
                        break;
                }
            }
        }
        else
        {
            
            /* mouse control */
            for (int i = 0; i < 2; ++i)
            {
                if (Input.GetMouseButtonDown(i))
                {   // mouse down
                    // get mouse position
                    TouchBegan(Input.mousePosition);

                }
                else if (Input.GetMouseButtonUp(i))
                {
                    TouchEnded();
                }
                else if (Input.GetMouseButton(i))
                {
                    TouchMoved(Input.mousePosition);
                }
            }
        }
    }

    private void TouchBegan(Vector3 touchPoint)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        

        Vector3 position = touchPoint;

        _imgBg.transform.position = position;
        _imgCtr.transform.position = position;
        _Direction = Vector2.zero;
        _Percent = 0;
        
        float per = (GB.UI.UIManager.I.Canvas.localScale.x + GB.UI.UIManager.I.Canvas.localScale.y ) / 2;
        float p = (_imgBg.GetComponent<RectTransform>().sizeDelta.x * 0.5f) - (_imgCtr.GetComponent<RectTransform>().sizeDelta.x * 0.5f) * per;
        //_Distance = (_imgBg.GetComponent<RectTransform>().sizeDelta.x - _imgCtr.GetComponent<RectTransform>().sizeDelta.x ) * per;
        _Distance = p;
        _IsTouch = true;
        IsOnPad = true;

    }
    private void TouchMoved(Vector3 touchPoint)
    {
        if (_IsTouch == false) return;

        var dist = Vector2.Distance(_imgBg.transform.position, touchPoint);
        _Direction = (touchPoint - _imgBg.transform.position).normalized;

        if (dist < _Distance)
        {
            _Percent = dist / _Distance;
            _imgCtr.transform.position = touchPoint;
        }
        else
        {
            _Percent = 1.0f;
            Vector2 p = _imgBg.transform.position;
            p += _Distance * _Direction;
            _imgCtr.transform.position = p;
        }
    }
    private void TouchEnded()
    {
        _Direction = Vector2.zero;
        _Percent = 0;
        _IsTouch = false;
        IsOnPad = false;
    }

}
