using Extensions;
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks; 
using UnityEngine;
using UnityEngine.Networking;
using Scenes.Data;

namespace Manager
{
    public enum ServerType
    { 
        Test = 0,
        Live,
    }

    public class ResponseData
    {
        public long responseCode;
        public string responseText;
        public UnityWebRequest response;
        public ResponseData(string response)
        {
            
            this.responseText = response;
            responseCode = 200;
        }
        public ResponseData(UnityWebRequest response)
        {
            if (response.downloadHandler != null)
                this.responseText = response.downloadHandler.text;
            else
                this.responseText = "not received downloadhandler";
            this.responseCode = response.responseCode; 
            this.response = response;
        }
    }

    public class NetworkManager : PersistentSingleton<NetworkManager>
    {
        public ServerType TestServerType;
        public string ServerURL => RequestDatas.ServerURLData.serverURL;
        public RequestDataSetting RequestDatas
        {
            get
            {
                if (requestDatas == null)
                {
                    requestDatas = Resources.Load<RequestDataSetting>($"Settings/RequestDataSetting");
                    requestDatas.Clear();
                }
                return requestDatas;
            }
        }
        public string AccessToken { get => accessToken; set => accessToken = value; } 

        private RequestDataSetting requestDatas;
        private string accessToken;

        public RequestDataSetting.RequestData GetRequestData(int dataIndex)
        {
            return RequestDatas.RequestDatas[dataIndex];
        }

        public void SwitchRequestServerType(ServerType newServerType)
        {
            RequestDatas.targetServerType = newServerType; 
        }

        public async Task<ResponseData> Send(RequestBase req)
        {
            if(req.IsTestRequest)
            {
                ResponseData testResData = new ResponseData(req.requestData.testResponse.res);
                return testResData;
            }

            string serverURL = ServerURL; 

            var www = req.ToUnityWebRequest(serverURL);
            Debug.Log("Send [" + www.method + "]" + www.url);
            await www.SendWebRequest();
            Debug.Log("End Send" + www.responseCode + " - " + www.url );
            
            if (www.CheckResponseCode(0, 502))
            {
                //throw new Exception();
            }
            if(www.downloadHandler != null)
                Debug.Log(www.downloadHandler.text);


            ResponseData resData = new ResponseData(www); 
            return resData;
        }
    }
}