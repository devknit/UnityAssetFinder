
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Animations;

namespace Knit.EditorWindow.AssetFinder
{
	internal sealed class Finder
	{
		internal enum Mode
		{
			None,
			ToDependencies,
			FromDependencies,
		}
		internal static void Execute( Mode mode,
			IEnumerable<string> targetGuids, bool recursive,
			out Dictionary<string, ElementSource> targets,
			out Dictionary<string, ElementSource> results)
		{
			switch( mode)
			{
				case Mode.ToDependencies:
				{
					ToDependencies( targetGuids, recursive, out targets, out results);
					break;
				}
				case Mode.FromDependencies:
				{
					FromDependencies( targetGuids, recursive, out targets, out results);
					break;
				}
				default:
				{
					targets = new Dictionary<string, ElementSource>();
					results = new Dictionary<string, ElementSource>();
					break;
				}
			}
		}
		static void ToDependencies( 
			IEnumerable<string> targetGuids, bool recursive,
			out Dictionary<string, ElementSource> targets,
			out Dictionary<string, ElementSource> results)
		{
			int targetGuidCount = targetGuids.Count();
			float unitProgress = 1.0f / targetGuidCount;
			int i0 = 0, i1;
			
			results = new Dictionary<string, ElementSource>();
			targets = new Dictionary<string, ElementSource>();
			
			if( OnProgress( "Find To Dependencies", 0) != false)
			{
				OnFinish();
				return;
			}
			foreach( string targetGuid in targetGuids)
			{
				string targetPath = AssetDatabase.GUIDToAssetPath( targetGuid);
				
				if( string.IsNullOrEmpty( targetPath) == false
				&&	AssetDatabase.IsValidFolder( targetPath) == false)
				{
					if( targets.ContainsKey( targetPath) == false)
					{
						string[] assetPaths = AssetDatabase.GetDependencies( targetPath, recursive);
						targets.Add( targetPath, new ElementSource( targetPath, string.Empty, assetPaths.Length, -1));
						
						for( i1 = 0; i1 < assetPaths.Length; ++i1)
						{
							float progress = i1 / (float)assetPaths.Length;
							progress /= targetGuidCount;
							progress += i0 * unitProgress;
							
							string assetPath = assetPaths[ i1];
							
							if( OnProgress( "Find To Dependencies", assetPath, progress) != false)
							{
								OnFinish();
								return;
							}
							if( results.TryGetValue( assetPath, out ElementSource elementSource) == false)
							{
								string assetGuid = AssetDatabase.AssetPathToGUID( assetPath);
								
								elementSource = new ElementSource( assetPath, string.Empty, 0, -1);
								results.Add( assetPath, elementSource);
								
								if( CheckMissing( assetPath, results, progress) == false)
								{
									OnFinish();
									return;
								}
								if( elementSource != null)
								{
									++elementSource.Reference;
								}
							}
						}
						if( CheckMissing( targetPath, targets, i0 * unitProgress) == false)
						{
							OnFinish();
							return;
						}
					}
				}
				++i0;
			}
			OnProgress( "Find To Dependencies", 1);
			OnFinish();
		}
		static void FromDependencies( 
			IEnumerable<string> targetGuids, bool recursive,
			out Dictionary<string, ElementSource> targets,
			out Dictionary<string, ElementSource> results)
		{
			results = new Dictionary<string, ElementSource>();
			targets = new Dictionary<string, ElementSource>();
			
			if( OnProgress( "Find From Dependencies", 0) != false)
			{
				OnFinish();
				return;
			}
			var targetAssets = new Dictionary<string, string>();
			
			foreach( string targetGuid in targetGuids)
			{
				string targetPath = AssetDatabase.GUIDToAssetPath( targetGuid);
				
				if( string.IsNullOrEmpty( targetPath) == false
				&&	AssetDatabase.IsValidFolder( targetPath) == false)
				{
					targetAssets.Add( targetPath, targetGuid);
					targets.Add( targetPath, new ElementSource( targetPath, string.Empty, 0, -1));
					
					if( CheckMissing( targetPath, targets, 0) == false)
					{
						OnFinish();
						return;
					}
				}
			}
			if( targetAssets.Count > 0)
			{
				string[] fromGuids = AssetDatabase.FindAssets( 
					"t:Scene t:Prefab t:Material t:AnimatorController t:ScriptableObject");
				
				for( int i0 = 0; i0 < fromGuids.Length; ++i0)
				{
					string fromGuid = fromGuids[ i0];
					string fromPath = AssetDatabase.GUIDToAssetPath( fromGuid);
					
					if( i0 % 3 == 2)
					{
						if( OnProgress( "Find From Dependencies", fromPath, i0 / (float)fromGuids.Length) != false)
						{
							OnFinish();
							return;
						}
					}
					if( results.ContainsKey( fromPath) == false)
					{
						string[] assetPaths = AssetDatabase.GetDependencies( fromPath, recursive);
						
						for( int i1 = 0; i1 < assetPaths.Length; ++i1)
						{
							string assetPath = assetPaths[ i1];
							
							if( targetAssets.TryGetValue( assetPath, out string targetGuid) != false)
							{
								if( targetGuid != fromGuid)
								{
									if( results.TryGetValue( fromPath, out ElementSource elementSource) == false)
									{
										elementSource = new ElementSource( fromPath, string.Empty, 0, -1);
										results.Add( fromPath, elementSource);
									}
									if( elementSource != null)
									{
										++elementSource.Reference;
									}
									if( targets.TryGetValue( assetPath, out var target) != false)
									{
										++target.Reference;
									}
								}
							}
						}
					}
				}
			}
			OnProgress( "Find From Dependencies", 1);
			OnFinish();
		}
		static bool CheckMissing( string targetPath, Dictionary<string, ElementSource> elements, float progress)
		{
			if( elements.TryGetValue( targetPath, out ElementSource target) == false)
			{
				return true;
			}
			if( string.IsNullOrEmpty( targetPath) == false && AssetDatabase.IsValidFolder( targetPath) == false)
			{
				System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath( targetPath);
				
				if( typeof( Material).Equals( assetType) != false
				||	typeof( GameObject).Equals( assetType) != false
				||	typeof( AnimationClip).Equals( assetType) != false
				||	typeof( AnimatorController).Equals( assetType) != false
				||	typeof( LightingDataAsset).Equals( assetType) != false
				||	typeof( ScriptableObject).IsAssignableFrom( assetType) != false)
				{
					Object[] assets = AssetDatabase.LoadAllAssetsAtPath( targetPath);
					string targetName = Path.GetFileName( targetPath);
					
					for( int i0 = 0; i0 < assets.Length; ++i0)
					{
						if( OnProgress( "Check Missing", targetPath, progress) != false)
						{
							OnFinish();
							return false;
						}
						Object asset = assets[ i0];
						
						if( asset == null)
						{
							continue;
						}
						if( asset.name == "Deprecated EditorExtensionImpl")
						{
							continue;
						}
						var serializedObject = new SerializedObject( asset);
						SerializedProperty property = serializedObject.GetIterator();
						
						var displayDirectory = new Stack<string>();
						string currentDisplayName = string.Empty;
						bool nextEnterChildren = true;
						
						while( property.Next( nextEnterChildren) != false)
						{
							if( OnProgress( "Check Missing", $"{targetName}@{property.propertyPath}", progress) != false)
							{
								OnFinish();
								return false;
							}
							nextEnterChildren = property.propertyType switch
							{
								SerializedPropertyType.Generic => (property.isArray == false) || property.arrayElementType switch
								{
									"byte" => false,
									"char" => false,
									"int" => false,
									"uint" => false,
									"float" => false,
									"float3" => false,
									"string" => false,
									"xform" => false,
									"Axes" => false,
									"Matrix4x4f" => false,
									"MinMaxAABB" => false,
									"VertexAttribute" => false,
									"SubMesh" => false,
									"SkeletonBone" => false,
									"HumanBone" => false,
									"ChannelInfo" => false,
									"BlendShapeVertex" => false,
									"MeshBlendShape" => false,
									"MeshBlendShapeChannel" => false,
									_ => true
								},
								SerializedPropertyType.Integer => false,
								SerializedPropertyType.Boolean => false,
								SerializedPropertyType.Float => false,
								SerializedPropertyType.String => false,
								SerializedPropertyType.Color => false,
								SerializedPropertyType.ObjectReference => true,
								SerializedPropertyType.LayerMask => false,
								SerializedPropertyType.Enum => false,
								SerializedPropertyType.Vector2 => false,
								SerializedPropertyType.Vector3 => false,
								SerializedPropertyType.Vector4 => false,
								SerializedPropertyType.Rect => false,
								SerializedPropertyType.ArraySize => true,
								SerializedPropertyType.Character => true,
								SerializedPropertyType.AnimationCurve => false,
								SerializedPropertyType.Bounds => false,
								SerializedPropertyType.Gradient => true,
								SerializedPropertyType.Quaternion => false,
								SerializedPropertyType.ExposedReference => true,
								SerializedPropertyType.FixedBufferSize => true,
								SerializedPropertyType.Vector2Int => false,
								SerializedPropertyType.Vector3Int => false,
								SerializedPropertyType.RectInt => false,
								SerializedPropertyType.BoundsInt => false,
								SerializedPropertyType.ManagedReference => true,
								SerializedPropertyType.Hash128 => false,
								_ => true
							};
							try
							{
								if( property.propertyPath.EndsWith( ".Array") == false
								&&	property.propertyPath.EndsWith( ".Array.size") == false)
								{
									if( displayDirectory.Count < property.depth)
									{
										displayDirectory.Push( currentDisplayName);
										currentDisplayName = property.displayName;
									}
									else if( displayDirectory.Count > property.depth)
									{
										while( displayDirectory.Count > property.depth)
										{
											displayDirectory.Pop();
											currentDisplayName = property.displayName;
										}
									}
									else
									{
										currentDisplayName = property.displayName;
									}
								}
							}
							catch( System.Exception e)
							{
								Debug.LogError( e);
							}
							if( property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
							{
								bool bMissing = false;
								
							#if UNITY_6000_4_OR_NEWER
								if( property.objectReferenceEntityIdValue.IsValid() != false)
								{
									bMissing = true;
								}
							#else
								if( property.objectReferenceInstanceIDValue != 0)
								{
									bMissing = true;
								}
							#endif
								else
								{
									if( property.hasChildren != false)
									{
										SerializedProperty fileIdProperty = property.FindPropertyRelative( "m_FileID");
										
										if( fileIdProperty != null && fileIdProperty.intValue != 0)
										{
											bMissing = true;
										}
									}
								}
								if( bMissing != false)
								{
									string hierarchyPath = string.Empty;
									long assetLocalId = 0;
								#if UNITY_6000_4_OR_NEWER
									EntityId intstanceId = default;
								#else
									int intstanceId = 0;
								#endif
									int tryCount = 0;
									
									Transform transform = asset switch
									{
										Component component => component.transform,
										GameObject gameObject => gameObject.transform,
										_ => null
									};
									if( transform != null)
									{
										AssetDatabase.TryGetGUIDAndLocalFileIdentifier( 
											transform, out string assetGuid, out assetLocalId);
									#if UNITY_6000_4_OR_NEWER
										intstanceId = transform.GetEntityId();
									#else
										intstanceId = transform.GetInstanceID();
									#endif
										
										while( transform != null)
										{
											hierarchyPath = (hierarchyPath.Length == 0)? 
												transform.name : (transform.name + "/" + hierarchyPath);
											transform = transform.parent;
										}
									}
									string componentPath = string.Format( 
										$"{targetPath}/{hierarchyPath}<{asset.GetType()}@{property.propertyPath.Replace( "/" , "#")}>");
									do
									{
										string foundKeyPath = string.Format( $"{componentPath}#{assetLocalId}-{tryCount}");
											
										if( elements.ContainsKey( foundKeyPath) == false)
										{
											string displayPath = string.Join( '/', displayDirectory.Reverse());
											displayPath = Path.Combine( displayPath, currentDisplayName).Replace( @"\", "/");
											
											string displayName = string.Format( $"{hierarchyPath}<{asset.GetType().Name}@{displayPath}>");
											
											elements.Add( foundKeyPath, new ElementComponentSource( displayName, 
												asset.GetType(), hierarchyPath, assetLocalId, foundKeyPath, -1, -2, string.Empty));
											break;
										}
										++tryCount;
									}
									while( true);
									
									int missingCount = target.Missing;
									
									if( missingCount < 0)
									{
										missingCount = 0;
									}
									target.Missing = ++missingCount;
								}
							}
						}
					}
				}
			}
			return true;
		}
		static bool OnProgress( string caption, float progress)
		{
			return EditorUtility.DisplayCancelableProgressBar( caption, string.Empty, progress);
		}
		static bool OnProgress( string caption, string message, float progress)
		{
			return EditorUtility.DisplayCancelableProgressBar( caption, message, progress);
		}
		static void OnFinish()
		{
			EditorUtility.ClearProgressBar();
		}
	#if false
		[MenuItem("Tools/Log Serialized Assets")]
		private static void LogSerializedAssets2()
		{
			var builder = new System.Text.StringBuilder();
			try
			{
				LogSerializedObject( Selection.activeObject, builder, "  ");
				
				if( Selection.activeObject is GameObject gameObject)
				{
					LogSerializedObject( gameObject.transform, builder, "  ");
				}
			}
			catch( System.Exception e)
			{
				Debug.LogError( e);
			}
			Debug.Log( builder.ToString());
		}
		[MenuItem("Assets/Log Serialized Assets")]
		private static void LogSerializedAssets()
		{
			var builder = new System.Text.StringBuilder();
			try
			{
				builder.AppendLine( "Show Serialized");
				
				var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
				var assets = AssetDatabase.LoadAllAssetsAtPath( assetPath);
				
				foreach( var asset in assets)
				{
					LogSerializedObject( asset, builder, "  ");
				}
			}
			catch( System.Exception e)
			{
				Debug.LogError( e);
			}
			Debug.Log( builder.ToString());
		}
		static void LogSerializedObject( Object asset, System.Text.StringBuilder builder, string indent)
		{
			if( asset != null)
			{
				string guid = string.Empty;
				long localId = 0, localIdInFile = 0;
				
				SerializedObject serializedObject = new SerializedObject( asset);
				PropertyInfo cachedInspectorModeInfo = typeof( SerializedObject).GetProperty( "inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
				cachedInspectorModeInfo.SetValue( serializedObject, InspectorMode.Debug, null);
				SerializedProperty serializedProperty = serializedObject.FindProperty( "m_LocalIdentfierInFile");
				if( serializedProperty != null)
				{
					localIdInFile = serializedProperty.longValue;
				}
				SerializedProperty property = serializedObject.GetIterator();
				
				if( AssetDatabase.TryGetGUIDAndLocalFileIdentifier( asset, out guid, out localId) == false)
				{
					builder.AppendFormat( $"{asset.name}<{asset.GetType()}> localIdInFile={localIdInFile}\n");
				}
				else
				{
					builder.AppendFormat( $"{asset.name}<{asset.GetType()}> guid={guid}, localId={localId}, localIdInFile={localIdInFile}\n");
				}
				while( property.Next( true) != false)
				{
					LogSerializedProperty( property, builder, indent);
				}
				builder.AppendLine( string.Empty);
			}
		}
		static void LogSerializedProperty( SerializedProperty property, System.Text.StringBuilder builder, string indent)
		{
			builder.AppendFormat( $"{indent}{property.propertyType} {property.name} = ");
			
			switch( property.propertyType)
			{
				case SerializedPropertyType.Generic:
				{
					builder.AppendLine( string.Empty);
					
					if( property.isArray == false)
					{
						var child = property.Copy();
						var end = property.GetEndProperty( true);
						if( child.Next( true) != false)
						{
							while( SerializedProperty.EqualContents( child, end) == false)
							{
								LogSerializedProperty( child, builder, indent + "  ");
								if( child.Next( true) == false)
								{
									break;
								}
							}
						}
					}
					else
					{
						for( int i0 = 0; i0 < property.arraySize; ++i0)
						{
							LogSerializedProperty( property.GetArrayElementAtIndex( i0), builder, indent + "    ");
						}
					}
					break;
				}
				case SerializedPropertyType.ObjectReference:
				{
					if( property.objectReferenceValue != null)
					{
						Object asset = property.objectReferenceValue;
						string guid = string.Empty;
						long localId = 0, localIdInFile = 0;
						
						SerializedObject serializedObject = new SerializedObject( property.objectReferenceValue);
						PropertyInfo cachedInspectorModeInfo = typeof( SerializedObject).GetProperty( "inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
						cachedInspectorModeInfo.SetValue( serializedObject, InspectorMode.Debug, null);
						SerializedProperty serializedProperty = serializedObject.FindProperty( "m_LocalIdentfierInFile");
						if( serializedProperty != null)
						{
							localIdInFile = serializedProperty.longValue;
						}
						if( AssetDatabase.TryGetGUIDAndLocalFileIdentifier( asset, out guid, out localId) == false)
						{
							builder.AppendFormat( $"{asset.name}<{asset.GetType()}> localIdInFile={localIdInFile}\n");
						}
						else
						{
							builder.AppendFormat( $"{asset.name}<{asset.GetType()}> guid={guid}, localId={localId}, localIdInFile={localIdInFile}\n");
						}
					}
					else
					{
						builder.AppendLine();
					}
					var child = property.Copy();
					var end = property.GetEndProperty( true);
					if( child.Next( true) != false)
					{
						while( SerializedProperty.EqualContents( child, end) == false)
						{
							LogSerializedProperty( child, builder, indent + "  ");
							if( child.Next( true) == false)
							{
								break;
							}
						}
					}
					break;
				}
				case SerializedPropertyType.Integer:
				case SerializedPropertyType.LayerMask:
				case SerializedPropertyType.ArraySize:
				{
					builder.AppendFormat( $"{property.intValue}\n");
					break;
				}
				case SerializedPropertyType.Boolean:
				{
					builder.AppendFormat( $"{property.boolValue}\n");
					break;
				}
				case SerializedPropertyType.Float:
				{
					builder.AppendFormat( $"{property.floatValue}\n");
					break;
				}
				case SerializedPropertyType.String:
				{
					builder.AppendFormat( $"{property.stringValue}\n");
					break;
				}
				case SerializedPropertyType.Color:
				{
					builder.AppendFormat( $"{property.colorValue}\n");
					break;
				}
				case SerializedPropertyType.Enum:
				{
					string enumValueName = string.Empty;
					
					if( property.enumNames != null)
					{
						if( property.enumValueIndex >= 0 && property.enumValueIndex < property.enumNames.Length)
						{
							enumValueName = property.enumNames[ property.enumValueIndex];
						}
					}
					builder.AppendFormat( $"{enumValueName}\n");
					break;
				}
				case SerializedPropertyType.Vector2:
				{
					builder.AppendFormat( $"{property.vector2Value}\n");
					break;
				}
				case SerializedPropertyType.Vector3:
				{
					builder.AppendFormat( $"{property.vector3Value}\n");
					break;
				}
				case SerializedPropertyType.Vector4:
				{
					builder.AppendFormat( $"{property.vector4Value}\n");
					break;
				}
				case SerializedPropertyType.Rect:
				{
					builder.AppendFormat( $"{property.rectValue}\n");
					break;
				}
				case SerializedPropertyType.Character:
				case SerializedPropertyType.AnimationCurve:
				case SerializedPropertyType.Gradient:
				{
					builder.AppendFormat( $"<Not compatible>\n");
					break;
				}
				case SerializedPropertyType.Bounds:
				{
					builder.AppendFormat( $"{property.boundsValue}\n");
					break;
				}
				case SerializedPropertyType.Quaternion:
				{
					builder.AppendFormat( $"{property.quaternionValue}\n");
					break;
				}
				default:
				{
					builder.AppendFormat( $"<Unexpected>\n");
					break;
				}
			}
		}
	#endif
	}
}
