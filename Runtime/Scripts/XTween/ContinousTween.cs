using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Toki.Tween
{
	public class ContinousTween : AbstractTween, IIXTweenObject
	{
		public override void Initialize( ITimer ticker, float position )
		{
			base.Initialize( ticker, position );
		}
			
		protected IUpdating _updater;
		protected IEasing _easing;

		public float Time
		{
			get { return _duration; }
			set { _duration = value; }
		}
		
		public IEasing Easing
		{
			get { return _easing; }
			set { _easing = value; }
		}
		
		public override void ResolveValues()
		{
			DisplayContinousUpdater display = (DisplayContinousUpdater)_updater;
			display.ResolveValues();
		}
			
		public IUpdating Updater
		{
			get { return _updater; }
			set 
			{ 
				_updater = value;
					
				if( _updater != null )
				{
					DisplayContinousUpdater display = (DisplayContinousUpdater)_updater;
					display.ticker = this._ticker;
					display.frameSkip = (int)this._frameSkip;
					display.Tweener = this;
				}
			}
		}
			
		protected override void InternalUpdate( float time )
		{
			float factor = 0.0f;
			
			if (time > 0.0f) {
				if (time < _duration) {
					factor = _easing.Calculate(time, 0.0f, 1.0f, _duration);
				}
				else {
					factor = 1.0f;
				}
			}
			_updater.Updating(factor);
		}

		public override bool TickNormal( float time )
		{
			if (!_isPlaying) {
				return true;
			}
			float t = time - _startTime;
			
			_position = t;
			InternalUpdate(t);
			
			if (_classicHandlers != null && _classicHandlers.OnUpdate != null) {
				_classicHandlers.OnUpdate.Execute();
			}

			if (_isPlaying) {
				if (t >= _duration) {
					_position = _duration;
				}
				return false;
			}
			return true;
		}

		public override IXTween Play()
		{
			if( !_isPlaying )
			{
				_position = 0;
			}
			base.Play();
			return this;
		}

		public override IXTween Play( float position )
		{
			//Not implement
			return null;
		}

		public override IXTween Seek( float position )
		{
			//Not implement
			return this;
		}
	}
}