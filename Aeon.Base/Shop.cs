﻿namespace Aeon.Base
{
	public class ShopUpdate
	{
		public Hero Hero { get; set; }
		public OfferData[] Offers { get; set; }
		public R Response { get; set; }
		public System.DateTimeOffset CloseIn { get; set; }

		public enum R { None, Opened, Closed, OK, NotEnough, AbilityOn, AbilityOff, AbilityError, OtherError }
	}
}