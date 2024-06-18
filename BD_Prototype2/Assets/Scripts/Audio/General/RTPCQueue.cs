using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Audio
{
    public class RTPCCoroutine
    {
        public string name;
        public IEnumerator coroutine;

        public RTPCCoroutine(string[] rtpcNames, IEnumerator coroutine)
        {
            name = "";
            foreach(string rtpcName in rtpcNames) { name += rtpcName; }
            this.coroutine = coroutine;
        }
    }

    public class RTPCQueue
    {
        List<RTPCCoroutine> _rtpcChangequeue;

        public RTPCQueue()
        {
            _rtpcChangequeue = new List<RTPCCoroutine>();
        }

        public void Add(RTPCCoroutine rtpcCoroutine)
        {
            _rtpcChangequeue.Add(rtpcCoroutine);
        }

        public void Remove(RTPCCoroutine rtpcCoroutine)
        {
            for(int i = 0; i < _rtpcChangequeue.Count; i++)
            {
                if(_rtpcChangequeue[i].name == rtpcCoroutine.name)
                {
                    _rtpcChangequeue.RemoveAt(i);
                    return;
                }
            }
        }

        public RTPCCoroutine GetCoroutine(string name)
        {
            for(int i = 0; i < _rtpcChangequeue.Count; i++)
            {
                if(_rtpcChangequeue[i].name == name)
                    return _rtpcChangequeue[i];
            }

            return null;
        }

        public RTPCCoroutine GetCoroutine(string[] rtpcNames)
        {
            string name = "";
            foreach(string rtpcName in rtpcNames) { name += rtpcName; }
            return GetCoroutine(name);
        }
    }
}