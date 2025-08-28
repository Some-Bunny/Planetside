using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planetside
{
	public class LabelablePlayerItem : PlayerItem
	{
		public void SetLabel(string label)
		{
			this.currentLabel = label;
		}
		public string currentLabel;
	}
}
