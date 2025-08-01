
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection.Emit;
using System.ComponentModel;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;
using System.Globalization;
using System.Xml;

public static class JsonToAttachPoint
{
    [MenuItem("Assets/Convert Jsons")]
    public static void DoTheThingy()
    {
        var paths = AssetDatabase.GetAllAssetPaths().Where(x => (x.EndsWith(".json") || x.EndsWith(".jtk2d")) && x.StartsWith("Assets/"));

        var collectionsAtPath = new Dictionary<string, tk2dSpriteCollection>();

        foreach (var path in paths)
        {
            var filename = Path.GetFileNameWithoutExtension(path);
            var folder = Path.GetDirectoryName(path);
            var parentFolder = Path.GetDirectoryName(folder);

            tk2dSpriteCollection coll;

            if (!collectionsAtPath.TryGetValue(parentFolder, out coll))
            {
                var guids = AssetDatabase.FindAssets("", new string[] { parentFolder });
                coll = guids.Select(x => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(x))).OfType<GameObject>().Select(x => x.GetComponent<tk2dSpriteCollection>()).FirstOrDefault(x => x != null);
            }

            if (coll != null)
            {
                var def = coll.textureParams.FirstOrDefault(x => x.name == filename);

                if (def != null)
                {
                    var txt = File.ReadAllText(path);
                    var info = JsonUtility.FromJson<AssetSpriteData>(txt);

                    foreach (var ainfo in info.attachPoints)
                    {
                        if (ainfo.name == "array") continue;

                        var attach = def.attachPoints.FirstOrDefault(x => x.name == ainfo.name);

                        if (attach == null)
                        {
                            def.attachPoints.Add(attach = new tk2dSpriteDefinition.AttachPoint());
                        }

                        attach.name = ainfo.name;
                        attach.position = new Vector3(ainfo.position.x * 16, def.texture.height - ainfo.position.y * 16f, ainfo.position.z * 16f);
                        attach.angle = ainfo.angle;
                    }
                }
            }
        }
    }

    public struct AssetSpriteData
    {
        // Leshy SpriteSheet Tool - https://www.leshylabs.com/apps/sstool/
        public string name;

        public int x;
        public int y;
        public int width;
        public int height;

        // Custom extensions for tk2d compatibility
        public int flip;

        public tk2dSpriteDefinition.AttachPoint[] attachPoints;

    }
}

public static partial class JSONHelper
{

    public static bool CheckOnRead =
#if DEBUG
        true;
#else
        false;
#endif

    public static JsonHelperReader OpenReadJSON(Stream stream)
    {
        StreamReader text = new StreamReader(stream);
        JsonHelperReader json = new JsonHelperReader(text);
        return json;
    }
    public static JsonHelperReader OpenReadJSON(string path)
    {
        JsonHelperReader json = OpenReadJSON(File.OpenRead(path));
        json.RelativeDir = path.Substring(0, path.Length - 5);
        return json;
    }

    public static object ReadJSON(Stream stream)
    {
        using (JsonHelperReader json = OpenReadJSON(stream))
        {
            json.Read(); // Go to Start
            return json.ReadObject();
        }
    }
    public static object ReadJSON(string path)
    {
        using (JsonHelperReader json = OpenReadJSON(path))
        {
            json.Read(); // Go to Start
            return json.ReadObject();
        }
    }
    public static T ReadJSON<T>(Stream stream)
    {
        using (JsonHelperReader json = OpenReadJSON(stream))
        {
            json.Read(); // Go to Start
            return json.ReadObject<T>();
        }
    }
    public static T ReadJSON<T>(string path)
    {
        using (JsonHelperReader json = OpenReadJSON(path))
        {
            json.Read(); // Go to Start
            return json.ReadObject<T>();
        }
    }

    public static object ReadObject(this JsonHelperReader json)
    {
        json.Read(); // Drop StartObject
        string metaProp = json.ReadPropertyName();

        if (metaProp == META.MARKER)
        {
            return json.ReadObjectFromMeta();
        }

        return json.ReadObject(json.ReadMetaObjectType(), true);
    }
    public static T ReadObject<T>(this JsonHelperReader json, bool skipHeader = false)
    {
        return (T)json.ReadObject(typeof(T), skipHeader);
    }
    public static object ReadObject(this JsonHelperReader json, Type type, bool skipHeader = false)
    {
        if (LOG)
        {
            Console.WriteLine("READING " + json.RelativeDir + ", ");
            Console.WriteLine(json.Path + " (" + json.LineNumber + ", " + json.LinePosition + "): " + type.FullName);
            Console.WriteLine("(" + json.TokenType + ", " + json.Value + ")");
        }

        if (type == t_string || type == t_byte_a)
        {
            object value = json.Value;
            json.Read(); // Drop value
            return value;
        }
        if (type.IsEnum)
        {
            object value = json.Value;
            json.Read(); // Drop value
            return Enum.ToObject(type, value);
        }
        if (
            type.IsPrimitive ||
            (json.TokenType != JsonToken.StartArray && json.TokenType != JsonToken.StartObject && json.TokenType != JsonToken.PropertyName)
           )
        {
            object value = json.Value;
            json.Read(); // Drop value
            if (value == null)
            {
                return null;
            }
            return Convert.ChangeType(value, type);
        }

        if (json.TokenType == JsonToken.StartArray)
        {
            json.Read(); // Drop StartArray
            int size;
            string arrayType = json.ReadMetaArrayData(out size);

            if (arrayType == META.ARRAYTYPE_LIST || arrayType == META.ARRAYTYPE_ARRAY)
            {
                Type itemType = type.GetListType();
                IList tmplist = null;
                IList list = null;
                if (type.IsArray)
                {
                    tmplist = new ArrayList();
                    list = Array.CreateInstance(type.GetElementType(), size);
                }
                else
                {
                    tmplist = list = (IList)ReflectionHelper.Instantiate(type);
                }

                json.RegisterReference(list);

                if (itemType != null)
                {
                    while (json.TokenType != JsonToken.EndArray)
                    {
                        tmplist.Add(json.ReadObject(itemType));
                    }
                }
                else
                {
                    while (json.TokenType != JsonToken.EndArray)
                    {
                        tmplist.Add(json.ReadObject());
                    }
                }

                if (type.IsArray)
                {
                    ((ArrayList)tmplist).CopyTo((Array)list);
                }

                json.Read(); // Drop EndArray
                return list;
            }

            if (arrayType == META.ARRAYTYPE_MAP)
            {
                // TODO Get key / value types
                IDictionary map = (IDictionary)ReflectionHelper.Instantiate(type);

                json.RegisterReference(map);

                while (json.TokenType != JsonToken.EndArray)
                {
                    DictionaryEntry entry = json.ReadObject<DictionaryEntry>();
                    map[entry.Key] = entry.Value;
                }

                json.Read(); // Drop EndArray
                return map;
            }
        }

        if (type == t_Type)
        {
            Type value = json.ReadMetaType();
            return value;
        }

        if (!skipHeader)
        {
            json.Read(); // Drop Start
        }

