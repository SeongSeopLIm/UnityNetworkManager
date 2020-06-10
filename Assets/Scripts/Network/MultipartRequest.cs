using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
    public class MultipartRequest : RequestBase
    {

        public class MultipartFileData
        {
            public string key = "";
            public string path = "";
            public ContentType contentType;

            public MultipartFileData(string key, string path, ContentType contentType)
            {
                this.key = key;
                this.path = path;
                this.contentType = contentType;
            }
        }

        public enum ContentType
        {
            Image,
            Audio,
            Video,
        }

        protected List<IMultipartFormSection> multiparts = new List<IMultipartFormSection>();

        public void AddImage(string key, string name, Texture2D texture)
        {
            var contentTypeString = GetContentTypeString(ContentType.Image); 
            Debug.Log("Start EncodeToPNG PreSend. Readable : " + texture.isReadable);
            
            var data = texture.EncodeToPNG(); 
            Debug.Log("End EncodeToPNG PreSend");
            multiparts.Add(new MultipartFormFileSection(key, data, string.Format("{0}.png", name), contentTypeString));
        }
         
        public void AddFile(string key, string path, ContentType contentType)
        {
            if (!File.Exists(path))
            {
                Debug.Log("Faild AddFile NotFoundFIld. Path : " + path);
                return;
            }
            var extension = Path.GetExtension(path);
            var name = Path.GetFileNameWithoutExtension(path);
            string sendFilePath = Path.GetDirectoryName(path) + "/multipart_temp_teacher";
            File.Copy(path, sendFilePath, true);
            var data = File.ReadAllBytes(sendFilePath);
            var contentTypeString = GetContentTypeString(contentType);
            multiparts.Add(new MultipartFormFileSection(key, data, string.Format("{0}{1}", name, extension), contentTypeString));

        }

        string GetContentTypeString(ContentType contentType)
        {
            string typeString = "image/png";
            switch (contentType)
            {
                case ContentType.Image:
                    typeString = "image/png";
                    break;
                case ContentType.Audio:
                    typeString = "audio/mpeg";
                    break;
                case ContentType.Video:
                    typeString = "video/mpeg";
                    break;
            }
            return typeString;
        }

        public virtual void PreSend(ref UnityWebRequest www)
        {
            if (multiparts.Count == 0)
                return;

            var boundary = UnityWebRequest.GenerateBoundary();
            var formSections = UnityWebRequest.SerializeFormSections(multiparts, boundary);
            var terminate = Encoding.UTF8.GetBytes(string.Concat("\r\n--", Encoding.UTF8.GetString(boundary), "--"));
            var body = new byte[formSections.Length + terminate.Length];

            Buffer.BlockCopy(formSections, 0, body, 0, formSections.Length);
            Buffer.BlockCopy(terminate, 0, body, formSections.Length, terminate.Length);

            var contentType = string.Concat("multipart/form-data; boundary=", Encoding.UTF8.GetString(boundary));
            var uploader = new UploadHandlerRaw(body)
            {
                contentType = contentType
            };

            www.uploadHandler = uploader;
            www.downloadHandler = new DownloadHandlerBuffer();
        }
    }
}