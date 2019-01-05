using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Toki.Tween
{
	public class ParallelTween : GroupTween
	{
		protected List<IIXTween> _destroyList;

		public void Initialize(IXTween[] targets, ITimer ticker, float position)
		{
			base.Initialize(ticker,position);
			int l = targets.Length;
				
			_duration = 0;
				
			if (l > 0) {
				_a = targets[0] as IIXTween;
				_duration = _a.Duration > _duration ? _a.Duration : _duration;
				if (l > 1) {
					_b = targets[1] as IIXTween;
					_duration = _b.Duration > _duration ? _b.Duration : _duration;
					if (l > 2) {
						_c = targets[2] as IIXTween;
						_duration = _c.Duration > _duration ? _c.Duration : _duration;
						if (l > 3) {
							_d = targets[3] as IIXTween;
							_duration = _d.Duration > _duration ? _d.Duration : _duration;
							if (l > 4) {
								int length = l - 4;
								_targets = new IIXTween[length];
								for (int i = 4; i < l; ++i) {
									IIXTween t = targets[i] as IIXTween;
									_targets[i - 4] = t;
									_duration = t.Duration > _duration ? t.Duration : _duration;
								}
							}
						}
					}
				}
			}
		}
			
		protected override void InternalUpdate( float time )
		{
			if (_a != null) {
				_a.UpdateTween(time);
				if (_b != null) {
					_b.UpdateTween(time);
					if (_c != null) {
						_c.UpdateTween(time);
						if (_d != null) {
							_d.UpdateTween(time);
							if (_targets != null) {
								IIXTween[] targets = _targets;
								int l = targets.Length;
								for (int i = 0; i < l; ++i) {
									targets[i].UpdateTween(time);
								}
							}
						}
					}
				}
			}
		}
			
		protected override AbstractTween NewInstance()
		{
			List<IXTween> targets = new List<IXTween>();
			if (_a != null) {
				targets.Add(_a.Clone());
			}
			if (_b != null) {
				targets.Add(_b.Clone());
			}
			if (_c != null) {
				targets.Add(_c.Clone());
			}
			if (_d != null) {
				targets.Add(_d.Clone());
			}
			if (_targets != null) {
				IIXTween[] t = _targets;
				int l = t.Length;
				for (int i = 0; i < l; ++i) {
					targets.Add(t[i].Clone());
				}
			}
			ParallelTween tween = new ParallelTween();
			tween.Initialize(targets.ToArray(), _ticker, 0);
			return tween;
		}
	}
}