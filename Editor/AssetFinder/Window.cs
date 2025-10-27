
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Knit.EditorWindow.AssetFinder
{
	sealed partial class Window : MDIEditorWindow, IHasCustomMenu
	{
		[MenuItem("Tools/Asset Finder/Select To Dependencies", true, priority = 1)]
		static bool IsFindToDependenciesInTools()
		{
			return Selection.assetGUIDs?.Length > 0;
		}
		[MenuItem("Tools/Asset Finder/Select To Dependencies", false, priority = 1)]
		static void FindToDependenciesInTools()
		{
			var window = CreateNewWindow<Window>( null, "Asset Finder");
			window.Show();
			window.m_Contents.FindAssets( Selection.assetGUIDs, Finder.Mode.ToDependencies);
		}
		[MenuItem("Tools/Asset Finder/Select From Dependencies", true, priority = 2)]
		static bool IsFindFromDependenciesInTools()
		{
			return Selection.assetGUIDs?.Length > 0;
		}
		[MenuItem("Tools/Asset Finder/Select From Dependencies", false, priority = 2)]
		static void FindFromDependenciesInTools()
		{
			var window = CreateNewWindow<Window>( null, "Asset Finder");
			window.Show();
			window.m_Contents.FindAssets( Selection.assetGUIDs, Finder.Mode.FromDependencies);
		}
		[MenuItem( "Tools/Asset Finder/Closes", priority = 3)]
		static void Closes()
		{
			if( s_ActiveWindows != null)
			{
				var windows = s_ActiveWindows.ToArray();
				
				for( int i0 = 0; i0 < windows.Length; ++i0)
				{
					windows[ i0].Close();
				}
				s_ActiveWindows = null;
			}
		}
		[MenuItem("Assets/Asset Finder/Select To Dependencies &f", true, priority = 22)]
		static bool IsFindToDependenciesInAssets()
		{
			return Selection.assetGUIDs?.Length > 0;
		}
		[MenuItem("Assets/Asset Finder/Select To Dependencies &f", false, priority = 22)]
		static void FindToDependenciesInAssets()
		{
			var window = CreateNewWindow<Window>( null, "Asset Finder");
			window.Show();
			window.m_Contents.FindAssets( Selection.assetGUIDs, Finder.Mode.ToDependencies);
		}
		[MenuItem("Assets/Asset Finder/Select From Dependencies", true, priority = 23)]
		static bool IsFindFromDependenciesInAssets()
		{
			return Selection.assetGUIDs?.Length > 0;
		}
		[MenuItem("Assets/Asset Finder/Select From Dependencies", false, priority = 23)]
		static void FindFromDependenciesInAssets()
		{
			var window = CreateNewWindow<Window>( null, "Asset Finder");
			window.Show();
			window.m_Contents.FindAssets( Selection.assetGUIDs, Finder.Mode.FromDependencies);
		}
		public void AddItemsToMenu( GenericMenu menu)
		{
			menu.AddItem
			(
				new GUIContent( "Close Tabs"),
				false,
				() => { Closes(); }
			);
		}
		protected override void OnProjectChange()
		{
			base.OnProjectChange();
			m_Contents.OnProjectChange();
		}
		protected override void OnEnable()
		{
			if( s_ActiveWindows == null)
			{
				s_ActiveWindows = new List<Window>();
			}
			if( s_ActiveWindows.Contains( this) == false)
			{
				s_ActiveWindows.Add( this);
			}
			base.OnEnable();
			
			if( m_Contents == null)
			{
				m_Contents = new Contents();
			}
			m_Contents.OnEnable( () =>
			{
				var window = CreateNewWindow<Window>( null, "Asset Finder");
				var windowPosition = position;
				windowPosition.x += 32;
				windowPosition.y += 32;
				window.position = windowPosition;
				window.Show();
				return window.m_Contents;
			});
		}
		protected override void OnDisable()
		{
			if( s_ActiveWindows != null)
			{
				if( s_ActiveWindows.Contains( this) != false)
				{
					s_ActiveWindows.Remove( this);
				}
			}
			base.OnDisable();
			
			if( m_Contents != null)
			{
				m_Contents.OnDisable();
			}
		}
		protected override void OnDrawGUI()
		{
			base.OnDrawGUI();
			
			Event ev = Event.current;
			
			if( ev.type == EventType.KeyDown)
			{
				switch( ev.keyCode)
				{
					case KeyCode.Escape:
					{
						Close();
						ev.Use();
						break;
					}
					case KeyCode.F5:
					{
						if( m_Contents.RefindAssets() != false)
						{
							ev.Use();
							Repaint();
						}
						break;
					}
					case KeyCode.F:
					{
						ev.Use();
						break;
					}
				}
			}
		}
		protected override void OnDrawToolBar()
		{
			m_Contents.OnToolbarGUI();
		}
		[SubWindow( "Project", SubWindowIcon.Project, false)]
		void OnProjectGUI( Rect rect)
		{
			GUILayout.BeginArea( rect);
			{
				m_Contents.OnProjectGUI();
			}
			GUILayout.EndArea();
		}
		[SubWindow( "Select", SubWindowIcon.Project)]
		void OnSelectGUI( Rect rect)
		{
			GUILayout.BeginArea( rect);
			{
				m_Contents.OnSelectGUI();
			}
			GUILayout.EndArea();
		}
		[SubWindow( "Dependent", SubWindowIcon.Search)]
		void OnDependentGUI( Rect rect)
		{
			GUILayout.BeginArea( rect);
			{
				m_Contents.OnDependentGUI();
			}
			GUILayout.EndArea();
		}
		static List<Window> s_ActiveWindows = null;
		
		[SerializeField]
		Contents m_Contents;
	}
}
