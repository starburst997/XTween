using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Toki;
using Toki.Tween;

public class XTween
{
	private static UpdateTicker _ticker;
	private static UpdateTickerReal _tickerReal;
	private static UpdaterFactory _updaterFactory;

	public static float RealDeltaTime
	{
		get
		{
			return _tickerReal.GetDeltaTime(0);
		}
	}
	// frameSkip Max == 4
	public static float DeltaTimeWithFrameSkip( int frameSkip )
	{
		return _ticker.GetDeltaTime( frameSkip );
	}
	// frameSkip Max == 4
	public static float DeltaTimeRealWithFrameSkip( int frameSkip )
	{
		return _tickerReal.GetDeltaTime( frameSkip );
	}

	public static ITimer GetTicker( bool isReal )
	{
		if( isReal )
		{
			return _tickerReal;
		}
		else
		{
			return _ticker;
		}
	}

	public static void Initialize( int size )
	{
		Pool<XHash>.Initialize(size * 2);
		Pool<DisplayUpdater>.Initialize(size);
		Pool<ObjectTween>.Initialize(size);
		Pool<XObjectHash>.Initialize(size);
		Pool<ObjectUpdater>.Initialize(size);
		Pool<XColorHash>.Initialize(size);
		Pool<WaitForTweenPlay>.Initialize(size);
	}

	static XTween()
	{
#if UNITY_EDITOR
		if( Application.isPlaying )
		{
			_ticker = UpdateTicker.To;
			_tickerReal = UpdateTickerReal.To;
		}
		else
		{
			_ticker = new UpdateTicker();
			_tickerReal = new UpdateTickerReal();
		}
#else
		_ticker = UpdateTicker.To;
		_tickerReal = UpdateTickerReal.To;
#endif
		_ticker.Initialize();
		_tickerReal.Initialize();
		
		_updaterFactory = new UpdaterFactory();
	}

#if UNITY_EDITOR
	public static void PlayModeChanged( bool isEnterEditorMode )
	{
		if( isEnterEditorMode )
		{
			_ticker.AddUpdate();
			_tickerReal.AddUpdate();
		}
		else
		{
			_ticker.RemoveUpdate();
			_tickerReal.RemoveUpdate();
		}
	}
#endif

	/*===================================== Transform ========================================*/
	//Transform
	public static IXTween To( GameObject target, XHash hash, float time, IEasing easing = null, bool realTime = false )
	{
		ObjectTween tween = Pool<ObjectTween>.Pop();
		tween.Initialize( realTime ? (ITimer)_tickerReal : _ticker, 0 );
		tween.Updater = UpdaterFactory.Create(target, hash, hash.GetStart());
		tween.ClassicHandlers = hash;
		tween.Time = time;
		tween.Easing = (easing != null) ? easing : Linear.easeNone;
		return tween;
    }

    /*===================================== Value ========================================*/
	//Value - Single
	public static IXTween ToValue( Action<float> setter, float start, float end, float time, IEasing easing = null, bool realTime = false )
	{
		return ToValueBezier(setter, start, end, null, time, easing, realTime);
	}

	//Value - Single Bezier
	public static IXTween ToValueBezier( Action<float> setter, float start, float end, float[] controlPoints, float time, IEasing easing = null, bool realTime = false )
	{
		XEventHash hash = Pool<XEventHash>.Pop();
		ObjectTween tween = Pool<ObjectTween>.Pop();
		tween.Initialize( realTime ? (ITimer)_tickerReal : _ticker , 0 );
		GetSetUpdater updater = UpdaterFactory.Create( setter, start, end, controlPoints, hash );
		tween.Updater = updater;
		tween.ClassicHandlers = hash;
		tween.Time = time;
		tween.Easing = ( easing != null ) ? easing : Linear.easeNone;
		return tween;
	}

	//Value - Multi, Bezier
	public static IXTween ToValueMulti( XObjectHash source, Action<XObjectHash> UpdateHandler, float time, IEasing easing = null, bool realTime = false )
	{
		ObjectTween tween = Pool<ObjectTween>.Pop();
		tween.Initialize( realTime ? (ITimer)_tickerReal : _ticker, 0 );
		ObjectUpdater updater = UpdaterFactory.Create( source );
		updater.UpdateHandler = UpdateHandler;
		tween.Updater = updater;
		tween.ClassicHandlers = source;
		tween.Time = time;
		tween.Easing = ( easing != null ) ? easing : Linear.easeNone;
		return tween;
	}

