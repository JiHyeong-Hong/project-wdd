using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Data;
using System;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;

public class DataTransformer : EditorWindow
{
#if UNITY_EDITOR
    [MenuItem("Tools/ParseExcel %#K")]
    public static void ParseExcelDataToJson()
    {
        ParseExcelDataToJson<MonsterDataLoader, MonsterData>("Monster");
        ParseExcelDataToJson<HeroDataLoader, HeroData>("Hero");
        ParseExcelDataToJson<HeroLevelDataLoader, HeroLevelData>("HeroLevel");
        ParseExcelDataToJson<SkillDataLoader, SkillData>("Skill");
        ParseExcelDataToJson<ProjectileDataLoader, ProjectileData>("Projectile");
        ParseExcelDataToJson<ItemDataLoader, ItemData>("Item");
        ParseExcelDataToJson<HpConditionDataLoader, HpConditionData>("HpCondition");
        ParseExcelDataToJson<PatternPerDataLoader, PatternPerData>("PatternPer");
        ParseExcelDataToJson<BreakthroughDataLoader, BreakthroughData>("Breakthrough");

        Debug.Log("DataTransformer Completed");
    }

    #region Helpers
    private static void ParseExcelDataToJson<Loader, LoaderData>(string filename) where Loader : new() where LoaderData : new()
    {
        Loader loader = new Loader();
        FieldInfo field = loader.GetType().GetFields()[0];
        field.SetValue(loader, ParseExcelDataToList<LoaderData>(filename));

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }

    private static List<LoaderData> ParseExcelDataToList<LoaderData>(string filename) where LoaderData : new()
    {
        List<LoaderData> loaderDatas = new List<LoaderData>();
        int errorIndex = 0;
        try
        {
            string filePath = $"{Application.dataPath}/Resources/Data/ExcelData/{filename}Data.csv";

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"파일이 존재하지 않습니다:{filename}");
                return loaderDatas; // 파일이 없으면 빈 리스트 반환
            }

            string[] lines = File.ReadAllText(filePath).Split("\n");

            for (int l = 1; l < lines.Length; l++)
            {
                errorIndex = l;
                string[] row = lines[l].Replace("\r", "").Split(',');
                if (row.Length == 0)
                    continue;
                if (string.IsNullOrEmpty(row[0]))
                    continue;

                LoaderData loaderData = new LoaderData();
                var fields = GetFieldsInBase(typeof(LoaderData));

                for (int f = 0; f < fields.Count; f++)
                {
                    FieldInfo field = loaderData.GetType().GetField(fields[f].Name);
                    Type type = field.FieldType;

                    if (type.IsGenericType)
                    {
                        object value = ConvertList(row[f], type);
                        field.SetValue(loaderData, value);
                    }
                    else
                    {
                        object value = ConvertValue(row[f], field.FieldType);
                        field.SetValue(loaderData, value);
                    }
                }

                loaderDatas.Add(loaderData);
            }

            return loaderDatas;
        }
        catch (Exception ex)
        {
            Debug.LogError($"파일을 읽는 도중 오류가 발생했습니다: {ex.Message} FileName : {filename} ErrorIndex : {errorIndex} ");
            throw;
        }
        
    }

    private static object ConvertValue(string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        TypeConverter converter = TypeDescriptor.GetConverter(type);
        return converter.ConvertFromString(value);
    }

    private static object ConvertList(string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        Type valueType = type.GetGenericArguments()[0];
        Type genericListType = typeof(List<>).MakeGenericType(valueType);
        var genericList = Activator.CreateInstance(genericListType) as IList;

        var list = value.Split('&').Select(x => ConvertValue(x, valueType)).ToList();

        foreach (var item in list)
            genericList.Add(item);

        return genericList;
    }

    public static List<FieldInfo> GetFieldsInBase(Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
    {
        List<FieldInfo> fields = new List<FieldInfo>();
        HashSet<string> fieldNames = new HashSet<string>(); // 중복방지
        Stack<Type> stack = new Stack<Type>();

        while (type != typeof(object))
        {
            stack.Push(type);
            type = type.BaseType;
        }

        while (stack.Count > 0)
        {
            Type currentType = stack.Pop();

            foreach (var field in currentType.GetFields(bindingFlags))
            {
                if (fieldNames.Add(field.Name))
                {
                    fields.Add(field);
                }
            }
        }

        return fields;
    }
    #endregion

#endif
}