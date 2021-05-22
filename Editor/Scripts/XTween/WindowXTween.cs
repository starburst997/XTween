/**********************************************************************************
/*		File Name 		: TestWindow.cs
/*		Author 			: Robin
/*		Description 	: 
/*		Created Date 	: 2016-7-27
/*		Modified Date 	: 
/**********************************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;
using Toki.Editors.EditorWindows;
using Toki.Common;

namespace Toki.Tween
{
	public class WindowXTween : EditorWindowAccordian
	{
		/************************************************************************
		*	 	 	 	 	Static Variable Declaration	 	 	 	 	 	    *
		************************************************************************/
		
		/************************************************************************
		*	 	 	 	 	Static Method Declaration	 	 	 	     	 	*
		************************************************************************/
		[MenuItem("Window/XTween #%9",priority=16)]
		public static void OpenXTweenEditorWindow()
		{
			EditorWindow.GetWindow<WindowXTween>(false, XTweenVersionController.NAME, true);
		}
		
		
		/************************************************************************
		*	 	 	 	 	Private Variable Declaration	 	 	 	 	 	*
		************************************************************************/
		
		
		/************************************************************************
		*	 	 	 	 	Protected Variable Declaration	 	 	 	 	 	*
		************************************************************************/
		
		
		/************************************************************************
		*	 	 	 	 	Public Variable Declaration	 	 	 	 	 		*
		************************************************************************/
		
		
		/************************************************************************
		*	 	 	 	 	Getter & Setter Declaration	 	 	 	 	 		*
		************************************************************************/
		
		
		/************************************************************************
		*	 	 	 	 	Initialize & Destroy Declaration	 	 	 		*
		************************************************************************/
		public override void Initialize()
		{
			this.AddModule(new ModuleUpdate(()=>{ return XTweenVersionController.To;}), new ModuleXTweenEasing());
			base.Initialize();
		}

		/************************************************************************
		*	 	 	 	 	Life Cycle Method Declaration	 	 	 	 	 	*
		************************************************************************/

		
		/************************************************************************
		*	 	 	 	 	Coroutine Declaration	 	  			 	 		*
		************************************************************************/
		
		
		/************************************************************************
		*	 	 	 	 	Private Method Declaration	 	 	 	 	 		*
		************************************************************************/
		
		
		/************************************************************************
		*	 	 	 	 	Protected Method Declaration	 	 	 	 	 	*
		************************************************************************/
		
		
		/************************************************************************
		*	 	 	 	 	Public Method Declaration	 	 	 	 	 		*
		************************************************************************/
		public override void UpdateGUI()
		{
			this._showHeader = false;
			GUILayout.BeginVertical();
			GUILayout.Label(XTweenVersionController.NAME, "BoldLabel");
			
			base.UpdateGUI();

			GUILayout.EndVertical();
		}
	}
}