	//Property - Single
	public static IXTween ToProperty<T>( T target, string propertyName, float end, float time, IEasing easing = null, bool realTime = false )
	{
		return ToPropertyMulti<T>(target, XObjectHash.New.Add(propertyName, end), time, easing, realTime);
	}

	//Property - Single
	public static IXTween ToProperty<T>( T target, string propertyName, float start, float end, float time, IEasing easing = null, bool realTime = false )
	{
		return ToPropertyMulti<T>(target, XObjectHash.New.Add(propertyName, start, end), time, easing, realTime);
	}

	//Property - Single Bezier
	public static IXTween ToPropertyBezier<T>( T target, string propertyName, float start, float end, float[] controlPoints, float time, IEasing easing = null, bool realTime = false )
	{
		return ToPropertyMulti<T>(target, XObjectHash.New.Add(propertyName, start, end).AddControlPoint(propertyName, controlPoints), time, easing, realTime);
	}

	//Proerpty - Multi, Bezier
	public static IXTween ToPropertyMulti<T>( T target, XObjectHash hash, float time, IEasing easing = null, bool realTime = false )
	{
		ObjectTween tween = Pool<ObjectTween>.Pop();
		ObjectUpdater<T> updater = UpdaterFactory.Create<T>( target, hash );
		tween.Initialize( realTime ? (ITimer)_tickerReal : _ticker, 0 );
		tween.Updater = updater;
		tween.ClassicHandlers = hash;
		tween.Time = time;
		tween.Easing = ( easing != null ) ? easing : Linear.easeNone;
		return tween;
    }

	/*===================================== Color ========================================*/
	//Sprite
	public static IXTween ToColor( SpriteRenderer target, XColorHash hash, float time, IEasing easing = null, bool realTime = false )
	{
		return ToColor<SpriteRenderer>(target, "color", hash, time, easing, realTime);
	}

	//UI
	public static IXTween ToColor( Graphic target, XColorHash hash, float time, IEasing easing = null, bool realTime = false )
	{
		return ToColor<Graphic>(target, "color", hash, time, easing, realTime);
	}

	//Color Property
	public static IXTween ToColor<T>( T target, string colorPropertyName, XColorHash hash, float time, IEasing easing = null, bool realTime = false )
	{
		ITimer tick = _ticker;
		ITimer tickReal = _tickerReal; 
		ObjectTween tween = Pool<ObjectTween>.Pop();
		ColorUpdater<T> updater = UpdaterFactory.Create<T>(target, colorPropertyName, hash, hash.GetStart() );
		tween.Initialize( realTime ? tickReal : tick, 0 );
		tween.Updater = updater;
		tween.ClassicHandlers = hash;
		tween.Time = time;
		tween.Easing = ( easing != null ) ? easing : Linear.easeNone;
		return tween;
    }

	/*===================================== Continue ========================================*/
	public static ContinousTween Continous( GameObject target, XHash hash, float time, IEasing easing = null, uint frameSkip = 0, bool realTime = false )
	{
		ITimer tick = _ticker;
		ITimer tickReal = _tickerReal; 
		ContinousTween tween = Pool<ContinousTween>.Pop();
		tween.Initialize( realTime ? tickReal : tick, 0 );
		tween.Time = time;
		tween.Easing = (easing != null) ? easing : Linear.easeNone;
		tween.Updater = UpdaterFactory.CreateContinous(target, hash, hash.GetStart());
		tween.ClassicHandlers = hash;
		return tween;
	}

	/*===================================== ParallelTweens ========================================*/
    public static IXTweenGroup ParallelTweens( bool realTime = false, params IXTween[] tweens )
	{
		ITimer tick = _ticker;
		ITimer tickReal = _tickerReal; 
        ParallelTween tween = Pool<ParallelTween>.Pop();
		tween.Initialize(tweens, realTime ? tickReal : tick, 0);
        return tween;
	}