        JSONRule rule = GetJSONRule(type);
        if (!skipHeader)
        {
            Type typeO = type;
            string metaProp = rule.ReadMetaHeader(json, ref type);
            if (metaProp != null)
            {
                if (metaProp != META.MARKER)
                {
                    throw new JsonReaderException("Invalid meta prop: Expected . or : , got " + metaProp);
                }
                return json.ReadObjectFromMeta();
            }
            if (typeO != type)
            {
                rule = GetJSONRule(type);
            }
        }
        return json.FillObject(rule.New(json, type), type, rule, true);
    }

    public static object FillObject(this JsonHelperReader json, object obj, Type type = null, JSONRule rule = null, bool skipHeader = false)
    {
        if (type == null)
        {
            type = obj.GetType();
        }
        if (!skipHeader)
        {
            json.Read(); // Drop Start
        }

        if (rule == null)
        {
            rule = GetJSONRule(type);
        }
        if (!skipHeader)
        {
            Type typeO = type;
            string metaProp = rule.ReadMetaHeader(json, ref type);
            if (metaProp != null)
            {
                if (metaProp != META.MARKER)
                {
                    throw new JsonReaderException("Invalid meta prop: Expected . or : , got " + metaProp);
                }
                return json.FillObjectFromMeta(obj);
            }
            if (typeO != type)
            {
                rule = GetJSONRule(type);
            }
        }
        int id = json.RegisterReference(obj);
        obj = rule.Deserialize(json, obj);
        _OnAfterDeserialize(obj);
        json.UpdateReference(obj, id);

        json.Read(); // Drop End
        return obj;
    }

    public static object ReadObjectFromMeta(this JsonHelperReader json)
    {
        string metaType = (string)json.Value;
        json.Read(); // Drop value

        if (metaType == META.EXTERNAL)
        {
            return json.ReadMetaExternal(true);
        }

        if (metaType == META.REF)
        {
            return json.ReadMetaReference(true);
        }

        throw new JsonReaderException("Invalid meta type: " + metaType + " invalid object replacement");
    }

    public static object FillObjectFromMeta(this JsonHelperReader json, object obj)
    {
        string metaType = (string)json.Value;
        json.Read(); // Drop value

        if (metaType == META.EXTERNAL)
        {
            using (JsonHelperReader ext = json.OpenMetaExternal(true))
            {
                ext.Read(); // Go to Start
                obj = ext.FillObject(obj);
                json.RegisterReference(obj); // External references are still being counted
            }
        }

        if (metaType == META.REF)
        {
            // FIXME Implement filling from references
            throw new JsonReaderException("Filling objects from references not supported!");
        }

        throw new JsonReaderException("Invalid meta type: " + metaType + " invalid object replacement");
    }

    public static JsonHelperReader OpenMetaExternal(this JsonHelperReader json, bool skipHeader = false)
    {
        string currentProp;
        if (!skipHeader)
        {
            json.ReadStartMetadata(META.EXTERNAL);
        }

        currentProp = json.ReadPropertyName();
        if (currentProp == META.EXTERNAL_PATH)
        {
            // No "in" property, which means this is a Resources.Load call! Return null!
            return null;
        }
        string @in = (string)json.Value;
        json.Read(); // Drop value
        string path = (string)json.ReadRawProperty(META.EXTERNAL_PATH);

        string fullpath = "";

        if (@in == META.EXTERNAL_IN_RELATIVE)
        {
            if (json.RelativeDir == null)
            {
                throw new NullReferenceException("json.RelativeDir == null, but found relative external reference!");
            }
            fullpath = Path.Combine(json.RelativeDir, path + ".json");
        }
        else if (@in == META.EXTERNAL_IN_SHARED)
        {
            if (SharedDir == null)
            {
                throw new NullReferenceException("JSONHelper.SharedDir == null, but found shared external reference!");
            }
            fullpath = Path.Combine(SharedDir, path.Replace('/', Path.DirectorySeparatorChar) + ".json");
        }

        json.ReadEndMetadata();

        JsonHelperReader ext = OpenReadJSON(fullpath);
        ext.Global.AddRange(json.Global);
        return ext;
    }

    public static object ReadMetaExternal(this JsonHelperReader json, bool skipHeader = false)
    {
        using (JsonHelperReader ext = json.OpenMetaExternal(skipHeader))
        {
            if (ext == null)
            {
                // Resources.Load call - path PropertyName dropped already.
                string path = (string)json.Value;
                json.Read(); // Drop value
                json.ReadEndMetadata();
                return Resources.Load(path);
            }

            ext.Read(); // Go to Start
            object obj = ext.ReadObject();
            json.RegisterReference(obj); // External references are still being counted
            return obj;
        }
    }

    public static object ReadMetaReference(this JsonHelperReader json, bool skipHeader = false)
    {
        if (!skipHeader)
        {
            json.ReadStartMetadata(META.REF);
        }
        int id = (int)(long)json.ReadRawProperty(META.REF_ID);
        int type = (int)(long)json.ReadRawProperty(META.REF_TYPE);
        json.ReadEndMetadata();

        return json.GetReference(id, type);
    }

    public static Type ReadMetaObjectType(this JsonHelperReader json)
    {
        return json.ReadMetaType_(META.OBJTYPE);
    }
    public static Type ReadMetaType(this JsonHelperReader json)
    {
        return json.ReadMetaType_(META.TYPE);
    }
    public static Type ReadMetaType_(this JsonHelperReader json, string metaType)
    {
        string currentProp;
        json.ReadStartMetadata(metaType);

        Type type;

        string fullname = (string)json.ReadRawProperty(META.TYPE_FULLNAME);
        /*currentProp = json.ReadPropertyName();
        if (currentProp == META.TYPE_SPLIT) {
            json.Read(); // Drop StartArray
            string ns = (string) json.Value; json.Read();
            string name = (string) json.Value; json.Read();
            json.Read(); // Drop EndArray
            type = FindType(fullname, ns, name);
        } else*/
        {
            type = FindType(fullname);
        }

        if (type == null)
        {
            throw new JsonReaderException("Could not find type " + fullname);
        }

        currentProp = json.ReadPropertyName();
        if (currentProp == META.TYPE_GENPARAMS)
        {
            type = type.MakeGenericType(json.ReadProperty<Type[]>(META.TYPE_GENPARAMS));
        }

        json.ReadEndMetadata();
        return type;
    }

    public static void ReadStartMetadata(this JsonHelperReader json, string metaType)
    {
        json.Read(); // Drop StartObject
        if (!CheckOnRead)
        {
            json.Read(); // Drop PropertyName
            json.Read(); // Drop String
        }
        else
        {
            string metaTypeR = (string)json.ReadRawProperty(META.MARKER);
            if (metaType != metaTypeR)
            {
                throw new JsonReaderException("Invalid meta type: Expected " + metaType + ", got " + metaTypeR);
            }
        }
    }
    public static void ReadEndMetadata(this JsonHelperReader json)
    {
        if (json.TokenType != JsonToken.EndObject)
        {
            // FAILSAFE
            // Currently only got so far that this got called at ReadMetaType_
            // with split, without genparams. This Read() caused problems.
            json.Read(); // @ EndObject
        }
        json.Read(); // Drop EndObject
    }

