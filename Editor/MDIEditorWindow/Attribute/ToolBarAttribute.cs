
using System;

namespace Knit.EditorWindow
{
	[AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ToolBarAttribute : Attribute
	{
		public ToolBarAttribute( string menuItem, int priority = 1000)
		{
			m_MenuItem = menuItem;
			m_Priority = priority;
		}
		public string MenuItem
		{
			get{ return m_MenuItem; }
		}
		public int Priority
		{
			get{ return m_Priority; }
		}
        readonly string m_MenuItem;
        readonly int m_Priority;
	}
}