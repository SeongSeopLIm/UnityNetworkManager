using System;
using System.Runtime.CompilerServices;
using UnityEngine.Networking;

namespace Network
{
    public class UnityWebRequestAwaiter : INotifyCompletion
    {
        UnityWebRequestAsyncOperation asyncOperation;

        public bool IsCompleted => asyncOperation.isDone;

        public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOperation)
        {
            this.asyncOperation = asyncOperation;
        }

        public void GetResult()
        {
            // NOTE : 결과는 UnityWebRequest에서 액세스 할 수 있으므로 여기에서 반환 필요는 없다 
        }

        public void OnCompleted(Action continuation)
        {
            asyncOperation.completed += _ => { continuation(); };
        }
    }
}