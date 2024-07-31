using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace GB.UI
{

    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if(_instance == null)
                    _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                    _instance = new GameObject(typeof(UIManager).Name).AddComponent<UIManager>();

                return _instance;
            }
        }


        [Header("하이라이키 팝업의 부모 이름을 입력하세요.")]
        [SerializeField] string _parentPopupName = "UIPopup";


        [Header("팝업의 프리팹 경로를 입력하세요. Resources.LoadAll(\"Path\")")]
        [SerializeField] string _PopupPath = "UI/Popup";

        private Dictionary<string, UIScreen> _UIScreenList = new Dictionary<string, UIScreen>();
        private UIScreen _scene = new UIScreen();
        [SerializeField] List<UIScreen> _popupList = new List<UIScreen>();

        Transform _popupParent;
        public Transform PopupParent
        {
            get
            {
                if (_popupParent == null)
                {
                   
                    var g = GameObject.Find(_parentPopupName);
                    if (g != null) _popupParent = g.transform;
                }
                return _popupParent;

            }

        }

        public int PopupCount { get { return _popupList.Count; } }

        Dictionary<string, GameObject> _PopupPrefabs = new Dictionary<string, GameObject>();
       

        private void Awake()
        {
            
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Init();
            DontDestroyOnLoad(this.gameObject);
        }

        public void Init()
        {

            var popups = Resources.LoadAll<GameObject>(_PopupPath);
            for (int i = 0; i < popups.Length; ++i) _PopupPrefabs[popups[i].name] = popups[i];

            //Popup Regist
            Transform popupParent = PopupParent;
            if (popupParent == null) return;


            //UIScreen Regist
            UIScreen[] allChildren = GetComponentsInChildren<UIScreen>(true);
            int len = allChildren.Length;
            for (int i = 0; i < len; ++i)
                allChildren[i].Initialize();

        }

        /// <summary>
        /// 씬 이동시 모든 내용 초기화
        /// </summary>
        public void Clear()
        {
            _UIScreenList.Clear();
            _scene = null;
            _popupList.Clear();
            
        }


        /// <summary>
        /// 씬변경
        /// </summary>
        /// <param name="sceneName">씬 네임</param>
        public static void ChangeScene(string sceneName)
        {
            Instance.Clear();
 
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// 스크린 등록
        /// </summary>
        /// <param name="UIScreen">스크린</param>
        public void RegistUIScreen(UIScreen UIScreen)
        {
            if (_UIScreenList.ContainsKey(UIScreen.gameObject.name))
                return;

            _UIScreenList.Add(UIScreen.gameObject.name, UIScreen);

            if (UIScreen.UIType == ScreenType.SCENE)
                _scene = UIScreen;
            else
                UIScreen.gameObject.SetActive(false);

        }

        /// <summary>
        /// 리프레쉬
        /// </summary>
        /// <param name="name">스크린 네임</param>
        public static void RefreshUIScreen(string name)
        {
            if (Instance._UIScreenList.ContainsKey(name))
                 Instance._UIScreenList[name].Refresh();
        }


        /// <summary>
        /// 모든 스크린 리프레쉬
        /// </summary>
        public static void RefreshAll()
        {

            if (_instance._scene != null)
                _instance._scene.Refresh();

            foreach (var v in Instance._UIScreenList)
                Instance._UIScreenList[v.Key].Refresh();
        }

        /// <summary>
        /// 스크린 찾기
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static UIScreen FindUIScreen(string name)
        {
            if (Instance._UIScreenList.ContainsKey(name))
                return Instance._UIScreenList[name];

            return null;
        }


        /// <summary>
        /// 팝업 켜기
        /// </summary>
        /// <param name="name">팝업 이름</param>
        /// <param name="extraValue"></param>
        public static void ShowPopup(string name)
        {
            Instance.SortingPopup();

            if (Instance._popupList.Count > 0)
            {

                if (Instance._popupList[0] == null)
                {
                    Instance._popupList.RemoveAt(0);
                    ShowPopup(name);
                    return;
                }
            }

            if (Instance._UIScreenList.ContainsKey(name))
            {
                Instance._UIScreenList[name].gameObject.SetActive(true);

                if (!Instance._popupList.Contains(Instance._UIScreenList[name]))
                    Instance._popupList.Add(Instance._UIScreenList[name]);

                Instance._UIScreenList[name].GetComponent<RectTransform>().SetAsLastSibling();
                Instance.SortingPopup();
            }
            else
            {
                Instance.LoadFromResources(name, true);
            }
        }

        private void LoadFromResources(string name, bool isPopup = false)
        {

            GameObject UIScreen = null;

            if (isPopup)
            {
                if (_PopupPrefabs.ContainsKey(name))
                    UIScreen = _PopupPrefabs[name];
                else
                    Debug.LogError("None Popup " + name);
            }
            else
            {
                UIScreen = Resources.Load<GameObject>(string.Format("{0}/{1}", _PopupPath, name));
            }

            if (UIScreen == null)
            {
                Debug.LogError(string.Format("can not load UI '{0}'", name));
                return;
            }

            UIScreen = Instantiate(UIScreen);
            UIScreen.name = name;
            UIScreen.transform.SetParent(PopupParent);
            
            // reset transform info
            UIScreen.GetComponent<RectTransform>().localScale = Vector3.one;
            UIScreen.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            UIScreen.GetComponent<RectTransform>().offsetMin = Vector2.zero;

            _UIScreenList.Add(name, UIScreen.GetComponent<UIScreen>());
            if (isPopup)
            {
                UIScreen.GetComponent<UIScreen>().Initialize();
                _popupList.Add(UIScreen.GetComponent<UIScreen>());
            }
 
            SortingPopup();
        }


        void OnBackKey()
        {
            if (_popupList.Count > 0)
            {
                _popupList[0].BackKey();
            }
            else
            {
                if(_scene != null)
                _scene.BackKey();
            }
        }

        public static void ClosePopup()
        {

            if (Instance._popupList.Count > 0)
            {
                UIScreen popup = Instance._popupList[0];
                popup.gameObject.SetActive(false);
                Instance._popupList.RemoveAt(0);
            }

            if (Instance._popupList.Count > 0)
                Instance._popupList[0].gameObject.SetActive(true);
        }
        public static void ClosePopup(UIScreen UIScreen)
        {
            UIScreen.gameObject.SetActive(false);
            Instance._popupList.Remove(UIScreen);
        }


        private void SortingPopup()
        {
            _popupList.Sort((tx, ty) => tx.Weight.CompareTo(ty.Weight));
            _popupList.Reverse();
        }

        private void Update()
        {
            //빽키 적용
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackKey();
            }
        }

    }

    public interface IUIData { public string DataKey { get;} };


    public static class UiUtil
    {
        public static UIScreen FindUIScreen(Transform tr)
        {
            UIScreen uIUIScreen = null;

            Transform parent = tr;

            for (int i = 0; i < 1000; ++i)
            {
                if (parent == null) break;

                if (parent.GetComponent<UIScreen>() != null)
                {
                    uIUIScreen = parent.GetComponent<UIScreen>();
                    break;
                }

                if (parent != null)
                    parent = parent.parent;

            }

            return uIUIScreen;

        }
    }

}