    public static string ReadMetaArrayData(this JsonHelperReader json, out int size)
    {
        json.ReadStartMetadata(META.ARRAYTYPE);
        string type = (string)json.ReadRawProperty(META.TYPE_FULLNAME);
        if (type == META.ARRAYTYPE_ARRAY)
        {
            size = (int)(long)json.ReadRawProperty(META.ARRAYTYPE_ARRAY_SIZE);
        }
        else
        {
            size = -1;
        }
        json.ReadEndMetadata();
        return type;
    }

    public static void ReadPropertyName(this JsonHelperReader json, string name)
    {
        if (CheckOnRead)
        {
            if (json.TokenType != JsonToken.PropertyName)
            {
                throw new JsonReaderException("Invalid token: Expected PropertyName, got " + json.TokenType);
            }
            if (name != (string)json.Value)
            {
                throw new JsonReaderException("Invalid property: Expected " + name + ", got " + json.Value);
            }
        }

        json.Read(); // Drop PropertyName
    }

    public static string ReadPropertyName(this JsonHelperReader json)
    {
        if (json.TokenType != JsonToken.PropertyName)
        {
            return null;
        }

        string name = (string)json.Value;
        json.Read(); // Drop PropertyName
        return name;
    }

    public static object ReadRawProperty(this JsonHelperReader json, string name, JsonToken type = JsonToken.Undefined)
    {
        if (name != null)
        {
            if (!CheckOnRead)
            {
                json.Read(); // Drop PropertyName
            }
            else
            {
                json.ReadPropertyName(name);
                if (type != JsonToken.Undefined && json.TokenType != type)
                {
                    throw new JsonReaderException("Invalid token: Expected " + type + ", got " + json.TokenType);
                }
            }
        }

        object value = json.Value;
        json.Read(); // Drop value
        return value;
    }

    public static T ReadProperty<T>(this JsonHelperReader json, string name)
    {
        return (T)json.ReadProperty(name, typeof(T));
    }
    public static object ReadProperty(this JsonHelperReader json, string name, Type type)
    {
        if (name != null)
        {
            if (!CheckOnRead)
            {
                json.Read(); // Drop PropertyName
            }
            else
            {
                json.ReadPropertyName(name);
            }
        }

        return json.ReadObject(type);
    }

}

public static partial class JSONHelper
{

    private readonly static Assembly _Asm;
    private readonly static Type[] _RuleTypes;
    private readonly static Type t_JSONRule = typeof(JSONRule);
    private readonly static Type t_string = typeof(string);
    private readonly static Type t_byte_a = typeof(byte[]);
    private readonly static Type t_Type = typeof(Type);
    private readonly static object[] a_object_0 = new object[0];

    private readonly static Dictionary<string, Type> _TypeCache = new Dictionary<string, Type>();

    public static bool LOG = false;

    public readonly static Dictionary<Type, JSONRule> Rules = new Dictionary<Type, JSONRule>();
    public readonly static JSONRule RuleValueType = new JSONValueTypeRule();

    public static string SharedDir;

    static JSONHelper()
    {
        _Asm = Assembly.GetExecutingAssembly();
        _RuleTypes = _Asm.GetExportedTypes();

        List<Type> _Types_Assignable = new List<Type>(_RuleTypes.Length);
        for (int i = 0; i < _RuleTypes.Length; i++)
        {
            Type t = _RuleTypes[i];
            if (!t_JSONRule.IsAssignableFrom(t))
            {
                continue;
            }
            _Types_Assignable.Add(t);
        }
        _RuleTypes = _Types_Assignable.ToArray();
    }

    public static JSONRule GetJSONRule(this object obj)
    {
        return obj.GetType().GetJSONRule();
    }
    public static JSONRule GetJSONRule(this Type type_)
    {
        Type type = type_;
        JSONRule config;
        if (Rules.TryGetValue(type_, out config))
        {
            return config;
        }

        while (type != null)
        {
            for (int i = 0; i < _RuleTypes.Length; i++)
            {
                Type t = _RuleTypes[i];
                Type bi = t;
                while ((bi = bi.BaseType) != null)
                {
                    if (!bi.IsGenericType || bi.GetGenericTypeDefinition() != typeof(JSONRule<>))
                    {
                        continue;
                    }
                    if (type == bi.GetGenericArguments()[0])
                    {
                        return Rules[type_] = ((JSONRule)t.GetConstructor(Type.EmptyTypes).Invoke(a_object_0)).Fill(type_);
                    }
                }
            }
            type = type.BaseType;
        }

        if (type_.IsValueType)
        {
            return Rules[type_] = new JSONValueTypeRule().Fill(type_);
        }

        return Rules[type_] = new JSONRule().Fill(type_);
    }

    public static Type FindType(string fullname, string ns = null, string name = null)
    {

        Type type;
        if (_TypeCache.TryGetValue(fullname, out type))
        {
            return type;
        }

        Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < asms.Length; i++)
        {
            Assembly asm = asms[i];
            if ((type = asm.GetType(fullname, false)) != null)
            {
                return _TypeCache[fullname] = type;
            }
        }

        if (type == null && ns != null && name != null)
        {
            for (int i = 0; i < asms.Length; i++)
            {
                Assembly asm = asms[i];
                Type[] types = asm.GetTypes();
                for (int ti = 0; ti < types.Length; ti++)
                {
                    type = types[ti];
                    if (type.Namespace == ns && type.Name == name)
                    {
                        return _TypeCache[fullname] = type;
                    }
                }
            }
        }

        return _TypeCache[fullname] = null;
    }

    private static void _OnBeforeSerialize(object obj)
    {
        var yeah = (obj as ISerializationCallbackReceiver);
        if (yeah != null)
        {
            yeah.OnBeforeSerialize();
        }
    }

    private static void _OnAfterDeserialize(object obj)
    {
        var yeah = (obj as ISerializationCallbackReceiver);
        if (yeah != null)
        {
            yeah.OnAfterDeserialize();
        }
    }

}

public static partial class JSONHelper
{

    public static class META
    {

        /// <summary>
        /// The metadata object marker. Use it inside metadata objects to specify which type of metadata it is.
        /// </summary>
        public const string MARKER = ".";
        /// <summary>
        /// The property "marker". Use it in normal objects as property name and the metadata object as value.
        /// </summary>
        public const string PROP = ":";
        /// <summary>
        /// The ValueType / struct "marker". Use it as value to PROP in value types.
        /// </summary>
        public const string VALUETYPE = "~";

        public const string REF = "ref";
        public const int REF_NONE = -1;
        public const string REF_ID = "#";
        public const string REF_TYPE = "=";
        public const int REF_TYPE_EQUAL = 0;
        public const int REF_TYPE_SAMEREF = 1;

        public const string TYPE = "type";
        public const string TYPE_FULLNAME = "name";
        // public const string TYPE_SPLIT      = "split";
        public const string TYPE_GENPARAMS = "params";

        public const string OBJTYPE = "objtype";

        public const string ARRAYTYPE = "arraytype";
        public const string ARRAYTYPE_ARRAY = "array";
        public const string ARRAYTYPE_LIST = "list";
        public const string ARRAYTYPE_MAP = "map";

