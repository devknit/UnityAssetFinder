
using UnityEngine;

namespace Knit.EditorWindow
{
	internal abstract class MsgBoxDrawer : ComponentDrawerBase
	{
		protected abstract Rectangle Rectangle
		{
			get;
		}
		public void DrawMsgBox( Rect rect, object obj)
		{
			Rect main = Rectangle.GetRect( rect);
			GUI.Box( main, "", GUIStyleCache.GetStyle( "WindowBackground"));
			OnDrawMsgBox( main, obj);
		}
		public virtual void Serialize()
		{
			if( IsInitialized != false)
			{
				OnSerialize();
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
		protected virtual void OnSerialize()
		{
		}
		protected abstract void OnDrawMsgBox( Rect rect, object  obj);
	}
}