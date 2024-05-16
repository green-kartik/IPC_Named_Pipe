using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FileMappingInterface
{
	/// <summary>
	///  These are the datatype Supported by IPC
	/// </summary>
	public enum DataType
	{
		CHAR,
		STRING,
		BYTE,
		UINT,
		INT,
		LONG,
		ULONG,
		FLOAT,
		DOUBLE,
		BOOL,
	}
	internal static class DataTypeExtension
	{
		internal static string GetStringFromPrimitive(this object value)
		{
			return value.ToString();
		}
		internal static object GetPrimitive(this DataType dataType, string value) 
		{
			switch (dataType)
			{
				case DataType.CHAR:
					return char.Parse(value);
				case DataType.STRING:
					return value;
				case DataType.BYTE:
					return byte.Parse(value);
				case DataType.UINT:
					return uint.Parse(value);
				case DataType.INT:
					return int.Parse(value);
				case DataType.LONG:
					return long.Parse(value);
				case DataType.ULONG:
					return ulong.Parse(value);
				case DataType.FLOAT:
					return float.Parse(value);
				case DataType.DOUBLE:
					return double.Parse(value);
				case DataType.BOOL:
					return bool.Parse(value);
				default:
					return value;
			}
		}
		internal static DataType GetDataType(this string dataType) 
		{
			return (DataType) int.Parse(dataType);
		}
		internal static string DataTypeToString(this DataType dataType)
		{
			return ((int)dataType).ToString();
		}
		public static DataType GetDataType(this Type type)
		{
			if (type == typeof(char))
			{
				return DataType.CHAR;
			}
			if (type == typeof(string))
			{
				return DataType.STRING;
			}
			if (type == typeof(byte))
			{
				return DataType.BYTE;
			}
			if (type == typeof(uint))
			{
				return DataType.UINT;
			}
			if (type == typeof(int))
			{
				return DataType.INT;
			}
			if (type == typeof(long))
			{
				return DataType.LONG;
			}
			if (type == typeof(ulong))
			{
				return DataType.ULONG;
			}
			if (type == typeof(float))
			{
				return DataType.FLOAT;
			}
			if (type == typeof(double))
			{
				return DataType.DOUBLE;
			}
			if (type == typeof(bool))
			{
				return DataType.BOOL;
			}
			return DataType.STRING;
		}
		public static DataType GetDataType(this object dataType)
		{
			var type = dataType.GetType();
			if (type == typeof(char))
			{
				return DataType.CHAR;
			}
			if (type == typeof(string))
			{
				return DataType.STRING;
			}
			if (type == typeof(byte))
			{
				return DataType.BYTE;
			}
			if (type == typeof(uint))
			{	
				return DataType.UINT;
			}
			if (type == typeof(int))
			{
				return DataType.INT;
			}
			if (type == typeof(long))
			{
				return DataType.LONG;
			}
			if (type == typeof(ulong))
			{
				return DataType.ULONG;
			}	
			if (type == typeof(float))
			{
				return DataType.FLOAT;
			}
			if (type == typeof(double))
			{
				return DataType.DOUBLE;
			}
			if (type == typeof(bool))
			{
				return DataType.BOOL;
			}
			return DataType.STRING;
		}
	}
}
