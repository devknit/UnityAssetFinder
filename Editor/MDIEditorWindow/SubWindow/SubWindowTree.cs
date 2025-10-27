
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Knit.EditorWindow
{
	internal class SubWindowTree : ComponentBase
	{
		public SubWindowTree( Action repaint, string windowName, string handleName)
		{
			m_Repaint = repaint;
			m_PreAction = PreDropAction;
			m_PostAction = PostDropAction;
			m_OnSubWindowClose = OnSubWindowClosw;
			m_Layout = new SubWindowLayout( windowName, handleName);
		}
		protected override void OnRegisterMethod( object container, MethodInfo method, object target)
		{
			object[] atts = method.GetCustomAttributes( typeof( SubWindowAttribute), false);
			
			for( int i0 = 0; i0 < atts.Length; ++i0)
			{
				var att = atts[ i0] as SubWindowAttribute;
				
				if( SubWindowFactory.CreateSubWindow( att.WindowStyle, att.Title, att.IconPath,
					att.Active, method, target, att.Toolbar, att.HelpBox) is SubWindow window)
				{
					m_SubWindowList.Add( window);
				}
			}
		}
		protected override void OnRegisterClass( object container, Type type)
		{
			if( container == null)
			{
				return;
			}
			if( type.IsSubclassOf( typeof( SubWindowCustomDrawer)) == false)
			{
				return;
			}
			object[] atts = type.GetCustomAttributes( typeof( SubWindowHandleAttribute), false);
			
			for( int i0 = 0; i0 < atts.Length; ++i0)
			{
                if( atts[ i0] is not SubWindowHandleAttribute att)
				{
                    continue;
				}
                if( att.ContainerType != container.GetType())
				{
					continue;
				}
				if( SubWindowFactory.CreateSubWindow( container, att.Active, att.WindowStyle, type) is SubWindow window)
				{
					m_SubWindowList.Add( window);
				}
			}
		}
		protected override void OnInit()
		{
			base.OnInit();
			
			if( UseLayout( "Current") == false)
			{
				UseDefaultLayout();
			}
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
			RemoveAllDynamicWindow();
			SaveCurrentLayout();
			DestroyAllWindow();
		}
		protected override void OnDisable()
		{
			base.OnDisable();
			SerializeAllWindow();
		}
		public void AddDynamicWindow( string title, string icon, SubWindowToolbarType toolbar, SubWindowHelpBoxType helpbox, Delegate method)
		{
			if( method == null)
			{
				return;
			}
			string id = SubWindowMethodDrawer.GetMethodID( method.Method, method.Target);
			
			if( ContainWindowID( id) != false)
			{
				return;
			}
            var window = new SubWindow( title, icon, true, method.Method, method.Target, toolbar, helpbox)
            {
                IsDynamic = true
            };
            m_SubWindowList.Add( window);
			window.Open();
			InsertWindow( window);
		}
		public void AddDynamicWindow<T>( T drawer) where T : SubWindowCustomDrawer
		{
			if (drawer == null)
			{
				return;
			}
			string id = SubWindowObjectDrawer.GetDrawerID( drawer, true);
			
			if( ContainWindowID( id) != false)
			{
				return;
			}
            var window = new SubWindow( true, drawer)
            {
                IsDynamic = true
            };
            m_SubWindowList.Add( window);
			window.Open();
			InsertWindow( window);
		}
		public bool RemoveDynamicWindow( Delegate method)
		{
			string id = null;
			
			if( method == null)
			{
				id = SubWindowMethodDrawer.GetMethodID( null, null);
			}
			else
			{
				id = SubWindowMethodDrawer.GetMethodID( method.Method, method.Target);
			}
			return RemoveWindowByID(id);
		}
		public bool RemoveDynamicWindow<T>(T drawer) where T : SubWindowCustomDrawer
		{
			string id = null;
			
			if( drawer == null)
			{
				id = SubWindowObjectDrawer.GetDrawerID( null, true);
			}
			else
			{
				id = SubWindowObjectDrawer.GetDrawerID( drawer, true);
			}
			return RemoveWindowByID( id);
		}
		void SerializeAllWindow()
		{
			foreach( var win in m_SubWindowList)
			{
				win.SerializeSubWindow();
			}
		}
		bool RemoveWindowByID( string windowId)
		{
			if( string.IsNullOrEmpty( windowId) != false)
			{
				return false;
			}
			if( m_SubWindowList != null)
			{
				var win = GetWindowByID( windowId);
				
				if( win != null)
				{
					if( win.IsOpen != false)
					{
						if( m_Root != null)
						{
							m_Root.RemoveWindow( win);
							m_Root.ClearEmptyNode();
							m_Root.Recalculate( 0, true);
						}
					}
					m_SubWindowList.Remove( win);
					return true;
				}
			}
			return false;
		}
		public void RemoveAllDynamicWindow()
		{
			if( m_SubWindowList != null)
			{
				bool hasRemoved = false;
				
				for( int i0 = 0; i0 < m_SubWindowList.Count; ++i0)
				{
					if( m_SubWindowList[ i0].IsDynamic != false)
					{
						var win = m_SubWindowList[ i0];
						
						if( win.IsOpen != false)
						{
							if( m_Root != null)
							{
								m_Root.RemoveWindow( win);
								hasRemoved = true;
							}
						}
						win.Destroy();
						m_SubWindowList.RemoveAt( i0);
					}
				}
				if( hasRemoved != false)
				{
					if( m_Root != null)
					{
						m_Root.ClearEmptyNode();
						m_Root.Recalculate( 0, true);
					}
				}
			}
		}
		void DestroyAllWindow()
		{
			if( m_SubWindowList != null)
			{
				for( int i0 = 0; i0 < m_SubWindowList.Count; ++i0)
				{
					if( m_SubWindowList[ i0] != null)
					{
						m_SubWindowList[ i0].Destroy();
					}
				}
				m_SubWindowList.Clear();
			}
		}
		bool ContainWindowID( string windowId)
		{
			return GetWindowByID( windowId) != null;
		}
		SubWindow GetWindowByID( string windowId)
		{
			if( m_SubWindowList != null)
			{
				return m_SubWindowList.Find( w => w.GetIndentifier() == windowId);
			}
			return null;
		}
		public void DrawWindowTree( Rect rect)
		{
			if( m_Root != null)
			{
				m_Root.DrawGUI( rect, m_Repaint);
			}
			ListenEvent();
		}
		public void DrawViewButton( Rect rect)
		{
			// Rect rect = EditorGUILayout.GetControlRect( GUILayout.Width( 70), GUILayout.Height( 17));
			if( GUIEx.ToolbarButton( rect, "Tabs") != false)
			{
				if( m_Root != null)
				{
					var menu = new GenericMenu();
					
					for( int i0 = 0; i0 < m_SubWindowList.Count; ++i0)
					{
						menu.AddItem( new GUIContent( m_SubWindowList[ i0].Title), m_SubWindowList[ i0].IsOpen, OnSetSubWindowActive, m_SubWindowList[ i0]);
					}
					menu.DropDown( rect);
				}
			}
		}
		public void DrawLayoutButton( Rect rect)
		{
			if( GUI.Button( rect, "Layout", GUIStyleCache.GetStyle( "ToolbarDropDown")) != false)
			{
				if( m_Layout != null)
				{
					var menu = new GenericMenu();
					menu.AddItem( new GUIContent( "Default"), false, UseDefaultLayout);
					
					if( m_Layout.Layouts != null && m_Layout.Layouts.Count > 0)
					{
						for( int i0 = 0; i0 < m_Layout.Layouts.Count; i0++)
						{
							menu.AddItem( new GUIContent( m_Layout.Layouts[ i0]), false, OnUseLayout, m_Layout.Layouts[ i0]);
						}
					}
					menu.AddSeparator( "");
					menu.AddItem( new GUIContent( "Save Layout..."), false, OnSaveLayout);
					menu.AddItem( new GUIContent( "Delete Layout..."), false, OnDeleteLayout);
					menu.AddItem( new GUIContent( "Clear All Layout..."), false, OnRevertLayout);
					menu.DropDown( rect);
				}
			}
		}
		public void SaveCurrentLayout()
		{
			SaveLayoutCfgs( "Current");
		}
		void InsertWindow( SubWindow window)
		{
			window.AddCloseEventListener( m_OnSubWindowClose);
			
			if( m_Root == null)
			{
				m_Root = new SubWindowNode( true, 0);
			}
			m_Root.AddWindow( window, 0);
		}
		void ListenEvent()
		{
			if( m_Root == null)
			{
				return;
			}
			if( Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				m_CurrentDrag = m_Root.DragWindow( Event.current.mousePosition);
				
				if( m_CurrentDrag != null)
				{
					m_IsDraging = true;
					Event.current.Use();
				}
			}
			if( Event.current.type == EventType.MouseUp && Event.current.button == 0)
			{
				if( m_IsDraging != false)
				{
					m_IsDraging = false;
					
					if( m_CurrentDrag != null)
					{
						m_Root.DropWindow( Event.current.mousePosition, 0, m_CurrentDrag, m_PreAction, m_PostAction);
						Event.current.Use();
					}
					m_CurrentDrag = null;
				}
			}
			if( m_IsDraging != false)
			{
				m_Root.DrawAnchorArea( Event.current.mousePosition, 0, m_CurrentDrag);
				DrawFloatingWindow( Event.current.mousePosition);
				
				if( m_Repaint != null)
				{
					m_Repaint();
				}
			}
		}
		void DrawFloatingWindow( Vector2 position)
		{
			if( m_CurrentDrag == null)
			{
				return;
			}
			GUI.Box( new Rect( position.x - 50, position.y - 10, 100, 40), m_CurrentDrag.Title, GUIStyleCache.GetStyle( "window"));
		}
		void OnSubWindowClosw( SubWindow subWindow)
		{
			PreDropAction( subWindow);
			PostDropAction();
		}
		void PreDropAction( SubWindow subWindow)
		{
			if( m_Root != null)
			{
				if( subWindow != null)
				{
					m_Root.RemoveWindow( subWindow);
				}
			}
		}
		void PostDropAction()
		{
			if( m_Root != null)
			{
				m_Root.ClearEmptyNode();
				m_Root.Recalculate( 0, true);
			}
		}
		void OnSetSubWindowActive( object subwindow)
		{
			if( subwindow is not SubWindow window)
			{
				return;
			}
			if( window.IsOpen != false)
			{
				window.Close();
			}
			else
			{
				InsertWindow( window);
			}
		}
		bool UseLayout( string layoutName)
		{
			if( string.IsNullOrEmpty( layoutName) != false)
			{
				return false;
			}
			string treeId = GetTreeIndentifier();
			
			if( string.IsNullOrEmpty( treeId) != false)
			{
				return false;
			}
			if( m_Layout == null)
			{
				return false;
			}
			if( m_Root == null)
			{
				m_Root = new SubWindowNode(true, 0);
			}
			var element = m_Layout.UseLayout( layoutName, treeId);
			
			if( element == null)
			{
				return false;
			}
			for( int i0 = 0; i0 < m_SubWindowList.Count; ++i0)
			{
				if (m_SubWindowList[ i0].IsOpen != false)
				{
					m_SubWindowList[ i0].Close();
				}
			}
			m_Root.CreateFromLayoutCfg( element, m_SubWindowList, m_OnSubWindowClose);
			m_Root.ClearEmptyNode();
			m_Root.Recalculate( 0, true);
			return true;
		}
		void SaveLayoutCfgs( string layoutName)
		{
			if( string.IsNullOrEmpty( layoutName) != false)
			{
				return;
			}
			string treeId = GetTreeIndentifier();
			
			if( string.IsNullOrEmpty( treeId) != false)
			{
				return;
			}
			if( m_Layout == null)
			{
				return;
			}
			m_Layout.SaveLayout( layoutName, treeId, m_Root);
		}
		void OnUseLayout( object layout)
		{
			if( layout is not string layoutName)
			{
				return;
			}
			UseLayout( layoutName);
		}
		void UseDefaultLayout()
		{
			if( UseLayout( "Default") == false)
			{
				var alreadyOpen = new List<SubWindow>();
				
				for( int i0 = 0; i0 < m_SubWindowList.Count; ++i0)
				{
					if( m_SubWindowList[ i0].DefaultOpen != false)
					{
						alreadyOpen.Add( m_SubWindowList[ i0]);
						m_SubWindowList[ i0].Close();
					}
				}
				for( int i0 = alreadyOpen.Count - 1; i0 >= 0; --i0)
				{
					InsertWindow( alreadyOpen[ i0]);
				}
			}
		}
		void OnSaveLayout()
		{
			SubWindowTreeLayoutWizard.CreateWizard( m_Layout, GetTreeIndentifier(), m_Root);
		}
		void OnDeleteLayout()
		{
			if( m_Layout != null)
			{
				SubWindowTreeDeleteLayoutWizard.CreateWizard( m_Layout);
			}
		}
		void OnRevertLayout()
		{
			if( m_Layout != null)
			{
				m_Layout.RevertLayout();
			}
		}
		string GetTreeIndentifier()
		{
			if( m_SubWindowList != null)
			{
				string text = "";
				var idlist = new List<string>();
				
				foreach( var window in m_SubWindowList)
				{
					idlist.Add( window.GetIndentifier());
				}
				idlist.Sort();
				
				for( int i0 = 0; i0 < idlist.Count; ++i0)
				{
					if( i0 != idlist.Count - 1)
					{
						text = text + idlist[ i0] + "_";
					}
					else
					{
						text = text + idlist[ i0];
					}
				}
				return text;
			}
			return null;
		}
		SubWindowNode m_Root;
        readonly Action m_Repaint;
		SubWindow m_CurrentDrag;
		bool m_IsDraging;
        readonly Action<SubWindow> m_PreAction;
        readonly Action m_PostAction;
        readonly Action<SubWindow> m_OnSubWindowClose;
        readonly List<SubWindow> m_SubWindowList = new();
        readonly SubWindowLayout m_Layout;
	}
}