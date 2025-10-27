
using System;

namespace Knit.EditorWindow
{
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class SubWindowHandleAttribute : Attribute
	{
		public SubWindowHandleAttribute( Type containerType, SubWindowStyle windowStyle = SubWindowStyle.Default, bool active = true)
		{
			m_ContainerType = containerType;
			m_WindowStyle = windowStyle;
			m_Active = active;
		}
		public Type ContainerType
		{
			get{ return m_ContainerType; }
		}
		public SubWindowStyle WindowStyle
		{
			get{ return m_WindowStyle; }
		}
		public bool Active
		{
			get{ return m_Active; }
		}
        readonly Type m_ContainerType;
        readonly SubWindowStyle m_WindowStyle;
        readonly bool m_Active;
	}
}
