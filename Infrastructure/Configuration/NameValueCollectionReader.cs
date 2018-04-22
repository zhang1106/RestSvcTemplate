using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;

namespace AiDollar.Infrastructure.Configuration
{
    public class NameValueCollectionReader
    {
        public static NameValueCollection ReadFrom(string path)
        {
            var doc = XDocument.Load(path);
            var q =
                from e in doc.Root?.Elements("add")
                select new KeyValuePair<string, string>(
                    e.Attribute("key").Value,
                    e.Attribute("value").Value);

            var collection = new NameValueCollection();
            foreach (var item in q)
            {
                collection.Add(item.Key, item.Value);
            }

            return collection;
        }
    }
}
