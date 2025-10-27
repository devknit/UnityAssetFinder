
using System;
using UnityEngine;

namespace Knit.EditorWindow
{
	[Serializable]
	public abstract class MsgBoxCustomDrawer : ComponentCustomDrawerBase
	{
		public abstract Rectangle Recttangle
		{
			get;
		}
		public Action closeAction;
		
		public void CloseMsgBox()
		{
			if( closeAction != null)
			{
				closeAction();
			}
		}
		public override void OnDestroy()
		{
		}
		public override void OnDisable()
		{
		}
		public override void OnEnable()
		{
		}
		public override void Init()
		{
		}
		public virtual void DrawMsgBox( Rect rect, object obj)
		{
		}
	}
}