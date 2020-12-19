using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomElements
{
    // Closed Elements
    public const string nc = "nc";
    public const string ec = "ec";
    public const string sc = "sc";
    public const string wc = "wc";

    // Open Elements
    public const string nm = "nm";
    const string no = "no";
    const string nne = "nne";
    const string ne = "ne";
    const string ene = "ene";
    const string em = "em";
    public const string eo = "eo";
    const string ese = "ese";
    const string se = "se";
    const string sse = "sse";
    public const string sm = "sm";
    const string so = "so";
    const string ssw = "ssw";
    const string sw = "sw";
    const string wsw = "wsw";
    const string wm = "wm";
    public const string wo = "wo";
    const string wnw = "wnw";
    const string nw = "nw";
    const string nnw = "nnw";

    public readonly List<string> openElements = new List<string> { nm, no, nne, ne, ene, em, eo, ese, se, sse, sm, so, ssw, sw, wsw, wm, wo, wnw, nw, nnw };
    public readonly List<string> adjoiningOpenElements = new List<string> { sm, so, sse, se, wnw, wm, wo, wsw, ne, nne, nm, no, nnw, nw, ese, em, eo, ene, sw, ssw };
    public readonly List<string> closedElements = new List<string> { nc, ec, sc, wc };

    public readonly string[] separator = { "-" };
}
