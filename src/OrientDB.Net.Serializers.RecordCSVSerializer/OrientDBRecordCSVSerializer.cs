using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using OrientDB.Net.Serializers.RecordCSVSerializer.Extensions;
using System.Collections;
using System.Globalization;
using OrientDB.Net.Serializers.RecordCSVSerializer.Models;
using System.IO;
using OrientDB.Net.Core.Abstractions;
using OrientDB.Net.Core;
using OrientDB.Net.Core.Models;
using OrientDB.Net.Core.Attributes;

namespace OrientDB.Net.Serializers.RecordCSVSerializer
{
    public class OrientDBRecordCSVSerializer : IOrientDBRecordSerializer<byte[]>
    {
        public OrientDBRecordFormat RecordFormat
        {
            get
            {
                return OrientDBRecordFormat.CSV;
            }
        }

        public OrientDBRecordCSVSerializer()
        {

        }

        public TResultType Deserialize<TResultType>(byte[] data) where TResultType : OrientDBEntity
        {
            var recordString = BinarySerializer.ToString(data).Trim();

            return Deserialize<TResultType>(recordString);
        }

        internal TResultType Deserialize<TResultType>(string recordString) where TResultType : OrientDBEntity
        {
            TResultType entity = Activator.CreateInstance<TResultType>();

            int atIndex = recordString.IndexOf('@');
            int colonIndex = recordString.IndexOf(':');
            int index = 0;

            // parse class name
            if ((atIndex != -1) && (atIndex < colonIndex))
            {
                entity.OClassName = recordString.Substring(0, atIndex);
                index = atIndex + 1;
            }

            // start document parsing with first field name
            IDictionary<string, object> waterBucket = new Dictionary<string, object>();

            do
            {               
                index = ParseFieldName(index, recordString, waterBucket);
            } while (index < recordString.Length);

            // Will probably need a check here to determine how many actual distinct documents we
            // have as this could very well be a collection. Will need a way to determine this here
            // (Normally done while requesting data with PayloadStatus.

            entity.Hydrate(waterBucket);

            return entity;
        }

        private int ParseFieldName(int index, string recordString, IDictionary<string, object> waterBucket)
        {
            int startIndex = index;

            int iColonPos = recordString.IndexOf(':', index);
            if (iColonPos == -1)
                return recordString.Length;

            index = iColonPos;

            // parse field name string from raw document string
            string fieldName = recordString.Substring(startIndex, index - startIndex);
            int pos = fieldName.IndexOf('@');
            if (pos > 0)
            {
                fieldName = fieldName.Substring(pos + 1, fieldName.Length - pos - 1);
            }

            fieldName = fieldName.Replace("\"", "");
           
            waterBucket.Add(fieldName, null);

            // move to position after colon (:)
            index++;

            // check if it's not the end of document which means that current field has null value
            if (index == recordString.Length)
            {
                return index;
            }

            // check what follows after parsed field name and start parsing underlying type
            switch (recordString[index])
            {
                case '"':
                    index = ParseString(index, recordString, waterBucket, fieldName);
                    break;
                case '#':
                    index = ParseRecordID(index, recordString, waterBucket, fieldName);
                    break;
                case '(':
                    index = ParseEmbeddedDocument(index, recordString, waterBucket, fieldName);
                    break;
                case '[':
                    index = ParseList(index, recordString, waterBucket, fieldName);
                    break;
                case '<':
                    index = ParseSet(index, recordString, waterBucket, fieldName);
                    break;
                case '{':
                    index = ParseEmbeddedDocument(index, recordString, waterBucket, fieldName);
                    break;
                case '%':
                    index = ParseRidBags(index, recordString, waterBucket, fieldName);
                    break;
                default:
                    index = ParseValue(index, recordString, waterBucket, fieldName);
                    break;
            }

            // check if it's not the end of document which means that current field has null value
            if (index == recordString.Length)
            {
                return index;
            }

            // single string value was parsed and we need to push the index if next character is comma
            if (recordString[index] == ',')
            {
                index++;
            }

            return index;
        }

