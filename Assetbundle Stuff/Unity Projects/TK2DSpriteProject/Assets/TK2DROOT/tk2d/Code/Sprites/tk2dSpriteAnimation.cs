using UnityEngine;
using System.Collections;

[System.Serializable]
/// <summary>
/// Defines an animation frame and associated data.
/// </summary>
public class tk2dSpriteAnimationFrame
{
	// Token: 0x06003F5E RID: 16222 RVA: 0x00141498 File Offset: 0x0013F698
	public void CopyFrom(tk2dSpriteAnimationFrame source)
	{
		this.CopyFrom(source, true);
	}

	// Token: 0x06003F5F RID: 16223 RVA: 0x001414A4 File Offset: 0x0013F6A4
	public void CopyTriggerFrom(tk2dSpriteAnimationFrame source)
	{
		this.triggerEvent = source.triggerEvent;
		this.eventInfo = source.eventInfo;
		this.eventInt = source.eventInt;
		this.eventFloat = source.eventFloat;
		this.eventAudio = source.eventAudio;
		this.eventVfx = source.eventVfx;
		this.eventStopVfx = source.eventStopVfx;
		this.eventOutline = source.eventOutline;
		this.forceMaterialUpdate = source.forceMaterialUpdate;
		this.finishedSpawning = source.finishedSpawning;
		this.eventLerpEmissive = source.eventLerpEmissive;
		this.eventLerpEmissivePower = source.eventLerpEmissivePower;
		this.eventLerpEmissiveTime = source.eventLerpEmissiveTime;
	}

	// Token: 0x06003F60 RID: 16224 RVA: 0x00141550 File Offset: 0x0013F750
	public void ClearTrigger()
	{
		this.triggerEvent = false;
		this.eventInt = 0;
		this.eventFloat = 0f;
		this.eventInfo = string.Empty;
		this.eventAudio = string.Empty;
		this.eventVfx = string.Empty;
		this.eventStopVfx = string.Empty;
		this.eventOutline = tk2dSpriteAnimationFrame.OutlineModifier.Unspecified;
		this.forceMaterialUpdate = false;
		this.finishedSpawning = false;
		this.eventLerpEmissive = false;
		this.eventLerpEmissivePower = 30f;
		this.eventLerpEmissiveTime = 0.5f;
	}

	// Token: 0x06003F61 RID: 16225 RVA: 0x001415D4 File Offset: 0x0013F7D4
	public void CopyFrom(tk2dSpriteAnimationFrame source, bool full)
	{
		this.spriteCollection = source.spriteCollection;
		this.spriteId = source.spriteId;
		this.invulnerableFrame = source.invulnerableFrame;
		this.groundedFrame = source.groundedFrame;
		this.requiresOffscreenUpdate = source.requiresOffscreenUpdate;
		if (full)
		{
			this.CopyTriggerFrom(source);
		}
	}

	// Token: 0x04003197 RID: 12695
	public tk2dSpriteCollectionData spriteCollection;

	// Token: 0x04003198 RID: 12696
	public int spriteId;

	// Token: 0x04003199 RID: 12697
	public bool invulnerableFrame;

	// Token: 0x0400319A RID: 12698
	public bool groundedFrame = true;

	// Token: 0x0400319B RID: 12699
	public bool requiresOffscreenUpdate;

	// Token: 0x0400319C RID: 12700
	public string eventAudio = string.Empty;

	// Token: 0x0400319D RID: 12701
	public string eventVfx = string.Empty;

	// Token: 0x0400319E RID: 12702
	public string eventStopVfx = string.Empty;

	// Token: 0x0400319F RID: 12703
	public bool eventLerpEmissive;

	// Token: 0x040031A0 RID: 12704
	public float eventLerpEmissiveTime = 0.5f;

	// Token: 0x040031A1 RID: 12705
	public float eventLerpEmissivePower = 30f;

	// Token: 0x040031A2 RID: 12706
	public bool forceMaterialUpdate;

	// Token: 0x040031A3 RID: 12707
	public bool finishedSpawning;

	// Token: 0x040031A4 RID: 12708
	public bool triggerEvent;

	// Token: 0x040031A5 RID: 12709
	public string eventInfo = string.Empty;

