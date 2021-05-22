using UnityEngine;
using System.Collections;

namespace Toki.Tween
{
	public class QuarticEaseIn : IEasing
	{
		public float Calculate( float t, float b, float c, float d )
		{
			return c * (t /= d) * t * t * t + b;
		}
	}
}