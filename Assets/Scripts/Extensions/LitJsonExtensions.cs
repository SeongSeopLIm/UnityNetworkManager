using LitJson;
using System.Collections;
using UnityEngine.Networking;

namespace Extensions
{
    public static class LitJsonExtensions
    {
        public static int ToInt(this JsonData json)
        {
            return json.ToString().ToInt();
        }
        public static JsonData ToJson(this UnityWebRequest res)
        {
            return JsonMapper.ToObject(res.downloadHandler.text);
        }
        public static bool ContainsKey(this JsonData data, string key)
        {
            if (data == null || !data.IsObject)
                return false;

            var dic = data as IDictionary;
            if (dic == null)
                return false;

            return dic.Contains(key);

        }
    }
}