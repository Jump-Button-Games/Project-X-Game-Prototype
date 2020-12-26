using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rooms
{
    // Corner Rooms
    public readonly List<string> nwCornerRooms = new List<string> { "cr-nc-eo-sc-wc" };
    public readonly List<string> neCornerRooms = new List<string> { "cr-nc-ec-sc-wo" };
    public readonly List<string> swCornerRooms = new List<string> { "cr-nc-eo-sc-wc" };
    public readonly List<string> seCornerRooms = new List<string> { "cr-nc-ec-sc-wo" };

    // Link Room Codes
    public readonly List<string> linkRooms = new List<string> { "lk-nc-eo-sc-wo", "lk-nc-eo-sc-wc", "lk-nc-ec-sc-wo", "lk-nc-eo-sm-wo", "lk-nm-eo-sm-wo", "lk-nm-eo-sc-wo" };
    // "lk-n-eo-s-wo", "lk-nc-eo-sc-wc", "lk-n-eo-sc-wc", "lk-nc-eo-s-wo", "lk-nc-ec-s-wo", "lk-nc-ec-sc-wo" "lk-nc-eo-s-wo", "lk-n-eo-sc-wo" 

    // Start and End Rooms
    readonly List<string> startRooms = new List<string> { "s-1" };
    readonly List<string> endRooms = new List<string> { "e-1" };
}
