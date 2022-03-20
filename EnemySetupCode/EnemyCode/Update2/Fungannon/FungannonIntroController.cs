using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;

namespace Planetside
{

	using System;
	using System.Collections.Generic;
	using Dungeonator;
	using UnityEngine;

	// Token: 0x02000FBB RID: 4027
	[RequireComponent(typeof(GenericIntroDoer))]
	public class FungannonIntroController : SpecificIntroDoer
	{
		public void Start()
		{
			tk2dSpriteAnimator component = this.healthHaver.GetComponent<tk2dSpriteAnimator>();
			component.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(component.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered));
		}



		public void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
		{
			bool flag = clip.GetFrame(frameIdx).eventInfo == "deathOno" && base.healthHaver != null;
			if (flag)
			{
				//base.StartCoroutine(this.OnDeathExplosionsCR());
			}
		}

	}

}
