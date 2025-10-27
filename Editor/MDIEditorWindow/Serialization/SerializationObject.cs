
using UnityEngine;
using System;
using System.Reflection;

namespace Knit.EditorWindow
{
	[Serializable]
	internal class SerializationObject
	{
		SerializationObject( object obj)
		{
			m_Obj = obj;
			m_ObjectAssemblyName = obj.GetType().Assembly.FullName;
			m_ObjectClassName = obj.GetType().FullName;
		}
		public static SerializationObject CreateInstance( object handle)
		{
			if( handle == null)
			{
				return null;
			}
			return new SerializationObject( handle);
		}
		public void SaveObject(string windowID)
		{
			if( m_Obj == null)
			{
				return;
			}
			string id = windowID + "." + m_ObjectAssemblyName + "." + m_ObjectClassName;
			EditorPrefsEx.SetObject( id, this.m_Obj);
		}
		public void LoadObject( string windowID)
		{
			if( m_Obj != null)
			{
				return;
			}
			string id = windowID + "." + m_ObjectAssemblyName + "." + m_ObjectClassName;
			
			if( EditorPrefsEx.HasKey( id) == false)
			{
				return;
			}
			Assembly assembly = Assembly.Load( m_ObjectAssemblyName);
			
			if( assembly != null)
			{
				Type type = assembly.GetType( m_ObjectClassName);
				
				if( type != null)
				{
					m_Obj = EditorPrefsEx.GetObject( id, type);
				}
			}
		}
		public void ClearObject( string windowID)
		{
			EditorPrefsEx.DeleteKey( windowID + "." + m_ObjectAssemblyName + "." + m_ObjectClassName);
		}
		public object Obj
		{
			get { return m_Obj; }
		}
		object m_Obj;
		[SerializeField]
		string m_ObjectClassName;
		[SerializeField]
		string m_ObjectAssemblyName;
	}
}