	// Token: 0x040031A6 RID: 12710
	public int eventInt;

	// Token: 0x040031A7 RID: 12711
	public float eventFloat;

	// Token: 0x040031A8 RID: 12712
	public tk2dSpriteAnimationFrame.OutlineModifier eventOutline;

	// Token: 0x02000BB2 RID: 2994
	public enum OutlineModifier
	{
		// Token: 0x040031AA RID: 12714
		Unspecified,
		// Token: 0x040031AB RID: 12715
		TurnOn = 10,
		// Token: 0x040031AC RID: 12716
		TurnOff = 20
	}
}
/*
public class tk2dSpriteAnimationFrame
{
	/// <summary>
	/// The sprite collection.
	/// </summary>
	public tk2dSpriteCollectionData spriteCollection;
	/// <summary>
	/// The sprite identifier.
	/// </summary>
	public int spriteId;
	
	/// <summary>
	/// When true will trigger an animation event when this frame is displayed
	/// </summary>
	public bool triggerEvent = false;
	/// <summary>
	/// Custom event data (string)
	/// </summary>
	public string eventInfo = "";
	/// <summary>
	/// Custom event data (int)
	/// </summary>
	public int eventInt = 0;
	/// <summary>
	/// Custom event data (float)
	/// </summary>
	public float eventFloat = 0.0f;
	
	public void CopyFrom(tk2dSpriteAnimationFrame source)
	{
		CopyFrom(source, true);
	}

	public void CopyTriggerFrom(tk2dSpriteAnimationFrame source)
	{
		triggerEvent = source.triggerEvent;
		eventInfo = source.eventInfo;
		eventInt = source.eventInt;
		eventFloat = source.eventFloat;		
	}

	public void ClearTrigger()
	{
		triggerEvent = false;
		eventInt = 0;
		eventFloat = 0;
		eventInfo = "";
	}
	
	public void CopyFrom(tk2dSpriteAnimationFrame source, bool full)
	{
		spriteCollection = source.spriteCollection;
		spriteId = source.spriteId;
		
		if (full) CopyTriggerFrom(source);
	}
}

[System.Serializable]
/// <summary>
/// Sprite Animation Clip contains a collection of frames and associated properties required to play it.
/// </summary>
public class tk2dSpriteAnimationClip
{
	/// <summary>
	/// Name of animation clip
	/// </summary>
	public string name = "Default";
	
	/// <summary>
	/// Array of frames
	/// </summary>
	public tk2dSpriteAnimationFrame[] frames = new tk2dSpriteAnimationFrame[0];
	
	/// <summary>
	/// FPS of clip
	/// </summary>
	public float fps = 30.0f;
	
	/// <summary>
	/// Defines the start point of the loop when <see cref="WrapMode.LoopSection"/> is selected
	/// </summary>
	public int loopStart = 0;
	
	/// <summary>
	/// Wrap mode for the clip
	/// </summary>
	public enum WrapMode
	{
		/// <summary>
		/// Loop indefinitely
		/// </summary>
		Loop,
		
		/// <summary>
		/// Start from beginning, and loop a section defined by <see cref="tk2dSpriteAnimationClip.loopStart"/>
		/// </summary>
		LoopSection,
		
		/// <summary>
		/// Plays the clip once and stops at the last frame
		/// </summary>
		Once,
		
		/// <summary>
		/// Plays the clip once forward, and then once in reverse, repeating indefinitely
		/// </summary>
		PingPong,
		
		/// <summary>
		/// Simply choses a random frame and stops
		/// </summary>
		RandomFrame,
		
		/// <summary>
		/// Starts at a random frame and loops indefinitely from there. Useful for multiple animated sprites to start at a different phase.
		/// </summary>
		RandomLoop,
		
		/// <summary>
		/// Switches to the selected sprite and stops.
		/// </summary>
		Single
	};
	
	/// <summary>
	/// The wrap mode.
	/// </summary>
	public WrapMode wrapMode = WrapMode.Loop;

	/// <summary>
	/// Default contstructor
	/// </summary>
	public tk2dSpriteAnimationClip() {

	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	public tk2dSpriteAnimationClip(tk2dSpriteAnimationClip source) {
		CopyFrom( source );
	}

	/// <summary>
	/// Copies the source animation clip into the current one.
	/// All frames are duplicated.
	/// </summary>
	public void CopyFrom(tk2dSpriteAnimationClip source)
	{
		name = source.name;
		if (source.frames == null) 
		{
			frames = null;
		}
		else
		{
			frames = new tk2dSpriteAnimationFrame[source.frames.Length];
			for (int i = 0; i < frames.Length; ++i)
			{
				if (source.frames[i] == null)
				{
					frames[i] = null;
				}
				else
				{
					frames[i] = new tk2dSpriteAnimationFrame();
					frames[i].CopyFrom(source.frames[i]);
				}
			}
		}
		fps = source.fps;
		loopStart = source.loopStart;
		wrapMode = source.wrapMode;
		if (wrapMode == tk2dSpriteAnimationClip.WrapMode.Single && frames.Length > 1)
		{
			frames = new tk2dSpriteAnimationFrame[] { frames[0] };
			Debug.LogError(string.Format("Clip: '{0}' Fixed up frames for WrapMode.Single", name));
		}
	}

	/// <summary>
	/// Clears the clip, removes all frames
	/// </summary>
	public void Clear()
	{
		name = "";
		frames = new tk2dSpriteAnimationFrame[0];
		fps = 30.0f;
		loopStart = 0;
		wrapMode = WrapMode.Loop;
	}

	/// <summary>
	/// Is the clip empty?
	/// </summary>
	public bool Empty
	{
		get { return name.Length == 0 || frames == null || frames.Length == 0; }
	}

	/// <summary>
	/// Gets the tk2dSpriteAnimationFrame for a particular frame
	/// </summary>
	public tk2dSpriteAnimationFrame GetFrame(int frame) {
		return frames[frame];
	}
}*/
[System.Serializable]

