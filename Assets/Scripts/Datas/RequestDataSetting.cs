using Manager;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Scenes.Data
{
    public class RequestDataSetting : ScriptableObject
    {

        public enum Request_Default
        {
            Login = 0,
            GetServerTime,
            End,

            None = -1,
            Start = Request_Default.Login,
        }

        public enum Request_MobileUser
        {
            PostUploadImage = Request_Default.End,
            End,

            None = Request_Default.None,
            Start = Request_MobileUser.PostUploadImage,
        }

        public enum Request_DesktopUser
        {
            PostUploadImage = Request_MobileUser.End,
            End,

            None = Request_Default.None,
            Start = Request_DesktopUser.PostUploadImage,
        }



        [System.Serializable]
        public class ServerURLs
        {
            public string serverURL; 
        }

        [System.Serializable]
        public class TestResponse
        {
            [TextArea]
            public string res; 
        }

        [System.Serializable]
        public class RequestData
        { 
            public string URI;
            public Network.Method Method;
            public bool IsAddAuthorization;
            public bool IsTest; 
            public TestResponse testResponse;
        }

        public ServerType targetServerType;
        [SerializeField] private ServerURLs[] serverURLDatas;
        [SerializeField] private List<RequestData> defaultRequestDatas = new List<RequestData>();
        [SerializeField] private List<RequestData> mobileUserRequestDatas = new List<RequestData>();
        [SerializeField] private List<RequestData> desktopUserRequestDatas = new List<RequestData>();

        private List<RequestData> requestDatas = new List<RequestData>();


        [HideInInspector]
        public ServerURLs ServerURLData => serverURLDatas[(int)targetServerType];

        [HideInInspector] public List<RequestData> RequestDatas 
        { 
            get
            {
                if(requestDatas.Count != (int)Request_DesktopUser.End)
                {
                    requestDatas.Clear(); 
                    requestDatas.AddRange(defaultRequestDatas);
                    requestDatas.AddRange(mobileUserRequestDatas);
                    requestDatas.AddRange(desktopUserRequestDatas);

                    foreach(var requestData in requestDatas)
                    {
                        if(requestData.URI == "")
                        {
                            Debug.LogError("Empty requestData URI");
                            continue;
                        }
                        requestData.URI = requestData.URI[0] == '/' ? requestData.URI : "/" + requestData.URI; 
                    }

                }
                
                return requestDatas;
            }
        }

        public void Clear()
        {
            requestDatas.Clear();
        }

        static RequestDataSetting asset;

#if UNITY_EDITOR
        [MenuItem("Setting/Create Request Data Setting")]
        private static void CreatingInstance()
        {
            var setting = CreateInstance<RequestDataSetting>();
            AssetDatabase.CreateAsset(setting, "Assets/Resources/Settings/RequestDataSetting_Created.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(); 
        }
#endif  

    }
}