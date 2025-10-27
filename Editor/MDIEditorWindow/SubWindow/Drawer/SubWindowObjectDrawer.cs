
using System;
using UnityEngine;
using UnityEditor;

namespace Knit.EditorWindow
{
	internal class SubWindowObjectDrawer : SubWindowDrawerBase
	{
		protected override SubWindowHelpBox helpBox
		{
			get{ return m_ObjDrawer.helpBox; }
		}
		public override GUIContent Title
		{
			get{ return m_ObjDrawer.Title; }
		}
		protected override SubWindowToolbarType toolBar
		{
			get { return m_ObjDrawer.toolBar; }
		}
		SubWindowCustomDrawer m_ObjDrawer;
		bool m_IsLock = false;
		
		public SubWindowObjectDrawer( SubWindowCustomDrawer drawer)
		{
			m_ObjDrawer = drawer;
			
			if( m_ObjDrawer == null)
			{
				return;
			}
			string id = GetID( false);
			
			if( EditorPrefsEx.HasKey( id) != false)
			{
				var obj = EditorPrefsEx.GetObject( id, drawer.GetType());
				
				if (obj != null)
				{
					drawer = obj as SubWindowCustomDrawer;
					drawer.SetContainer( m_ObjDrawer.Container);
					m_ObjDrawer = drawer;
				}
			}
		}
		internal static string GetDrawerID( SubWindowCustomDrawer drawer, bool dynamic)
		{
			string result = null;
			
			if( drawer == null)
			{
				return result;
			}
			result = "__CLASS__" + drawer.GetType().FullName;
			
			if( dynamic != false)
			{
				if( drawer.Title != null && string.IsNullOrEmpty( drawer.Title.text) == false)
				{
					result += "." + drawer.GetHashCode();
				}
				else
				{
					result += ".UnKnown";
				}
			}
			return result;
		}
		internal static string GetDrawerIDByType( Type type, string title, bool dynamic)
		{
			string result = null;
			
			if( type == null)
			{
				return null;
			}
			if( type.IsSubclassOf( typeof( SubWindowCustomDrawer)) != false)
			{
				result = "__CLASS__" + type.FullName;
			}
			if( dynamic != false)
			{
				if( string.IsNullOrEmpty( title) == false)
				{
					result += "." + title;
				}
				else
				{
					result += ".UnKnownTitle";
				}
			}
			return result;
		}
		public override string GetID( bool dynamic)
		{
			return GetDrawerID( m_ObjDrawer, dynamic);
		}
		public override void DrawWindow( Rect mainRect, Rect toolbarRect, Rect helpboxRect)
		{
			m_ObjDrawer.DrawMainWindow( mainRect);
			
			if( toolbarRect.width > 0 && toolbarRect.height > 0)
			{
				m_ObjDrawer.DrawToolBar( toolbarRect);
			}
			if( helpboxRect.width > 0 && helpboxRect.height > 0)
			{
				m_ObjDrawer.DrawHelpBox( helpboxRect);
			}
		}
		public override void DrawLeafToolBar( Rect rect)
		{
			base.DrawLeafToolBar( rect);
			
			if( m_ObjDrawer is ISubWinCustomMenu windowMenu)
			{
				var popRect = new Rect( rect.x + rect.width - 12, rect.y + 7, 13, 11);
				
				if( GUI.Button( popRect, string.Empty, GUIStyleCache.GetStyle( "PaneOptions")) != false)
				{
					var menu = new GenericMenu();
					
					windowMenu.AddCustomMenu( menu);
					
					if( menu.GetItemCount() > 0)
					{
						menu.DropDown(popRect);
					}
				}
				rect = new Rect( rect.x + rect.width - 40, rect.y, rect.width - 40, rect.height);
			}
			if( m_ObjDrawer is ISubWinLock windowLock)
			{
				EditorGUI.BeginChangeCheck();
				
				m_IsLock = GUI.Toggle( new Rect( 
					rect.x + rect.width - 20, rect.y + 3, 13, 11), m_IsLock, 
					string.Empty, GUIStyleCache.GetStyle( "IN LockButton"));
				
				if( EditorGUI.EndChangeCheck() != false)
				{
					windowLock.SetLockActive( m_IsLock);
				}
			}
		}
		protected override void OnDisable()
		{
			base.OnDisable();
			m_ObjDrawer.OnDisable();
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
			m_ObjDrawer.OnDestroy();
			string id = GetID( false);
			
			if( EditorPrefsEx.HasKey( id) != false)
			{
				EditorPrefsEx.DeleteKey( id);
			}
		}
		protected override void OnEnable()
		{
			base.OnEnable();
			m_ObjDrawer.OnEnable();
		}
		protected override bool OnInit()
		{
			m_ObjDrawer.Init();
			return true;
		}
		protected override void OnSerialize( bool dynamic)
		{
			base.OnSerialize( dynamic);
			
			if( dynamic == false)
			{
				string id = GetID( false);
				EditorPrefsEx.SetObject( id, m_ObjDrawer);
			}
		}
	}
}