using System.Collections.Generic;

namespace BulletDance.Audio
{
    [System.Serializable]
    public class SFX
    {
        public string sfxName;
        public AK.Wwise.Event sfxEvent;

        public SFX()
        {
            sfxName  = "New SFX";
            sfxEvent = null;
        }
    }

    [System.Serializable]
    public class SFXGroup
    {
        public string groupName;
        public List<SFX> sfxList;
    }
}