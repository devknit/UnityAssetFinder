
using UnityEngine;
using UnityEditor;

namespace Knit.EditorWindow
{
	public struct GUITweenParam
	{
		public GUITweenParam(bool isTweening = true)
		{
			m_IsTweening = isTweening;
			m_TweenTime = 0;
			m_Rect = default;
		}
		public bool CheckRect( Rect rect)
		{
			if( m_Rect != rect)
			{
				m_Rect = rect;
				return false;
			}
			return true;
		}
		public float TweenTime
		{
			get{ return m_TweenTime; }
			set{ m_TweenTime = value; }
		}
		public bool IsTweening
		{
			get{ return m_IsTweening; }
			set{ m_IsTweening = value; }
		}
		float m_TweenTime;
		bool m_IsTweening;
		Rect m_Rect;
	}
	public class GUIEx
	{
		public static bool ToolbarButton( Rect rect, string text)
		{
			return GUI.Button( rect, text, "toolbarbutton");
		}
		public static GUITweenParam ScaleTweenBox( Rect rect, GUITweenParam param, string text, GUIStyle style = null)
		{
			param = ScaleTweenInternal( ref rect, param);
			
			if( style != null)
			{
				GUI.Box( rect, text, style);
			}
			else
			{
				GUI.Box( rect, text);
			}
			return param;
		}
		public static GUITweenParam ScaleTweenBox( Rect rect, GUITweenParam param, GUIContent content, GUIStyle style = null)
		{
			param = ScaleTweenInternal(ref rect, param);
			
			if( style != null)
			{
				GUI.Box( rect, content, style);
			}
			else
			{
				GUI.Box( rect, content);
			}
			return param;
		}
		private static GUITweenParam ScaleTweenInternal( ref Rect rect, GUITweenParam param)
		{
			if( param.CheckRect( rect) == false)
			{
				param.TweenTime = 0;
			}
			param.TweenTime += 0.03f;
			//float scaleTweenTime = ((float)(EditorApplication.timeSinceStartup - param.tweenTime) / 0.1f);
			
			if( param.TweenTime > 1)
			{
				param.TweenTime = 1;
				param.IsTweening = false;
			}
			else
			{
				param.IsTweening = true;
			}
			float x = Mathf.Lerp( rect.x + rect.width / 2, rect.x, param.TweenTime);
			float y = Mathf.Lerp( rect.y + rect.height / 2, rect.y, param.TweenTime);
			float w = Mathf.Lerp( 0, rect.width, param.TweenTime);
			float h = Mathf.Lerp( 0, rect.height, param.TweenTime);
			rect = new Rect( x, y, w, h);
			return param;
		}
		public static string GetIconPath( SubWindowIcon icon)
		{
            return icon switch
            {
                SubWindowIcon.None => null,
                SubWindowIcon.Animation => EditorGUIUtility.isProSkin ? "d_UnityEditor.AnimationWindow" : "UnityEditor.AnimationWindow",
                SubWindowIcon.Animator => "UnityEditor.Graphs.AnimatorControllerTool",
                SubWindowIcon.AssetStore => EditorGUIUtility.isProSkin ? "d_Asset Store" : "Asset Store",
                SubWindowIcon.AudioMixer => EditorGUIUtility.isProSkin ? "d_Audio Mixer" : "Audio Mixer",
                SubWindowIcon.Web => EditorGUIUtility.isProSkin ? "d_BuildSettings.Web.Small" : "BuildSettings.Web.Small",
                SubWindowIcon.Console => EditorGUIUtility.isProSkin ? "d_UnityEditor.ConsoleWindow" : "UnityEditor.ConsoleWindow",
                SubWindowIcon.Game => EditorGUIUtility.isProSkin ? "d_UnityEditor.GameView" : "UnityEditor.GameView",
                SubWindowIcon.Hierarchy => EditorGUIUtility.isProSkin ? "d_UnityEditor.HierarchyWindow" : "UnityEditor.HierarchyWindow",
                SubWindowIcon.Inspector => EditorGUIUtility.isProSkin ? "d_UnityEditor.InspectorWindow" : "UnityEditor.InspectorWindow",
                SubWindowIcon.Lighting => EditorGUIUtility.isProSkin ? "d_Lighting" : "Lighting",
                SubWindowIcon.Navigation => EditorGUIUtility.isProSkin ? "d_Navigation" : "Navigation",
                SubWindowIcon.Occlusion => EditorGUIUtility.isProSkin ? "d_Occlusion" : "Occlusion",
                SubWindowIcon.Profiler => EditorGUIUtility.isProSkin ? "d_ZUnityEditor.ProfilerWindow" : "UnityEditor.ProfilerWindow",
                SubWindowIcon.Project => EditorGUIUtility.isProSkin ? "d_Project" : "Project",
                SubWindowIcon.Scene => EditorGUIUtility.isProSkin ? "d_UnityEditor.SceneView" : "UnityEditor.SceneView",
                SubWindowIcon.BuildSetting => EditorGUIUtility.isProSkin ? "d_BuildSettings.SelectedIcon" : "BuildSettings.SelectedIcon",
                SubWindowIcon.Shader => "Shader Icon",
                SubWindowIcon.Avator => "Avatar Icon",
                SubWindowIcon.GameObject => EditorGUIUtility.isProSkin ? "d_GameObject Icon" : "GameObject Icon",
                SubWindowIcon.Camera => "Camera Icon",
                SubWindowIcon.JavaScript => "js Script Icon",
                SubWindowIcon.CSharp => "cs Script Icon",
                SubWindowIcon.Sprite => "Sprite Icon",
                SubWindowIcon.Text => "TextAsset Icon",
                SubWindowIcon.AnimatorController => "AnimatorController Icon",
                SubWindowIcon.MeshRenderer => "MeshRenderer Icon",
                SubWindowIcon.Terrain => "Terrain Icon",
                SubWindowIcon.Audio => EditorGUIUtility.isProSkin ? "d_SceneviewAudio" : "SceneviewAudio",
                SubWindowIcon.IPhone => EditorGUIUtility.isProSkin ? "d_BuildSettings.iPhone.small" : "BuildSettings.iPhone.small",
                SubWindowIcon.Font => "Font Icon",
                SubWindowIcon.Material => "Material Icon",
                SubWindowIcon.GameManager => "GameManager Icon",
                SubWindowIcon.Player => "Animation Icon",
                SubWindowIcon.Texture => "Texture Icon",
                SubWindowIcon.Scriptable => "ScriptableObject Icon",
                SubWindowIcon.Movie => "MovieTexture Icon",
                SubWindowIcon.CGProgram => "CGProgram Icon",
                SubWindowIcon.Search => "Search Icon",
                SubWindowIcon.Favorite => "Favorite Icon",
                SubWindowIcon.Android => EditorGUIUtility.isProSkin ? "d_BuildSettings.Android.small" : "BuildSettings.Android.small",
                SubWindowIcon.Setting => EditorGUIUtility.isProSkin ? "d_SettingsIcon" : "SettingsIcon",
                SubWindowIcon.TimelineSelector => EditorGUIUtility.isProSkin ? "d_TimelineSelector" : "TimelineSelector",
                _ => null,
            };
        }
	}
}