        private int ParseValue(int index, string recordString, IDictionary<string, object> waterBucket, string fieldName)
        {
            int startIndex = index;

            // search for end of parsed field value
            while (
                (index < recordString.Length) &&
                (recordString[index] != ',') &&
                (recordString[index] != ')') &&
                (recordString[index] != ']') &&
                (recordString[index] != '}') &&
                (recordString[index] != '>'))
            {
                index++;
            }

            // determine the type of field value

            string stringValue = recordString.Substring(startIndex, index - startIndex);
            object value = new object();

            if (stringValue.Length > 0)
            {
                // binary content
                if ((stringValue.Length > 2) && (stringValue[0] == '_') && (stringValue[stringValue.Length - 1] == '_'))
                {
                    stringValue = stringValue.Substring(1, stringValue.Length - 2);

                    // need to be able for base64 encoding which requires content to be devidable by 4
                    int mod4 = stringValue.Length % 4;

                    if (mod4 > 0)
                    {
                        stringValue += new string('=', 4 - mod4);
                    }

                    value = Convert.FromBase64String(stringValue);
                }
                // datetime or date
                else if ((stringValue.Length > 2) && (stringValue[stringValue.Length - 1] == 't') || (stringValue[stringValue.Length - 1] == 'a'))
                {
                    // Unix timestamp is miliseconds past epoch
                    // FIXME: this assumes the server JVM timezone is UTC when it might not be
                    // instead the timezone should be read from the server
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    string foo = stringValue.Substring(0, stringValue.Length - 1);
                    double d = double.Parse(foo);
                    value = epoch.AddMilliseconds(d).ToUniversalTime();
                }
                // boolean
                else if ((stringValue.Length > 2) && (stringValue == "true") || (stringValue == "false"))
                {
                    value = (stringValue == "true") ? true : false;
                }
                // null
                else if ((stringValue.Length > 2) && (stringValue == "null"))
                {
                    value = null;
                }
                // numbers
                else
                {
                    char lastCharacter = stringValue[stringValue.Length - 1];

                    switch (lastCharacter)
                    {
                        case 'b':
                            value = byte.Parse(stringValue.Substring(0, stringValue.Length - 1));
                            break;
                        case 's':
                            value = short.Parse(stringValue.Substring(0, stringValue.Length - 1));
                            break;
                        case 'l':
                            value = long.Parse(stringValue.Substring(0, stringValue.Length - 1));
                            break;
                        case 'f':
                            value = float.Parse(stringValue.Substring(0, stringValue.Length - 1), CultureInfo.InvariantCulture);
                            break;
                        case 'd':
                            value = double.Parse(stringValue.Substring(0, stringValue.Length - 1), CultureInfo.InvariantCulture);
                            break;
                        case 'c':
                            value = decimal.Parse(stringValue.Substring(0, stringValue.Length - 1), CultureInfo.InvariantCulture);
                            break;
                        default:
                            value = int.Parse(stringValue);
                            break;
                    }
                }
            }
            // null
            else if (stringValue.Length == 0)
            {
                value = null;
            }

            //assign field value
            if (waterBucket[fieldName] == null)
            {
                waterBucket[fieldName] = value;
            }
            else if (waterBucket[fieldName] is HashSet<object>)
            {
                ((HashSet<object>)waterBucket[fieldName]).Add(value);
            }
            else
            {
                ((List<object>)waterBucket[fieldName]).Add(value);
            }

            return index;
        }

        private int ParseRidBags(int index, string recordString, IDictionary<string, object> waterBucket, string fieldName)
        {
            //move to first base64 char
            index++;

            StringBuilder builder = new StringBuilder();

            while (recordString[index] != ';')
            {
                builder.Append(recordString[index]);
                index++;
            }

            // use a list as it preserves order at this stage which may be important when using ordered edges
            var rids = new List<ORID>();

            var value = Convert.FromBase64String(builder.ToString());
            using (var stream = new MemoryStream(value))
            using (var reader = new BinaryReader(stream))
            {
                var first = reader.ReadByte();
                int offset = 1;
                if ((first & 2) == 2)
                {
                    // uuid parsing is not implemented
                    offset += 16;
                }

                if ((first & 1) == 1) // 1 - embedded,0 - tree-based 
                {
                    var entriesSize = reader.ReadInt32EndianAware();
                    for (int j = 0; j < entriesSize; j++)
                    {
                        var clusterid = reader.ReadInt16EndianAware();
                        var clusterposition = reader.ReadInt64EndianAware();
                        rids.Add(new ORID(clusterid, clusterposition));
                    }
                }

                // else => There is some lazy loading logic in original implementation here. Not sure how to integrate this into new architecture yet.
            }

            waterBucket[fieldName] = rids;
            //move past ';'
            index++;

            return index;
        }

