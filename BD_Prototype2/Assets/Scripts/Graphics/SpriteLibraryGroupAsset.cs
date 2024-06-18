using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.U2D.Animation;


namespace BulletDance.Graphics
{
    [Serializable]
    public class SpriteLibraryGroup
    {
        [SerializeField]
        string _name;

        [SerializeField]
        List<SpriteLibraryInfo> _spriteLibraries;


        public string name { get { return _name;  } }
        public List<SpriteLibraryInfo> spriteLibraries { get => _spriteLibraries; }


        /// <summary>
        /// Returns the first library group that contains the keyword.
        /// Both the group name and keyword will be compared in undercase.
        /// </summary>
        /// <param name="keyword">The Keyword to compare</param>
        public SpriteLibraryAsset GetLibByKeyword(string keyword)
        {
            foreach(var lib in _spriteLibraries)
            {
                string libName = lib.name.ToLower();
                string key     = keyword.ToLower();

                if(libName.Contains(key))
                    return lib.spriteLibraryAsset;
            }

            return null;
        }


        /// <summary>
        /// Returns the first library asset that contains all the keywords.
        /// Both the group name and keywords will be compared in undercase.
        /// </summary>
        /// <param name="keywords">Array of Keywords to compare</param>
        public SpriteLibraryAsset GetLibByKeywords(string[] keywords)
        {
            //Loop through each library group
            foreach(var lib in _spriteLibraries)
            {
                string libName = lib.name.ToLower();
                bool   match   = true;

                //Loop through each keyword
                for(int i = 0; i < keywords.Length; i++)
                {
                    string key = keywords[i].ToLower();

                    //Break keyword loop if key doesn't match
                    if(!libName.Contains(key))
                    {
                        match = false;
                        break;
                    }
                }

                //Match is unchanged -> Found the library
                if(match) return lib.spriteLibraryAsset;
            }

            return null;
        }

    }


    [Serializable]
    public class SpriteLibraryInfo
    {
        [SerializeField]
        string _name;

        [SerializeField]
        SpriteLibraryAsset _spriteLibraryAsset;

        public string name { get { return _name;  } }
        public SpriteLibraryAsset spriteLibraryAsset { get => _spriteLibraryAsset; }
    }

    [Serializable]
    public class SpriteAnimationInfo
    {
        [SerializeField]
        string _name;

        [SerializeField]
        float _animationState;

        [SerializeField]
        float _frames;

        [SerializeField]
        float _xOffsetInFrames;

        public string name            { get { return _name;  } }
        public float  animationState  { get { return _animationState;  } }
        public float  frames          { get { return _frames;  } }
        public float  xOffsetInFrames { get { return _xOffsetInFrames;  } }
    }


    [CreateAssetMenu(fileName = "New SpriteLibraryGroupAsset", menuName = "2D/SpriteLibraryGroupAsset", order = 4)]
    public class SpriteLibraryGroupAsset : ScriptableObject
    {
        [SerializeField]
        private List<SpriteLibraryGroup> _libraries = new List<SpriteLibraryGroup>();

        [SerializeField]
        private Vector2 _spritesheetSize;

        [SerializeField]
        private List<SpriteAnimationInfo> _animationInfo = new List<SpriteAnimationInfo>();


        public List<SpriteLibraryGroup> libraries { get => _libraries; }
        public Vector2 spritesheetSize { get { return _spritesheetSize; } }


        private Dictionary<string, SpriteLibraryAsset> _libLookup = null;
        private Dictionary<float, SpriteAnimationInfo> _animInfoLookup = null;

        public void CreateSpriteLibraryLookup()
        {
            if(_libraries.Count > 0 &&
               _libraries[0].spriteLibraries.Count > 0 &&
               _libraries[0].spriteLibraries[0].spriteLibraryAsset != null)
               _libLookup = new Dictionary<string, SpriteLibraryAsset>();
            else return;

            foreach(var libGroup in _libraries)
            {
                string _libGroupName = libGroup.name.ToLower() + " ";

                foreach(var lib in libGroup.spriteLibraries)
                {
                    string libName = lib.name.ToLower();

                    if(lib.spriteLibraryAsset != null)
                        _libLookup.Add(_libGroupName + libName, lib.spriteLibraryAsset);
                }
            }
        }

        public void CreateAnimationInfoLookup()
        {
            if(_animationInfo.Count > 0 &&
               _animationInfo[0].frames > 0)
               _animInfoLookup = new Dictionary<float, SpriteAnimationInfo>();
            else return;

            foreach(var animInfo in _animationInfo)
            {
                _animInfoLookup.Add(animInfo.animationState, animInfo);
            }
        }


        /// <summary>
        /// Returns the specified sprite library asset.
        /// The lookup dictionary keys use the "libraryGroupName libraryName" naming convention.
        /// </summary>
        /// <param name="keyword">The Keyword to compare</param>
        public SpriteLibraryAsset GetLibrary(string libraryGroupName, string libraryName)
        {
            string lookupKey = libraryGroupName.ToLower() + " " + libraryName.ToLower();

            if(_libLookup == null || !_libLookup.ContainsKey(lookupKey))
                return null;
            else
                return _libLookup[lookupKey];
        }


        /// <summary>
        /// Returns the first library group that contains the keyword.
        /// Both the group name and keyword will be compared in undercase.
        /// </summary>
        /// <param name="keyword">The Keyword to compare</param>
        public SpriteLibraryGroup GetLibGroupByKeyword(string keyword)
        {
            foreach(var libGroup in _libraries)
            {
                string libName = libGroup.name.ToLower();
                string key     = keyword.ToLower();

                if(libName.Contains(key))
                    return libGroup;
            }

            return null;
        }


        /// <summary>
        /// Returns the first library group that contains all the keywords.
        /// Both the group name and keywords will be compared in undercase.
        /// </summary>
        /// <param name="keywords">Array of Keywords to compare</param>
        public SpriteLibraryGroup GetLibGroupByKeywords(string[] keywords)
        {
            //Loop through each library group
            foreach(var libGroup in _libraries)
            {
                string libName = libGroup.name.ToLower();
                bool   match   = true;

                //Loop through each keyword
                for(int i = 0; i < keywords.Length; i++)
                {
                    string key = keywords[i].ToLower();

                    //Break keyword loop if key doesn't match
                    if(!libName.Contains(key))
                    {
                        match = false;
                        break;
                    }
                }

                //Match is unchanged -> Found the group
                if(match) return libGroup;
            }

            return null;
        }


        public SpriteAnimationInfo GetAnimationInfo(float animationState)
        {
            if(_animInfoLookup == null || !_animInfoLookup.ContainsKey(animationState))
                return null;
            else
                return _animInfoLookup[animationState];
        }
    }
}