        public const string ARRAYTYPE_ARRAY_SIZE = "size";

        public const string COMPONENTTYPE_DEFINITION = "=";
        public const string COMPONENTTYPE_REFERENCE = "~";

        public const string EXTERNAL = "external";
        public const string EXTERNAL_PATH = "path";
        public const string EXTERNAL_IN = "in";
        public const string EXTERNAL_IN_RESOURCES = "Resources.Load";
        public const string EXTERNAL_IN_RELATIVE = "relative";
        public const string EXTERNAL_IN_SHARED = "shared";

        public const string ARRAYAT = "at";
        public const string ARRAYAT_INDEX = "index";
        public const string ARRAYAT_VALUE = "value";

        public const string UNSUPPORTED = "UNSUPPORTED";
        public const string UNSUPPORTED_USE_EXTERNAL = "REPLACE THIS WITH EXTERNAL";

    }

}

public class JSONRule
{

    public bool ForceSerializeProperties = false;

    protected MemberInfo[] _Properties;
    protected MemberInfo[] _Fields;
    protected Dictionary<string, MemberInfo> _MemberMap = new Dictionary<string, MemberInfo>();

    public virtual JSONRule Fill(Type type)
    {
        List<MemberInfo> infos;

        infos = new List<MemberInfo>();
        Fill_(infos, type.GetProperties(BindingFlags.Public | BindingFlags.Instance), false);
        Fill_(infos, type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance), true);
        _Properties = infos.ToArray();

        infos = new List<MemberInfo>();
        Fill_(infos, type.GetFields(BindingFlags.Public | BindingFlags.Instance), false);
        Fill_(infos, type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance), true);
        _Fields = infos.ToArray();

        return this;
    }
    protected virtual void Fill_(List<MemberInfo> to, MemberInfo[] from, bool isPrivate)
    {
        for (int i = 0; i < from.Length; i++)
        {
            MemberInfo info = from[i];
            if (!CanSerialize(info, isPrivate))
            {
                continue;
            }
            to.Add(info);
            _MemberMap[info.Name] = info;
        }
    }

    public virtual bool CanSerialize(MemberInfo info, bool isPrivate)
    {
        if (info is PropertyInfo)
        {
            PropertyInfo pi = (PropertyInfo)info;
            if (!pi.CanRead || !pi.CanWrite)
            {
                return false;
            }
        }

        if (isPrivate && info.GetCustomAttributes(typeof(SerializeField), true).Length == 0)
        {
            return false;
        }
        if (!isPrivate && info.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length != 0)
        {
            return false;
        }

        return true;
    }

    public virtual void Serialize(JsonHelperWriter json, object obj, MemberInfo info)
    {
        json.WritePropertyName(info.Name);
        json.Write(ReflectionHelper.GetValue(info, obj));
    }

    public virtual void WriteMetaHeader(JsonHelperWriter json, object obj)
    {
        json.WritePropertyName(JSONHelper.META.PROP);
        json.WriteMetaObjectType(obj);
    }

    public virtual void Serialize(JsonHelperWriter json, object obj)
    {
        if (obj is UnityEngine.Object && !(obj is Component))
        {
            json.WriteProperty("name", ((UnityEngine.Object)obj).name);
        }

        if (ForceSerializeProperties)
        {
            for (int i = 0; i < _Properties.Length; i++)
            {
                Serialize(json, obj, _Properties[i]);
            }
        }

        for (int i = 0; i < _Fields.Length; i++)
        {
            Serialize(json, obj, _Fields[i]);
        }
    }

    public virtual object New(JsonHelperReader json, Type type)
    {
        try
        {
            return ReflectionHelper.Instantiate(type);
        }
        catch (Exception e)
        {
            throw new JsonReaderException("Could not instantiate type " + type.FullName + "!", e);
        }
    }

    public virtual void Deserialize(JsonHelperReader json, object obj, string prop)
    {
        MemberInfo info;
        if (!_MemberMap.TryGetValue(prop, out info))
        {
            // Forcibly throw here - can't parse the following data anymore
            throw new JsonReaderException("Invalid property " + prop + "!");
        }

        object value = json.ReadObject(info.GetValueType());
        if (obj == null)
        {
            // Just drop the value.
            return;
        }
        ReflectionHelper.SetValue(info, obj, value);
    }

    public virtual string ReadMetaHeader(JsonHelperReader json, ref Type type)
    {
        string metaProp = json.ReadPropertyName();
        if (metaProp != JSONHelper.META.PROP)
        {
            return metaProp;
        }

        Type typeR = json.ReadMetaObjectType();
        if (JSONHelper.CheckOnRead && !type.IsAssignableFrom(typeR))
        {
            throw new JsonReaderException("Type mismatch! Expected " + type.FullName + ", got " + typeR.FullName);
        }
        type = typeR;

        return null;
    }

    public virtual object Deserialize(JsonHelperReader json, object obj)
    {
        if (obj is UnityEngine.Object && !(obj is Component))
        {
            ((UnityEngine.Object)obj).name = (string)json.ReadRawProperty("name");
        }

        while (json.TokenType != JsonToken.EndObject)
        {
            Deserialize(json, obj, json.ReadPropertyName());
        }

        return obj;
    }

}

public class JSONRule<T> : JSONRule
{

    protected Type _T = typeof(T);

    public override JSONRule Fill(Type type)
    {
        _T = type;
        return base.Fill(type);
    }

}

public delegate object DynamicMethodDelegate(object target, params object[] args);
/// <summary>
/// Stolen from http://theinstructionlimit.com/fast-net-reflection and FEZ. Thanks, Renaud!
/// </summary>

// I will be in the list of contributors! And no one will know that all I did was change
// field names from camelCase to PascalCase.

public static class ReflectionHelper
{
    private static readonly Type[] _ManyObjects = new Type[2] { typeof(object), typeof(object[]) };
    private static readonly Dictionary<MethodInfo, DynamicMethodDelegate> _MethodCache = new Dictionary<MethodInfo, DynamicMethodDelegate>();
    private static readonly Dictionary<Type, DynamicMethodDelegate> _ConstructorCache = new Dictionary<Type, DynamicMethodDelegate>();

    public static DynamicMethodDelegate CreateDelegate(this MethodBase method)
    {
        var dynam = new DynamicMethod(string.Empty, typeof(object), _ManyObjects, typeof(ReflectionHelper).Module, true);
        ILGenerator il = dynam.GetILGenerator();

        ParameterInfo[] args = method.GetParameters();

        Label argsOK = il.DefineLabel();

        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldlen);
        il.Emit(OpCodes.Ldc_I4, args.Length);
        il.Emit(OpCodes.Beq, argsOK);

        il.Emit(OpCodes.Newobj, typeof(TargetParameterCountException).GetConstructor(Type.EmptyTypes));
        il.Emit(OpCodes.Throw);

        il.MarkLabel(argsOK);

        if (!method.IsStatic && !method.IsConstructor)
        {
            il.Emit(OpCodes.Ldarg_0);
            if (method.DeclaringType.IsValueType)
            {
                il.Emit(OpCodes.Unbox, method.DeclaringType);
            }
        }

