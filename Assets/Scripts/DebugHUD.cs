using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DebugHUD : MonoBehaviour
{

    // private const string CAMERA_NAME = "CenterEyeAnchor";
    private const string CAMERA_NAME = "Main Camera";


    public struct HUDTextElement
    {
        public string id;
        public Vector3 position;
        public int width;
        public Color color;
        public string defaultText;
    }


    private struct DebugRendering
    {
        public int FontSize;
        public int LineHeight;
        public int BottomMargin;
    }

    private DebugRendering UnityRendering = new DebugRendering()
    {
        FontSize = 14,
        LineHeight = 20,
        BottomMargin = 200
    };

    private DebugRendering OculusRendering = new DebugRendering()
    {
        FontSize = 40,
        LineHeight = 60,
        BottomMargin = 600
    };


    private Camera _camera;
    private GameObject _hud;
    private Canvas _canvas;


    private List<GameObject> _toastMessages = new List<GameObject>();
    private Dictionary<string, GameObject> _textElements = new Dictionary<string, GameObject>();
    private List<HUDTextElement> _createElements = new List<HUDTextElement>();


    private static DebugHUD _debugHUD = null;


    public static DebugHUD GetInstance()
    {
        if (_debugHUD == null)
        {
            GameObject gameObject = GameObject.Find(CAMERA_NAME);
            while (gameObject.GetComponent<DebugHUD>() == null)
            {
                if (gameObject.transform.parent == null)
                {
                    throw new System.NullReferenceException();
                }
                gameObject = gameObject.transform.parent.gameObject;
            }

            _debugHUD = gameObject.GetComponent<DebugHUD>();
        }

        return _debugHUD;
    }


    void Start()
    {
        _camera = GameObject.Find(CAMERA_NAME).GetComponent<Camera>();
        // UnityEngine.Debug.Log(_camera);
        // UnityEngine.Debug.Log(_camera.transform);

        _hud = new GameObject("HUD", typeof(RectTransform));
        _hud.transform.SetParent(_camera.transform);
        RectTransform rt = (RectTransform)_hud.transform;
        rt.localPosition = new Vector3(0, 0, 1);
        rt.sizeDelta = new Vector2(1360, 700);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localScale = new Vector3(0.002853067f, 0.002853067f, 0.002853067f);

        _canvas = _hud.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvas.worldCamera = _camera;
        _canvas.sortingOrder = 1;
        _canvas.planeDistance = 1;

        CanvasScaler scaler = _hud.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor = 1;
        scaler.referencePixelsPerUnit = 100;

        GraphicRaycaster raycaster = _hud.AddComponent<GraphicRaycaster>();
        raycaster.ignoreReversedGraphics = true;
        raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
        raycaster.blockingMask = ~0;

        CreateElements();
    }


    //example of both static text element and toast
/*    
    DebugHUD.FindDebugHud().AddTextElement(new DebugHUD.HUDTextElement() { id = "time", position = new Vector3(-60, -40, 0), width = 1200, color = Color.yellow, defaultText = "Time"});
    DebugHUD.FindDebugHud().UpdateTextElement("time", DateTime.Now.ToString());

    DateTime _next = DateTime.MinValue;
    void Update()
    {
        if (_next < DateTime.Now)
        {
            _next = DateTime.Now.AddSeconds(1);
            UpdateTextElement("time", DateTime.Now.ToString());
            PresentToast(DateTime.Now.ToString());
        }
    }
*/

    void Update()
    {
        CreateElements();
    }


    public void AddTextElement(HUDTextElement textElement)
    {
        _createElements.Add(textElement);
    }


    private void CreateElements()
    {
        lock(_createElements)
        {
            foreach (var element in _createElements)
            {
                AddTextElement(element.id, element.position, element.width, element.color, element.defaultText);
            }

            _createElements.Clear();
        }
    }


    private void AddTextElement(string id, Vector3 position, int width, Color color, string defaultText)
    {
        DebugRendering rendering = Application.isEditor ? UnityRendering : OculusRendering;

        RectTransform objectRectTransform = _canvas.gameObject.GetComponent<RectTransform>();
        float top = -((objectRectTransform.rect.height / 2) - rendering.BottomMargin);
        top += position.y;

        GameObject textElement = new GameObject("GameObject");
        textElement.name = "TextElement";
        textElement.transform.parent = _canvas.transform;

        Text text = textElement.AddComponent<Text>();
        text.fontSize = rendering.FontSize;
        text.color = color;
        text.text = defaultText;
        text.alignment = TextAnchor.MiddleCenter;
        text.rectTransform.pivot.Scale(new Vector2(1, 1));
        text.rectTransform.pivot.Set(0.5f, 0.5f);
        text.rectTransform.localScale = new Vector3(1, 1, 1);
        text.rectTransform.sizeDelta = new Vector2(width, rendering.LineHeight);
        text.rectTransform.localRotation = new Quaternion(0, 0, 0, 0);
        text.rectTransform.localPosition =  new Vector3(position.x, top, position.z);


        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = ArialFont;
        text.material = ArialFont.material;  

        if (_textElements.ContainsKey(id))
        {
            Destroy(_textElements[id]);
            _textElements.Remove(id);
        }

        _textElements.Add(id, textElement);
    }


    public void UpdateTextElement(string id, string newText)
    {
        if (!_textElements.ContainsKey(id)) return;

        _textElements[id].GetComponent<Text>().text = newText;
    }


#region Toast


    public void PresentToast(string toastMessage)
    {
        PresentToast(toastMessage, 0.5f, 1f, 0.5f);
    }


    public void PresentToast(string toastMessage, float fadeInDuration, float displayDuration, float fadeOutDuration)
    {
        PushToastMessagesUp();

        GameObject gameObject = CreateTextObject(toastMessage);
        _toastMessages.Add(gameObject);  

        try { StartCoroutine(showToastCOR(gameObject, fadeInDuration, displayDuration, fadeOutDuration)); } catch (Exception) { }
    }


    private void PushToastMessagesUp()
    {
        DebugRendering rendering = Application.isEditor ? UnityRendering : OculusRendering;

        lock(_toastMessages)
        {
            //push them up
            foreach (GameObject gameObject in _toastMessages)
            {
                Text text2 = gameObject.GetComponent<Text>();
                text2.rectTransform.localPosition = new Vector3(0, text2.rectTransform.localPosition.y + rendering.LineHeight, 0);
            }
        }
    }


    private GameObject CreateTextObject(string toastMessage)
    {
        DebugRendering rendering = Application.isEditor ? UnityRendering : OculusRendering;

        RectTransform objectRectTransform = _canvas.gameObject.GetComponent<RectTransform>();
        float top = -((objectRectTransform.rect.height / 2) - rendering.BottomMargin);

        GameObject toast = new GameObject("GameObject");
        toast.name = "toastMessage";
        toast.transform.parent = _canvas.transform;

        Text text = toast.AddComponent<Text>();
        text.fontSize = rendering.FontSize;
        text.text = toastMessage;
        text.alignment = TextAnchor.MiddleCenter;
        text.rectTransform.pivot.Scale(new Vector2(1, 1));
        text.rectTransform.pivot.Set(0.5f, 0.5f);
        text.rectTransform.localScale = new Vector3(1, 1, 1);
        text.rectTransform.sizeDelta = new Vector2(100000, rendering.LineHeight);
        text.rectTransform.localRotation = new Quaternion(0, 0, 0, 0);
        text.rectTransform.localPosition =  new Vector3(0, top, 0);


        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = ArialFont;
        text.material = ArialFont.material;  

        return toast; 
    }


    private IEnumerator showToastCOR(GameObject gameObject, float fadeInDuration, float displayDuration, float fadeOutDuration)
    {
        Text textObject = gameObject.GetComponent<Text>();

        Color orginalColor = new Color(255, 255, 255, 255);

        //Fade in
        yield return fadeToastInAndOut(textObject, true, fadeInDuration);

        //Wait for the duration
        float counter = 0;
        while (counter < displayDuration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeToastInAndOut(textObject, false, fadeOutDuration);

        textObject.enabled = false;
        textObject.color = orginalColor;

        _toastMessages.Remove(gameObject);
    }


    private IEnumerator fadeToastInAndOut(Text targetText, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = new Color(0, 0, 255, 0);// Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }


#endregion


}
