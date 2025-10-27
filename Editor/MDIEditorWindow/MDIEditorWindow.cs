
using System;
using UnityEngine;
using UnityEditor;

namespace Knit.EditorWindow
{
	public class MDIEditorWindow : UnityEditor.EditorWindow, IMessageDispatcher
	{
		public static T CreateWindow<T>( object handle, string title) where T : MDIEditorWindow
		{
			T window = GetWindow<T>();
			title ??= typeof( T).Name;
			
			if( handle != null)
			{
				window.m_Handle = SerializationObject.CreateInstance( handle);
			}
			else
			{
				window.m_Handle = null;
			}
			window.Clear();
			window.Init();
			window.titleContent = new GUIContent( title);
			window.m_IsInitialized = true;
			return window;
		}
		public static T CreateWindow<T>( Type type, string title, params object[] args) where T : MDIEditorWindow
		{
			object obj = null;
			
			if( type != null)
			{
				obj = System.Activator.CreateInstance( type, args);
			}
			return CreateWindow<T>( obj, title);
		}
		public static T CreateNewWindow<T>( object handle, string title) where T : MDIEditorWindow
		{
		#if UNITY_2019_2_OR_NEWER
			T window = CreateWindow<T>();
		#else
			T window = CreateInstance<T>();
		#endif
			title ??= typeof( T).Name;
			
			if( handle != null)
			{
				window.m_Handle = SerializationObject.CreateInstance( handle);
			}
			else
			{
				window.m_Handle = null;
			}
			window.Clear();
			window.Init();
			window.titleContent = new GUIContent( title);
			window.m_IsInitialized = true;
			return window;
		}
		void OnGUI()
		{
			bool guienable = GUI.enabled;
			
			if( m_MsgBox != null && m_MsgBox.IsShowing)
			{
				GUI.enabled = false;
			}
			else
			{
				GUI.enabled = true;
			}
			GUI.BeginGroup( new Rect( 0, 0, position.width, position.height), GUIStyleCache.GetStyle( "LODBlackBox"));
			OnDrawGUI();
			GUI.EndGroup();
			
			GUI.enabled = guienable;
			DrawMsgBox( new Rect( 0, 0, position.width, position.height));
		}
		public void ShowMsgBox( int id, object obj)
		{
			if( m_MsgBox != null)
			{
				m_MsgBox.ShowMsgBox( id, obj);
			}
		}
		public void HideMsgBox()
		{
			if( m_MsgBox != null)
			{
				m_MsgBox.HideMsgBox();
			}
		}
		public void AddDynamicSubWindow( string title, string icon, Action<Rect> action)
		{
			AddDynamicSubWindowInternal( title, icon, SubWindowToolbarType.None, SubWindowHelpBoxType.None, action);
		}
		public void AddDynamicSubWindow( string title, SubWindowIcon icon, Action<Rect> action)
		{
			AddDynamicSubWindowInternal( title, icon, SubWindowToolbarType.None, SubWindowHelpBoxType.None, action);
		}
		public void AddDynamicSubWindowWithToolBar( string title, string icon, SubWindowToolbarType toolbar, Action<Rect, Rect> action)
		{
			if( toolbar != SubWindowToolbarType.None)
			{
				AddDynamicSubWindowInternal( title, icon, toolbar, SubWindowHelpBoxType.None, action);
			}
		}
		public void AddDynamicSubWindowWithToolBar( string title, SubWindowIcon icon, SubWindowToolbarType toolbar, Action<Rect, Rect> action)
		{
			if( toolbar != SubWindowToolbarType.None)
			{
				AddDynamicSubWindowInternal( title, icon, toolbar, SubWindowHelpBoxType.None, action);
			}
		}
		public void AddDynamicSubWindowWithHelpBox( string title, string icon, SubWindowHelpBoxType helpBoxType, Action<Rect, Rect> action)
		{
			if( helpBoxType != SubWindowHelpBoxType.None)
			{
				AddDynamicSubWindowInternal( title, icon, SubWindowToolbarType.None, helpBoxType, action);
			}
		}
		public void AddDynamicSubWindowWithHelpBox( string title, SubWindowIcon icon, SubWindowHelpBoxType helpBoxType, Action<Rect, Rect> action)
		{
			if (helpBoxType != SubWindowHelpBoxType.None)
			{
				AddDynamicSubWindowInternal(title, icon, SubWindowToolbarType.None, helpBoxType, action);
			}
		}
		public void AddDynamicFullSubWindow( string title, string icon, SubWindowToolbarType toolbar, SubWindowHelpBoxType helpBoxType, Action<Rect, Rect, Rect> action)
		{
			if( helpBoxType != SubWindowHelpBoxType.None && toolbar == SubWindowToolbarType.None)
			{
				AddDynamicSubWindowInternal( title, icon, toolbar, helpBoxType, action);
			}
		}
		public void AddDynamicFullSubWindow( string title, SubWindowIcon icon, SubWindowToolbarType toolbar, SubWindowHelpBoxType helpBoxType, Action<Rect, Rect, Rect> action)
		{
			if( helpBoxType != SubWindowHelpBoxType.None && toolbar == SubWindowToolbarType.None)
			{
				AddDynamicSubWindowInternal( title, icon, toolbar, helpBoxType, action);
			}
		}
		public void AddDynamicSubWindow<T>( T drawer) where T : SubWindowCustomDrawer
		{
			if( drawer != null && m_WindowTree != null)
			{
				m_WindowTree.AddDynamicWindow( drawer);
			}
		}
		public void RemoveDynamicSubWindow( Action<Rect> action)
		{
			RemoveDynamicSubWindowInternal( action);
		}
		public void RemoveDynamicSubWindow( Action<Rect, Rect> action)
		{
			RemoveDynamicSubWindowInternal( action);
		}
		public void RemoveDynamicSubWindow( Action<Rect, Rect, Rect> action)
		{
			RemoveDynamicSubWindowInternal( action);
		}
		public bool RemoveDynamicSubWindow<T>( T drawer)where T : SubWindowCustomDrawer
		{
			if( drawer != null && m_WindowTree != null)
			{
				return m_WindowTree.RemoveDynamicWindow( drawer);
			}
			return false;
		}
		public void RemoveAllDynamicSubWindow()
		{
			if( m_WindowTree != null)
			{
				m_WindowTree.RemoveAllDynamicWindow();
			}
		}
		protected virtual void OnEnable()
		{
			if( m_IsInitialized != false)
			{
				Init();
			}
		}
		protected virtual void OnProjectChange()
		{
			Init();
		}
		protected virtual void OnDisable()
		{
			if( m_WindowTree != null)
			{
				m_WindowTree.Disable();
			}
			if( m_ToolbarTree != null)
			{
				m_ToolbarTree.Disable();
			}
			if( m_MsgBox != null)
			{
				m_MsgBox.Disable();
			}
			SaveHandle();
		}
		protected virtual void OnDestroy()
		{
			if( m_WindowTree != null)
			{
				m_WindowTree.Destroy();
				m_WindowTree = null;
			}
			if( m_ToolbarTree != null)
			{
				m_ToolbarTree.Destroy();
				m_ToolbarTree = null;
			}
			if( m_MsgBox != null)
			{
				m_MsgBox.Destroy();
				m_MsgBox = null;
			}
			ClearHandle();
		}
		protected virtual void Clear()
		{
			if( m_WindowTree != null)
			{
				m_WindowTree.Destroy();
				m_WindowTree = null;
			}
			if( m_ToolbarTree != null)
			{
				m_ToolbarTree.Destroy();
				m_ToolbarTree = null;
			}
			if( m_MsgBox != null)
			{
				m_MsgBox.Destroy();
				m_MsgBox = null;
			}
		}
		protected virtual void Init()
		{
			LoadHandle();
			
			if( m_WindowTree == null)
			{
				if( Handle != null)
				{
					m_WindowTree = new SubWindowTree( Repaint, GetType().Name, Handle.GetType().Name);
				}
				else
				{
					m_WindowTree = new SubWindowTree( Repaint, GetType().Name, null);
				}
			}
			if( m_ToolbarTree == null)
			{
				m_ToolbarTree = new ToolBarTree();
			}
			if( m_MsgBox == null)
			{
				m_MsgBox = new MsgBox();
			}
			Type[] handleTypes = null;
			object[] handles = null;
			
			if( Handle != null)
			{
				handleTypes = new Type[]{ Handle.GetType(), GetType() };
				handles = new object[]{ Handle, this };
			}
			else
			{
				handleTypes = new Type[]{ GetType() };
				handles = new object[]{ this };
			}
			ComponentsInitializer.InitComponents( this, handleTypes, handles, m_WindowTree, m_ToolbarTree, m_MsgBox);
		}
		protected virtual void OnDrawGUI()
		{
			DrawWindowTree( new Rect( 0, EditorStyles.toolbar.fixedHeight, position.width, position.height - EditorStyles.toolbar.fixedHeight));
			DrawToolbar( new Rect( 0, 0, position.width, EditorStyles.toolbar.fixedHeight));
		}
		protected void DrawWindowTree( Rect rect)
		{
			if( m_WindowTree != null)
			{
				m_WindowTree.DrawWindowTree( rect);
			}
		}
		protected void DrawToolbar( Rect rect)
		{
			GUI.Box( rect, string.Empty, EditorStyles.toolbar);
			float offset = 80;
			
			if( m_WindowTree != null)
			{
				// m_WindowTree.DrawLayoutButton( new Rect( rect.x + rect.width - offset, rect.y, 70, rect.height));
				// offset += 70;
				m_WindowTree.DrawViewButton( new Rect( rect.x + rect.width - offset, rect.y, 70, rect.height));
				// offset += 70;
			}
			GUILayout.BeginArea( new Rect( rect.x + 6, rect.y, rect.width - offset, rect.height));
			GUILayout.BeginHorizontal();
			
			if( m_ToolbarTree != null)
			{
				m_ToolbarTree.DrawToolbar();
			}
			OnDrawToolBar();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		protected void DrawMsgBox(Rect rect)
		{
			if( m_MsgBox != null)
			{
				m_MsgBox.DrawMsgBox( rect);
			}
		}
		protected virtual void OnDrawToolBar()
		{
		}
		private void AddDynamicSubWindowInternal( string title, SubWindowIcon icon, SubWindowToolbarType toolbar, SubWindowHelpBoxType helpbox, Delegate action)
		{
			AddDynamicSubWindowInternal( title, GUIEx.GetIconPath( icon), toolbar, helpbox, action);
		}
		private void AddDynamicSubWindowInternal( string title, string icon, SubWindowToolbarType toolbar, SubWindowHelpBoxType helpbox, Delegate action)
		{
			if( m_WindowTree != null)
			{
				m_WindowTree.AddDynamicWindow( title, icon, toolbar, helpbox, action);
			}
		}
		private bool RemoveDynamicSubWindowInternal( Delegate action)
		{
			if( m_WindowTree != null)
			{
				return m_WindowTree.RemoveDynamicWindow( action);
			}
			return false;
		}
		public Type GetContainerType()
		{
			return GetType();
		}
		private void SaveHandle()
		{
			if( m_Handle != null)
			{
				string id = GetType().FullName;
				m_Handle.SaveObject( id);
			}
		}
		private void LoadHandle()
		{
			if( m_Handle != null)
			{
				string id = GetType().FullName;
				m_Handle.LoadObject( id);
			}
		}
		private void ClearHandle()
		{
			if( m_Handle != null)
			{
				string id = GetType().FullName;
				m_Handle.ClearObject(id);
			}
		}
		protected object Handle
		{
			get
			{
				if( m_Handle == null)
				{
					return null;
				}
				return m_Handle.Obj;
			}
		}
		[SerializeField]
		SerializationObject m_Handle;
		
		SubWindowTree m_WindowTree;
		ToolBarTree m_ToolbarTree;
		MsgBox m_MsgBox;
		bool m_IsInitialized;
	}
}