        for (int i = 0; i < args.Length; i++)
        {
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldc_I4, i);
            il.Emit(OpCodes.Ldelem_Ref);

            if (args[i].ParameterType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, args[i].ParameterType);
            }
        }

        if (method.IsConstructor)
        {
            il.Emit(OpCodes.Newobj, (ConstructorInfo)method);
        }
        else if (method.IsFinal || !method.IsVirtual)
        {
            il.Emit(OpCodes.Call, (MethodInfo)method);
        }
        else
        {
            il.Emit(OpCodes.Callvirt, (MethodInfo)method);
        }

        Type returnType = method.IsConstructor ? method.DeclaringType : ((MethodInfo)method).ReturnType;
        if (returnType != typeof(void))
        {
            if (returnType.IsValueType)
            {
                il.Emit(OpCodes.Box, returnType);
            }
        }
        else
        {
            il.Emit(OpCodes.Ldnull);
        }

        il.Emit(OpCodes.Ret);

        return (DynamicMethodDelegate)dynam.CreateDelegate(typeof(DynamicMethodDelegate));
    }

    public static DynamicMethodDelegate GetDelegate(this MethodInfo method)
    {
        DynamicMethodDelegate dmd;
        if (_MethodCache.TryGetValue(method, out dmd))
        {
            return dmd;
        }

        dmd = CreateDelegate(method);
        _MethodCache.Add(method, dmd);

        return dmd;
    }

    public static object Instantiate(Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        if (type.IsArray)
        {
            return Array.CreateInstance(type.GetElementType(), 0);
        }
        DynamicMethodDelegate dmd;
        lock (_ConstructorCache)
        {
            if (!_ConstructorCache.TryGetValue(type, out dmd))
            {
                dmd = CreateDelegate(type.GetConstructor(Type.EmptyTypes));
                _ConstructorCache.Add(type, dmd);
            }
        }
        return dmd(null, new object[0]);
    }

    public static object InvokeMethod(MethodInfo info, object targetInstance, params object[] arguments)
    {
        return GetDelegate(info)(targetInstance, arguments);
    }

    public static object GetValue(PropertyInfo member, object instance)
    {
        return InvokeMethod(member.GetGetMethod(true), instance, new object[0]);
    }

    public static object GetValue(MemberInfo member, object instance)
    {
        if (member is PropertyInfo)
        {
            return GetValue((PropertyInfo)member, instance);
        }
        else if (member is FieldInfo)
        {
            return ((FieldInfo)member).GetValue(instance);
        }
        throw new NotImplementedException();
    }

    public static void SetValue(PropertyInfo member, object instance, object value)
    {
        InvokeMethod(member.GetSetMethod(true), instance, new object[1] { value });
    }

    public static void SetValue(MemberInfo member, object instance, object value)
    {
        if (member is PropertyInfo)
        {
            SetValue((PropertyInfo)member, instance, value);
        }
        else if (member is FieldInfo)
        {
            ((FieldInfo)member).SetValue(instance, value);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public static Action<byte[]> CreateRPCDelegate(this MethodInfo info, object instance = null)
    {
        object[] args = new object[1];
        DynamicMethodDelegate d = GetDelegate(info);
        return delegate (byte[] b) {
            args[0] = b;
            d(instance, args);
        };
    }

}

public class JsonHelperReader : JsonTextReader
{

    public readonly List<object> _RefList = new List<object>(512);

    public readonly Dictionary<string, object> Global = new Dictionary<string, object>();

    public string RelativeDir;

    public JsonHelperReader(TextReader reader)
        : base(reader)
    {
    }

    public object GetReference(int id, int type)
    {
        if (id < 0 || _RefList.Count <= id)
        {
            StringBuilder message = new StringBuilder();
            message.Append("Reference ID out of bounds!\nID: ").Append(id)
                   .Append("\nRegistered references:");
            for (int i = 0; i < _RefList.Count; i++)
            {
                message.AppendLine().Append(i).Append(": ").Append(_RefList[i]);
            }
            throw new JsonReaderException(message.ToString());
        }

        if (type == JSONHelper.META.REF_TYPE_SAMEREF)
        {
            return _RefList[id];
        }

        if (type == JSONHelper.META.REF_TYPE_EQUAL)
        {
            // TODO create shallow copy
            return _RefList[id];
        }

        return null;
    }
    public int GetReferenceType(int id, object a)
    {
        if (id == JSONHelper.META.REF_NONE || a is Component)
        {
            return JSONHelper.META.REF_NONE;
        }
        object b = _RefList[id];
        if (ReferenceEquals(a, b))
        {
            return JSONHelper.META.REF_TYPE_SAMEREF;
        }
        return JSONHelper.META.REF_TYPE_EQUAL;
    }

    public int RegisterReference(object obj)
    {
        // This breaks stuff. FIXME FIXME FIXME
        /*if (obj is Component) {
            return JSONHelper.META.REF_NONE;
        }*/
        int id = _RefList.Count;
        _RefList.Add(obj);
        return id;
    }

    public void UpdateReference(object obj, int id)
    {
        if (id == JSONHelper.META.REF_NONE || obj is Component)
        {
            return;
        }
        _RefList[id] = obj;
    }

    /*
    public override bool Read() {
        bool value = base.Read();
        if (Value == null) {
            Console.WriteLine("JSON Read(): " + TokenType);
        } else {
            Console.WriteLine("JSON Read(): " + TokenType + ": " + Value);
        }
        Console.WriteLine(Environment.StackTrace);
        return value;
    }
    */

}

public static partial class ETGMod
{
    // ETGMod helper extension methods.
    /// <summary>
    /// Just like object.ToString(), but returns string.Empty if the object is null.
    /// </summary>
    /// <param name="o">The object to convert into string.</param>
    /// <returns>The result string.</returns>
    public static string ToStringSafe(this object o)
    {
        return o == null ? string.Empty : o is string ? (string)o : o.ToString();
    }

    /// <summary>
    /// Removes the characters ", \, -, and \n from the string.
    /// </summary>
    /// <param name="str">The string to modify.</param>
    /// <returns>The string with then unacceptable characters removed.</returns>
    public static string RemoveUnacceptableCharactersForEnum(this string str)
    {
        if (str == null)
        {
            return null;
        }
        return str.Replace("\"", "").Replace("\\", "").Replace(" ", "").Replace("-", "_").Replace("\n", "");
    }

    /// <summary>
    /// Removes the characters ", \, and \n from the string.
    /// </summary>
    /// <param name="str">The string to modify.</param>
    /// <returns>The string with then unacceptable characters removed.</returns>
    public static string RemoveUnacceptableCharactersForGUID(this string str)
    {
        if (str == null)
        {
            return null;
        }
        return str.Replace("\"", "").Replace("\\", "").Replace("\n", "");
    }

    /// <summary>
    /// Makes the string all lowercase, removes the characters \n, \, \", . and - and replaces spaces with underscores.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToID(this string str)
    {
        return str.ToLowerInvariant().Replace("\n", "").Replace("\\", "").Replace("\"", "").Replace(" ", "_").Replace(".", "").Replace("-", "");
    }

    private static readonly long _DoubleNegativeZero = BitConverter.DoubleToInt64Bits(-0D);
    public static bool IsNegativeZero(this double d)
    {
        return BitConverter.DoubleToInt64Bits(d) == _DoubleNegativeZero;
    }
    public static bool IsNegativeZero(this float f)
    {
        return BitConverter.DoubleToInt64Bits(f) == _DoubleNegativeZero;
    }

    public static int Count(this string @in, char c)
    {
        int count = 0;
        for (int i = 0; i < @in.Length; i++)
        {
            if (@in[i] == c) count++;
        }
        return count;
    }

    internal static bool ContainsWhitespace(this string s)
    {
        if (string.IsNullOrEmpty(s))
            return false;

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (char.IsWhiteSpace(c))
                return true;
        }

        return false;
    }

    public static T GetFirst<T>(this IEnumerable<T> e)
    {
        foreach (T t in e)
        {
            return t;
        }
        return default(T);
    }

    public static string ToTitleCaseInvariant(this string s)
    {
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s);
    }

    public static string ToStringInvariant(this float o)
    {
        return o.ToString(CultureInfo.InvariantCulture);
    }

    public static string RemovePrefix(this string str, string prefix)
    {
        return str.StartsWithInvariant(prefix) ? str.Substring(prefix.Length) : str;
    }

    public static string RemoveSuffix(this string str, string suffix)
    {
        return str.StartsWithInvariant(suffix) ? str.Substring(0, suffix.Length - suffix.Length) : str;
    }
    public static int IndexOfInvariant(this string s, string a)
    {
        return s.IndexOf(a, StringComparison.InvariantCulture);
    }
    public static int IndexOfInvariant(this string s, string a, int i)
    {
        return s.IndexOf(a, i, StringComparison.InvariantCulture);
    }
    public static int LastIndexOfInvariant(this string s, string a)
    {
        return s.LastIndexOf(a, StringComparison.InvariantCulture);
    }
    public static int LastIndexOfInvariant(this string s, string a, int i)
    {
        return s.LastIndexOf(a, i, StringComparison.InvariantCulture);
    }
    public static bool StartsWithInvariant(this string s, string a)
    {
        return s.StartsWith(a, StringComparison.InvariantCulture);
    }
    public static bool EndsWithInvariant(this string s, string a)
    {
        return s.EndsWith(a, StringComparison.InvariantCulture);
    }

    public static string Combine(this IList<string> sa, string c)
    {
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < sa.Count; i++)
        {
            s.Append(sa[i]);
            if (i < sa.Count - 1)
            {
                s.Append(c);
            }
        }
        return s.ToString();
    }

    public static string CombineReversed(this IList<string> sa, string c)
    {
        StringBuilder s = new StringBuilder();
        for (int i = sa.Count - 1; 0 <= i; i--)
        {
            s.Append(sa[i]);
            if (0 < i)
            {
                s.Append(c);
            }
        }
        return s.ToString();
    }

    public static Type GetValueType(this MemberInfo info)
    {
        if (info is FieldInfo)
        {
            return ((FieldInfo)info).FieldType;
        }
        if (info is PropertyInfo)
        {
            return ((PropertyInfo)info).PropertyType;
        }
        if (info is MethodInfo)
        {
            return ((MethodInfo)info).ReturnType;
        }
        return null;
    }

    public static T AtOr<T>(this T[] a, int i, T or)
    {
        if (i < 0 || a.Length <= i) return or;
        return a[i];
    }

    public static void AddRange(this IDictionary to, IDictionary from)
    {
        foreach (DictionaryEntry entry in from)
        {
            to.Add(entry.Key, entry.Value);
        }
    }

    public static void ForEach<T>(this BindingList<T> list, Action<T> a)
    {
        for (int i = 0; i < list.Count; i++)
        {
            a(list[i]);
        }
    }
    public static void AddRange<T>(this BindingList<T> list, BindingList<T> other)
    {
        for (int i = 0; i < other.Count; i++)
        {
            list.Add(other[i]);
        }
    }

    public static int IndexOf(this object[] array, object elem)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == elem)
            {
                return i;
            }
        }
        return -1;
    }

    public static Texture2D Copy(this Texture2D texture, TextureFormat? format = TextureFormat.ARGB32) // an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s an3s
    {
        if (texture == null)
        {
            return null;
        }
        RenderTexture copyRT = RenderTexture.GetTemporary(
            texture.width, texture.height, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.Default
        );

        Graphics.Blit(texture, copyRT);

        RenderTexture previousRT = RenderTexture.active;
        RenderTexture.active = copyRT;

        Texture2D copy = new Texture2D(texture.width, texture.height, format != null ? format.Value : texture.format, 1 < texture.mipmapCount);
        copy.name = texture.name;
        copy.ReadPixels(new Rect(0, 0, copyRT.width, copyRT.height), 0, 0);
        copy.Apply(true, false);

        RenderTexture.active = previousRT;
        RenderTexture.ReleaseTemporary(copyRT);

        return copy;
    }

    public static Texture2D GetRW(this Texture2D texture)
    {
        if (texture == null)
        {
            return null;
        }
        if (texture.IsReadable())
        {
            return texture;
        }
        return texture.Copy();
    }

    public static bool IsReadable(this Texture2D texture)
    {
        // return texture.GetRawTextureData().Length != 0; // spams log
        try
        {
            texture.GetPixels();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static Type GetListType(this Type list)
    {
        if (list.IsArray)
        {
            return list.GetElementType();
        }

        Type[] ifaces = list.GetInterfaces();
        for (int i = 0; i < ifaces.Length; i++)
        {
            Type iface = ifaces[i];
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return list.GetGenericArguments()[0];
            }
        }

        return null;
    }

    public static string GetPath(this Transform t)
    {
        List<string> path = new List<string>();
        do
        {
            path.Add(t.name);
        } while ((t = t.parent) != null);
        return path.CombineReversed("/");
    }

    public static GameObject AddChild(this GameObject go, string name, params Type[] components)
    {
        GameObject child = new GameObject(name, components);
        child.transform.SetParent(go.transform);
        child.transform.SetAsLastSibling();
        return child;
    }
}

