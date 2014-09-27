using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decoratid.Utils
{
    /// <summary>
    /// provides methods for generating random primitives and hydrating objects with them
    /// </summary>
    public static class PrimitivesUtil
    {
        #region Declarations
        private static System.Random _rnd = new System.Random();
        private static StringGenerator _gen = new StringGenerator();
        #endregion

        #region Random Primitive Generator
        public static bool BuildBool()
        {
            var i = _rnd.Next(0, 1);
            return i == 0;
        }
        public static byte BuildByte()
        {
            //between 0 and 255
            return (byte)_rnd.Next(0, 255);
        }
        public static sbyte BuildSByte()
        {
            //-128 to 127
            return (sbyte)_rnd.Next(-128, 127);
        }
        public static char BuildChar()
        {
            return _gen.GenerateAlphaNumeric(1, 1)[0];
        }
        public static decimal BuildDecimal()
        {
            return decimal.Parse(_gen.GenerateNumeric(3, 5) + "." + _gen.GenerateNumeric(3, 5));
        }
        public static double BuildDouble()
        {
            return double.Parse(_gen.GenerateNumeric(3, 5) + "." + _gen.GenerateNumeric(3, 5));
        }
        public static float BuildFloat()
        {
            return float.Parse(_gen.GenerateNumeric(3, 5) + "." + _gen.GenerateNumeric(3, 5));
        }
        public static int BuildInt()
        {
            return int.Parse(_gen.GenerateNumeric(3, 5));
        }
        public static uint BuildUInt()
        {
            return uint.Parse(_gen.GenerateNumeric(3, 5));

        }
        public static long BuildLong()
        {
            return long.Parse(_gen.GenerateNumeric(3, 5));

        }
        public static ulong BuildULong()
        {
            return ulong.Parse(_gen.GenerateNumeric(3, 5));

        }
        public static short BuildShort()
        {
            return (short)_rnd.Next(-32768, 32767);
        }
        public static ushort BuildUShort()
        {
            return (ushort)_rnd.Next(0, 65535);

        }
        public static string BuildString()
        {
            return _gen.GenerateAlphaNumeric(10, 50);
        }
        /// <summary>
        /// builds a list of each primitive system type
        /// </summary>
        /// <returns></returns>
        public static List<object> BuildPrimitives()
        {
            List<object> rv = new List<object>();

            rv.Add(BuildBool());
            rv.Add(BuildByte());
            rv.Add(BuildChar());
            rv.Add(BuildDecimal());
            rv.Add(BuildDouble());
            rv.Add(BuildFloat());
            rv.Add(BuildInt());
            rv.Add(BuildLong());
            rv.Add(BuildSByte());
            rv.Add(BuildShort());
            rv.Add(BuildString());
            rv.Add(BuildUInt());
            rv.Add(BuildULong());
            rv.Add(BuildUShort());

            return rv;
        }

        public static object GetRandomSystemPrimitive(Type primitiveType)
        {
            Condition.Requires(primitiveType).IsNotNull();
            var types = GetAllSystemPrimitiveTypes();
            if (!types.Contains(primitiveType))
                throw new InvalidOperationException();

            switch (primitiveType.FullName)
            {
                case "System.Boolean":
                    return BuildBool();
                case "System.Byte":
                    return BuildByte();
                case "System.SByte":
                    return BuildSByte();
                case "System.Char":
                    return BuildChar();
                case "System.Decimal":
                    return BuildDecimal();
                case "System.Double":
                    return BuildDouble();
                case "System.Single":
                    return BuildFloat();
                case "System.Int32":
                    return BuildInt();
                case "System.UInt32":
                    return BuildUInt();
                case "System.Int64":
                    return BuildLong();
                case "System.UInt64":
                    return BuildULong();
                //case "System.Object":
                //    return BuildObject();
                case "System.Int16":
                    return BuildShort();
                case "System.UInt16":
                    return BuildUShort();
                case "System.String":
                    return BuildString();
            }
            return null;
        }
        #endregion

        #region General Primitive Parsing
        public static List<Type> GetAllSystemPrimitiveTypes()
        {
            List<Type> rv = new List<Type>();
            rv.Add(typeof(bool));
            rv.Add(typeof(byte));
            rv.Add(typeof(char));
            rv.Add(typeof(decimal));
            rv.Add(typeof(double));
            rv.Add(typeof(float));
            rv.Add(typeof(int));
            rv.Add(typeof(long));
            //rv.Add(typeof(object));
            rv.Add(typeof(sbyte));
            rv.Add(typeof(short));
            rv.Add(typeof(string));
            rv.Add(typeof(uint));
            rv.Add(typeof(ulong));
            rv.Add(typeof(ushort));

            return rv;
        }

        private static List<Type> _primitiveTypes = GetAllSystemPrimitiveTypes();

        public static bool IsSystemPrimitive(object obj)
        {
            if (obj == null)
                return false;

            return _primitiveTypes.Contains(obj.GetType());
        }
        public static string ConvertSystemPrimitiveToString(object obj)
        {
            if (obj == null)
                return null;

            Type objType = obj.GetType();

            if (!_primitiveTypes.Contains(objType))
                throw new ArgumentOutOfRangeException();

            return obj.ToString();
        }

        public static Type GetSystemPrimitiveTypeBySimpleName(string name)
        {
            switch (name)
            {
                case "Boolean":
                    return typeof(bool);
                case "Byte":
                    return typeof(Byte);
                case "SByte":
                    return typeof(SByte);
                case "Char":
                    return typeof(Char);
                case "Decimal":
                    return typeof(Decimal);
                case "Double":
                    return typeof(Double);
                case "Single":
                    return typeof(Single);
                case "Int32":
                    return typeof(Int32);
                case "UInt32":
                    return typeof(UInt32);
                case "Int64":
                    return typeof(Int64);
                case "UInt64":
                    return typeof(UInt64);
                case "Int16":
                    return typeof(Int16);
                case "UInt16":
                    return typeof(UInt16);
                case "String":
                    return typeof(String);;
            }

            throw new InvalidOperationException();
        }
        public static object ConvertStringToSystemPrimitive(string data, Type primType)
        {
            Condition.Requires(data).IsNotNullOrEmpty();
            Condition.Requires(primType).IsNotNull();
           
            if (!_primitiveTypes.Contains(primType))
                throw new ArgumentOutOfRangeException();

            switch (primType.FullName)
            {
                case "System.Boolean":
                    return bool.Parse(data);
                case "System.Byte":
                    return byte.Parse(data);
                case "System.SByte":
                    return sbyte.Parse(data);
                case "System.Char":
                    return char.Parse(data);
                case "System.Decimal":
                    return decimal.Parse(data);
                case "System.Double":
                    return double.Parse(data);
                case "System.Single":
                    return Single.Parse(data);
                case "System.Int32":
                    return Int32.Parse(data);
                case "System.UInt32":
                    return UInt32.Parse(data);
                case "System.Int64":
                    return Int64.Parse(data);
                case "System.UInt64":
                    return UInt64.Parse(data);
                //case "System.Object":
                //    return BuildObject();
                case "System.Int16":
                    return Int16.Parse(data);
                case "System.UInt16":
                    return UInt16.Parse(data);
                case "System.String":
                    return data;
            }

            throw new InvalidOperationException();
        }
        #endregion

        #region Hydration Util
        public static void HydratePrimitives(object obj)
        {
            var primTypes = GetAllSystemPrimitiveTypes();

            Type thisType = obj.GetType();
            var fields = thisType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                if (field.IsInitOnly)
                    continue;

                if (primTypes.Contains(field.FieldType))
                {
                    var val = GetRandomSystemPrimitive(field.FieldType);
                    field.SetValue(obj, val);
                }
            }
        }
        #endregion
    }
}
