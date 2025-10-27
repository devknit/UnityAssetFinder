
using System;
using UnityEngine;

namespace Knit.EditorWindow
{
	[Serializable]
	public abstract class ComponentCustomDrawerBase : IMessageDispatcher
	{
		public void SetContainer( object container)
		{
			if( m_Container != null)
			{
				Debug.LogError( "Error Containers are only allowed to be set during initialization!");
				return;
			}
			m_Container = container;
		}
		public abstract void Init();
		public abstract void OnEnable();
		public abstract void OnDisable();
		public abstract void OnDestroy();
		
		public Type GetContainerType()
		{
			if( m_Container == null)
			{
				return null;
			}
			return m_Container.GetType();
		}
		public object Container
		{
			get { return m_Container; }
		}
		object m_Container;
	}
}