public class JSONValueTypeBaseRule<T> : JSONRule<T>
{

    public override void WriteMetaHeader(JsonHelperWriter json, object obj)
    {
        // json.WriteProperty(JSONHelper.META.PROP, JSONHelper.META.VALUETYPE);
    }

    public override string ReadMetaHeader(JsonHelperReader json, ref Type type)
    {
        /*
        string metaProp = json.ReadPropertyName();
        if (metaProp != JSONHelper.META.PROP) {
            return metaProp;
        }
        string valuetype = (string) json.Value;
        json.Read(); // Drop String
        if (JSONHelper.CheckOnRead && valuetype != JSONHelper.META.VALUETYPE) {
            throw new JsonReaderException("Type mismatch! Expected " + JSONHelper.META.VALUETYPE + ", got " + valuetype);
        }
        */
        return null;
    }

}

public class JSONValueTypeRule : JSONValueTypeBaseRule<ValueType>
{

}

public class JSONVector2Rule : JSONValueTypeBaseRule<Vector2>
{
    public override void Serialize(JsonHelperWriter json, object obj)
    {
        Vector2 v = (Vector2)obj;
        json.WriteProperty("x", v.x);
        json.WriteProperty("y", v.y);
    }
    public override object New(JsonHelperReader json, Type type)
    {
        return new Vector2();
    }
    public override object Deserialize(JsonHelperReader json, object obj)
    {
        Vector2 v = (Vector2)obj;
        v.Set(
            (float)(double)json.ReadRawProperty("x"),
            (float)(double)json.ReadRawProperty("y")
        );
        return v;
    }
}