        private int ParseSet(int index, string recordString, IDictionary<string, object> waterBucket, string fieldName)
        {
            // move to the first element of this set
            index++;

            if (waterBucket[fieldName] == null)
            {
                waterBucket[fieldName] = new HashSet<object>();
            }

            while (recordString[index] != '>')
            {
                // check what follows after parsed field name and start parsing underlying type
                switch (recordString[index])
                {
                    case '"':
                        index = ParseString(index, recordString, waterBucket, fieldName);
                        break;
                    case '#':
                        index = ParseRecordID(index, recordString, waterBucket, fieldName);
                        break;
                    case '(':
                        index = ParseEmbeddedDocument(index, recordString, waterBucket, fieldName);
                        break;
                    case '{':
                        index = ParseMap(index, recordString, waterBucket, fieldName);
                        break;
                    case ',':
                        index++;
                        break;
                    default:
                        index = ParseValue(index, recordString, waterBucket, fieldName);
                        break;
                }
            }

            // move past closing bracket of this set
            index++;

            return index;
        }

        private int ParseMap(int i, string recordString, IDictionary<string, object> waterBucket, string fieldName)
        {
            int startIndex = i;
            int nestingLevel = 1;

            // search for end of parsed map
            while ((i < recordString.Length) && (nestingLevel != 0))
            {
                // check for beginning of the string to prevent finding an end of map within string value
                if (recordString[i + 1] == '"')
                {
                    // move to the beginning of the string
                    i++;

                    // go to the end of string
                    while ((i < recordString.Length) && (recordString[i + 1] != '"'))
                    {
                        i++;
                    }

                    // move to the end of string
                    i++;
                }
                else if (recordString[i + 1] == '{')
                {
                    // move to the beginning of the string
                    i++;

                    nestingLevel++;
                }
                else if (recordString[i + 1] == '}')
                {
                    // move to the beginning of the string
                    i++;

                    nestingLevel--;
                }
                else
                {
                    i++;
                }
            }

            // move past the closing bracket character
            i++;

            // do not include { and } in field value
            startIndex++;

            //assign field value
            if (waterBucket[fieldName] == null)
            {
                waterBucket[fieldName] = recordString.Substring(startIndex, i - 1 - startIndex);
            }
            else if (waterBucket[fieldName] is HashSet<object>)
            {
                ((HashSet<object>)waterBucket[fieldName]).Add(recordString.Substring(startIndex, i - 1 - startIndex));
            }
            else
            {
                ((List<object>)waterBucket[fieldName]).Add(recordString.Substring(startIndex, i - 1 - startIndex));
            }

            return i;
        }

        private int ParseList(int index, string recordString, IDictionary<string, object> waterBucket, string fieldName)
        {
            // move to the first element of this list
            index++;

            if (waterBucket[fieldName] == null)
            {
                waterBucket[fieldName] = new List<object>();
            }

            while (recordString[index] != ']')
            {
                // check what follows after parsed field name and start parsing underlying type
                switch (recordString[index])
                {
                    case '"':
                        index = ParseString(index, recordString, waterBucket, fieldName);
                        break;
                    case '#':
                        index = ParseRecordID(index, recordString, waterBucket, fieldName);
                        break;
                    case '(':
                        index = ParseEmbeddedDocument(index, recordString, waterBucket, fieldName);
                        break;
                    case '{':
                        index = ParseEmbeddedDocument(index, recordString, waterBucket, fieldName);
                        break;
                    case ',':
                        index++;
                        break;
                    default:
                        index = ParseValue(index, recordString, waterBucket, fieldName);
                        break;
                }
            }

            // move past closing bracket of this list
            index++;

            return index;
        }

        private int ParseEmbeddedDocument(int index, string recordString, IDictionary<string, object> waterBucket, string fieldName)
        {
            // move to the inside of embedded document (go past starting bracket character)
            index++;


            if ((index < 15) && (recordString.Length > 15) && (recordString.Substring(index, 15).Equals("ORIDs@pageSize:")))
            {
                OLinkCollection linkCollection = new OLinkCollection();
                index = ParseLinkCollection(index, recordString, linkCollection);
                waterBucket[fieldName] = linkCollection;
            }
            else
            {
                // create new dictionary which would hold K/V pairs of embedded document        
                IDictionary<string, object> embeddedDocument = new Dictionary<string, object>();

                // assign embedded object
                if (!embeddedDocument.ContainsKey(fieldName))
                {
                    waterBucket[fieldName] = embeddedDocument;
                }
                else if (waterBucket[fieldName] is HashSet<object>)
                {
                    ((HashSet<object>)waterBucket[fieldName]).Add(embeddedDocument);
                }
                else
                {
                    ((List<object>)waterBucket[fieldName]).Add(embeddedDocument);
                }

                // start parsing field names until the closing bracket of embedded document is reached

                while (recordString[index] != ')' && recordString[index] != '}')
                {
                    index = ParseFieldName(index, recordString, embeddedDocument);
                }
            }

            // move past close bracket of embedded document
            index++;

            return index;
        }

