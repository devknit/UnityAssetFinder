
using System;
using System.Reflection;

namespace Knit.EditorWindow
{
	public abstract class ComponentBase
	{
		public void RegisterMethod( object container, MethodInfo method, object target)
		{
			if( m_Initialized == false)
			{
				OnRegisterMethod( container, method, target);
			}
		}
		public void RegisterClass( object container, Type type)
		{
			if( m_Initialized == false)
			{
				OnRegisterClass( container, type);
			}
		}
		public void Init()
		{
			if( m_Initialized == false)
			{
				OnInit();
				m_Initialized = true;
			}
		}
		public void Destroy()
		{
			if( m_Initialized != false)
			{
				OnDestroy();
				m_Initialized = false;
			}
		}
		public void Disable()
		{
			if( m_Initialized != false)
			{
				OnDisable();
			}
		}
		protected abstract void OnRegisterMethod( object container, MethodInfo method, object target);
		protected abstract void OnRegisterClass( object container, Type type);
		
		protected virtual void OnInit()
		{
		}
		protected virtual void OnDestroy()
		{
		}
		protected virtual void OnDisable()
		{
		}
		public bool IsInitialized
		{
			get{ return m_Initialized; }
		}
		bool m_Initialized;
	}
}