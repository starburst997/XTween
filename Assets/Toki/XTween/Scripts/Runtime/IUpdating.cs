using System;
using UnityEngine;

namespace Toki.Tween
{
	public interface IUpdating : IDisposable
	{		
		Action StopOnDestroyHandler
		{
			set;
		}
			
		IClassicHandlable Start
		{
			set;
		}
			
		IClassicHandlable Finish
		{
			set;
		}
			
		void ResolveValues();
		void Updating( float factor );
		void Release();
	}
}