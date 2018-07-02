using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace FancyScrollView
{
    [CustomEditor(typeof(BaseFancyScrollView),true)]
    [CanEditMultipleObjects]
    public class FancyScrollViewEditor : Editor
    {
        protected SerializedProperty script;
        protected SerializedProperty cellInterval;
        protected SerializedProperty cellOffset;
        protected SerializedProperty loop;
        protected SerializedProperty cellBase;
        protected SerializedProperty cellContainer;
        protected SerializedProperty ResNameMode;
        private SerializedObject childController;

        ScrollPositionControllerEditor controllerEditor;
        private GameObject cellRes;

        void OnEnable()
        {
            script = serializedObject.FindProperty("m_Script");
            ResNameMode = serializedObject.FindProperty("ResNameMode");
            cellInterval = serializedObject.FindProperty("cellInterval");
            cellOffset = serializedObject.FindProperty("cellOffset");
            loop = serializedObject.FindProperty("loop");
            cellBase = serializedObject.FindProperty("cellBase");
            cellContainer = serializedObject.FindProperty("cellContainer");

            BaseFancyScrollView view = target as BaseFancyScrollView;
            controllerEditor = new ScrollPositionControllerEditor();
            childController = new SerializedObject(view.controller);
            controllerEditor.OnEnable(childController);

        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(script);
            serializedObject.Update();

            GUILayout.Space(1);
            GUILayout.Label("Fancy Cell Attributes",GUILayout.Width(200));
            GUILayout.Space(1);
            EditorGUILayout.BeginVertical("window");
            EditorGUILayout.PropertyField(ResNameMode);
            EditorGUILayout.Slider(cellInterval,float.Epsilon,1f);
            EditorGUILayout.Slider(cellOffset,0, 1f);

            EditorGUILayout.PropertyField(loop);
            EditorGUILayout.PropertyField(cellContainer);

            ShowCellBase();
            EditorGUILayout.EndVertical();

            if(controllerEditor != null && childController != null)
            {
                GUILayout.Space(2);
                EditorGUILayout.BeginVertical("window");
                controllerEditor.OnInspectorGUI(target, childController);
                EditorGUILayout.EndVertical();

                childController.ApplyModifiedProperties();
            }

            serializedObject.ApplyModifiedProperties();
        }

        void ShowCellBase()
        {
            FancyScrollViewResName resNameMode = (FancyScrollViewResName)ResNameMode.intValue;
            string baseRes = cellBase.stringValue;

            if (cellRes == null)
            {
                if(!string.IsNullOrEmpty(baseRes))
                {
                    string[] guids = AssetDatabase.FindAssets("t:GameObject");
                    int samecnt = 0;
                    for (int i = 0; i < guids.Length; ++i)
                    {
                        string filepath = AssetDatabase.GUIDToAssetPath(guids[i]);
                        if(resNameMode == FancyScrollViewResName.FullName)
                        {
                            if (filepath.Equals(baseRes))
                            {
                                cellRes = AssetDatabase.LoadAssetAtPath<GameObject>(filepath);
                                break;
                            }
                        }
                        else if(resNameMode == FancyScrollViewResName.FilePath)
                        {
                            string fixpath = Path.GetFileName(filepath);
                            if (fixpath.Equals(baseRes))
                            {
                                cellRes = AssetDatabase.LoadAssetAtPath<GameObject>(filepath);
                                samecnt++;
                            }
                        }
                        else if(resNameMode == FancyScrollViewResName.FilePathWithoutExtension)
                        {
                            string fixpath = Path.GetFileNameWithoutExtension(filepath);
                            if (fixpath.Equals(baseRes))
                            {
                                cellRes = AssetDatabase.LoadAssetAtPath<GameObject>(filepath);
                                samecnt++;
                            }
                        }
                    }

                    if(samecnt >1)
                    {
                        Debug.LogErrorFormat(" Named :{0} Count is :{2}", baseRes, samecnt);
                    }
                }
            }

            if (!string.IsNullOrEmpty(baseRes))
            {
                if (resNameMode == FancyScrollViewResName.FullName)
                {
                    cellBase.stringValue = baseRes;
                }
                else if (resNameMode == FancyScrollViewResName.FilePath)
                {
                    cellBase.stringValue = Path.GetFileName(baseRes);
                }
                else if (resNameMode == FancyScrollViewResName.FilePathWithoutExtension)
                {
                    cellBase.stringValue = Path.GetFileNameWithoutExtension(baseRes);
                }
            }

            GameObject newcell = (GameObject)EditorGUILayout.ObjectField("CellBase", cellRes, typeof(GameObject), false);
            if (newcell != cellRes)
            {
                if(newcell != null)
                {
                    string filepath = AssetDatabase.GetAssetPath(newcell);
                    if (resNameMode == FancyScrollViewResName.FullName)
                    {
                        cellBase.stringValue = filepath;
                    }
                    else if(resNameMode ==  FancyScrollViewResName.FilePath)
                    {
                        cellBase.stringValue = Path.GetFileName(filepath);
                    }
                    else if(resNameMode == FancyScrollViewResName.FilePathWithoutExtension)
                    {
                        cellBase.stringValue = Path.GetFileNameWithoutExtension(filepath);
                    }   
                }
                else
                {
                    cellBase.stringValue = "";
                }

                cellRes = newcell;
            }
        } 
    }
}


