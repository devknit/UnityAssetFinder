
using UnityEngine;
using UnityEditor;
using System;

namespace Knit.EditorWindow
{
	internal static class EditorPrefsEx
	{
		public static void SetBool( string key, bool value)
		{
			EditorPrefs.SetBool( KeyWarpper( key), value);
		}
		public static bool GetBool( string key, bool defaultValue = false)
		{
			return EditorPrefs.GetBool( KeyWarpper( key), defaultValue);
		}
		public static void SetString( string key, string value)
		{
			EditorPrefs.SetString( KeyWarpper( key), value);
		}
		public static string GetString( string key, string defaultValue)
		{
			return EditorPrefs.GetString( KeyWarpper( key), defaultValue);
		}
		public static void SetInt( string key, int value)
		{
			EditorPrefs.SetInt( KeyWarpper( key), value);
		}
		public static int GetInt( string key, int defaultValue)
		{
			return EditorPrefs.GetInt( KeyWarpper( key), defaultValue);
		}
		public static void SetFloat( string key, float value)
		{
			EditorPrefs.SetFloat( KeyWarpper( key), value);
		}
		public static float GetFloat( string key, float defaultValue)
		{
			return EditorPrefs.GetFloat( KeyWarpper( key), defaultValue);
		}
		public static void SetObject( string key, object obj)
		{
			if( obj == null)
			{
				return;
			}
			string value = JsonUtility.ToJson( obj);
			
			if( string.IsNullOrEmpty( value) == false)
			{
				EditorPrefs.SetString( KeyWarpper( key), value);
			}
		}
		public static object GetObject( string key, Type type)
		{
			string value = EditorPrefs.GetString( KeyWarpper( key), null);
			
			if( string.IsNullOrEmpty( value) != false)
			{
				return null;
			}
			return JsonUtility.FromJson( value, type);
		}
		public static bool HasKey( string key)
		{
			return EditorPrefs.HasKey( KeyWarpper( key));
		}
		public static void DeleteKey( string key)
		{
			EditorPrefs.DeleteKey( KeyWarpper( key));
		}
		private static string KeyWarpper( string key)
		{
			if( string.IsNullOrEmpty( key) != false)
			{
				key = "Default";
			}
			return Application.productName + ".EditorPrefsEx." + key;
		}
	}
}