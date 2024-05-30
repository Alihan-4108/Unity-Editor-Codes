using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class EditorScript : EditorWindow
{
    #region Object Cloning Variables
    private int cloneCount = 1;
    #endregion

    #region Create Folder Variables
    private string folderName = "";
    #endregion

    #region Select Objects Variables
    private enum Tag //You can write the desired tag
    {
        Player,
        Ground,
        Platform,
    }

    private Tag tag;
    #endregion

    #region MoveSelectedObjects
    private Object[] selectedObjects;
    string destinationFolder = "Assets";
    #endregion
  
    private Vector2 verticalScroll = Vector2.zero;

    [MenuItem("Window/Editor")]
    private static void OpenWindow()
    {
        EditorScript window = (EditorScript)GetWindow(typeof(EditorScript));
        window.minSize = new Vector2(200, 250);
        window.maxSize = new Vector2(350, 400);
        window.Show();
    }

    private void OnGUI()
    {
        verticalScroll = EditorGUILayout.BeginScrollView(verticalScroll);

        #region Clone the selected object. Basic
        GUILayout.Label("Clone The Selected Object", EditorStyles.boldLabel);
        GUILayout.Space(3);

        //Sets the number of objects to be cloned
        //I limited it between 1 and 10, but you can adjust it as you wish
        cloneCount = EditorGUILayout.IntSlider("Clone Count", cloneCount, 1, 10);

        //It creates a button and checks whether it is pressed or not
        if (GUILayout.Button("Clone the Object"))
        {
            if (Selection.activeGameObject != null)
            {
                for (int i = 0; i < cloneCount; i++)
                {
                    //Clone the selected object and store it in the obj variable.
                    GameObject obj = Instantiate(Selection.activeGameObject);

                    //Select the cloned object in the hierarchy
                    Selection.activeGameObject = obj;

                    //The process of editing the object's name.
                    //If this is not done, at the end of each cloned object, '(Clone)' will be written
                    obj.name = obj.name.Replace("(Clone)", "");
                }
            }
            else // If the object is not selected, display a warning message
            {
                Debug.LogWarning("Please Select an Object!!");
            }
        }
        #endregion

        #region Create folder
        GUILayout.Space(10);
        GUILayout.Label("Create Folder", EditorStyles.boldLabel);

        //The variable that determines the file name
        folderName = EditorGUILayout.TextField(folderName);

        //It creates a button and checks whether it is pressed or not
        if (GUILayout.Button("Create Folder"))
        {
            //Warning message
            if (folderName == string.Empty)
            {
                Debug.LogWarning("Please enter the file name.");
            }
            else
            {
                //It creates a file with the name I selected under Assets
                AssetDatabase.CreateFolder("Assets", folderName);

                folderName = string.Empty;
            }
        }
        GUILayout.Space(5);
        #endregion

        #region Select objects
        GUILayout.Space(10);
        GUILayout.Label("Select Objects", EditorStyles.boldLabel);

        tag = (Tag)EditorGUILayout.EnumPopup("Tag:", tag);

        //It creates a button and checks whether it is pressed or not
        if (GUILayout.Button("Select Objects"))
        {
            //Create an array and store all objects with the selected tag in the array
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag.ToString());

            Selection.objects = gameObjects;

            //If array is empty
            if (gameObjects.Length == 0)
            {
                Debug.LogWarning("The selected object could not be found in the scene.");
            }
        }
        #endregion

        #region Move Selected Objects
        GUILayout.Space(10);
        GUILayout.Label("Move Selected Objects", EditorStyles.boldLabel);
        destinationFolder = EditorGUILayout.TextField("Destination folder:", destinationFolder);

        if (GUILayout.Button("Move Selected"))
        {
            selectedObjects = Selection.objects;
            MoveSelectedObjects();
            selectedObjects = null;
            destinationFolder = "Assets/";
        }
        #endregion

        EditorGUILayout.EndScrollView();

    }

    private void MoveSelectedObjects()
    {
        if (selectedObjects != null && selectedObjects.Length > 0)
        {
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                Object obj = selectedObjects[i];

                //Her varlýk için, öncelikle varlýðýn AssetDatabase içindeki yolunu (assetPath) alýr. Bu yol, varlýðýn mevcut konumunu temsil eder.
                string assetPath = AssetDatabase.GetAssetPath(obj);

                //Path.GetFileName() fonksiyonu ile varlýðýn dosya adýný(fileName) alýr.Bu, varlýðýn adýný temsil eder.
                string fileName = Path.GetFileName(assetPath);

                //Path.Combine() fonksiyonu ile hedef klasör ile varlýðýn dosya adý birleþtirilir ve yeni bir varlýk yolu(newAssetPath) oluþturulur.
                //Bu, varlýðýn yeni konumunu temsil eder.
                string newAssetPath = Path.Combine(destinationFolder, fileName);

                AssetDatabase.MoveAsset(assetPath, newAssetPath);
            }
            AssetDatabase.Refresh();
        }
    }
}