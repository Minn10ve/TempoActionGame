using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TempoEditor : EditorWindow
{
    private VisualElement _mainVisualElement;
    
    private Dictionary<Button, AtkTempoData> _tempoList = new Dictionary<Button, AtkTempoData>();

    private AtkTempoData _curData;

    [MenuItem("Tools/Tempo Setting")]
    public static void ShowExample()
    {
        TempoEditor wnd = GetWindow<TempoEditor>();
        //불러옵니다.
        wnd.titleContent = new GUIContent("Tempo Setting");
        Vector2 size = new Vector2(400f, 300f);
        wnd.minSize = size;
        wnd.maxSize = size;
        //사이즈 설정
    }

    private void CreateGUI()
    {
        VisualElement root = rootVisualElement; // 최상의 화면을 변수 root로 설정합니다

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/TempoEditor/TempoEditor.uxml");
        root.Add(visualTree.Instantiate()); // uxml를 불러옵니다.

        _mainVisualElement = root.Q<ScrollView>("MainVisualElement");
        _mainVisualElement.style.visibility = Visibility.Hidden;     

        AddData("MainTempo1");
        AddData("MainTempo2");
        AddData("MainTempo3");
        AddData("MainTempo4");
        AddData("PointTempo1");
        AddData("PointTempo2");

        InitData();
    }

    private void AddData(string name)
    {

        string[] datas = Directory.GetFiles("Assets/ScriptableObjects/AtkTempoDatas", $"{name}.asset",
            SearchOption.AllDirectories);

        foreach (string data in datas)
        {
            
            string cleanedPath = data.Replace("\\", "/"); // \\를 / 로 바꿔줍니다. Assets\\Data 가 아닌 Assets/Data 로 쓰기때문에
            _tempoList.Add(rootVisualElement.Q<Button>(name), (AtkTempoData)AssetDatabase.LoadAssetAtPath(cleanedPath,
                typeof(AtkTempoData)));

        }
    }

    private void InitData()
    {
        var slowSlider = rootVisualElement.Q<MinMaxSlider>("SlowRangth");

        foreach (var t in _tempoList)
        {
            
            t.Key.clickable.clicked += () =>
            {
                _curData = t.Value;
                UpdateData();
                
            };

          
        }
    }
 
    private void UpdateData()
    {

        rootVisualElement.Q<Label>("TempoTitle").text = _curData.name;

        SerializedObject so = new SerializedObject(_curData);
        _mainVisualElement.Bind(so);
        //바인딩해줍니다.
        if (_curData.type == Define.TempoType.MAIN)
        {
            rootVisualElement.Q<FloatField>("PerfectStamina").label = "스태미나";
            rootVisualElement.Q<FloatField>("GoodStamina").style.visibility = Visibility.Hidden;
        }
        else
        {
            rootVisualElement.Q<FloatField>("PerfectStamina").label = "Perfect 스태미나";
            rootVisualElement.Q<FloatField>("GoodStamina").style.visibility = Visibility.Visible;
        }

        _mainVisualElement.style.visibility = Visibility.Visible;


    }
  
}