public class JSONVector3Rule : JSONValueTypeBaseRule<Vector3>
{
    public override void Serialize(JsonHelperWriter json, object obj)
    {
        Vector3 v = (Vector3)obj;
        json.WriteProperty("x", v.x);
        json.WriteProperty("y", v.y);
        json.WriteProperty("z", v.z);
    }
    public override object New(JsonHelperReader json, Type type)
    {
        return new Vector3();
    }
    public override object Deserialize(JsonHelperReader json, object obj)
    {
        Vector3 v = (Vector3)obj;
        v.Set(
            (float)(double)json.ReadRawProperty("x"),
            (float)(double)json.ReadRawProperty("y"),
            (float)(double)json.ReadRawProperty("z")
        );
        return v;
    }
}

public class JSONVector4Rule : JSONValueTypeBaseRule<Vector4>
{
    public override void Serialize(JsonHelperWriter json, object obj)
    {
        Vector4 v = (Vector4)obj;
        json.WriteProperty("x", v.x);
        json.WriteProperty("y", v.y);
        json.WriteProperty("z", v.z);
        json.WriteProperty("w", v.w);
    }
    public override object New(JsonHelperReader json, Type type)
    {
        return new Vector4();
    }
    public override object Deserialize(JsonHelperReader json, object obj)
    {
        Vector4 v = (Vector4)obj;
        v.Set(
            (float)(double)json.ReadRawProperty("x"),
            (float)(double)json.ReadRawProperty("y"),
            (float)(double)json.ReadRawProperty("z"),
            (float)(double)json.ReadRawProperty("w")
        );
        return v;
    }
}

public class JSONQuaternionRule : JSONValueTypeBaseRule<Quaternion>
{
    public override void Serialize(JsonHelperWriter json, object obj)
    {
        Quaternion q = (Quaternion)obj;
        json.WriteProperty("x", q.x);
        json.WriteProperty("y", q.y);
        json.WriteProperty("z", q.z);
        json.WriteProperty("w", q.w);
    }
    public override object New(JsonHelperReader json, Type type)
    {
        return new Quaternion();
    }
    public override object Deserialize(JsonHelperReader json, object obj)
    {
        Quaternion q = (Quaternion)obj;
        q.Set(
            (float)(double)json.ReadRawProperty("x"),
            (float)(double)json.ReadRawProperty("y"),
            (float)(double)json.ReadRawProperty("z"),
            (float)(double)json.ReadRawProperty("w")
        );
        return q;
    }
}

public class JSONAttachPointDataRule : JSONRule<AttachPointData>
{

    public override object New(JsonHelperReader json, Type type)
    {
        return new AttachPointData(null);
    }

}

public class JsonHelperWriter : JsonTextWriter
{

    public bool RootWritten = false;

    private readonly List<object> _RefList = new List<object>(512);
    private readonly Dictionary<object, int> _RefIdMap = new Dictionary<object, int>(512);

    private readonly List<object> _ObjPath = new List<object>(512);

    public string RelativeDir;
    public bool DumpRelatively = false;

    public JsonHelperWriter(TextWriter writer)
        : base(writer)
    {
    }

    public int GetReferenceID(object obj)
    {
        if (obj is Component)
        {
            return JSONHelper.META.REF_NONE;
        }
        int id;
        if (!_RefIdMap.TryGetValue(obj, out id))
        {
            return JSONHelper.META.REF_NONE;
        }
        return id;
    }
    public int GetReferenceType(int id, object a)
    {
        if (id == JSONHelper.META.REF_NONE || a is Component)
        {
            return JSONHelper.META.REF_NONE;
        }
        object b = _RefList[id];
        if (ReferenceEquals(a, b))
        {
            return JSONHelper.META.REF_TYPE_SAMEREF;
        }
        return JSONHelper.META.REF_TYPE_EQUAL;
    }

    public int RegisterReference(object obj)
    {
        // This breaks stuff. FIXME FIXME FIXME
        /*if (obj is Component) {
            return JSONHelper.META.REF_NONE;
        }*/
        int id = _RefList.Count;
        _RefList.Add(obj);
        _RefIdMap[obj] = id;
        return id;
    }

    public void AddPath(JsonHelperWriter json)
    {
        _ObjPath.AddRange(json._ObjPath);
    }
    public void Push(object obj)
    {
        _ObjPath.Add(obj);
    }
    public void Pop()
    {
        _ObjPath.RemoveAt(_ObjPath.Count - 1);
    }
    public object At(int pos, bool throwIfOOB = false)
    {
        if (pos < 0)
        {
            pos += _ObjPath.Count;
        }

        if (pos < 0 || _ObjPath.Count <= pos)
        {
            if (throwIfOOB)
            {
                StringBuilder message = new StringBuilder();
                message.Append("Requested object path position out of bounds!\nPosition: ").Append(pos)
                       .Append("\nObject path:");
                for (int i = 0; i < _ObjPath.Count; i++)
                {
                    message.AppendLine().Append(i).Append(": ").Append(_ObjPath[i]);
                }
                throw new JsonReaderException(message.ToString());
            }
            return null;
        }

        return _ObjPath[pos];
    }

}

public static partial class JSONHelper
{

    private readonly static Dictionary<UnityEngine.Object, string> _DumpObjPathMap = new Dictionary<UnityEngine.Object, string>();
    private readonly static Dictionary<string, int> _DumpNameIdMap = new Dictionary<string, int>();

    public static JsonHelperWriter OpenWriteJSON(string path)
    {
        File.Delete(path);
        Stream stream = File.OpenWrite(path);
        StreamWriter text = new StreamWriter(stream);
        JsonHelperWriter json = new JsonHelperWriter(text);
        json.RelativeDir = path.Substring(0, path.Length - 5);
        json.Formatting = Newtonsoft.Json.Formatting.Indented;
        return json;
    }

    public static void WriteJSON(this object obj, string path)
    {
        if (obj == null)
        {
            return;
        }
        if (obj is JToken)
        {
            File.Delete(path);
            File.WriteAllText(path, obj.ToString());
            return;
        }

        Type type = obj.GetType();
        if (obj is Enum || obj is string || obj is byte[] || type.IsPrimitive)
        {
            JToken.FromObject(obj).WriteJSON(path);
            return;
        }

        using (JsonHelperWriter json = OpenWriteJSON(path))
        {
            json.Write(obj);
        }
    }

