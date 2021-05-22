using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Toki.Tween
{
	public class ObjectTween : AbstractTween, IIXTweenObject
	{
		public override void Initialize( ITimer ticker, float position )
		{
			base.Initialize(ticker, 0);
		}
			
		protected IEasing _easing;
		protected IUpdating _updater;
			
		public float Time
		{
			get { return _duration; }
			set { _duration = value; }
		}

		public override bool Disposed
		{
			get
			{
				return _updater == null;
			}
		}
			
		public IEasing Easing
		{
			get { return _easing; }
			set { _easing = value; }
		}
			
		public IUpdating Updater
		{
			get { return _updater; }
			set 
			{ 
				_updater = value;
					
				if( _updater != null )
				{
					_updater.Tweener = this;
				}
			}
		}

		public override void ResolveValues()
		{
			this._updater.ResolveValues();
		}
			
		protected override void InternalUpdate( float time )
		{
			float factor = 0f;

			if (time > 0f) 
			{
				if (time < _duration) 
				{
					factor = _easing.Calculate(time, 0f, 1f, _duration);
				}
				else 
				{
					factor = 1f;
				}
			}
			_updater.Updating(factor);
		}
			
		protected override AbstractTween NewInstance()
		{
			AbstractTween tween = new ObjectTween();
			tween.Initialize(_ticker, 0);
			return tween;
		}

		protected override void InternalRelease()
		{
			if( this._autoDispose ) this.PoolPush();
		}

		public override void Dispose()
		{
			base.Dispose();
			if( this._updater != null ) 
				this._updater.Release();
			this._easing = null;
			this._updater = null;
		}
	}
}