#define WITH_SEARCHSTRING
#define WITH_VIEWTYPELIST

using System.Text;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace Knit.EditorWindow.AssetFinder
{
	[System.Serializable]
	internal sealed class Explorer : ISerializationCallbackReceiver
	{
		internal Explorer( List<Element> elements, TreeView.Column columnMask) 
		{
			m_HeaderState = TreeView.CreateHeaderState( columnMask);
			m_ViewState = new TreeViewState();
			m_SerializableElement = new SerializableElementRoot();
			m_Elements = elements;
		}
		internal void OnEnable( ClickType clickType)
		{
			if( EditorGUIUtility.isProSkin == false)
			{
			#if WITH_VIEWTYPELIST
				m_ListTexture = "ListView";
				m_TreeTexture = "UnityEditor.SceneHierarchyWindow";
			#endif
				m_TypeTexture = "FilterByType";
			}
			else
			{
			#if WITH_VIEWTYPELIST
				m_ListTexture = "d_ListView";
				m_TreeTexture = "d_UnityEditor.SceneHierarchyWindow";
			#endif
				m_TypeTexture = "d_FilterByType";
			}
			MultiColumnHeaderState headerState = TreeView.CreateHeaderState();
			
			if( MultiColumnHeaderState.CanOverwriteSerializedFields( m_HeaderState, headerState) != false)
			{
				MultiColumnHeaderState.OverwriteSerializedFields( m_HeaderState, headerState);
			}
			m_HeaderState = headerState;
			m_SearchFilter = new SearchFilter( OnFilterChange);
			
			m_TreeView = new TreeView( m_ViewState, new MultiColumnHeader( m_HeaderState), m_SearchFilter);
			m_TreeView.SetClickType( clickType);
			
			m_SearchField = new SearchField();
			m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
			
			m_ObjectTypes = new PopupList.InputData();
			m_ObjectTypes.onSelectCallback = OnPopupSelect;
			
			for( int i0 = 0; i0 < AssetTypes.kTypeNames.Length; ++i0)
			{
				m_ObjectTypes.NewOrMatchingElement( AssetTypes.kTypeNames[ i0]);
			}		
			m_SearchFilter.Change( m_TreeView.searchString);
			Apply( m_Elements);
		}
		internal void OnDisable()
		{
			m_SearchField.downOrUpArrowKeyPressed -= m_TreeView.SetFocusAndEnsureSelectedItem;
		}
		internal void SetClickType( ClickType type)
		{
			m_TreeView.SetClickType( type);
		}
		internal void Apply( List<Element> src)
		{
			m_Elements = src;
			m_TreeView.Apply( m_Elements, m_ViewMode);
		}
		internal void ExpandAll()
		{
			m_TreeView.ExpandAll();
		}
		internal void CollapseAll()
		{
			m_TreeView.CollapseAll();
		}
		internal void ColumnHeaderResizeToFit()
		{
			m_TreeView.multiColumnHeader.ResizeToFit();
		}
		internal void SetColumnHeaderEnable( TreeView.Column column, bool bEnable)
		{
			m_TreeView.SetColumnHeaderEnable( column, bEnable);
		}
		internal void OnGUI( Contents contents)
		{
			Event ev = Event.current;
			
			if( ev.type == EventType.KeyDown)
			{
				switch( ev.keyCode)
				{
					case KeyCode.F:
					{
						FilterPath();
						ev.Use();
						break;
					}
					case KeyCode.L:
					{
						if( ev.control != false)
						{
							m_SearchField.SetFocus();
							ev.Use();
						}
						break;
					}
				}
			}
			using( new EditorGUILayout.VerticalScope())
			{
				using( new EditorGUILayout.HorizontalScope( EditorStyles.toolbar))
				{
				#if WITH_SEARCHSTRING
					string newSearchString = m_SearchField.OnToolbarGUI( m_SearchString, GUILayout.ExpandWidth( true));
					if( m_SearchString != newSearchString)
					{
						m_SearchString = newSearchString;
						m_SearchFilter.Change( m_SearchString);
						m_TreeView.Reload();
					}
				#else
					m_TreeView.searchString = searchField.OnToolbarGUI( m_TreeView.searchString, GUILayout.ExpandWidth( true));
				#endif
					GUILayout.Space( 4);
					
					var assetTypeContent = EditorGUIUtility.TrIconContent( m_TypeTexture, "Search by Type");
					Rect rect = GUILayoutUtility.GetRect( assetTypeContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth( false));
					
					if( EditorGUI.DropdownButton( rect, assetTypeContent,
						FocusType.Passive, EditorStyles.toolbarButton) != false)
					{
						PopupWindow.Show( rect, new PopupList( m_ObjectTypes));
					}
				#if WITH_VIEWTYPELIST
					var viewModeContent = EditorGUIUtility.TrIconContent( 
						(m_ViewMode != ViewMode.Tree)? m_ListTexture : m_TreeTexture, "View Mode");
					
					if( GUILayout.Button( viewModeContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth( false)) != false)
					{
						m_ViewMode = m_ViewMode switch
						{
							ViewMode.Tree => ViewMode.List,
							ViewMode.List => ViewMode.Tree,
							_ => m_ViewMode
						};
						m_TreeView.Apply( m_Elements, m_ViewMode);
					}
				#endif
				}
				using( var scope = new EditorGUILayout.VerticalScope( GUILayout.ExpandHeight( true)))
				{
					OnContextMenuEvent( contents, Event.current);
					m_TreeView.OnGUI( scope.rect);
				}
			}
		}
		void OnContextMenuEvent( Contents contents, Event ev)
		{
			if( ev.type == EventType.MouseUp && ev.button == 1)
			{
				if( m_TreeView.GetViewRect().Contains( ev.mousePosition) != false)
				{
					int selectedCount = m_TreeView.GetSelectedCount();
					if( selectedCount > 0)
					{
						var contextMenu = new GenericMenu();
						
						if( selectedCount == 1)
						{
							contextMenu.AddItem( new GUIContent( "Open"), false, () =>
							{
								Element element = m_TreeView.FirstSelectedElements( ( x => x.CanOpenAsset()));
								if( element.Directory != false)
								{
									m_TreeView.SetExpanded( element.id, !m_TreeView.IsExpanded( element.id));
								}
								else
								{
									AssetDatabase.OpenAsset( AssetDatabase.LoadMainAssetAtPath( element.Path));
								}
							});
							contextMenu.AddItem( new GUIContent( "Show in Explorer"), false, () =>
							{
								Element element = m_TreeView.FirstSelectedElements( x => true);
								EditorUtility.RevealInFinder( element.Path);
							});
						}
						if( selectedCount == 1 && m_TreeView.ContainsSeelctedElements( true, x => x.Directory) != false)
						{
							contextMenu.AddItem( new GUIContent( "Filter Path"), false, () =>
							{
								FilterPath();
							});
						}
						contextMenu.AddItem( new GUIContent( "Copy Path"), false, () =>
						{
							var elements = m_TreeView.SelectSelectedElements( x => x.Path);
							var builder = new System.Text.StringBuilder();
							foreach( var element in elements)
							{
								builder.AppendLine( element);
							}
							EditorGUIUtility.systemCopyBuffer = builder.ToString();
						});
						contextMenu.AddItem( new GUIContent( "Copy Guid"), false, () =>
						{
							var elements = m_TreeView.SelectSelectedElements( x => x.Guid);
							var builder = new System.Text.StringBuilder();
							foreach( var element in elements)
							{
								builder.AppendLine( element);
							}
							EditorGUIUtility.systemCopyBuffer = builder.ToString();
						});
						contextMenu.AddItem( new GUIContent( "Export Package/Select Only"), false, () =>
						{
							var assetPaths = m_TreeView.SelectSelectedElements( x => x.IsFile(), x => x.Path).ToArray();
							if( assetPaths.Length > 0)
							{
								string directory = System.IO.Path.GetFullPath( "Assets/../");
								string fileName = System.DateTime.Now.ToString( "yyyy-MM-dd_HH-mm-ss");
								string savePath = EditorUtility.SaveFilePanel( "Export Package", directory, fileName, "unitypackage");
								if( string.IsNullOrEmpty( savePath) == false)
								{
									AssetDatabase.ExportPackage( assetPaths, savePath, ExportPackageOptions.Default | ExportPackageOptions.Interactive);
								}
							}
						});
						contextMenu.AddItem( new GUIContent( "Export Package/Include Dependencies"), false, () =>
						{
							var assetPaths = m_TreeView.SelectSelectedElements( x => x.IsFile(), x => x.Path).ToArray();
							if( assetPaths.Length > 0)
							{
								string directory = System.IO.Path.GetFullPath( "Assets/../");
								string fileName = System.DateTime.Now.ToString( "yyyy-MM-dd_HH-mm-ss");
								string savePath = EditorUtility.SaveFilePanel( "Export Package", directory, fileName, "unitypackage");
								if( string.IsNullOrEmpty( savePath) == false)
								{
									AssetDatabase.ExportPackage( assetPaths, savePath, ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Interactive);
								}
							}
						});
						if( selectedCount == 1)
						{
							contextMenu.AddItem( new GUIContent( "Ping"), false,
							() =>
							{
								Element element = m_TreeView.FirstSelectedElements( x => x.CanPingObject());
								element?.PingObject( true);
							});
							contextMenu.AddItem( new GUIContent( "Active"), false,
							() =>
							{
								Element element = m_TreeView.FirstSelectedElements( x => x.CanActiveObject());
								element?.ActiveObject( true);
							});
						}
						contextMenu.AddItem( new GUIContent( "Select To Dependencies/New Window"), false, () =>
						{
							var elements = m_TreeView.SelectSelectedElements( x => x.Guid);
							contents?.OpenFindAssets( elements, Finder.Mode.ToDependencies);
						});
						contextMenu.AddItem( new GUIContent( "Select To Dependencies/Current Window"), false, () =>
						{
							var elements = m_TreeView.SelectSelectedElements( x => x.Guid);
							contents?.FindAssets( elements, Finder.Mode.ToDependencies);
						});
						contextMenu.AddItem( new GUIContent( "Select From Dependencies/New Window"), false, () =>
						{
							var elements = m_TreeView.SelectSelectedElements( x => x.Guid);
							contents?.OpenFindAssets( elements, Finder.Mode.FromDependencies);
						});
						contextMenu.AddItem( new GUIContent( "Select From Dependencies/Current Window"), false, () =>
						{
							var elements = m_TreeView.SelectSelectedElements( x => x.Guid);
							contents?.FindAssets( elements, Finder.Mode.FromDependencies);
						});
					#if false
						if( m_TreeView.ContainsSeelctedElements( AssetType.kMaterial, x => x.AssetType) != false)
						{
							contextMenu.AddItem( new GUIContent( "Material Cleaner"), false, () =>
							{
								var elements = m_TreeView.SelectSelectedElements( x => x.Guid);
								MaterialCleaner.Clean( elements);
							});
						}
					#endif
						contextMenu.ShowAsContext();
						ev.Use();
					}
				}
			}
		}
		void FilterPath()
		{
			if( m_TreeView.GetSelectedCount() == 1)
			{
				Element element = m_TreeView.FirstSelectedElements( ( x => true));
				var builder = new StringBuilder();
				
				builder.AppendFormat( "p:~/{0} ", element.Path);
				m_SearchFilter.ToBuildStringTypes( builder);
				m_SearchFilter.ToBuildStringNames( builder);
				
				string newSearchString = builder.ToString();
				if( m_SearchString != newSearchString)
				{
					m_SearchString = newSearchString;
					m_SearchFilter.Change( m_SearchString);
					m_TreeView.Reload();
				}
			}
		}
		void OnFilterChange( bool filterValid)
		{
			foreach( var element in m_ObjectTypes.elements)
			{
				element.Selected = m_SearchFilter.ContainsTypeValue( element.Label);
			}
		}
		void OnPopupSelect( PopupList.Element selectElement)
		{
			if( Event.current.control == false)
			{
				foreach( var element in m_ObjectTypes.elements)
				{
					if( element != selectElement)
					{
						element.Selected = false;
					}
				}
			}
			selectElement.Selected = !selectElement.Selected;
			
			IEnumerable<string> selectedDisplayNames = (from item in m_ObjectTypes.elements where item.Selected select item.Label);
			
			var builder = new StringBuilder();
			
			foreach( var typeName in selectedDisplayNames)
			{
				if( builder.Length > 0)
				{
					builder.Append( " ");
				}
				builder.Append( "t:");
				builder.Append( typeName);
			}
			m_SearchFilter.ToBuildStringPaths( builder);
			m_SearchFilter.ToBuildStringNames( builder);
			
		#if WITH_SEARCHSTRING
			string newSearchString = builder.ToString();
			if( m_SearchString != newSearchString)
			{
				m_SearchString = newSearchString;
				m_SearchFilter.Change( m_SearchString);
				m_TreeView.Reload();
			}
		#else
			m_TreeView.searchString = builder.ToString();
		#endif
		}
		public void OnBeforeSerialize()
		{
			m_SerializableElement.OnBeforeSerialize
			(
				new Element
				{
					ChildElements = m_Elements,
				}
			);
		}
		public void OnAfterDeserialize()
		{
			Element rootElement = m_SerializableElement.OnAfterDeserialize();
			m_Elements = rootElement.ChildElements;
		}
		[SerializeField]
        TreeViewState m_ViewState;
		[SerializeField]
		ViewMode m_ViewMode;
		[SerializeField]
		MultiColumnHeaderState m_HeaderState;
		[SerializeField]
        SerializableElementRoot m_SerializableElement;
		[SerializeField]
		string m_SearchString;
		
		[System.NonSerialized]
		SearchField m_SearchField;
		[System.NonSerialized]
		SearchFilter m_SearchFilter;
		[System.NonSerialized]
		List<Element> m_Elements;
		[System.NonSerialized]
		TreeView m_TreeView;
		[System.NonSerialized]
		PopupList.InputData m_ObjectTypes;
		
	#if WITH_VIEWTYPELIST
		[System.NonSerialized]
		string m_ListTexture;
		[System.NonSerialized]
		string m_TreeTexture;
	#endif
		[System.NonSerialized]
		string m_TypeTexture;
	}
}
