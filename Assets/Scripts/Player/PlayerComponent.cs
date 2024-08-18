using System;
using System.Collections.Generic;
using System.Linq;
using Scrapy.Util;
using UnityEngine;

namespace Scrapy.Player
{
    public abstract class PlayerComponent : MonoBehaviour
    {
        [SerializeField] private List<Transform> attachmentPoints;
        [SerializeField] private int minRequiredAttachedPoints;
        [SerializeField] private int maxRequiredAttachedPoints;
        [NonSerialized] public PlayerComponentConfig Config;
        [NonSerialized] public BodyPlayerComponent Parent;

        public virtual CanAttachResult CanAttachToPlayer()
        {
            var playerBodyLayerMask = LayerMask.GetMask("PlayerBody");
            int attachmentPointCount = 0;
            Dictionary<BodyPlayerComponent, int> parents = new();
            int noParentPoints = 0;
            foreach (var attachmentPoint in attachmentPoints)
            {
                var collider = Physics2D.OverlapPoint(attachmentPoint.position.XY(), playerBodyLayerMask);
                if (collider != null)
                {
                    attachmentPointCount++;
                    var component = collider.GetComponentInParent<BodyPlayerComponent>();
                    if (component == null) noParentPoints++;
                    else
                    {
                        if (parents.ContainsKey(component)) parents[component]++;
                        else parents[component] = 1;
                    }
                }
            }

            // return attachmentPointCount > minRequiredAttachedPoints && attachmentPointCount < maxRequiredAttachedPoints;
            if (attachmentPointCount < minRequiredAttachedPoints)
            {
                return new CanAttachResult { Error = "Too few contact points" };
            }

            if (attachmentPointCount > maxRequiredAttachedPoints)
            {
                return new CanAttachResult { Error = "Too many contact points" };
            }

            BodyPlayerComponent parent = null;
            var parentsMaxPoints = parents.Count > 0 ? parents.Max(x => x.Value) : 0;
            if (parentsMaxPoints > noParentPoints)
            {
                parent = parents.First(x => x.Value == parentsMaxPoints).Key;
            }

            return new CanAttachResult(){Parent = parent};
        }
    }

    public struct CanAttachResult
    {
        public bool Value => Error == null;
        public string Error;
        public BodyPlayerComponent Parent;
    }
}