        private int ParseRecordID(int index, string recordString, IDictionary<string, object> waterBucket, string fieldName)
        {
            int startIndex = index;

            // search for end of parsed record ID value
            while (
                (index < recordString.Length) &&
                (recordString[index] != ',') &&
                (recordString[index] != ')') &&
                (recordString[index] != ']') &&
                (recordString[index] != '}') &&
                (recordString[index] != '>'))
            {
                index++;
            }


            //assign field value
            if (waterBucket[fieldName] == null)
            {
                // there is a special case when OEdge InV/OutV fields contains only single ORID instead of HashSet<ORID>
                // therefore single ORID should be deserialized into HashSet<ORID> type
                if (fieldName.Equals("in_") || fieldName.Equals("out_"))
                {
                    waterBucket[fieldName] = new HashSet<ORID>();
                    ((HashSet<ORID>)waterBucket[fieldName]).Add(new ORID(recordString, startIndex));
                }
                else
                {
                    waterBucket[fieldName] = new ORID(recordString, startIndex);
                }
            }
            else if (waterBucket[fieldName] is HashSet<object>)
            {
                ((HashSet<object>)waterBucket[fieldName]).Add(new ORID(recordString, startIndex));
            }
            else
            {
                ((List<object>)waterBucket[fieldName]).Add(new ORID(recordString, startIndex));
            }

            return index;
        }

        private int ParseString(int index, string recordString, IDictionary<string, object> waterBucket, string fieldName)
        {
            // move to the inside of string
            index++;

            int startIndex = index;

            // search for end of the parsed string value
            while (recordString[index] != '"')
            {
                // strings must escape these characters:
                // " -> \"
                // \ -> \\
                // therefore there needs to be a check for valid end of the string which
                // is quote character that is not preceeded by backslash character \
                if ((recordString[index] == '\\') && (recordString[index + 1] == '\\' || recordString[index + 1] == '"'))
                {
                    index = index + 2;
                }
                else
                {
                    index++;
                }
            }

            string value = recordString.Substring(startIndex, index - startIndex);
            // escape quotes
            value = value.Replace("\\" + "\"", "\"");
            // escape backslashes
            value = value.Replace("\\\\", "\\");

            // assign field value
            if (waterBucket[fieldName] == null)
            {
                waterBucket[fieldName] = value;
            }
            else if (waterBucket[fieldName] is HashSet<object>)
            {
                ((HashSet<object>)waterBucket[fieldName]).Add(value);
            }
            else
            {
                ((List<object>)waterBucket[fieldName]).Add(value);
            }

            // move past the closing quote character
            index++;

            return index;
        }

        public byte[] Serialize<T>(T input) where T : OrientDBEntity
        {
            return Encoding.UTF8.GetBytes($"{input.OClassName}@{SerializeEntity(input)}");
        }

        private int ParseLinkCollection(int i, string recordString, OLinkCollection linkCollection)
        {
            // move to the start of pageSize value
            i += 15;

            int index = recordString.IndexOf(',', i);

            linkCollection.PageSize = int.Parse(recordString.Substring(i, index - i));

            // move to root value
            i = index + 6;
            index = recordString.IndexOf(',', i);

            linkCollection.Root = new ORID(recordString.Substring(i, index - i));

            // move to keySize value
            i = index + 9;
            index = recordString.IndexOf(')', i);

            linkCollection.KeySize = int.Parse(recordString.Substring(i, index - i));

            // move past close bracket of link collection
            i++;

            return i;
        }

        private object SerializeEntity(OrientDBEntity input)
        {
            StringBuilder stringBuilder = new StringBuilder();

            PropertyInfo[] properties = input.GetType().GetProperties();
            
            if(properties.Any())
            {
                foreach(PropertyInfo propertyInfo in properties)
                {
                    switch(propertyInfo.Name)
                    {
                        case "OClassName":
                            continue;
                        case "ORID":
                            continue;
                        case "OVersion":
                            continue;
                        case "OClassId":
                            continue;
                        default:
                            bool isSerializable = (!string.IsNullOrWhiteSpace(propertyInfo.Name)) && (propertyInfo.Name[ 0 ] != '@');
                            if (!isSerializable) {
                                continue;
                            }
                            OrientDBProperty orientDBPropertyAttribute = propertyInfo.GetCustomAttribute<OrientDBProperty>(true);
                            if (orientDBPropertyAttribute == null || orientDBPropertyAttribute.Serializable) {
                                if (stringBuilder.Length > 0)
                                    stringBuilder.Append(",");

                                stringBuilder.AppendFormat("{0}:{1}", propertyInfo.Name, SerializeValue(propertyInfo.GetValue(input), propertyInfo.PropertyType));
                            }
                            break;
                    }                    
                }
            }

