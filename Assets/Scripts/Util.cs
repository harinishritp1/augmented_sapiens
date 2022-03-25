using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public enum Orientation
{
    NONE,
    PORTRAIT,
    LANDSCAPE
}

public enum DeviceType
{
    NONE,
    MOBILE,
    TABLET
}

public static class Util
{
    /// <summary>
    /// resolutions
    /// </summary>
    //tablet
    //return 800f;//800x1280-Portrait
    //return 1280f;//800x1280-Landscape
    //return 1536f;//1536x2048-Portrait
    //return 2048f;//1536x2048-Landscape
    //mobile
    //return 1440f;//2960x1440-Portrait
    //return 2960f;//2960x1440-Landscape
    //return 1080f;//1920x1080-Portrait
    //return 1920f;//1920x1080-Landscape
    static Vector2 _screenSize = new Vector2(0f, 0f);
    public static float ScreenWidth
    {
        get
        {
            /*if (_screenSize == Vector2.zero) */GetMainGameViewSize();
            return _screenSize.x;
        }
    }
    public static float ScreenHeight
    {
        get
        {
            /*if (_screenSize == Vector2.zero) */GetMainGameViewSize();
            return _screenSize.y;
        }
    }
    public static float ScreenSmallerSize
    {
        get
        {
            GetMainGameViewSize();
            return _screenSize.x < _screenSize.y ? _screenSize.x : _screenSize.y;
        }
    }
    public static float ScreenBiggerSize
    {
        get
        {
            GetMainGameViewSize();
            return _screenSize.x > _screenSize.y ? _screenSize.x : _screenSize.y;
        }
    }
    static void GetMainGameViewSize()
    {//refresh too
#if UNITY_EDITOR
        Type T = Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
        _screenSize = (Vector2)Res;
#else
        _screenSize = new Vector2(Screen.width, Screen.height);
#endif
    }

    public static Orientation GetOrientation()
    {
        GetMainGameViewSize();
        return ScreenWidth > ScreenHeight ? Orientation.LANDSCAPE : Orientation.PORTRAIT;
    }
    public static List<T> GetEnumList<T>()
    {
        var enumList = Enum.GetValues(typeof(T))
            .Cast<T>().ToList();
        return enumList;
    }
    static DeviceType _deviceType = DeviceType.NONE;
    public static DeviceType GetDevice()
    {
        if (_deviceType != DeviceType.NONE) return _deviceType;
        Debug.Log("GetDevice: " + (DeviceDiagonalSizeInInches() > 6.5f ? DeviceType.TABLET : DeviceType.MOBILE));
#if !UNITY_EDITOR
        _deviceType =  (DeviceDiagonalSizeInInches() > 6.5f ? DeviceType.TABLET : DeviceType.MOBILE);
#else
        Debug.Log("GetDevice() device = tablet?" + ((ScreenWidth < ScreenHeight && ScreenHeight / ScreenWidth < 1.7f)
           ||
            (ScreenWidth > ScreenHeight && ScreenWidth / ScreenHeight < 1.7f)));
        if ((ScreenWidth < ScreenHeight && ScreenHeight / ScreenWidth < 1.7f)//portrait
           ||
            (ScreenWidth > ScreenHeight && ScreenWidth / ScreenHeight < 1.7f))//landscape
            _deviceType = DeviceType.TABLET;
        else _deviceType = DeviceType.MOBILE;
#endif
        return _deviceType;
    }

