
using System;

namespace Knit.EditorWindow
{
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class MsgBoxHandleAttribute : Attribute
	{
		public MsgBoxHandleAttribute( Type type, int id)
		{
			m_Id = id;
			m_Type = type;
		}
		public int Id
		{
			get{ return m_Id; }
		}
		public Type Type
		{
			get{ return m_Type; }
		}
        readonly int m_Id;
        readonly Type m_Type;
	}
}