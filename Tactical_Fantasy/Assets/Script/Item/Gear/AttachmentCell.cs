using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AttachmentCell
{
    public enum KindOfAttachment
    {
        Ammo = 400,
        Magazine = 301,
        Handle = 302,
        Flash = 303,
        Scope = 304,
        Reverse = 305,
        Spray = 306,
    }

    public KindOfAttachment kindOfAttachment;
    public Vector2 cellPosition;
    public Magazine.KindOfMagazine kindOfMagazine;
}
