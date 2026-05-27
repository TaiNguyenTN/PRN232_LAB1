using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace PRN232.Lab1.Services.Helpers
{
    public static class DataShaper
    {
        public static IEnumerable<ExpandoObject> ShapeData<T>(IEnumerable<T> entities, string fieldsString)
        {
            var propertyInfoList = GetPropertyInfos<T>(fieldsString);
            var shapedEntities = new List<ExpandoObject>();

            foreach (var entity in entities)
            {
                var shapedObject = ShapeDataForEntity(entity, propertyInfoList);
                shapedEntities.Add(shapedObject);
            }

            return shapedEntities;
        }

        public static ExpandoObject ShapeDataForEntity<T>(T entity, string fieldsString)
        {
            var propertyInfoList = GetPropertyInfos<T>(fieldsString);
            return ShapeDataForEntity(entity, propertyInfoList);
        }

        private static List<PropertyInfo> GetPropertyInfos<T>(string fieldsString)
        {
            var propertyInfoList = new List<PropertyInfo>();
            if (string.IsNullOrWhiteSpace(fieldsString))
            {
                var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsAfterSplit = fieldsString.Split(',');
                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();
                    var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        continue; // Bỏ qua nếu property không tồn tại
                    }

                    propertyInfoList.Add(propertyInfo);
                }
            }
            return propertyInfoList;
        }

        private static ExpandoObject ShapeDataForEntity<T>(T entity, IEnumerable<PropertyInfo> propertyInfoList)
        {
            var shapedObject = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)shapedObject;

            foreach (var propertyInfo in propertyInfoList)
            {
                var propertyValue = propertyInfo.GetValue(entity);
                // Dùng tên thuộc tính viết thường ký tự đầu tiên để chuẩn CamelCase (hoặc giữ nguyên)
                string propName = propertyInfo.Name;
                if (!string.IsNullOrEmpty(propName))
                {
                    propName = char.ToLowerInvariant(propName[0]) + propName.Substring(1);
                }
                expandoDict.Add(propName, propertyValue);
            }

            return shapedObject;
        }
    }
}