    public static bool IsIPhoneX()
    {
        return ScreenHeight / ScreenWidth >= 2;
    }
    //calculate physical inches with pythagoras theorem
    public static float DeviceDiagonalSizeInInches()
    {
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));

        Debug.Log("Getting device inches: " + diagonalInches);

        return diagonalInches;
    }


    #region valid_mail
    public const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
    public static bool IsEmail(string email)
    {
        if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
        else return false;
    }
    #endregion

    #region rect_transform
    //PanelSpacerRectTransform.offsetMin = new Vector2(0, 0); // new Vector2(left, bottom); 
    //PanelSpacerRectTransform.offsetMax = new Vector2(-360, -0); // new Vector2(-right, -top);
    public static void SetOffsetLeft(RectTransform _rect, float left)
    {
        _rect.offsetMin = new Vector2(left, _rect.offsetMin.y);
    }
    public static void SetOffsetBottom(RectTransform _rect, float bottom)
    {
        _rect.offsetMin = new Vector2(_rect.offsetMin.x, bottom);
    }
    public static void SetOffsetLeftBottom(RectTransform _rect, float left, float bottom)
    {
        _rect.offsetMin = new Vector2(left, bottom);
    }
    public static void SetOffsetRight(RectTransform _rect, float right)
    {
        _rect.offsetMax = new Vector2(-right, _rect.offsetMax.y);
    }
    public static void SetOffsetTop(RectTransform _rect, float top)
    {
        _rect.offsetMax = new Vector2(_rect.offsetMax.x, -top);
    }
    public static void SetOffsetRightTop(RectTransform _rect, float right, float top)
    {
        _rect.offsetMax = new Vector2(-right, -top);
    }
    public static float GetLeft(RectTransform _rect)
    {
        return _rect.offsetMin.x;
    }
    public static float GetBottom(RectTransform _rect)
    {
        return _rect.offsetMin.y;
    }
    public static float GetRight(RectTransform _rect)
    {
        return _rect.offsetMax.x;
    }
    public static float GetTop(RectTransform _rect)
    {
        return _rect.offsetMax.y;
    }
    #endregion

    public static int GetFirstDiffChar(string s1, string s2)
    {
        int N = s1.Length < s2.Length ? s1.Length : s2.Length;
        for (int i = 0; i < N; i++)
            if (s1[i] != s2[i]) return i;
        return N;
    }

    public static void SetImageText(Text text, string name, int maxCharNo)
    {
        if (name.Length <= maxCharNo)
            text.text = name;
        else
            text.text = name.Substring(0, maxCharNo) + "...";
    }


    public static GameObject[] GetClosestBoxColliderNodes(Transform centerPoint, BoxCollider boxCollider, Vector3 position, float size)
    {//returns closest point + 3 nodes that are connected by edges with it
        float[] sizes = new float[3] { boxCollider.bounds.size.x * size / 2, boxCollider.bounds.size.y * size / 2, boxCollider.bounds.size.z * size / 2 };
        GameObject[] nodes = new GameObject[8];
        for (int i = 0; i < 8; i++)
        {
            nodes[i] = new GameObject();
            nodes[i].name = "node" + i;
            nodes[i].transform.SetParent(centerPoint);
            nodes[i].transform.localPosition = new Vector3(
                ((i & (1 << 0)) != 0 ? 1f : -1f) * (sizes[0] + 0.001f),
                ((i & (1 << 1)) != 0 ? 1f : -1f) * (sizes[1] + 0.001f),
                ((i & (1 << 2)) != 0 ? 1f : -1f) * (sizes[2] + 0.001f)
                );
        }
        int minIndex = 0;
        float minDistance = -1;
        int j = 0;
        do
        {
            float distance = Vector3.Distance(position, nodes[j].transform.position);

            if (nodes[j].transform.localPosition.y > 0f)
                if (minDistance == -1 || distance < minDistance)
                {
                    minDistance = distance;
                    minIndex = j;
                }

            j++;
        } while (j < 8);
        //nodes[minIndex].name += "closest";
        //return nodes[minIndex];

        GameObject[] returnNodes = new GameObject[4];
        GameObject closestNode = nodes[minIndex];
        returnNodes[0] = closestNode;

        int k = 1;
        foreach (GameObject node in nodes)
        {
            if (closestNode == node) continue;
            if ((node.transform.localPosition.x == closestNode.transform.localPosition.x && node.transform.localPosition.y == closestNode.transform.localPosition.y)
                || (node.transform.localPosition.x == closestNode.transform.localPosition.x && node.transform.localPosition.z == closestNode.transform.localPosition.z)
                || (node.transform.localPosition.y == closestNode.transform.localPosition.y && node.transform.localPosition.z == closestNode.transform.localPosition.z)
                    )
                returnNodes[k++] = node;
        }

        return returnNodes;
    }
    public static Vector3 GetClosestBoxColliderPoint(Vector3 colliderPosition, BoxCollider boxCollider, Vector3 position)
    {
        Bounds bounds = boxCollider.bounds;
        float xSize = bounds.size.x / 2, ySize = bounds.size.y / 2, zSize = bounds.size.z / 2;
        //Vector3 colliderPosition = boxCollider.transform.position + boxCollider.center;
        colliderPosition -= boxCollider.center;
        Vector3[] points = new Vector3[8]{
        new Vector3(colliderPosition.x - xSize, colliderPosition.y - ySize, colliderPosition.z - zSize),
        new Vector3(colliderPosition.x - xSize, colliderPosition.y - ySize, colliderPosition.z + zSize),
        new Vector3(colliderPosition.x - xSize, colliderPosition.y + ySize, colliderPosition.z - zSize),
        new Vector3(colliderPosition.x - xSize, colliderPosition.y + ySize, colliderPosition.z + zSize),
        new Vector3(colliderPosition.x + xSize, colliderPosition.y - ySize, colliderPosition.z - zSize),
        new Vector3(colliderPosition.x + xSize, colliderPosition.y - ySize, colliderPosition.z + zSize),
        new Vector3(colliderPosition.x + xSize, colliderPosition.y + ySize, colliderPosition.z - zSize),
        new Vector3(colliderPosition.x + xSize, colliderPosition.y + ySize, colliderPosition.z + zSize)
        };
        int minIndex = 0;
        float minDistance = -1;

        int i = 0;
        do
        {
            float distance = Vector3.Distance(position, points[i]);

            if (minDistance == -1 || distance < minDistance)
            {
                minDistance = distance;
                minIndex = i;
            }            

            i++;
        } while (i < 8);

        return points[minIndex];
    }

    public static bool IsPointerOverGameObject()
    {
        foreach (Touch touch in Input.touches)
        {
            int id = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id))
            {
                return true;
            }
        }
        return false;
    }

