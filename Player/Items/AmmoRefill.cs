
// TODO: Ammo refill on buy.
public class AmmoRefill : ShopItem
{
    public override void Buy(Character character)
    {
        Magazine m = character.CurrentWeapon.Magazine;
        m.currentAmmoCount = m.maxAmmoCount;
    }
}