// Token: 0x02000BB3 RID: 2995

public class tk2dSpriteAnimationClip
{
	// Token: 0x06003F62 RID: 16226 RVA: 0x0014162C File Offset: 0x0013F82C
	public tk2dSpriteAnimationClip()
	{
	}

	// Token: 0x06003F63 RID: 16227 RVA: 0x00141660 File Offset: 0x0013F860
	public tk2dSpriteAnimationClip(tk2dSpriteAnimationClip source)
	{
		this.CopyFrom(source);
	}

	// Token: 0x17000996 RID: 2454
	// (get) Token: 0x06003F64 RID: 16228 RVA: 0x0014169C File Offset: 0x0013F89C
	public float BaseClipLength
	{
		get
		{
			return (float)this.frames.Length / this.fps;
		}
	}

	// Token: 0x06003F65 RID: 16229 RVA: 0x001416B0 File Offset: 0x0013F8B0
	public void CopyFrom(tk2dSpriteAnimationClip source)
	{
		this.name = source.name;
		if (source.frames == null)
		{
			this.frames = null;
		}
		else
		{
			this.frames = new tk2dSpriteAnimationFrame[source.frames.Length];
			for (int i = 0; i < this.frames.Length; i++)
			{
				if (source.frames[i] == null)
				{
					this.frames[i] = null;
				}
				else
				{
					this.frames[i] = new tk2dSpriteAnimationFrame();
					this.frames[i].CopyFrom(source.frames[i]);
				}
			}
		}
		this.fps = source.fps;
		this.loopStart = source.loopStart;
		this.wrapMode = source.wrapMode;
		this.minFidgetDuration = source.minFidgetDuration;
		this.maxFidgetDuration = source.maxFidgetDuration;
		if (this.wrapMode == tk2dSpriteAnimationClip.WrapMode.Single && this.frames.Length > 1)
		{
			this.frames = new tk2dSpriteAnimationFrame[]
			{
				this.frames[0]
			};
			Debug.LogError(string.Format("Clip: '{0}' Fixed up frames for WrapMode.Single", this.name));
		}
	}

	// Token: 0x06003F66 RID: 16230 RVA: 0x001417CC File Offset: 0x0013F9CC
	public void Clear()
	{
		this.name = string.Empty;
		this.frames = new tk2dSpriteAnimationFrame[0];
		this.fps = 30f;
		this.loopStart = 0;
		this.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
	}

