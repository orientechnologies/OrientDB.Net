using OrientDB.Net.Core.Models;
using System.Reflection;

namespace OrientDB.Net.SqlCommandBuilder.Extensions
{
    internal static class OrientDBEntityExtensions
    {
        public static DictionaryOrientDBEntity ToDictionaryOrientDBEntity<T>(this T obj)
        {
            DictionaryOrientDBEntity entity = new DictionaryOrientDBEntity();
            var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach(var property in properties)
            {
                entity.SetField(property.Name, property.GetValue(obj));
            }
            return entity;
        }

        public static DictionaryOrientDBEntity ToDictionaryOrientDBEntity(this OrientDBEntity entity)
        {
            DictionaryOrientDBEntity dentity = new DictionaryOrientDBEntity();
            var properties = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var property in properties)
            {
                dentity.SetField(property.Name, property.GetValue(entity));
            }
            return dentity;
        }
    }
}
