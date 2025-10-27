
using System;

namespace Knit.EditorWindow
{
	public enum SubWindowIcon
	{
		None,
		BuildSetting,
		Hierarchy,
		Scene,
		Inspector,
		Game,
		Console,
		Project,
		Animation,
		Profiler,
		AudioMixer,
		AssetStore,
		Animator,
		Lighting,
		Occlusion,
		Navigation,
		Web,
		IPhone,
		Android,
		Shader,
		Avator,
		GameObject,
		Camera,
		JavaScript,
		CSharp,
		Sprite,
		Text,
		AnimatorController,
		Terrain,
		MeshRenderer,
		Font,
		Material,
		GameManager,
		Texture,
		Scriptable,
		CGProgram,
		Favorite,
		Search,
		Player,
		Movie,
		Audio,
		Setting,
		TimelineSelector,
	}
	public enum SubWindowToolbarType
	{
		None,
		Normal,
		Mini,
	}
	[AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class SubWindowAttribute : Attribute
	{
		public SubWindowAttribute( string title, SubWindowIcon icon = SubWindowIcon.None, bool active = true, SubWindowStyle windowStyle = SubWindowStyle.Default, SubWindowToolbarType toolbar = SubWindowToolbarType.None, SubWindowHelpBoxType helpBox = SubWindowHelpBoxType.None)
		{
			m_Title = title;
			m_IconPath = GUIEx.GetIconPath( icon);
			m_Active = active;
			m_WindowStyle = windowStyle;
			m_Toolbar = toolbar;
			m_HelpBox = helpBox;
		}
		public SubWindowAttribute( string title, string icon, bool active = true, SubWindowStyle windowStyle = SubWindowStyle.Default, SubWindowToolbarType toolbar = SubWindowToolbarType.None, SubWindowHelpBoxType helpBox = SubWindowHelpBoxType.None)
		{
			m_Title = title;
			m_IconPath = icon;
			m_Active = active;
			m_WindowStyle = windowStyle;
			m_Toolbar = toolbar;
			m_HelpBox = helpBox;
		}
		public string Title
		{
			get{ return m_Title; }
		}
		public string IconPath
		{
			get{ return m_IconPath; }
		}
		public bool Active
		{
			get{ return m_Active; }
		}
		public SubWindowStyle WindowStyle
		{
			get{ return m_WindowStyle; }
		}
		public SubWindowToolbarType Toolbar
		{
			get{ return m_Toolbar; }
		}
		public SubWindowHelpBoxType HelpBox
		{
			get{ return m_HelpBox; }
		}
        readonly string m_Title;
        readonly string m_IconPath;
        readonly bool m_Active;
        readonly SubWindowStyle m_WindowStyle;
		readonly SubWindowToolbarType m_Toolbar;
		readonly SubWindowHelpBoxType m_HelpBox;
	}
}