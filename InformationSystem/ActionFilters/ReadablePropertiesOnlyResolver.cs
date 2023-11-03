using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;


namespace ICCModule.ActionFilters
{
    /// <summary>
    /// JsonSerializer 讀取屬性的解析器設定
    /// </summary>
    class ReadablePropertiesOnlyResolver : DefaultContractResolver
    {
        /// <summary>
        /// 建立可呈現（解析）的屬性
        /// </summary>
        /// <returns>呈現的屬性</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (typeof(Stream).IsAssignableFrom(property.PropertyType))
            {
                property.Ignored = true;
            }
            return property;
        }
    }
}
