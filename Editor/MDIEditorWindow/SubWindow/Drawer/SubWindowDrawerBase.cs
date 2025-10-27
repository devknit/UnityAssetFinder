
using UnityEngine;
using UnityEditor;

namespace Knit.EditorWindow
{
	internal abstract class SubWindowDrawerBase : ComponentDrawerBase
	{
		public abstract GUIContent Title
		{
			get;
		}
		protected abstract SubWindowToolbarType toolBar
		{
			get;
		}
		protected abstract SubWindowHelpBox helpBox
		{
			get;
		}
		public virtual void DrawLeafToolBar( Rect rect)
		{
		}
		public abstract string GetID( bool dynamic);
		
		public Rect DrawToolBar( ref Rect rect)
		{
			if( toolBar == SubWindowToolbarType.Normal)
			{
				var h = new Rect( rect.x, rect.y, rect.width, 18);
				rect = new Rect( rect.x, rect.y + 18, rect.width, rect.height - 18);
				GUI.Box( h, string.Empty, EditorStyles.toolbar);
				return h;
			}
			else if( toolBar == SubWindowToolbarType.Mini)
			{
				var h = new Rect( rect.x, rect.y, rect.width, 15);
				rect = new Rect( rect.x, rect.y + 15, rect.width, rect.height - 15);
				GUI.Box( h, string.Empty, GUIStyleCache.GetStyle( "MiniToolbarButton"));
				return h;
			}
			return new Rect( rect.x, rect.y, 0, 0);
		}
		public Rect DrawHelpBox( ref Rect rect)
		{
			if( helpBox != null)
			{
				return helpBox.DrawHelpBox( ref rect);
			}
			return new Rect( rect.x, rect.y, 0, 0);
		}
		public void Serialize( bool dynamic)
		{
			if( IsInitialized != false)
			{
				OnSerialize( dynamic);
			}
		}
		protected override void OnDestroy()
		{
		}
		protected override void OnEnable()
		{
		}
		protected override void OnDisable()
		{
		}
		protected virtual void OnSerialize(bool dynamic)
		{
		}
		public abstract void DrawWindow( Rect mainRect, Rect toolbarRect, Rect helpboxRect);
	}
}