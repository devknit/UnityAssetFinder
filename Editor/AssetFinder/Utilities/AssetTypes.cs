
using System.Collections.Generic;

namespace Knit.EditorWindow
{
	public enum AssetType
	{
		Unknown,
		Directory,
		Component,
		Bundle,
		
		AnimationController,
		AnimationClip,
		AudioClip,
		AudioMixer,
		ComputeShader,
	//	Cubemap,
		Font,
		GUISkin,
		Material,
		Model,
		PhysicMaterial,
		Prefab,
		Scene,
		Script,
		ScriptableObject,
		Shader,
		TextAsset,
		Texture,
		VideoClip,
	}
	/* AssetDatabase.GetMainAssetTypeAtPath() 
	 * を使った型判断だと読み込みが発生してしまうため
	 * 暫定として拡張子から型を判断する
	 */
	public sealed class AssetTypes
	{
		public static readonly string[] kTypeNames = new string[]
		{
			"AnimationController",
			"AnimationClip",
			"AudioClip",
			"AudioMixer",
			"ComputeShader",
			"Font",
			"GUISkin",
			"Material",
			"Model",
			"PhysicMaterial",
			"Prefab",
			"Scene",
			"Script",
			"ScriptableObject",
			"Shader",
			"TextAsset",
			"Texture",
			"VideoClip",
		};
		internal static readonly Dictionary<string, AssetType> kFilters = 
			new( System.StringComparer.OrdinalIgnoreCase)
		{
			{ "t:Bundle", AssetType.Bundle },
			
			{ "t:AnimationController", AssetType.AnimationController },
			{ "t:Animator", AssetType.AnimationController },
			
			{ "t:AnimationClip", AssetType.AnimationClip },
			{ "t:Animation", AssetType.AnimationClip },
			
			{ "t:AudioClip", AssetType.AudioClip },
			{ "t:Audio", AssetType.AudioClip },
			
			{ "t:AudioMixer", AssetType.AudioMixer },
			{ "t:Mixer", AssetType.AudioMixer },
			
			{ "t:ComputeShader", AssetType.ComputeShader },
			
			{ "t:Font", AssetType.Font },
			
			{ "t:GUISkin", AssetType.GUISkin },
			
			{ "t:Material", AssetType.Material },
			
			{ "t:Model", AssetType.Model },
		//	{ "t:Mesh", AssetType.Model },
			
			{ "t:PhysicMaterial", AssetType.PhysicMaterial },
			
			{ "t:Prefab", AssetType.Prefab },
			
			{ "t:Scene", AssetType.Scene },
			
			{ "t:Script", AssetType.Script },
			
			{ "t:ScriptableObject", AssetType.ScriptableObject },
			{ "t:Asset", AssetType.ScriptableObject },
			
			{ "t:Shader", AssetType.Shader },
			
			{ "t:TextAsset", AssetType.TextAsset },
			{ "t:Text", AssetType.TextAsset },
			
			{ "t:Texture", AssetType.Texture },
			
			{ "t:VideoClip", AssetType.VideoClip },
			{ "t:Video", AssetType.VideoClip },
		};
		public static readonly Dictionary<string, AssetType> kExtensions =
			new Dictionary<string, AssetType>( System.StringComparer.OrdinalIgnoreCase)
		{
			/* Bundle */
			{ ".bundle", AssetType.Bundle },
			
			/* AnimationController */
			{ ".controller", AssetType.AnimationController },
			
			/* AnimationClip */
			{ ".anim", AssetType.AnimationClip },
			
			/* AudioClip */
			{ ".wav", AssetType.AudioClip },
			{ ".mp3", AssetType.AudioClip },
			{ ".ogg", AssetType.AudioClip },
			{ ".aif", AssetType.AudioClip },
			{ ".aiff", AssetType.AudioClip },
			{ ".xm", AssetType.AudioClip },
			{ ".mod", AssetType.AudioClip },
			{ ".it", AssetType.AudioClip },
			{ ".s3m", AssetType.AudioClip },
			
			/* AudioMixer */
			{ ".mixer", AssetType.AudioMixer },
			
			/* ComputeShader */
			{ ".compute", AssetType.ComputeShader },
			
			/* Cubemap */
	//		{ ".hdr", AssetType.Cubemap },
	//		{ ".cubemap", AssetType.Cubemap },
			
			/* Font */
			{ ".ttf", AssetType.Font },
			{ ".otf", AssetType.Font },
			{ ".dfont", AssetType.Font },
			
			/* GUISkin */
			{ ".guiskin", AssetType.GUISkin },
			
			/* Material */
			{ ".mat", AssetType.Material },
			{ ".material", AssetType.Material },
			
			/* Model */
			{ ".3ds", AssetType.Model },
			{ ".blend", AssetType.Model },
			{ ".blender", AssetType.Model },
			{ ".c3d", AssetType.Model },
			{ ".c4d", AssetType.Model },
			{ ".dae", AssetType.Model },
			{ ".dfx", AssetType.Model },
			{ ".fbx", AssetType.Model },
			{ ".obj", AssetType.Model },
			{ ".ma", AssetType.Model },
			{ ".mb", AssetType.Model },
			{ ".max", AssetType.Model },
			{ ".lxo", AssetType.Model },
			{ ".lwo", AssetType.Model },
			{ ".jas", AssetType.Model },
			{ ".skp", AssetType.Model },
			
			/* PhysicMaterial */
			{ ".physicMaterial", AssetType.PhysicMaterial },
			{ ".physicsMaterial2D", AssetType.PhysicMaterial },
			
			/* Prefab */
			{ ".prefab", AssetType.Prefab },
			
			/* Scene */
			{ ".unity", AssetType.Scene },
			
			/* Script */
			{ ".cs", AssetType.Script },
			{ ".js", AssetType.Script },
			
			/* ScriptableObject */
			{ ".asset", AssetType.ScriptableObject },
			
			/* Shader */
			{ ".shader", AssetType.Shader },
			
			/* TextAsset */
			{ ".txt", AssetType.TextAsset },
			{ ".html", AssetType.TextAsset },
			{ ".htm", AssetType.TextAsset },
			{ ".xml", AssetType.TextAsset },
			{ ".bytes", AssetType.TextAsset },
			{ ".json", AssetType.TextAsset },
			{ ".csv", AssetType.TextAsset },
			{ ".yaml", AssetType.TextAsset },
			{ ".fnt", AssetType.TextAsset },
			
			/* Texture */
			{ ".jpg", AssetType.Texture },
			{ ".jpeg", AssetType.Texture },
			{ ".tif", AssetType.Texture },
			{ ".tiff", AssetType.Texture },
			{ ".tga", AssetType.Texture },
			{ ".gif", AssetType.Texture },
			{ ".png", AssetType.Texture },
			{ ".psd", AssetType.Texture },
			{ ".bmp", AssetType.Texture },
			{ ".iff", AssetType.Texture },
			{ ".pict", AssetType.Texture },
			{ ".pic", AssetType.Texture },
			{ ".pct", AssetType.Texture },
			{ ".exr", AssetType.Texture },
			{ ".hdr", AssetType.Texture },
			{ ".cubemap", AssetType.Texture },
			
			/* VideoClip */
			{ ".mov", AssetType.VideoClip },
			{ ".mpg", AssetType.VideoClip },
			{ ".mpeg", AssetType.VideoClip },
			{ ".mp4", AssetType.VideoClip },
			{ ".avi", AssetType.VideoClip },
			{ ".asf", AssetType.VideoClip },
		};
	}
}