	// Token: 0x17000997 RID: 2455
	// (get) Token: 0x06003F67 RID: 16231 RVA: 0x00141800 File Offset: 0x0013FA00
	public bool Empty
	{
		get
		{
			return this.name.Length == 0 || this.frames == null || this.frames.Length == 0;
		}
	}

	// Token: 0x06003F68 RID: 16232 RVA: 0x0014182C File Offset: 0x0013FA2C
	public tk2dSpriteAnimationFrame GetFrame(int frame)
	{
		return this.frames[frame];
	}

	// Token: 0x040031AD RID: 12717
	public string name = "Default";

	// Token: 0x040031AE RID: 12718
	public tk2dSpriteAnimationFrame[] frames;

	// Token: 0x040031AF RID: 12719
	public float fps = 30f;

	// Token: 0x040031B0 RID: 12720
	public int loopStart;

	// Token: 0x040031B1 RID: 12721
	public tk2dSpriteAnimationClip.WrapMode wrapMode;

	// Token: 0x040031B2 RID: 12722
	public float minFidgetDuration = 1f;

	// Token: 0x040031B3 RID: 12723
	public float maxFidgetDuration = 2f;

	// Token: 0x02000BB4 RID: 2996
	public enum WrapMode
	{
		// Token: 0x040031B5 RID: 12725
		Loop,
		// Token: 0x040031B6 RID: 12726
		LoopSection,
		// Token: 0x040031B7 RID: 12727
		Once,
		// Token: 0x040031B8 RID: 12728
		PingPong,
		// Token: 0x040031B9 RID: 12729
		RandomFrame,
		// Token: 0x040031BA RID: 12730
		RandomLoop,
		// Token: 0x040031BB RID: 12731
		Single,
		// Token: 0x040031BC RID: 12732
		LoopFidget
	}
}


[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteAnimation")]
/// <summary>
/// Holds a collection of clips
/// </summary>
public class tk2dSpriteAnimation : MonoBehaviour 
{
	/// <summary>
	/// Array of <see cref="tk2dSpriteAnimationClip">clips</see>
	/// </summary>
	public tk2dSpriteAnimationClip[] clips;
	
	/// <summary>
	/// Resolves an animation clip by name and returns a reference to it
	/// </summary>
	/// <returns> tk2dSpriteAnimationClip reference, null if not found </returns>
	/// <param name='name'>Case sensitive clip name, as defined in <see cref="tk2dSpriteAnimationClip"/>. </param>
	public tk2dSpriteAnimationClip GetClipByName(string name)
	{
		for (int i = 0; i < clips.Length; ++i)
			if (clips[i].name == name) return clips[i];
		return null;
	}

	/// <summary>
	/// Resolves an animation clip by id and returns a reference to it
	/// </summary>
	/// <returns> tk2dSpriteAnimationClip reference, null if not found </returns>
	public tk2dSpriteAnimationClip GetClipById(int id) {
		if (id < 0 || id >= clips.Length || clips[id].Empty) {
			return null;
		}
		else {
			return clips[id];
		}
	}

	/// <summary>
	/// Resolves an animation clip by name and returns a clipId
	/// </summary>
	/// <returns> Unique clip id, -1 if not found </returns>
	/// <param name='name'>Case sensitive clip name, as defined in <see cref="tk2dSpriteAnimationClip"/>. </param>
	public int GetClipIdByName(string name) {
		for (int i = 0; i < clips.Length; ++i)
			if (clips[i].name == name) return i;
		return -1;
	}

	/// <summary>
	/// Gets a clip id from a clip
	/// </summary>
	/// <returns> Unique clip id, -1 if not found in the animation collection </returns>
	public int GetClipIdByName(tk2dSpriteAnimationClip clip) {
		for (int i = 0; i < clips.Length; ++i)
			if (clips[i] == clip) return i;
		return -1;
	}

	/// <summary>
	/// The first valid clip in the animation collection. Null if no valid clips are found.
	/// </summary>
	public tk2dSpriteAnimationClip FirstValidClip {
		get {
			for (int i = 0; i < clips.Length; ++i) {
				if (!clips[i].Empty && clips[i].frames[0].spriteCollection != null && clips[i].frames[0].spriteId != -1) {
					return clips[i];
				}
			}
			return null;
		}
	}
}


