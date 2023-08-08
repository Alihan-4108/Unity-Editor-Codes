using UnityEngine;
using UnityEditor;

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
        Enemy
    }

    private Tag tag;
    #endregion

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
        #region Object Cloning
        GUILayout.Label("Clone Object", EditorStyles.boldLabel);
        GUILayout.Space(3);

        //Sets the number of objects to be cloned
        //I limited it between 1 and 10, but you can adjust it as you wish
        cloneCount = EditorGUILayout.IntField("Clone Count", Mathf.Clamp(cloneCount, 1, 10));

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

        #region Create Folder
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

        #region Select Objects
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
    }
}