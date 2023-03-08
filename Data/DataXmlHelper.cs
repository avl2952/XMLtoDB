using System.Runtime.Serialization;
using System.Xml;

namespace Data
{
	public sealed  class DataXmlHelper
	{
		public static T Deserialize<T>(string data)
		{
			using (StringReader reader = new StringReader(data))
			{
				using (XmlReader xmlReader = XmlReader.Create(reader))
				{
					var serializer = new DataContractSerializer(typeof(T));
					T theObject = (T)serializer.ReadObject(xmlReader);
					return theObject;
				}
			}
		}

		public static string Serialize<T>(T theObject)
		{
			using (var memoryStream = new MemoryStream())
			{
				var serializer = new DataContractSerializer(typeof(T));
				serializer.WriteObject(memoryStream, theObject);

				memoryStream.Seek(0, SeekOrigin.Begin);

				var reader = new StreamReader(memoryStream);
				string content = reader.ReadToEnd();
				return content;
			}
		}
	}
}