//     public static bool IsTrackingGood()
//     {
//         return
// #if UNITY_WSA
//             MainTrackableEventHandler.Tracking;
// #else
//             InteractionController.Instance.placementPoseIsValid;
// #endif
//     }

    public static int CompareVersions(string v1, string v2)
    {
        var version1 = new Version(v1);
        var version2 = new Version(v2);

        var result = version1.CompareTo(version2);
        if (result > 0)
            Debug.Log("version1 is greater");
        else if (result < 0)
            Debug.Log("version2 is greater");
        else
            Debug.Log("versions are equal");
        return result;
    }

    public static Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    public static void SetTextAlpha(Text text, float alpha)
    {
        var tempColor = text.color;
        tempColor.a = alpha;
        text.color = tempColor;
    }

    // public static Dictionary<string, ModelData> ParseDictionary(string json)
    // {
    //     Dictionary<string, ModelData> dic = new Dictionary<string, ModelData>();
    //     try
    //     {
    //         dic = JsonConvert.DeserializeObject<Dictionary<string, ModelData>>(json);
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.Log("json exception: " + e.Message + "\n" + json);
    //     }
    //     return dic == null ? new Dictionary<string, ModelData>() : dic;
    // }
}

public class FolderData
{
    public int folder_id;
    public string name;
    public int parent_id;
    public List<int> children_ids;
    public List<ImageData> images_data;

    public FolderData(int folder_id, int parent_id, string name)
    {
        this.folder_id = folder_id;
        this.parent_id = parent_id;
        this.name = name;
        this.children_ids = new List<int>();
        this.images_data = new List<ImageData>();
    }
}

public class LibraryData
{
    public Dictionary<int, FolderData> storageFolders;
    public Dictionary<int, FolderData> modelFolders;
    public int root_folder_id;
}

public class ImageData
{
    public string key;
    public string name;
}