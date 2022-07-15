using System;
using System.Collections.Generic;
using UnityEngine;

namespace GungeonAPI
{
	// Token: 0x0200000F RID: 15
	public abstract class SimpleInteractable : BraveBehaviour
	{
		public Action<PlayerController, GameObject> OnAccept;

		public Action<PlayerController, GameObject> OnDecline;

		public List<string> conversation;

		public List<string> conversation2;

		public List<string> conversation3;

		public List<string> conversation4;

		public Func<PlayerController, GameObject, bool> CanUse;

		public Transform talkPoint;

		public string text;

		public string acceptText;

		public string acceptText2;

		public string declineText;

		public string declineText2;

		public bool isToggle;

		public GameObject roomIcon;

		protected bool m_isToggled;

		protected bool m_canUse = true;

		public bool HasRoomIcon;
	}
}
