using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;

namespace AiDollar.Infrastructure.Configuration
{
    public abstract class AppSettingsBase
    {
        protected string Prefix { get; }

        protected AppSettingsBase(string prefix = "")
        {
            Prefix = prefix;
        }

        protected T GetGlobalValue<T>(Expression<Func<T>> property, T defaultValue = default(T))
        {
            return GetValue(GetPropertyName(property), string.Empty, defaultValue);
        }

        protected T GetValue<T>(Expression<Func<T>> property, T defaultValue = default(T))
        {
            return GetValue(GetPropertyName(property), Prefix, defaultValue);
        }

        protected T GetValue<T>(string appSetting, T defaultValue = default(T))
        {
            return GetValue(appSetting, Prefix, defaultValue);
        }

        protected Dictionary<string, string> GetValueCollection(string prefix)
        {
            var returnDictionary = new Dictionary<string, string>();

            var namedValueCollection = ConfigurationManager.GetSection(prefix) as NameValueCollection;
            if (namedValueCollection == null) return returnDictionary;

            foreach (var kvp in namedValueCollection.AllKeys)
                returnDictionary[kvp] = namedValueCollection[kvp];

            return returnDictionary;
        }

        protected T GetValue<T>(string appSetting, string prefix, T defaultValue = default(T))
        {
            var namedValueCollection = ConfigurationManager.GetSection(prefix) as NameValueCollection;

            var value = namedValueCollection == null ? ConfigurationManager.AppSettings[prefix + appSetting] : namedValueCollection[appSetting];

            if (value == null)
            {
                return defaultValue;
            }

            if (typeof(T).IsArray)
            {
                string[] data = value.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).ToArray();

                var elementType = typeof(T).GetElementType();
                var converter = TypeDescriptor.GetConverter(elementType);
                var result = Array.CreateInstance(elementType, data.Length);
                for (int i = 0; i < data.Length; i++)
                {
                    result.SetValue(converter.ConvertFromString(data[i]), i);
                }

                return (T) (object) result;
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                return (T) converter.ConvertFrom(value);
            }
        }

        protected string GetPropertyName<T>(Expression<Func<T>> property)
        {
            var expr = property.Body as MemberExpression;
            return expr?.Member.Name;
        }
    }
}