    /*===================================== SerialTweens ========================================*/
    public static IXTweenGroup SerialTweens( bool realTime = false, params IXTween[] tweens )
	{
        SerialTween tween = Pool<SerialTween>.Pop();
		tween.Initialize(tweens, realTime ? (ITimer)_tickerReal : _ticker, 0);
		return tween;
	}
}

public static class XTweenShorcutExtensions
{
	//Poistion -Trasform
	public static IXTween To(this Transform trans, XHash hash, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.To(trans.gameObject, hash, time, easing, realTime);
	}

	//Position - GameObject
	public static IXTween To(this GameObject gameObject, XHash hash, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.To(gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToPosition2D(this Transform trans, float? x, float? y, float time, IEasing easing = null, bool realTime = false )
	{
		XHash hash = XHash.New;
		if( x != null ) hash.X = (float)x;
		if( y != null ) hash.Y = (float)y;
		return XTween.To(trans.gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToPosition3D(this Transform trans, float? x, float? y, float? z, float time, IEasing easing = null, bool realTime = false )
	{
		XHash hash = XHash.New;
		if( x != null ) hash.X = (float)x;
		if( y != null ) hash.Y = (float)y;
		if( z != null ) hash.Z = (float)z;
		return XTween.To(trans.gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToPosition2D(this GameObject gameObject, float? x, float? y, float time, IEasing easing = null, bool realTime = false )
	{
		XHash hash = XHash.New;
		if( x != null ) hash.X = (float)x;
		if( y != null ) hash.Y = (float)y;
		return XTween.To(gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToPosition3D(this GameObject gameObject, float? x, float? y, float? z, float time, IEasing easing = null, bool realTime = false )
	{
		XHash hash = XHash.New;
		if( x != null ) hash.X = (float)x;
		if( y != null ) hash.Y = (float)y;
		if( z != null ) hash.Z = (float)z;
		return XTween.To(gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToScale2D(this Transform trans, float? x, float? y, float time, IEasing easing = null, bool realTime = false )
	{
		XHash hash = XHash.New;
		if( x != null ) hash.ScaleX = (float)x;
		if( y != null ) hash.ScaleY = (float)y;
		return XTween.To(trans.gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToScale3D(this Transform trans, float? x, float? y, float? z, float time, IEasing easing = null, bool realTime = false )
	{
		XHash hash = XHash.New;
		if( x != null ) hash.ScaleX = (float)x;
		if( y != null ) hash.ScaleY = (float)y;
		if( z != null ) hash.ScaleZ = (float)z;
		return XTween.To(trans.gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToScale2D(this GameObject gameObject, float? x, float? y, float time, IEasing easing = null, bool realTime = false )
	{
		XHash hash = XHash.New;
		if( x != null ) hash.ScaleX = (float)x;
		if( y != null ) hash.ScaleY = (float)y;
		return XTween.To(gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToScale3D(this GameObject gameObject, float? x, float? y, float? z, float time, IEasing easing = null, bool realTime = false )
	{
		XHash hash = XHash.New;
		if( x != null ) hash.ScaleX = (float)x;
		if( y != null ) hash.ScaleY = (float)y;
		if( z != null ) hash.ScaleZ = (float)z;
		return XTween.To(gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToRotation2D(this Transform trans, float z, float time, bool clockwise, IEasing easing = null, bool realTime = false )
	{
		XHash hash = XHash.New;
		hash.AddRotationZ( z, clockwise, 0 );
		return XTween.To(trans.gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToRotation3D(this Transform trans, float? x, float? y, float? z, float time, IEasing easing = null, bool realTime = false )
	{
		XHash hash = XHash.New;
		if( x != null ) hash.AddRotationX((float)x);
		if( y != null ) hash.AddRotationY((float)y);
		if( z != null ) hash.AddRotationZ((float)z);
		return XTween.To(trans.gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToRotation2D(this GameObject gameObject, float z, float time, bool clockwise, IEasing easing = null, bool realTime = false )
	{
		XHash hash = XHash.New;
		hash.AddRotationZ( z, clockwise, 0 );
		return XTween.To(gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToRotation3D(this GameObject gameObject, float? x, float? y, float? z, float time, IEasing easing = null, bool realTime = false )
	{
		XHash hash = XHash.New;
		if( x != null ) hash.AddRotationX((float)x);
		if( y != null ) hash.AddRotationY((float)y);
		if( z != null ) hash.AddRotationZ((float)z);
		return XTween.To(gameObject, hash, time, easing, realTime);
	}

	public static IXTween ToPosition(this Transform trans, Vector3 position, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.To(trans.gameObject, XHash.Position(position), time, easing, realTime);
	}

	public static IXTween ToPosition(this GameObject gameObject, Vector3 position, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.To(gameObject, XHash.Position(position), time, easing, realTime);
	}

	//Value - setter
	public static IXTween ToValue(this object obj, Action<float> setter, float start, float end, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.ToValueBezier(setter, start, end, null, time, easing, realTime);
	}

	//Value - setter Bezier
	public static IXTween ToValueBezier(this object obj, Action<float> setter, float start, float end, float[] controlPoints, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.ToValueBezier(setter, start, end, controlPoints, time, easing, realTime);
	}

	//Value - Multi value
	public static IXTween ToValueMulti(this object obj, XObjectHash source, Action<XObjectHash> UpdateHandler, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.ToValueMulti(source, UpdateHandler, time, easing, realTime);
	}

	//Property - Property Type
	public static IXTween ToProperty<T>(this T target, string propertyName, float end, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.ToProperty<T>(target, propertyName, end, time, easing, realTime);
	}

	//Property - Property Type
	public static IXTween ToProperty<T>(this T target, string propertyName, float start, float end, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.ToProperty<T>(target, propertyName, start, end, time, easing, realTime);
	}

	//Property - Property Bezier
	public static IXTween ToPropertyBezier<T>(this T target, string propertyName, float start, float end, float[] controlPoints, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.ToPropertyBezier<T>(target, propertyName, start, end, controlPoints, time, easing, realTime);
	}

	//Property - Multi property
	public static IXTween ToPropertyMulti<T>(this T target, XObjectHash hash, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.ToPropertyMulti<T>(target, hash, time, easing, realTime);
	}

	//Color - Sprite
	public static IXTween ToColor(this SpriteRenderer target, XColorHash hash, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.ToColor<SpriteRenderer>(target, "color", hash, time, easing, realTime);
	}

	//Color - Sprite
	public static IXTween ToColor(this SpriteRenderer target, float? r, float? g, float? b, float? a, float time, IEasing easing = null, bool realTime = false )
	{
		XColorHash hash = XColorHash.New;
		if( r != null ) hash.AddRed((float)r);
		if( g != null ) hash.AddRed((float)g);
		if( b != null ) hash.AddRed((float)b);
		if( a != null ) hash.AddRed((float)a);
		return XTween.ToColor<SpriteRenderer>(target, "color", hash, time, easing, realTime);
	}

	//Color - UI
	public static IXTween ToColor(this Graphic target, XColorHash hash, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.ToColor<Graphic>(target, "color", hash, time, easing, realTime);
	}

	//Color - UI
	public static IXTween ToColor(this Graphic target, float? r, float? g, float? b, float? a, float time, IEasing easing = null, bool realTime = false )
	{
		XColorHash hash = XColorHash.New;
		if( r != null ) hash.AddRed((float)r);
		if( g != null ) hash.AddRed((float)g);
		if( b != null ) hash.AddRed((float)b);
		if( a != null ) hash.AddRed((float)a);
		return XTween.ToColor<Graphic>(target, "color", hash, time, easing, realTime);
	}

	//Color - Property
	public static IXTween ToColor<T>(this T target, string colorPropertyName, XColorHash hash, float time, IEasing easing = null, bool realTime = false )
	{
		return XTween.ToColor<T>(target, colorPropertyName, hash, time, easing, realTime);
	}

	//Color - Property
	public static IXTween ToColor<T>(this T target, string colorPropertyName, float? r, float? g, float? b, float? a, float time, IEasing easing = null, bool realTime = false )
	{
		XColorHash hash = XColorHash.New;
		if( r != null ) hash.AddRed((float)r);
		if( g != null ) hash.AddRed((float)g);
		if( b != null ) hash.AddRed((float)b);
		if( a != null ) hash.AddRed((float)a);
		return XTween.ToColor<T>(target, colorPropertyName, hash, time, easing, realTime);
	}
}
