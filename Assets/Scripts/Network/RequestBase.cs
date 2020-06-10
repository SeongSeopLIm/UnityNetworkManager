using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static Scenes.Data.RequestDataSetting;
using NetworkManager = Manager.NetworkManager;

namespace Network
{
    public enum Method
    {
        Get,
        Post,
        Put,
        Delete,
        Patch
    }

    public class RequestBase
    {
        public virtual string EndPoint { get; set; }
        public Method Method;
        public bool IsAddAuthorization;
        public RequestData requestData;
        public bool IsTestRequest => requestData.IsTest;

        public Dictionary<string, string> Headers = new Dictionary<string, string>();
        public Dictionary<string, string> Queries = new Dictionary<string, string>();


        public async Task<Manager.ResponseData> Send()
        {
            return await NetworkManager.Instance.Send(this);
        }

        public string GetEndPoint()
        {
            var endPoint = EndPoint;

            if (Queries.Count > 0)
            {
                endPoint = string.Concat(endPoint, "?", GetQueries());
            }

            return endPoint;
        }

        public virtual WWWForm GetWWWForm()
        {
            var form = new WWWForm();
            return form;
        }

        private string GetQueries()
        {
            return string.Join("&", Queries.Select(l => $"{l.Key}={l.Value}"));
        }

        public void Set(RequestData requestData)
        {
            EndPoint = requestData.URI;
            Method = requestData.Method;
            IsAddAuthorization = requestData.IsAddAuthorization;
            this.requestData = requestData;
        } 

        public UnityWebRequest ToUnityWebRequest(string url)
        {
            UnityWebRequest www;
            var endPoint = GetEndPoint();
            switch (Method)
            {
                case Method.Post:
                    if (this is MultipartRequest)
                    {
                        www = new UnityWebRequest(url + endPoint, "post");
                    }
                    else
                    {
                        www = UnityWebRequest.Post(url + endPoint, GetWWWForm());
                    }
                    break;
                case Method.Put:
                    www = UnityWebRequest.Put(url + endPoint, GetWWWForm().data);
                    break;
                case Method.Delete:
                    www = UnityWebRequest.Delete(url + endPoint);
                    break;
                case Method.Get:
                    www = UnityWebRequest.Get(url + endPoint);
                    break;
                case Method.Patch:
                    if (this is MultipartRequest)
                    {
                        www = new UnityWebRequest(url + endPoint, "patch");
                    }
                    else
                    {
                        www = new UnityWebRequest(
                            url + endPoint,
                            "patch",
                            new DownloadHandlerBuffer(),
                            new UploadHandlerRaw(GetWWWForm().data)
                            {
                                contentType = "application/x-www-form-urlencoded"
                            });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CheckAuthorization();

            if (Headers != null)
            {
                var enumerator = Headers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    www.SetRequestHeader(enumerator.Current.Key, enumerator.Current.Value);
                }

                enumerator.Dispose();
            }

            if (this is MultipartRequest)
            {
                var fileProcess = this as MultipartRequest;
                fileProcess.PreSend(ref www);
            }

            return www;
        }

        private void CheckAuthorization()
        {
            if (!IsAddAuthorization) return;
            var accessToken = NetworkManager.Instance.AccessToken;
            if (string.IsNullOrEmpty(accessToken))
                return; 

            if (Headers == null)
                Headers = new Dictionary<string, string>();

            accessToken = "Bearer " + accessToken;

            Debug.Log("AccessToken : " + accessToken);

            Headers.Add("Authorization", accessToken);
        }
    }

}

namespace Network.Request
{ 
    #region GET 
    
    public class GetServerTime : RequestBase
    {
        public override string EndPoint { get => string.Format(base.EndPoint); set => base.EndPoint = value; }


        public GetServerTime(RequestData newRequestData)
        {
            Set(newRequestData);
        }
    }
     

    #endregion

    #region POST 

    public class PostUploadImage : MultipartRequest
    { 
        public override string EndPoint { get => string.Format(base.EndPoint, userSeq); set => base.EndPoint = value; }
        public string userSeq;
        public Texture2D image;  

        public PostUploadImage(RequestData newRequestData, string newUserSeq, Texture2D image)
        {
            userSeq = newUserSeq;
            this.image = image;   
            Set(newRequestData);
        }

        public override void PreSend(ref UnityWebRequest www)
        { 
            multiparts.Clear(); 
            AddImage("file", "thumb", image); 
            base.PreSend(ref www);
        }
    }
    #endregion
}