            return stringBuilder.ToString();
        }

        private string SerializeValue(object value, Type valueType)
        {
            if (value == null)
                return string.Empty;

            if (valueType == typeof(byte[]))
            {
                var bytes = value as byte[];
                if (bytes != null)
                {
                    return "_" + Convert.ToBase64String(bytes) + "_";
                }
            }

            switch(TypeExtensionMethods.GetTypeCode(valueType))
            {
                case TypeCode.Empty:
                    break;
                case TypeCode.Boolean:
                    return value.ToString().ToLower();
                case TypeCode.Byte:
                    return value.ToString() + "b";
                case TypeCode.Int16:
                    return value.ToString() + "s";
                case TypeCode.Int32:
                    return value.ToString();
                case TypeCode.Int64:
                    return value.ToString() + "l";
                case TypeCode.Single:
                    return ((float)value).ToString(CultureInfo.InvariantCulture) + "f";
                case TypeCode.Double:
                    return ((double)value).ToString(CultureInfo.InvariantCulture) + "d";
                case TypeCode.Decimal:
                    return ((decimal)value).ToString(CultureInfo.InvariantCulture) + "c";
                case TypeCode.DateTime:
                    DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    return ((long)((DateTime)value - unixEpoch).TotalMilliseconds).ToString() + "t";
                case TypeCode.String:
                case TypeCode.Char:
                    // strings must escape these characters:
                    // " -> \"
                    // \ -> \\
                    string stringValue = value.ToString();
                    // escape quotes
                    stringValue = stringValue.Replace("\\", "\\\\");
                    // escape backslashes
                    stringValue = stringValue.Replace("\"", "\\" + "\"");

                    return "\"" + stringValue + "\"";
                case TypeCode.Object:
                    return SerializeObjectValue(value, valueType);
            }
            throw new NotImplementedException();
        }

        private string SerializeObjectValue(object value, Type valueType)
        {
            StringBuilder bld = new StringBuilder();

            if ((valueType.IsArray) || (valueType.GetTypeInfo().IsGenericType))
            {
                if (valueType.Name == "Dictionary`2")
                {
                    bld.Append("{");

                    IDictionary<string, object> collection = ((IDictionary)value).Cast<dynamic>().ToDictionary(e => (string)e.Key, e => e.Value);

                    bool first = true;
                    foreach (var keyVal in collection)
                    {
                        if (!first)
                            bld.Append(",");

                        first = false;

                        string serialized = SerializeValue(keyVal.Value);
                        bld.Append("\"" + keyVal.Key + "\":" + serialized);
                    }

                    bld.Append("}");
                }
                else
                {
                    bld.Append(valueType.Name == "HashSet`1" ? "<" : "[");

                    IEnumerable collection = (IEnumerable)value;

                    bool first = true;
                    foreach (object val in collection)
                    {
                        if (!first)
                            bld.Append(",");

                        first = false;
                        bld.Append(SerializeValue(val));
                    }

                    bld.Append(valueType.Name == "HashSet`1" ? ">" : "]");
                }
            }
            // if property is ORID type it needs to be serialized as ORID
            else if (valueType.GetTypeInfo().IsClass && (valueType.Name == "ORID"))
            {
                bld.Append(((ORID)value).RID);
            }
            // Not sure this is possible with this architecture.
            //else if (valueType.GetTypeInfo().IsClass && (valueType.Name == "ODocument"))
            //{
            //    bld.AppendFormat("({0})", SerializeDocument((ODocument)value));
            //}
            // Not sure on this one either, testing will tell.
            else if (valueType.GetTypeInfo().IsClass && (valueType.Name == "OEmbeddedRidBag"))
            {
                //bld.AppendFormat("({0})", SerializeDocument((ODocument)value));
                List<ORID> ridbag = (List<ORID>)value;
                if (ridbag.Count > 0)
                {
                    BinaryBuffer buffer = new BinaryBuffer();
                    bld.Append("%");
                    buffer.Write((byte)1); // config
                    buffer.Write(ridbag.Count); //size
                    foreach (var item in ridbag)
                    {
                        buffer.Write(item);
                    }
                    bld.Append(Convert.ToBase64String(buffer.ToArray()));
                    bld.Append(";");
                }
            }

            return bld.ToString();
        }

        private string SerializeValue(object val)
        {
            return SerializeValue(val, val.GetType());
        }
    }
}
