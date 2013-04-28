using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using To_Space.Items;

namespace To_Space.Inv
{
	public class ItemStack
	{
		//Item contained in this stack
		public Item Item;

		//Amount of the item contained in this stack
		public int ItemAmount;

		public ItemStack(Item item, int amt)
		{
			this.Item = item;
			this.ItemAmount = amt;
		}
	}
}