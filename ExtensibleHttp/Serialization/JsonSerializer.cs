﻿/*
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using ExtensibleHttp.Interfaces;
using Newtonsoft.Json;
using System.IO;

namespace ExtensibleHttp.Serialization
{
    public class JsonSerializer : ISerializer
    {
        public TPayload Deserialize<TPayload>(string content)
        {
            return JsonConvert.DeserializeObject<TPayload>(content);
        }

        /// <summary>
        /// Converts to json string and returns
        /// </summary>
        /// <returns></returns>
        public string Serialize<TPayload>(TPayload item)
        {
            var stringWriter = new StringWriter();
            using (JsonWriter jsonWriter = new JsonTextWriter(stringWriter))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.Serialize(jsonWriter, item);
            }
            return stringWriter.ToString();
        }
    }
}
