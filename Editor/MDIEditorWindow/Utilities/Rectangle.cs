
using UnityEngine;
using System;

namespace Knit.EditorWindow
{
	[Serializable]
	public struct Rectangle
	{
		public Rectangle( float x, float y, float z, float w, bool anchorLeft, bool anchorRight, bool anchorTop, bool anchorBottom)
		{
			m_UseSimplePercentage = false;
			m_X = x;
			m_Y = y;
			m_Z = z;
			m_W = w;
			m_AnchorLeft = anchorLeft;
			m_AnchorRight = anchorRight;
			m_AnchorTop = anchorTop;
			m_AnchorBottom = anchorBottom;
		}
		public Rectangle( float x, float y, float width, float height)
		{
			m_UseSimplePercentage = true;
			m_X = x;
			m_Y = y;
			m_Z = width;
			m_W = height;
			m_AnchorLeft = false;
			m_AnchorRight = false;
			m_AnchorTop = false;
			m_AnchorBottom = false;
		}
		public Rect GetRect( Rect rect)
		{
			if( m_UseSimplePercentage != false)
			{
				return new Rect( rect.x + rect.width * m_X, rect.y + rect.height * m_Y, rect.width * m_Z, rect.height * m_W);
			}
			else
			{
				Rect result = default;
				
				if( m_AnchorLeft != false && m_AnchorRight != false)
				{
					result.x = rect.x + m_X;
					result.width = rect.width - m_X - m_Z;
				}
				else if( m_AnchorLeft != false)
				{
					result.x = rect.x + m_X;
					result.width = m_Z;
				}
				else if( m_AnchorRight != false)
				{
					result.x = rect.x + rect.width - m_X - m_Z;
					result.width = m_Z;
				}
				else
				{
					result.x = rect.x + rect.width / 2 + m_X - m_Z / 2;
					result.width = m_Z;
				}
				if( m_AnchorTop != false && m_AnchorBottom != false)
				{
					result.y = rect.y + m_Y;
					result.height = rect.height - m_Y - m_W;
				}
				else if( m_AnchorTop != false)
				{
					result.y = rect.y + m_Y;
					result.height = m_W;
				}
				else if( m_AnchorBottom != false)
				{
					result.y = rect.y + rect.height - m_Y - m_W;
					result.height = m_W;
				}
				else
				{
					result.y = rect.y + rect.height / 2 + m_Y - m_W / 2;
					result.height = m_W;
				}
				return result;
			}
		}
		public bool UseSimplePercentage
		{
			get{ return m_UseSimplePercentage; }
			set{ m_UseSimplePercentage = value; }
		}
		public float X
		{
			get{ return m_X; }
			set{ m_X = value; }
		}
		public float Y
		{
			get{ return m_Y; }
			set{ m_Y = value; }
		}
		public float Z
		{
			get{ return m_Z; }
			set{ m_Z = value; }
		}
		public float W
		{
			get{ return m_W; }
			set{ m_W = value; }
		}
		public bool AnchorLeft
		{
			get{ return m_AnchorLeft; }
			set{ m_AnchorLeft = value; }
		}
		public bool AnchorTop
		{
			get{ return m_AnchorTop; }
			set{ m_AnchorTop = value; }
		}
		public bool AnchorRight
		{
			get{ return m_AnchorRight; }
			set{ m_AnchorRight = value; }
		}
		public bool AnchorBottom
		{
			get{ return m_AnchorBottom; }
			set{ m_AnchorBottom = value; }
		}
		bool m_UseSimplePercentage;
		float m_X;
		float m_Y;
		float m_Z;
		float m_W;
		bool m_AnchorLeft;
		bool m_AnchorTop;
		bool m_AnchorRight;
		bool m_AnchorBottom;
	}
}