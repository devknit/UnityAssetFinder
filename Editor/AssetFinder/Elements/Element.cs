#define WITH_SERIALIZE_LOCALFILEIDENTIFIER

using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Knit.EditorWindow
{
	internal sealed class Element : TreeViewItem
	{
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
		internal static Element Create( ElementSource source)
		{
			if( source is ElementComponentSource component)
			{
                var element = new Element
                {
                    id = component.Path.GetHashCode(),
                    name = component.Name,
                    Extension = string.Empty,
                    Path = component.Path,
                    Guid = component.LocalId.ToString(),
                    Directory = false,
                    Reference = component.Reference,
                    Missing = component.Missing,
                    AssetType = AssetType.Component,
                    LocalId = component.LocalId,
                    FindPath = component.FindPath
                };
                var content = EditorGUIUtility.ObjectContent( null, component.Type);
				element.icon = content.image as Texture2D;
				
				return element;
			}
			return Create( source.Path, source.Reference, source.Missing);
		}
		internal static Element Create( string path, int reference=-1, int missing=-1)
		{
			if( string.IsNullOrEmpty( path) == false && path.IndexOf( ":") < 0)
			{
				bool directory;
				string guid;
				
				switch( path)
				{
					case "Library":
					case "Packages":
					case "ProjectSettings":
					{
						guid = path;
						directory = true;
						break;
					}
					default:
					{
						guid = AssetDatabase.AssetPathToGUID( path);
						directory = AssetDatabase.IsValidFolder( path);
						break;
					}
				}
				if( string.IsNullOrEmpty( guid) == false)
				{
                    var element = new Element
                    {
                        id = path.GetHashCode(),
                        Path = path,
                        Guid = guid,
                        icon = AssetDatabase.GetCachedIcon( path) as Texture2D,
                        Directory = directory,
						Missing = missing
                    };
                    if( element.Directory != false)
					{
						element.name = System.IO.Path.GetFileName( path);
						element.Extension = string.Empty;
						element.AssetType = AssetType.Directory;
						element.Reference = -1;
					}
					else
					{
						element.name = System.IO.Path.GetFileNameWithoutExtension( path);
						element.Extension = System.IO.Path.GetExtension( path);
						
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
					return string.Compare( src1.Path, src2.Path);
				});
			}
		}
		internal Element()
		{
			ChildElements = new List<Element>();
			children = new List<TreeViewItem>();
		}
		internal Element( Element src)
		{
			id = src.id;
			depth = src.depth;
			name = src.name;
			Extension = src.Extension;
			Path = src.Path;
			Guid = src.Guid;
			AssetType = src.AssetType;
			icon = src.icon;
			FindPath = src.FindPath;
			Directory = src.Directory;
			Reference = src.Reference;
			Missing = src.Missing;
			LocalId = src.LocalId;
			ParentElement = src.ParentElement;
			ChildElements = src.ChildElements;
			parent = src.parent;
			children = src.children;
		}
		internal Element( SerializableElementNode node, List<Element> srcChildElements, List<TreeViewItem> srcChildren)
		{
			id = node.id;
			depth = node.depth;
			name = node.name;
			Extension = node.Extension;
			Path = node.Path;
			Guid = node.Guid;
			AssetType = node.AssetType;
			icon = node.icon;
			FindPath = node.FindPath;
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
				AssetDatabase.OpenAsset( AssetDatabase.LoadMainAssetAtPath( Path));
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
					EditorGUIUtility.PingObject( AssetDatabase.LoadMainAssetAtPath( Path));
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
					Selection.activeObject = AssetDatabase.LoadMainAssetAtPath( Path);
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
						Selection.instanceIDs = selectObjects.Select( x => x.GetInstanceID()).ToArray();
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
		internal string Path{ get; private set; }
		internal string Guid{ get; private set; }
		internal string FindPath{ get; private set; }
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
		
			var children = new List<TreeViewItem>();
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
			Path = element.Path;
			Guid = element.Guid;
			AssetType = element.AssetType;
			icon = element.icon;
			FindPath = element.FindPath;
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
		internal string Path;
		[SerializeField]
		internal string Guid;
		[SerializeField]
		internal string FindPath;
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