    public static void Write(this JsonHelperWriter json, object obj)
    {
        if (obj == null)
        {
            json.WriteNull();
            return;
        }
        if (obj is JToken)
        {
            json.WriteRawValue(obj.ToString());
            return;
        }

        Type type = obj.GetType();
        if (obj is Enum || obj is string || obj is byte[] || type.IsPrimitive)
        {
            json.WriteValue(obj);
            return;
        }

        if (obj is Type)
        {
            json.WriteMetaType((Type)obj);
            return;
        }

        if (json.TryWriteMetaReference(obj, true))
        {
            return;
        }

        json.Push(obj);

        JSONRule rule = type.GetJSONRule();
        if (rule.GetType() == t_JSONRule)
        {

            if (obj is IList)
            {
                IList list = (IList)obj;
                json.WriteStartArray();
                if (type.IsArray)
                {
                    json.WriteMetaArrayData(META.ARRAYTYPE_ARRAY, list.Count);
                }
                else
                {
                    json.WriteMetaArrayData(META.ARRAYTYPE_LIST);
                }

                foreach (object o in list)
                {
                    json.Write(o);
                }
                json.WriteEndArray();
                json.Pop();
                return;
            }

            if (obj is IDictionary)
            {
                IDictionary dict = (IDictionary)obj;
                json.WriteStartArray();
                json.WriteMetaArrayData(META.ARRAYTYPE_MAP);
                foreach (DictionaryEntry e in dict)
                {
                    json.Write(e);
                }
                json.WriteEndArray();
                json.Pop();
                return;
            }

        }

        UnityEngine.Object so = (UnityEngine.Object)(
            ((object)(obj as GameObject)) ??
            ((object)(obj as ScriptableObject)) ??
            ((object)(obj as Component))
        );
        string name = so != null ? so.name : null;
        if (json.RootWritten && (json.DumpRelatively || SharedDir != null) && !string.IsNullOrEmpty(name) && !(obj is Transform))
        {
            if (SharedDir == null && json.DumpRelatively)
            {
                Directory.CreateDirectory(json.RelativeDir);
                string dumppath = Path.Combine(json.RelativeDir, name + ".json");
                if (!File.Exists(dumppath))
                {
                    using (JsonHelperWriter ext = OpenWriteJSON(dumppath))
                    {
                        ext.AddPath(json);
                        ext.RelativeDir = Path.Combine(json.RelativeDir, name);
                        ext.Write(obj);
                    }
                }
                json.WriteMetaExternal(name, META.EXTERNAL_IN_RELATIVE);

            }
            else if (SharedDir != null)
            {
                string path;
                if (_DumpObjPathMap.TryGetValue(so, out path))
                {
                    json.WriteMetaExternal(path, META.EXTERNAL_IN_SHARED);
                    json.Pop();
                    return;
                }
                path = type.Name + "s/" + name;

                int id;
                if (!_DumpNameIdMap.TryGetValue(path, out id))
                {
                    id = -1;
                }
                _DumpNameIdMap[name] = ++id;

                if (id != 0)
                {
                    path += "." + id;
                }
                _DumpObjPathMap[so] = path;

                string dumppath = Path.Combine(SharedDir, path.Replace('/', Path.DirectorySeparatorChar) + ".json");
                Directory.GetParent(dumppath).Create();
                if (!File.Exists(dumppath))
                {
                    using (JsonHelperWriter ext = OpenWriteJSON(dumppath))
                    {
                        ext.AddPath(json);
                        ext.Write(obj);
                    }
                }
                json.WriteMetaExternal(path, META.EXTERNAL_IN_SHARED);
            }
            json.Pop();
            return;
        }

        json.RootWritten = true;
        json.WriteStartObject();
        rule.WriteMetaHeader(json, obj);
        _OnBeforeSerialize(obj);
        rule.Serialize(json, obj);
        json.WriteEndObject();
        json.Pop();
    }

    public static void WriteStartMetadata(this JsonHelperWriter json, string metaType)
    {
        json.WriteStartObject();
        json.WriteProperty(META.MARKER, metaType);
    }
    public static void WriteEndMetadata(this JsonHelperWriter json)
    {
        json.WriteEndObject();
    }

    public static bool TryWriteMetaReference(this JsonHelperWriter json, object obj, bool register = false)
    {
        int id = json.GetReferenceID(obj);

        if (id != META.REF_NONE)
        {
            json.WriteStartMetadata(META.REF);

            json.WriteProperty(META.REF_ID, id);
            json.WriteProperty(META.REF_TYPE, json.GetReferenceType(id, obj));

            json.WriteEndMetadata();
            return true;
        }

        if (register)
        {
            json.RegisterReference(obj);
        }
        return false;
    }

    public static void WriteMetaObjectType(this JsonHelperWriter json, object obj)
    {
        json.WriteMetaType_(obj.GetType(), META.OBJTYPE);
    }
    public static void WriteMetaType(this JsonHelperWriter json, Type type)
    {
        json.WriteMetaType_(type, META.TYPE);
    }
    private static void WriteMetaType_(this JsonHelperWriter json, Type type, string metaType)
    {
        json.WriteStartMetadata(metaType);

        json.WriteProperty(META.TYPE_FULLNAME, type.FullName);
        /*string ns = type.Namespace;
        if (ns != null) {
            json.WritePropertyName(META.TYPE_SPLIT);
            json.WriteStartArray();
            json.Write(ns);
            json.Write(type.Name);
            json.WriteEndArray();
        }*/
        Type[] genparams = type.GetGenericArguments();
        if (genparams.Length != 0)
        {
            json.WriteProperty(META.TYPE_GENPARAMS, genparams);
        }

        json.WriteEndMetadata();
    }

    public static void WriteMetaExternal(this JsonHelperWriter json, string path, string @in = META.EXTERNAL_IN_RESOURCES)
    {
        json.WriteStartMetadata(META.EXTERNAL);

        if (@in != META.EXTERNAL_IN_RESOURCES)
        {
            json.WriteProperty(META.EXTERNAL_IN, @in);
        }
        json.WriteProperty(META.EXTERNAL_PATH, path);

        json.WriteEndMetadata();
    }

    public static void WriteMetaArrayData(this JsonHelperWriter json, string type, int size = -1)
    {
        json.WriteStartMetadata(META.ARRAYTYPE);
        json.WriteProperty(META.TYPE_FULLNAME, type);
        if (type == META.ARRAYTYPE_ARRAY)
        {
            json.WriteProperty(META.ARRAYTYPE_ARRAY_SIZE, size);
        }
        json.WriteEndMetadata();
    }

    public static void WriteProperty(this JsonHelperWriter json, string name, object obj)
    {
        json.WritePropertyName(name);
        json.Write(obj);
    }

    public static void Write(this JsonHelperWriter json, JSONRule rule, object obj, MemberInfo info)
    {
        rule.Serialize(json, obj, info);
    }

    public static void WriteAll(this JsonHelperWriter json, JSONRule rule, object obj, MemberInfo[] infos)
    {
        for (int i = 0; i < infos.Length; i++)
        {
            rule.Serialize(json, obj, infos[i]);
        }
    }

}
