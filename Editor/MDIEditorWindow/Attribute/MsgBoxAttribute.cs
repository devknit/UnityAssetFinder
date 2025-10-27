
using System;

namespace Knit.EditorWindow
{
	[AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class MsgBoxAttribute : Attribute
	{
		public MsgBoxAttribute( int id, float x = 0.2f, float y = 0.2f, float width = 0.6f, float height = 0.6f)
		{
			m_Id = id;
			Rectangle = new Rectangle( x, y, width, height);
		}
		public MsgBoxAttribute( int id, float x, float y, float z, float w, bool anchorLeft, bool anchorRight, bool anchorTop, bool anchorBottom)
		{
			m_Id = id;
			Rectangle = new Rectangle( x, y, z, w, anchorLeft, anchorRight, anchorTop, anchorBottom);
		}
		public Rectangle Rectangle
		{
			get; private set;
		}
		public int Id
		{
			get{ return m_Id; }
		}
		public int m_Id;
	}
}