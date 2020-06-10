using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Network
{
    public class ResponseBase<T>
    {
        public T result;
        public string error;

        public bool IsSuccess => error == "";

        public static ResponseBase<T> Parse(string json)
        { 
            var result = JsonUtility.FromJson<ResponseBase<T>>(json);
            return result;
        }
    }
}