#define WITH_SEARCHSTRING

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;

namespace Knit.EditorWindow
{
	public enum ClickType
	{
		None,
		Ping,
		PingFileOnly,
		Active,
		ActiveFileOnly
	}
	public enum ViewMode
	{
		Tree,
		List
	}
	public sealed class TreeView : UnityEditor.IMGUI.Controls.TreeView
	{
		internal enum Column
		{
			None = 0x00,
			Name = 0x01,
			Extension = 0x02,
			AssetPath = 0x04,
			AssetGuid = 0x08,
			BundleName = 0x10,
			Missing = 0x20,
			Reference = 0x40,
			
			All = Name | Extension | AssetPath | AssetGuid | Missing | Reference
		}
		internal static MultiColumnHeaderState CreateHeaderState( Column useColumnMask=Column.All, Column defaultColumnMask=Column.None)
		{
			var columns = new List<MultiColumnHeaderState.Column>
			{
				new()
				{
					headerContent = new GUIContent("Name"),
					headerTextAlignment = TextAlignment.Center,
					canSort = false,
					width = 200,
					minWidth = 50,
					autoResize = true,
					allowToggleVisibility = false,
					userData = (int)Column.Name,
				},
				new()
				{
					headerContent = new GUIContent("Extension"),
					headerTextAlignment = TextAlignment.Center,
					canSort = false,
					width = 80,
					minWidth = 50,
					autoResize = false,
					allowToggleVisibility = true,
					userData = (int)Column.Extension,
				},
				new()
				{
					headerContent			= new GUIContent( "Path"),
					headerTextAlignment 	= TextAlignment.Center,
					canSort 				= false,
					width					= 250, 
					minWidth				= 50,
					autoResize				= true,
					allowToggleVisibility	= true,
					userData 				= (int)Column.AssetPath,
				},
				new()
				{
					headerContent			= new GUIContent( "Guid"),
					headerTextAlignment 	= TextAlignment.Center,
					canSort 				= false,
					width					= 240, 
					minWidth				= 50,
					autoResize				= false,
					allowToggleVisibility	= true,
					userData 				= (int)Column.AssetGuid,
				},
			};
			if( ((int)useColumnMask & (int)Column.BundleName) != 0)
			{
				columns.Add( new MultiColumnHeaderState.Column
				{
					headerContent			= new GUIContent( "Bundle"),
					headerTextAlignment 	= TextAlignment.Center,
					canSort 				= false,
					width					= 250, 
					minWidth				= 50,
					autoResize				= false,
					allowToggleVisibility	= true,
					userData 				= (int)Column.BundleName,
				});
			}
			if( ((int)useColumnMask & (int)Column.Missing) != 0)
			{
				columns.Add( new MultiColumnHeaderState.Column
				{
					headerContent			= new GUIContent( "Missing"),
					headerTextAlignment 	= TextAlignment.Center,
					canSort 				= false,
					width					= 80, 
					minWidth				= 50,
					autoResize				= false,
					allowToggleVisibility	= true,
					userData 				= (int)Column.Missing,
				});
			}
			if( ((int)useColumnMask & (int)Column.Reference) != 0)
			{
				columns.Add( new MultiColumnHeaderState.Column
				{
					headerContent			= new GUIContent( "Reference"),
					headerTextAlignment 	= TextAlignment.Center,
					canSort 				= false,
					width					= 80, 
					minWidth				= 50,
					autoResize				= false,
					allowToggleVisibility	= true,
					userData 				= (int)Column.Reference,
				});
			}
			var headerState = new MultiColumnHeaderState( columns.ToArray());
			
			if( defaultColumnMask != Column.None)
			{
				var visibleColumns = new List<int>();
				
				for( int i0 = 0; i0 < columns.Count; ++i0)
				{
					if( (columns[ i0].userData & (int)defaultColumnMask) != 0)
					{
						visibleColumns.Add( i0);
					}
				}
				headerState.visibleColumns = visibleColumns.ToArray();
			}
			return headerState;
		}
		internal TreeView( TreeViewState treeViewState, 
			MultiColumnHeader multiColumnHeader, 
			SearchFilter exploereSearchFilter) :
			base( treeViewState, multiColumnHeader) 
		{
			m_SearchFilter = (exploereSearchFilter != null)? 
				exploereSearchFilter : new SearchFilter();
			showAlternatingRowBackgrounds = true;
			getNewSelectionOverride = (clickedItem, keepMultiSelection, useShiftAsActionKey) => 
			{
				if( clickedItem is Element element)
				{
					/* フィルタリングされている場合のみ、親階層を展開する */
					if( m_SearchFilter.Valid != false)
					{
						Element extendElement = element.ParentElement;
						
						while( extendElement != null)
						{
							SetExpanded( extendElement.id, true);
							extendElement = extendElement.ParentElement;
						}
					}
					if( m_ClickType == ClickType.Ping
					||	m_ClickType == ClickType.PingFileOnly)
					{
						element.PingObject( m_ClickType == ClickType.Ping);
					}
				}
				var visibleRows = GetRows();
				var allIDs = visibleRows.Select( x => x.id).ToList();
				bool allowMultiselection = CanMultiSelect( clickedItem);
				
				return InternalEditorUtility.GetNewSelection(
					clickedItem.id, allIDs, state.selectedIDs, state.lastClickedID, 
					keepMultiSelection, useShiftAsActionKey, allowMultiselection);
			};
			multiColumnHeader.height = EditorGUIUtility.singleLineHeight + 4;
			multiColumnHeader.sortingChanged += OnSortingChanged;
		}
		internal void SetClickType( ClickType type)
		{
			m_ClickType = type;
		}
		internal void Apply( ViewMode viewMode)
		{
			Apply( m_Elements, viewMode);
		}
		internal void Apply( List<Element> elements, ViewMode viewMode)
		{
			switch( viewMode)
			{
				case ViewMode.Tree:
				{
					m_PreBuildRows = PreBuildRows;
					m_BuildRows = BuildTreeRows;
					m_PreBuildFilterRows = PreBuildTreeFilterRows;
					m_BuildFilterRows = BuildTreeFilterRows;
					break;
				}
				case ViewMode.List:
				{
					m_PreBuildRows = PreBuildRows;
					m_BuildRows = BuildListRows;
					m_PreBuildFilterRows = PreBuildRows;
					m_BuildFilterRows = BuildListFilterRows;
					break;
				}
				default:
				{
					m_PreBuildRows = null;
					m_BuildRows = null;
					m_PreBuildFilterRows = null;
					m_BuildFilterRows = null;
					break;
				}
			}
			if( m_BuildRows != null)
			{
				m_Elements = elements;
				Reload();
				// SetExpanded( "Assets".GetHashCode(), true);
			}
		}
		internal bool Contains( Vector2 mousePosition)
		{
			return treeViewRect.Contains( mousePosition);
		}
		internal void SetColumnHeaderEnable( Column column, bool bEnable)
		{
			var visibleColumns = multiColumnHeader.state.visibleColumns.ToList();
			
			for( int i0 = 0, mask = 1; mask < (int)Column.All; ++i0, mask <<= 1)
			{
				if( ((int)column & mask) != 0)
				{
					if( bEnable != false)
					{
						if( visibleColumns.Contains( i0) == false)
						{
							visibleColumns.Add( i0);
						}
					}
					else
					{
						if( visibleColumns.Contains( i0) != false)
						{
							visibleColumns.Remove( i0);
						}
					}
				}
			}
			multiColumnHeader.state.visibleColumns = visibleColumns.ToArray();
		}
		internal Rect GetViewRect()
		{
			return treeViewRect;
		}
		internal int GetSelectedCount()
		{
			return state.selectedIDs.Count;
		}
		internal IEnumerable<TreeViewItem> FindRowElements( List<int> ids)
		{
			return GetRows().Where( x => ids.Contains( x.id));
		}
		internal IEnumerable<TreeViewItem> FindRowElements( System.Func<Element, bool> onWhere)
		{
			return GetRows().Where( x => onWhere?.Invoke( x as Element) ?? false);
		}
		internal bool ContainsSeelctedElements<T>( T value, System.Func<Element, T> onComparer)
		{
			if( state.selectedIDs.Count > 0)
			{
				var items = FindRowElements( state.selectedIDs);
				
				foreach( var item in items)
				{
					if( value.Equals( onComparer( item as Element)) != false)
					{
						return true;
					}
				}
			}
			return false;
		}
		internal Element FirstSelectedElements( System.Func<Element, bool> onPredicate)
		{
			List<int> ids = state.selectedIDs;
			return GetRows().First( x => ids.Contains( x.id) && onPredicate( x as Element)) as Element;
		}
		internal IEnumerable<T> SelectSelectedElements<T>( System.Func<Element, T> onSelector)
		{
			if( state.selectedIDs.Count > 0)
			{
				return FindRowElements( state.selectedIDs).Select( x => onSelector( x as Element));
			}
			return null;
		}
		internal IEnumerable<T> SelectSelectedElements<T>( System.Func<Element, bool> onWhere, System.Func<Element, T> onSelector)
		{
			if( state.selectedIDs.Count > 0)
			{
				List<int> ids = state.selectedIDs;
				
				return FindRowElements( (element) =>
				{
					if( ids.Contains( element.id) != false)
					{
						return onWhere?.Invoke( element) ?? true;
					}
					return false;
					
				}).Select( x => onSelector( x as Element));
			}
			return null;
		}
		protected override void KeyEvent()
		{
			Event ev = Event.current;
			
			switch( ev.type)
			{
				case EventType.KeyDown:
				{
					switch( ev.keyCode)
					{
						case KeyCode.Return:
						case KeyCode.KeypadEnter:
						{
							if( GUIExpansion.HasKeyControl( ev.keyCode) == false)
							{
								string path = SelectSelectedElements( x => x.AssetPath)?.First();
								if( string.IsNullOrEmpty( path) == false)
								{
									AssetDatabase.OpenAsset( AssetDatabase.LoadMainAssetAtPath( path));
									GUIUtility.ExitGUI();
								}
								GUIExpansion.GrabKeyControl( ev.keyCode, treeViewControlID);
							}
							break;
						}
					}
					break;
				}
				case EventType.KeyUp:
				case EventType.Ignore:
				{
					GUIExpansion.ReleaseKeyControl( ev.keyCode);
					break;
				}
			}
		}
		protected override void SelectionChanged( IList<int> selectedIds)
		{
			if( m_ClickType >= ClickType.Active)
			{
				var newSelections = GetRows().Where( (x) =>
				{
					if( x is Element element)
					{
						if( selectedIds.Contains( element.id) != false)
						{
							if( m_ClickType == ClickType.ActiveFileOnly)
							{
								return element.IsFile();
							}
							return true;
						}
					}
					return false;
				}).Select( (x) =>
				{
					return AssetDatabase.LoadMainAssetAtPath( (x as Element).AssetPath);
				}).ToArray();
				
				if( newSelections.Length > 0)
				{
					Selection.objects = newSelections;
				}
			}
		}
		protected override void DoubleClickedItem( int id)
		{
			if( FindItem( id, rootItem) is Element element)
			{
				if( element.Directory != false)
				{
					SetExpanded( id, !IsExpanded( id));
				}
				else if( OnDoubleClickedItem != null)
				{
					OnDoubleClickedItem.Invoke( element);
				}
				else
				{
					element.OpenAsset();
					GUIUtility.ExitGUI();
				}
			}
		}
	#if !WITH_SEARCHSTRING
		protected override void SearchChanged( string newSearch)
		{
			searchFilter.Change( newSearch);
		}
	#endif
		protected override void RowGUI( RowGUIArgs args)
		{
			if( args.item is Element element)
			{
				int columnCount = args.GetNumVisibleColumns();
				Color? contentColor = null;
				
				if( element.Missing == -2)
				{
					if( Event.current.type == EventType.Repaint)
					{
						DefaultStyles.label.Draw( args.rowRect, 
							EditorGUIUtility.IconContent( "Warning"), isHover: false, isActive: false, args.selected, args.focused);
					}
					contentColor = GUI.contentColor;
					GUI.contentColor = Color.yellow;
				}
				for( int i0 = 0; i0 < columnCount; ++i0)
				{
					var cellRect = args.GetCellRect( i0);
					var columnIndex = args.GetColumn( i0);
					var column = multiColumnHeader.GetColumn( columnIndex);
					
					CenterRectUsingSingleLineHeight( ref cellRect);
					
					switch( (Column)column.userData)
					{
						case Column.Name:
						{
							base.RowGUI( args);
							break;
						}
						case Column.Extension:
						{
							DefaultGUI.Label( cellRect, element.Extension, args.selected, args.focused);
							break;
						}
						case Column.AssetPath:
						{
							DefaultGUI.Label( cellRect, element.AssetPath, args.selected, args.focused);
							break;
						}
						case Column.AssetGuid:
						{
							DefaultGUI.Label( cellRect, element.AssetGuid, args.selected, args.focused);
							break;
						}
						case Column.BundleName:
						{
							DefaultGUI.Label( cellRect, element.BundleName, args.selected, args.focused);
							break;
						}
						case Column.Missing:
						{
							if( element.Missing >= 0)
							{
								DefaultGUI.LabelRightAligned( cellRect, element.Missing.ToString(), args.selected, args.focused);
							}
							break;
						}
						case Column.Reference:
						{
							if( element.Reference >= 0)
							{
								DefaultGUI.LabelRightAligned( cellRect, element.Reference.ToString(), args.selected, args.focused);
							}
							break;
						}
						default:
						{
							base.RowGUI( args);
							break;
						}
					}
				}
				if( contentColor.HasValue != false)
				{
					GUI.contentColor = contentColor.Value;
				}
			}
		}
		protected override TreeViewItem BuildRoot()
		{
			return new TreeViewItem{ id = 0, depth = -1, displayName = string.Empty };
		}
		protected override IList<TreeViewItem> BuildRows( TreeViewItem root)
		{
			var rows = GetRows() ?? new List<TreeViewItem>();
			rows.Clear();
			
			if( m_Elements != null)
			{
				if( m_SearchFilter.Valid == false)
				{
					m_PreBuildRows?.Invoke( root, m_Elements);
					m_BuildRows?.Invoke( m_Elements, rows);
				}
				else
				{
					
					m_PreBuildFilterRows?.Invoke( root, m_Elements);
					m_BuildFilterRows?.Invoke( m_Elements, rows);
				}
			}
			return rows;
		}
		void PreBuildRows( TreeViewItem root, List<Element> elements)
		{
			foreach( var element in elements)
			{
				root.AddChild( element);
			}
		}
		void BuildTreeRows( List<Element> elements, IList<TreeViewItem> rows)
		{
			foreach( var element in elements)
			{
				var item = new Element( element);
				
				rows.Add( item);
				
				if( element.ChildElements.Count > 0)
				{
					if( IsExpanded( item.id) == false)
					{
						item.children = CreateChildListForCollapsedParent();
					}
					else
					{
						BuildTreeRows( element.ChildElements, rows);
					}
				}
			}
		}
		void BuildListRows( List<Element> elements, IList<TreeViewItem> rows)
		{
			foreach( var element in elements)
			{
				Element item = null;
				
				if( element.Directory != false)
				{
					if( element.depth == 0)
					{
						item = new Element( element);
						rows.Add( item);
					}
				}
				else
				{
					item = new Element( element)
					{
						depth = 1
					};
					rows.Add( item);
				}
				if( element.ChildElements.Count > 0)
				{
					if( item != null && IsExpanded( element.id) == false)
					{
						item.children = CreateChildListForCollapsedParent();
					}
					else
					{
						BuildListRows( element.ChildElements, rows);
					}
				}
			}
		}
		void PreBuildTreeFilterRows( TreeViewItem root, List<Element> elements)
		{
			foreach( var element in elements)
			{
				element.CheckFilter( m_SearchFilter);
				root.AddChild( element);
			}
		}
		void BuildTreeFilterRows( List<Element> elements, IList<TreeViewItem> rows)
		{
			foreach( var element in elements)
			{
				if( element.ValidCount > 0)
				{
					rows.Add( new Element( element));
					
					if( element.ChildElements.Count > 0)
					{
						BuildTreeFilterRows( element.ChildElements, rows);
					}
				}
			}
		}
		void BuildListFilterRows( List<Element> elements, IList<TreeViewItem> rows)
		{
			foreach( var element in elements)
			{
				Element item = null;
				
				if( element.Directory != false)
				{
					if( element.depth == 0)
					{
						item = new Element( element);
						rows.Add( item);
					}
				}
				else
				{
					if( m_SearchFilter.Check( element) != false)
					{
						item = new Element( element)
						{
							depth = 1
						};
						rows.Add( item);
					}
				}
				if( element.ChildElements.Count > 0)
				{
					if( item != null && IsExpanded( element.id) == false)
					{
						item.children = CreateChildListForCollapsedParent();
					}
					else
					{
						BuildListFilterRows( element.ChildElements, rows);
					}
				}
			}
		}
		void OnSortingChanged( MultiColumnHeader multiColumnHeader)
		{
			if( multiColumnHeader.sortedColumnIndex >= 0)
			{
			}
		}
		internal Action<Element> OnDoubleClickedItem
		{
			get;
			set;
		}
		Action<TreeViewItem, List<Element>> m_PreBuildRows;
		Action<List<Element>, IList<TreeViewItem>> m_BuildRows;
		Action<TreeViewItem, List<Element>> m_PreBuildFilterRows;
		Action<List<Element>, IList<TreeViewItem>> m_BuildFilterRows;
		readonly SearchFilter m_SearchFilter;
		List<Element> m_Elements;
		ClickType m_ClickType;
	}
}
