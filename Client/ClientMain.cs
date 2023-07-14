using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LemonUI;
using LemonUI.Menus;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace jph_basics.Client
{
    public class ClientMain : BaseScript
    {
        ObjectPool pool;
        NativeMenu menu;
        int lastTime = 0;
        public ClientMain() {
            pool = new ObjectPool();
            WeaponMenu();
        }
        public void Notify(string text) {
            BeginTextCommandThefeedPost("STRING");
            AddTextComponentSubstringPlayerName(text);
            EndTextCommandThefeedPostTicker(true, true);
        }
        public bool GiveWeapon(int ped, string model, int ammo, bool inHand) {
            var hash = (uint) GetHashKey(model);
            if (GetWeapontypeModel(hash) == 0) {
                Debug.WriteLine("ERROR! Invalid weapon model");
                Notify(("~r~ERROR! Invalid weapon model"));
                return false;
            }
            if (ammo < 0) {
                Debug.WriteLine("ERROR! Ammo amount can't be lower than 0!");
                Notify(("~r~ERROR! Ammo amount can't be lower than 0!"));
                return false;
            }
            GiveWeaponToPed(ped, hash, ammo, false, inHand);
            return true;
        }

        [Command("weapon")]
        public void GiveWeaponCommand(int src, List<object> args, string rawCommand) {
            if (args.Count < 3 || args.Count > 3)  {
                Debug.WriteLine("ERROR! Missing arguments (player, weapon model, ammo count)!");
                Notify("~r~ERROR! Missing arguments (player, weapon model, ammo count)!");
                return;
            }
            var targetSrc = src;
            var ammoCount = 0;
            try {
                if (args[0].ToString() != "me") {
                    targetSrc = Int32.Parse(args[0].ToString());
                    if (!NetworkIsPlayerActive(GetPlayerFromServerId(targetSrc))) {
                        Debug.WriteLine("ERROR! Unknow player!");
                        Notify(("~r~ERROR! Unknow player!"));
                        return;
                    }
                }
                ammoCount = Int32.Parse(args[2].ToString());
            } catch (FormatException) {
                Debug.WriteLine("ERROR! Unable to convert args into integer!");
                Notify(("~r~ERROR! Unable to convert args into integer!"));
                return;
            }
            if (GiveWeapon(GetPlayerPed(GetPlayerFromServerId(targetSrc)), args[1].ToString(), ammoCount, true)) {
                Debug.WriteLine($"Successfully give {args[1]} to player {args[0]}!");
                Notify(($"~g~Successfully give {args[1]} to player {args[0]}!"));
            }
        }

        public void WeaponMenu() {
            menu = new NativeMenu("Weapons", "~b~Get all the weapons you want!");
            pool.Add(menu);
        }

        // [Tick]
        // public Task WeaponProcessMenu()
        // {
        //     var currentTime = GetGameTimer();
        //     if (IsControlPressed(0, 29)) {
        //         if (currentTime - lastTime >= 600) {
        //             lastTime = GetGameTimer();
        //             menu.Visible = !menu.Visible;
        //         }
        //     }
        //     if (menu.Visible) {
        //         pool.Process();
        //     }
        //     return Task.FromResult(5);
        // }
    }
}