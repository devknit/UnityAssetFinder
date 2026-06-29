#define WITH_SERIALIZE_LOCALFILEIDENTIFIER

using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Knit.EditorWindow
{
#if UNITY_6000_4_OR_NEWER
	internal sealed class Element : TreeViewItem<int>
#else
	internal sealed class Element : TreeViewItem
#endif
	{
		internal static Element Create( ElementSource source)
		{
			if( source is ElementComponentSource component)
			{
                var element = new Element
                {
                    id = component.AssetPath.GetHashCode(),
                    name = component.Name,
                    Extension = string.Empty,
                    AssetPath = component.AssetPath,
                    AssetGuid = component.LocalId.ToString(),
					FilePath = component.AssetPath,
                    FindPath = component.FindPath,
					BundleName = string.Empty,
                    Directory = false,
                    Reference = component.Reference,
                    Missing = component.Missing,
                    AssetType = AssetType.Component,
                    LocalId = component.LocalId,
                };
                var content = EditorGUIUtility.ObjectContent( null, component.Type);
				element.icon = content.image as Texture2D;
				
				return element;
			}
			return Create( source.AssetPath, source.BundleName, source.Reference, source.Missing);
		}
		internal static Element Create( string path, string bundleName=null, int reference=-1, int missing=-1)
		{
			if( string.IsNullOrEmpty( path) == false && path.IndexOf( ":") < 0)
			{
				string assetGuid;
				bool directory;
				string assetPath;
				
				int bundlePrefixIndex = path.IndexOf( "<");
				int bundleSuffixIndex = path.IndexOf( ">/");
				
				if( bundlePrefixIndex == 0 && bundleSuffixIndex > 0)
				{
					assetPath = path.Substring( bundleSuffixIndex + 2, path.Length - (bundleSuffixIndex + 2));
				}
				else
				{
					assetPath = path;
				}
				path = path.Replace( "<", "");
				path = path.Replace( ">", "");
				
				switch( assetPath)
				{
					case "Library":
					case "Packages":
					case "ProjectSettings":
					{
						assetGuid = "00000000000000001000000000000000";
						directory = true;
						break;
					}
					default:
					{
						assetGuid = AssetDatabase.AssetPathToGUID( assetPath);
						
						if( string.IsNullOrEmpty( assetGuid) != false)
						{
							if( bundlePrefixIndex == 0 && bundleSuffixIndex < 0)
							{
								assetPath = string.Empty;
							}
							directory = bundlePrefixIndex == 0;
						}
						else
						{
							directory = AssetDatabase.IsValidFolder( assetPath);
						}
						break;
					}
				}
				if( string.IsNullOrEmpty( assetGuid) == false || directory != false)
				{
                    var element = new Element
                    {
                        id = path.GetHashCode(),
						AssetPath = assetPath,
                        AssetGuid = assetGuid,
                        icon = AssetDatabase.GetCachedIcon( AssetDatabase.GUIDToAssetPath( assetGuid)) as Texture2D,
                        Directory = directory,
						Missing = missing
                    };
                    if( element.Directory != false)
					{
						element.icon = AssetDatabase.GetCachedIcon( 
							AssetDatabase.GUIDToAssetPath( "00000000000000001000000000000000")) as Texture2D;
						element.name = System.IO.Path.GetFileName( path);
						element.Extension = string.Empty;
						element.AssetType = AssetType.Directory;
						element.Reference = -1;
					}
					else
					{
						element.icon = AssetDatabase.GetCachedIcon( AssetDatabase.GUIDToAssetPath( assetGuid)) as Texture2D;
						element.name = System.IO.Path.GetFileNameWithoutExtension( path);
						element.Extension = System.IO.Path.GetExtension( path);
						element.BundleName = bundleName ?? string.Empty;
						
						if( AssetTypes.kExtensions.TryGetValue( element.Extension, out AssetType assetType) != false)
						{
							element.AssetType = assetType;
						}
						else
						{
							element.AssetType = AssetType.Unknown;
						}
						element.Reference = reference;
					}
					return element;
				}
			}
			return null;
		}
		internal static void TreeViewSort( List<Element> elements)
		{
			if( elements.Count > 0)
			{
				elements.Sort( (src1, src2) =>
				{
					return string.Compare( 
						src1.OnCompareString(), 
						src2.OnCompareString());
				});
			}
		}
		internal static void ListViewSort( List<Element> elements)
		{
			if( elements.Count > 0)
			{
				elements.Sort( (src1, src2) =>
				{
					return src2.AssetPath.CompareTo( src1.AssetPath);
				});
			}
		}
		internal Element()
		{
			ChildElements = new List<Element>();
		#if UNITY_6000_4_OR_NEWER
			children = new List<TreeViewItem<int>>();
		#else
			children = new List<TreeViewItem>();
		#endif
		}
		internal Element( Element src)
		{
			id = src.id;
			depth = src.depth;
			name = src.name;
			Extension = src.Extension;
			AssetPath = src.AssetPath;
			AssetGuid = src.AssetGuid;
			AssetType = src.AssetType;
			icon = src.icon;
			FilePath = src.AssetPath;
			FindPath = src.FindPath;
			BundleName = src.BundleName;
			Directory = src.Directory;
			Reference = src.Reference;
			Missing = src.Missing;
			LocalId = src.LocalId;
			ParentElement = src.ParentElement;
			ChildElements = src.ChildElements;
			parent = src.parent;
			children = src.children;
		}
	#if UNITY_6000_4_OR_NEWER
		internal Element( SerializableElementNode node, List<Element> srcChildElements, List<TreeViewItem<int>> srcChildren)
	#else
		internal Element( SerializableElementNode node, List<Element> srcChildElements, List<TreeViewItem> srcChildren)
	#endif
		{
			id = node.id;
			depth = node.depth;
			name = node.name;
			Extension = node.Extension;
			AssetPath = node.AssetPath;
			AssetGuid = node.AssetGuid;
			AssetType = node.AssetType;
			icon = node.icon;
			FilePath = node.FilePath;
			FindPath = node.FindPath;
			BundleName = node.BundleName;
			Directory = node.Directory;
			Reference = node.Reference;
			Missing = node.Missing;
			LocalId = node.LocalId;
			ChildElements = srcChildElements;
			
			foreach( var child in ChildElements)
			{
				child.ParentElement = this;
			}
			children = srcChildren;
			
			foreach( var child in srcChildren)
			{
				child.parent = this;
			}
		}
		internal void Add( Element element)
		{
			if( element.ParentElement != null)
			{
				element.ParentElement.Remove( element);
			}
			children.Add( element);
			parent = this;
			ChildElements.Add( element);
			element.ParentElement = this;
			element.depth = depth + 1;
		}
		internal void Remove( Element element)
		{
			if( ChildElements.Contains( element) != false)
			{
				children.Remove( element);
				element.parent = null;
				ChildElements.Remove( element);
				element.ParentElement = null;
				element.depth = 0;
			}
		}
		internal bool IsFile()
		{
			return Directory == false && AssetType != AssetType.Component;
		}
		internal bool CheckFilter( SearchFilter filter)
		{
			bool bValid = false;
			
			ValidCount = 0;
			
			foreach( var child in ChildElements)
			{
				if( child.CheckFilter( filter) != false)
				{
					++ValidCount;
				}
			}
			if( ValidCount > 0 || (Directory == false && filter.Check( this) != false))
			{
				if( Directory == false)
				{
					++ValidCount;
				}
				bValid = true;
			}
			return bValid;
		}
		internal void SetDepth( int offset=0)
		{
			depth = Mathf.Max( 0, FilePath.Split( '/').Length - 1 - offset);
		}
		internal bool CanOpenAsset()
		{
			return true;
		}
		internal void OpenAsset()
		{
			if( LocalId != 0 && string.IsNullOrEmpty( FindPath) == false)
			{
				GameObject gameObject = FindGameObject( FindPath, LocalId);
				if( gameObject != null)
				{
					Selection.activeObject = gameObject;
				}
			}
			else
			{
				AssetDatabase.OpenAsset( AssetDatabase.LoadMainAssetAtPath( AssetPath));
			}
		}
		internal bool CanPingObject()
		{
			return true;
		}
		internal void PingObject( bool bDirectory)
		{
			if( bDirectory != false || Directory == false)
			{
				if( LocalId != 0 && string.IsNullOrEmpty( FindPath) == false)
				{
					GameObject gameObject = FindGameObject( FindPath, LocalId);
					if( gameObject != null)
					{
						EditorGUIUtility.PingObject( gameObject);
					}
				}
				else
				{
					EditorGUIUtility.PingObject( AssetDatabase.LoadMainAssetAtPath( AssetPath));
				}
			}
		}
		internal bool CanActiveObject()
		{
			return true;
		}
		internal void ActiveObject( bool bDirectory)
		{
			if( bDirectory != false || Directory == false)
			{
				if( LocalId != 0 && string.IsNullOrEmpty( FindPath) == false)
				{
					GameObject gameObject = FindGameObject( FindPath, LocalId);
					if( gameObject != null)
					{
						Selection.activeObject = gameObject;
					}
				}
				else
				{
					Selection.activeObject = AssetDatabase.LoadMainAssetAtPath( AssetPath);
				}
			}
		}
		internal string OnCompareString()
		{
			string compare = string.Empty;
			
			if( Directory != false)
			{
				compare = kComparePrefix;
			}
			compare += name;
			
			return compare;
		}
		static GameObject FindGameObject( string findPath, long localId)
		{
			var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
			if( prefabStage != null)
			{
				string[] directories = findPath.Split( '/');
				var selectObjects = new List<GameObject>();
				SelectGameObjects( selectObjects, prefabStage.prefabContentsRoot, directories, 0);
				if( selectObjects.Count > 0)
				{
					if( selectObjects.Count == 1)
					{
						return selectObjects[ 0];
					}
					else
					{
					#if UNITY_6000_4_OR_NEWER
						Selection.entityIds = selectObjects.Select( x => x.GetEntityId()).ToArray();
					#else
						Selection.instanceIDs = selectObjects.Select( x => x.GetInstanceID()).ToArray();
					#endif
					}
				}
			}
			else
			{
				var gameObjects = Resources.FindObjectsOfTypeAll( typeof( GameObject)) as GameObject[];
				var objects = gameObjects.Where( c => (c.hideFlags & kNotHierarchy) == 0);
				return objects.FirstOrDefault( x => GetLocalIdFromGameObject( x) == localId);
			}
			return null;
		}
		static void SelectGameObjects( List<GameObject> selectObjects, GameObject gameObject, string[] directories, int depth)
		{
			if( gameObject.name == directories[ depth])
			{
				if( depth == directories.Length - 1)
				{
					selectObjects.Add( gameObject);
				}
				else
				{
					Transform transform = gameObject.transform;
					foreach( Transform child in transform)
					{
						SelectGameObjects( selectObjects, child.gameObject, directories, depth + 1);
					}
				}
			}
		}
		static bool TryGetLocalFileIdentifier( Object targetObject, out long localId)
		{
			if( targetObject != null)
			{
			#if WITH_SERIALIZE_LOCALFILEIDENTIFIER
				if( s_CachedInspectorModeInfo == null)
				{
					s_CachedInspectorModeInfo = typeof( SerializedObject).GetProperty( 
						"inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
				}
				var serializedObject = new SerializedObject( targetObject);
				s_CachedInspectorModeInfo.SetValue( serializedObject, InspectorMode.Debug, null);
				SerializedProperty property = serializedObject.FindProperty( "m_LocalIdentfierInFile");
				
				if( property != null)
				{
					localId = property.longValue;
					return localId != 0;
				}
			#else
				return AssetDatabase.TryGetGUIDAndLocalFileIdentifier( targetObject, out string guid, out localId);
			#endif
			}
			localId = 0;
			return false;
		}
		static long GetLocalIdFromGameObject( GameObject instanceObject)
		{
			long ret = 0;
			
			if( instanceObject != null)
			{
				if( TryGetLocalFileIdentifier( instanceObject.transform, out long localId) != false)
				{
					ret = localId;
				}
				else
				{
					var serializedObject = new SerializedObject( instanceObject);
					SerializedProperty property = 
						serializedObject.FindProperty( "m_CorrespondingSourceObject");
					
					if( property != null && property.objectReferenceValue != null)
					{
						var gameObject = property.objectReferenceValue as GameObject;
						if( TryGetLocalFileIdentifier( gameObject.transform, out localId) != false)
						{
							ret = localId;
						}
					}
				}
			}
			return ret;
		}
		static string EnclosedString( string src, string begin, string end)
		{
			int beginIndex = src.LastIndexOf( begin);
			int endIndex = src.IndexOf( end);
			if( beginIndex >= 0 && endIndex >= 0 && beginIndex < endIndex)
			{
				return src.Substring( ++beginIndex, endIndex - beginIndex);
			}
			return string.Empty;
		}
	#if WITH_SERIALIZE_LOCALFILEIDENTIFIER
		static PropertyInfo s_CachedInspectorModeInfo = null;
	#endif
		const HideFlags kNotHierarchy = HideFlags.NotEditable | HideFlags.HideAndDontSave;
		static readonly string kComparePrefix = 
			System.Text.Encoding.ASCII.GetString( 
				Enumerable.Repeat( (byte)0x20, 260).ToArray());
		
		public override string displayName{ get{ return name; } set{} }
		internal string name{ get; private set; }
		internal string Extension{ get; private set; }
		internal string AssetPath{ get; private set; }
		internal string AssetGuid{ get; private set; }
		internal string FilePath{ get; private set; }
		internal string FindPath{ get; private set; }
		internal string BundleName{ get; private set; }
		internal AssetType AssetType{ get; private set; }
		internal bool Directory{ get; private set; }
		internal int Reference{ get; private set; }
		internal int Missing{ get; private set; }
		internal long LocalId{ get; private set; }
		internal Element ParentElement{ get; set; }
		internal List<Element> ChildElements{ get; set; }
		internal int ValidCount{ get; set; }
	}
	[System.Serializable]
	internal class SerializableElementRoot
	{
		internal SerializableElementRoot()
		{
			root = new List<SerializableElementNode>();
		}
		internal void OnBeforeSerialize( Element element)
		{
			root.Clear();
			Serialize( element);
		}
		internal Element OnAfterDeserialize()
		{
			if( root.Count > 0)
			{
				int count;
				return Deserialize( 0, out count);
			}
			return new Element();
		}
		void Serialize( Element element)
		{
			root.Add( new SerializableElementNode( element, root.Count + 1));
			
			if( (element.ChildElements?.Count ?? 0) > 0)
			{
				foreach( var child in element.ChildElements)
				{
					Serialize( child);
				}
			}
		}
		Element Deserialize( int index, out int count)
		{
			SerializableElementNode node = root[ index];
			
		#if UNITY_6000_4_OR_NEWER
			var children = new List<TreeViewItem<int>>();
		#else
			var children = new List<TreeViewItem>();
		#endif
			var ChildElements = new List<Element>();
			Element element;
			int childCount;
			int offset = 0;
			
			for( int i0 = 0; i0 < node.ChildCount; ++i0)
			{
				element = Deserialize( node.IndexOfFirstChild + offset + i0, out childCount);
				children.Add( element);
				ChildElements.Add( element);
				offset += childCount;
			}
			count = node.ChildCount + offset;
			
			return new Element( node, ChildElements, children);
		}
		[SerializeField]
		List<SerializableElementNode> root;
	}
	[System.Serializable]
	internal class SerializableElementNode
	{
		internal SerializableElementNode( Element element, int index)
		{
			id = element.id;
			depth = element.depth;
			name = element.name;
			Extension = element.Extension;
			AssetPath = element.AssetPath;
			AssetGuid = element.AssetGuid;
			AssetType = element.AssetType;
			icon = element.icon;
			FilePath = element.FilePath;
			FindPath = element.FindPath;
			BundleName = element.BundleName;
			Directory = element.Directory;
			Reference = element.Reference;
			Missing = element.Missing;
			LocalId = element.LocalId;
			ChildCount = element.ChildElements?.Count ?? 0;
			IndexOfFirstChild = index;
		}
		[SerializeField]
		internal int id;
		[SerializeField]
		internal int depth;
		[SerializeField]
		internal string name;
		[SerializeField]
		internal string Extension;
		[SerializeField]
		internal string AssetPath;
		[SerializeField]
		internal string AssetGuid;
		[SerializeField]
		internal string FilePath;
		[SerializeField]
		internal string FindPath;
		[SerializeField]
		internal string BundleName;
		[SerializeField]
		internal AssetType AssetType;
		[SerializeField]
		internal Texture2D icon;
		[SerializeField]
		internal bool Directory;
		[SerializeField]
		internal int Reference;
		[SerializeField]
		internal int Missing;
		[SerializeField]
		internal long LocalId;
		[SerializeField]
		internal int ChildCount;
		[SerializeField]
		internal int IndexOfFirstChild;
	}
}
