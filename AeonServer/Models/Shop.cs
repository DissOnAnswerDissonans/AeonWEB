namespace AeonServer.Models;

public class ShopUpdate
{
	public Hero Hero { get; set; } = null!;
	public OfferData[] Offers { get; set; } = null!;
	public R Response { get; set; }
	public DateTimeOffset CloseIn { get; set; }

	public enum R { None, Opened, Closed, OK, NotEnough, AbilityOn, AbilityOff, AbilityError, OtherError }
}