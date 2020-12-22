using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TDG_game {


        public abstract class WarEntity : GameBehavior
    {
        WarFactory originFactory;

        public WarFactory OriginFactory
        {
            get => originFactory;
            set
            {
                Debug.Assert(originFactory == null, "Redefined origin factory!");
                originFactory = value;
            }
        }

        public override void Recycle()
        {
            originFactory.Reclaim(this);
        }

    }

}

