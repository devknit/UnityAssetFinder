
using UnityEngine;
using System.Reflection;

namespace Knit.EditorWindow
{
	[SubWindowStyle( SubWindowStyle.Default)]
	public class SubWindow
	{
		public SubWindow( string title, string icon, bool defaultOpen, MethodInfo method, 
			object target, SubWindowToolbarType toolbar, SubWindowHelpBoxType helpbox)
		{
			DefaultOpen = defaultOpen;
			m_Drawer = new SubWindowMethodDrawer( title, icon, method, target, toolbar, helpbox);
			m_Drawer.Init();
		}
		public SubWindow(bool defaultOpen, SubWindowCustomDrawer drawer)
		{
			DefaultOpen = defaultOpen;
			m_Drawer = new SubWindowObjectDrawer( drawer);
			m_Drawer.Init();
		}
		public string GetIndentifier()
		{
			if (m_Drawer != null)
			{
				return m_Drawer.GetID( m_IsDynamic);
			}
			return "Unknown.UnknownId";
		}
		public void AddCloseEventListener( System.Action<SubWindow> onClose)
		{
			m_OnClose = onClose;
		}
		public void DrawSubWindow( Rect rect)
		{
			Rect tb = m_Drawer.DrawToolBar( ref rect);
			Rect hb = m_Drawer.DrawHelpBox( ref rect);
			Rect mb = DrawMainArea(rect);
			m_Drawer.DrawWindow( mb, tb, hb);
		}
		public void DrawToolBarExt( Rect rect)
		{
			if( GUI.Button( new Rect( rect.x + rect.width - 21, rect.y + 2, 13, 13), string.Empty, GUIStyleCache.GetStyle( "ToolbarSearchCancelButton")) != false)
			{
				Close();
			}
			m_Drawer.DrawLeafToolBar( new Rect( rect.x, rect.y, rect.width - 27, rect.height));
		}
		public void Close()
		{
			if( IsOpen == false)
			{
				return;
			}
			IsOpen = false;
			
			if( m_OnClose != null)
			{
				m_OnClose( this);
			}
			m_Drawer.Disable();
		}
		public void Open()
		{
			if( IsOpen != false)
			{
				return;
			}
			IsOpen = true;
			m_Drawer.Enable();
		}
		public void Destroy()
		{
			IsOpen = false;
			m_Drawer.Destroy();
		}
		public void SerializeSubWindow()
		{
			m_Drawer.Serialize( m_IsDynamic);
		}
		protected virtual Rect DrawMainArea( Rect rect)
		{
			return rect;
		}
		public GUIContent Title
		{
			get{ return m_Drawer.Title; }
		}
		public bool DefaultOpen
		{
			get; private set;
		}
		public bool IsOpen
		{
			get; private set;
		}
		public bool IsDynamic
		{
			get{ return m_IsDynamic; }
			set{ m_IsDynamic = value; }
		}
		bool m_IsDynamic;
		System.Action<SubWindow> m_OnClose;
		readonly SubWindowDrawerBase m_Drawer;
	}
}