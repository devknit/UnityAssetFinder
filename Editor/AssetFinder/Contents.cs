
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace Knit.EditorWindow.AssetFinder
{
	[System.Serializable]
	internal sealed class Contents
	{
		internal void OnEnable( System.Func<Contents> onCreateWindowContens)
		{
			m_ChangeProject = true;
			m_OnCreateWindowContens = onCreateWindowContens;
			
			if( m_Project != null)
			{
				m_Project.OnEnable( m_ClickType);
				m_Project.ColumnHeaderResizeToFit();
			}
			if( m_Select != null)
			{
				m_Select.OnEnable( m_ClickType);
				m_Select.ColumnHeaderResizeToFit();
			}
			if( m_Dependent != null)
			{
				m_Dependent.OnEnable( m_ClickType);
				m_Dependent.ColumnHeaderResizeToFit();
			}
		}
		internal void OnDisable()
		{
			if( m_Project != null)
			{
				m_Project.OnDisable();
			}
			if( m_Select != null)
			{
				m_Select.OnDisable();
			}
			if( m_Dependent != null)
			{
				m_Dependent.OnDisable();
			}
		}
		internal void OnToolbarGUI()
		{
			if( m_FindMode != Finder.Mode.None)
			{
				Rect searchTypeRect = EditorGUILayout.GetControlRect( GUILayout.Width( 160), GUILayout.Height( 17));
				
				var toolbarDropDownToggleRight = typeof( EditorStyles).GetProperty( 
					"toolbarDropDownToggleRight", BindingFlags.Static | BindingFlags.NonPublic).GetValue( null) as GUIStyle;
				var toolbarDropDownToggleButton = typeof( EditorStyles).GetProperty( 
					"toolbarDropDownToggleButton", BindingFlags.Static | BindingFlags.NonPublic).GetValue( null) as GUIStyle;
				
				Rect buttonRect = searchTypeRect;
				Rect popupRect = searchTypeRect;
				buttonRect.xMax -= 16;
				popupRect.xMin = popupRect.xMax - 16;
				
				if( EditorGUI.DropdownButton( popupRect, GUIContent.none, FocusType.Passive, toolbarDropDownToggleButton) != false)
				{
					var menu = new GenericMenu();
					
					menu.AddItem( new GUIContent( ObjectNames.NicifyVariableName( Finder.Mode.ToDependencies.ToString())), false, () =>
					{
						m_FindMode = Finder.Mode.ToDependencies;
						RefindAssets();
					});
					menu.AddItem( new GUIContent( ObjectNames.NicifyVariableName( Finder.Mode.FromDependencies.ToString())), false, () =>
					{
						m_FindMode = Finder.Mode.FromDependencies;
						RefindAssets();
					});
					menu.DropDown( buttonRect);
				}
				else if( GUI.Button( buttonRect, new GUIContent( ObjectNames.NicifyVariableName( m_FindMode.ToString())), toolbarDropDownToggleRight))
				{
					RefindAssets();
				}
				using( new EditorGUILayout.HorizontalScope( EditorStyles.toolbar))
				{
					bool newRecursive = EditorGUILayout.ToggleLeft( "Recursive", m_Recursive, GUILayout.Width( 80));
					
					if( m_Recursive != newRecursive)
					{
						m_Recursive = newRecursive;
						RefindAssets();
					}
				}
			}
			GUILayout.FlexibleSpace();
			
			var newClickType = (ClickType)EditorGUILayout.Popup( (int)m_ClickType, kClickTypes, EditorStyles.toolbarPopup, GUILayout.Width( 120));
			if( m_ClickType != newClickType)
			{
				if( m_Project != null)
				{
					m_Project.SetClickType( newClickType);
				}
				if( m_Select != null)
				{
					m_Select.SetClickType( newClickType);
				}
				if( m_Dependent != null)
				{
					m_Dependent.SetClickType( newClickType);
				}
				m_ClickType = newClickType;
			}
		}
		internal void OnProjectChange()
		{
			m_ChangeProject = true;
		}
		internal void OnProjectGUI()
		{
			if( m_Project == null)
			{
				m_Project = new Explorer( GetAllAssetElements(), TreeView.Column.Project);
				m_Project.OnEnable( m_ClickType);
				m_Project.ColumnHeaderResizeToFit();
			}
			else if( m_ChangeProject != false)
			{
				m_Project.Apply( GetAllAssetElements());
			}
			m_ChangeProject = false;
			m_Project.OnGUI( this);
		}
		internal void OnSelectGUI()
		{
			if( m_Select == null)
			{
				m_Select = new Explorer( new List<Element>(), TreeView.Column.Select);
				m_Select.OnEnable( m_ClickType);
				m_Select.ColumnHeaderResizeToFit();
			}
			m_Select.OnGUI( this);
		}
		internal void OnDependentGUI()
		{
			if( m_Dependent == null)
			{
				m_Dependent = new Explorer( new List<Element>(), TreeView.Column.Dependent);
				m_Dependent.OnEnable( m_ClickType);
				m_Dependent.ColumnHeaderResizeToFit();
			}
			m_Dependent.OnGUI( this);
		}
		List<Element> GetAllAssetElements()
		{
			EditorUtility.DisplayProgressBar( "Enumerating Assets", "", 0);
			var paths = AssetDatabase.GetAllAssetPaths();
			var builder = new ElementBuilder();
			
			if( paths != null)
			{
				for( int i0 = 0; i0 < paths.Length; ++i0)
				{
					EditorUtility.DisplayProgressBar( "Enumerating Assets", paths[ i0], i0 / (float)paths.Length);
					builder.Append( paths[ i0]);
				}
			}
			EditorUtility.DisplayProgressBar( "Enumerating Assets", "Done", 1);
			EditorUtility.ClearProgressBar();
			return builder.ToList();
		}
		internal void OpenFindAssets( IEnumerable<string> assetGuids, Finder.Mode newFindType)
		{
			Contents contents = m_OnCreateWindowContens?.Invoke();
			contents?.FindAssets( assetGuids, newFindType);
		}
		internal void FindAssets( IEnumerable<string> assetGuids, Finder.Mode newFindType)
		{
			Finder.Execute( 
				newFindType, assetGuids, m_Recursive, 
				out Dictionary<string, ElementSource> targets, 
				out Dictionary<string, ElementSource> results);
			var builder = new ElementBuilder();
			
			foreach( var result in results)
			{
				builder.Append( result.Value);
			}
			if( m_Dependent == null)
			{
				m_Dependent = new Explorer( new List<Element>(), TreeView.Column.Dependent);
				m_Dependent.OnEnable( m_ClickType);
				m_Dependent.ColumnHeaderResizeToFit();
			}
			m_Dependent.Apply( builder.ToList());
			m_Dependent.ExpandAll();
			
			builder = new ElementBuilder();
			
			foreach( var target in targets)
			{
				builder.Append( target.Value);
			}
			if( m_Select == null)
			{
				m_Select = new Explorer( new List<Element>(), TreeView.Column.Select);
				m_Select.OnEnable( m_ClickType);
				m_Select.ColumnHeaderResizeToFit();
			}
			m_Select.Apply( builder.ToList());
			m_Select.ExpandAll();
			
			m_SelectGuids = assetGuids.ToArray();
			m_FindMode = newFindType;
		}
		internal bool RefindAssets()
		{
			if( m_SelectGuids != null && m_FindMode != Finder.Mode.None)
			{
				FindAssets( m_SelectGuids, m_FindMode);
				return true;
			}
			return false;
		}
		static readonly string[] kClickTypes = new []
		{
			"None", 
			"Ping", "Ping - file only", 
			"Active", "Active - file only"
		};
		[SerializeField]
		Explorer m_Project;
		[SerializeField]
		Explorer m_Select;
		[SerializeField]
		Explorer m_Dependent;
		[SerializeField]
		bool m_ChangeProject;
		[SerializeField]
		bool m_Recursive;
		[SerializeField]
		Finder.Mode m_FindMode;
		[SerializeField]
		string[] m_SelectGuids;
		[SerializeField]
		ClickType m_ClickType = ClickType.ActiveFileOnly;
		
		System.Func<Contents> m_OnCreateWindowContens;
	}
}
