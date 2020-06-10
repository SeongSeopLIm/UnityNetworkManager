using LitJson;
using Network;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Extensions
{
    public static class UnityExtensions
    {
        public static void SetActive(this GameObject[] objs, bool isActive)
        {
            for (var i = 0; i < objs.Length; i++)
            {
                objs[i].SetActive(isActive);
            }
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() ?? component.gameObject.AddComponent<T>();
        }
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }
        public static Texture2D ToTexture2D(this Texture texture)
        {
            Texture2D capture = null;
            RenderTexture renderTexture = null;

            if (capture == null)
                capture = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
            if (renderTexture == null)
                renderTexture = new RenderTexture(texture.width, texture.height, 32);

            Graphics.Blit(texture, renderTexture); 
            RenderTexture.active = renderTexture;

            capture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            capture.Apply();

            return capture;
        }
        

        #region WWW
        public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAwaiter(asyncOp);
        }

        public static bool IsSuccess(this UnityWebRequest www)
        {
            return www.responseCode == 200;
        }

        public static bool CheckResponseCode(this UnityWebRequest res, params int[] responseCodes)
        {
            foreach (var t in responseCodes)
            {
                if (res.responseCode == t)
                    return true;
            }

            return false;
        }

        public static bool CheckErrorType(this UnityWebRequest res, string key, string value)
        {
            return res.GetErrorType(key).Equals(value);
        }

        public static bool CheckErrorType(this UnityWebRequest res, int responseCode, string key, string value)
        {
            return res.responseCode == responseCode && res.GetErrorType(key).Equals(value);
        }

        public static string GetErrorType(this UnityWebRequest res, string key)
        {
            var jsonData = JsonMapper.ToObject(res.downloadHandler.text);
            if (!jsonData.ContainsKey(key))
                return string.Empty;

            return jsonData[key].ToString();
        }
        public static void Add(this WWWForm form, string fieldName, int i)
        {
            form.Add(fieldName, i.ToString());
        }

        public static void Add(this WWWForm form, string fieldName, string value)
        {
            form.Add(fieldName, value, Encoding.UTF8);
        }

        public static void Add(this WWWForm form, string fieldName, string value, Encoding e)
        {
            form.AddField(fieldName, value, e);
        }
        #endregion
    }

}