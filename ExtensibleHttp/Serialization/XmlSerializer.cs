/*
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
using System.IO;
using System.Text;

namespace ExtensibleHttp.Serialization
{
	public class XmlSerializer : ISerializer
	{
		private sealed class Utf8StringWriter : StringWriter
		{
			public override Encoding Encoding => Encoding.UTF8;
		}

		public TPayload Deserialize<TPayload>(string content)
		{
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TPayload));
			using (var reader = new StringReader(content))
			{
				// This is verified in that we are deserializing to our own defined DTD
				// and not using the internal schema from the provided data.  
#pragma warning disable CA5369
#pragma warning disable CA3075
				return (TPayload)serializer.Deserialize(reader);
#pragma warning restore CA3075
#pragma warning restore CA5369
			}
		}

		/// <summary>
		/// Converts to xml string and returns
		/// </summary>
		/// <returns></returns>
		public string Serialize<TPayload>(TPayload item)
		{
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TPayload));
			using (var stringWriter = new Utf8StringWriter())
			{
				//if (((IPayload) item).Xmlns.Any())
				//    serializer.Serialize(stringWriter, item, ((IPayload) item).Xmlns);
				//else
				serializer.Serialize(stringWriter, item);

				return stringWriter.ToString();
			}
		}
	}
}
