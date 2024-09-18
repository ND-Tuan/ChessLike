using UnityEngine;

public class CoinAndAmmoManager
{
    private static int PlayerCoin;
    private static int CurrentAmmo;
    private static int MaxAmmo = 120;


    public static int GetPlayerCoin(){
        return PlayerCoin;
    }

    public static void AddCoins(int amount){
        PlayerCoin += amount;
    }

    public static bool CanSpendCoins(int amount){
        return PlayerCoin >= amount;
    }

    public static void SpendCoins(int amount){
            PlayerCoin -= amount;
    }

    public static int GetAmmo(){
        return CurrentAmmo;
    }

    public static void SetMaxAmmo(int amount){
        MaxAmmo = amount;
    }

    public static void AddAmmo(int amount){
        
        CurrentAmmo += amount;

        if(CurrentAmmo + amount > MaxAmmo){
            CurrentAmmo = MaxAmmo;
        }
    }

    public static int ReloadAmmo(int AmmoCapacity){
        int ammo;

        if (CurrentAmmo >= AmmoCapacity){
            ammo = AmmoCapacity;
            CurrentAmmo -= AmmoCapacity;
        }else {
            ammo = CurrentAmmo;
            CurrentAmmo = 0;
        }

        return ammo;
    }
}
