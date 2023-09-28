using UnityEngine;
using UnityEditor;
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

        #region Cloning objects at an intermediate level
        EditorGUILayout.Space(10);

        GUILayout.Label("Cloning Objects At An Intermediate Level", EditorStyles.boldLabel);

        EditorGUILayout.Space(1);

        if (GUILayout.Button("Clone The Object In Detail"))
        {
            CloneTheObjectInDetail.OpenWindow();
        }

        #endregion

        EditorGUILayout.EndScrollView();
       
    }   
}



public class CloneTheObjectInDetail : EditorWindow
{
    public static CloneTheObjectInDetail instance;

    #region Variables
    List<Object> allCloneObjects = new List<Object>();
    List<Object> lastCloneObjects = new List<Object>();

    private Vector2 verticalScroll = Vector2.zero;

    private Object objectField;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    public int cloneNumber;
    public int leftValue = 1;
    public int rightValue = 10;

    private bool positionToogle;
    #endregion

    public static void OpenWindow()
    {
        CloneTheObjectInDetail window = (CloneTheObjectInDetail)GetWindow(typeof(CloneTheObjectInDetail));
        window.minSize = new Vector2(200, 250);
        window.maxSize = new Vector2(350, 400);
        window.Show();
    }

    private void OnEnable()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else if (instance == null)
        {
            instance = this;
        }
    }

    private void OnGUI()
    {
        verticalScroll = EditorGUILayout.BeginScrollView(verticalScroll);

        EditorGUILayout.Space(2);
        EditorGUILayout.LabelField("Create Random Objects", EditorStyles.boldLabel);
        EditorGUILayout.Space(2);

        //Determine the object to be cloned
        objectField = EditorGUILayout.ObjectField("Object:", objectField, typeof(Object), true);

        //Toggle the Random Position fields on and off
        positionToogle = EditorGUILayout.Toggle("Set Random Position:", positionToogle);

        if (positionToogle)
        {
            minX = EditorGUILayout.FloatField("MinX:", minX);
            maxX = EditorGUILayout.FloatField("MaxX:", maxX);
            minY = EditorGUILayout.FloatField("MinY:", minY);
            maxY = EditorGUILayout.FloatField("MaxY:", maxY);
        }

        EditorGUILayout.Space(2);

        //Sets the number of objects to be cloned
        //You can change the value range from the settings menu
        cloneNumber = EditorGUILayout.IntSlider("Number of Clones:", cloneNumber, leftValue, rightValue); 

        EditorGUILayout.Space(5);

        #region Create Objects
        if (GUILayout.Button("Create", GUILayout.Height(30)))
        {
            if (objectField != null)
            {
                lastCloneObjects.Clear();
                for (int i = 1; i <= cloneNumber; i++)
                {
                    Object obj = Instantiate(objectField, new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY)), Quaternion.identity);
                    allCloneObjects.Add(obj);
                    lastCloneObjects.Add(obj);

                    obj.name = objectField.name + "_" + i;
                }
            }
            else
            {
                Debug.LogWarning("Please place an object into the ObjectField");
            }
        }
        #endregion

        EditorGUILayout.Space(10);

        #region Delete All Clone Objects
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete All Clone Objects", GUILayout.Height(25)))
        {
            if (allCloneObjects.Count != 0)
            {
                for (int i = 0; i < allCloneObjects.Count; i++)
                {
                    Object obj = allCloneObjects[i];
                    DestroyImmediate(obj);
                }
            }
        }
        #endregion

        #region Delete Last Clone Objects
        if (GUILayout.Button("Delete Last Clone Objects", GUILayout.Height(25)))
        {
            if (lastCloneObjects.Count != 0)
            {
                for (int i = 0; i < lastCloneObjects.Count; i++)
                {
                    Object obj = lastCloneObjects[i];
                    DestroyImmediate(obj);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        EditorGUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        //SettingsButton
        if (GUILayout.Button("Settings", GUILayout.Width(80)))
        {
            CloneTheObjectInDetailSettings.OpenWindow();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }
}


//-----------------Settings Menu---------------------\\
public class CloneTheObjectInDetailSettings : EditorWindow
{
    private int settingLeftValue = 1;
    private int settingRightValue;

    public static void OpenWindow()
    {
        CloneTheObjectInDetailSettings window = (CloneTheObjectInDetailSettings)GetWindow(typeof(CloneTheObjectInDetailSettings));
        window.minSize = new Vector2(150, 200);
        window.maxSize = new Vector2(250, 300);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Number Of Clones Settings", EditorStyles.boldLabel);
        settingLeftValue = EditorGUILayout.IntField("Min Value:", Mathf.Abs(settingLeftValue));
        settingRightValue = EditorGUILayout.IntField("Max Value:", Mathf.Abs(settingRightValue));

        if (GUILayout.Button("Save And Exit"))
        {
            if (settingLeftValue < settingRightValue)
            {
                CloneTheObjectInDetail.instance.leftValue = settingLeftValue;
                CloneTheObjectInDetail.instance.rightValue = settingRightValue;
                CloneTheObjectInDetail.instance.cloneNumber = CloneTheObjectInDetail.instance.leftValue;
                Close();
            }
            else
            {
                Debug.LogError("settingLeftValue cannot be the same as or greater than settingRightValue");
            }
        }
    }
}