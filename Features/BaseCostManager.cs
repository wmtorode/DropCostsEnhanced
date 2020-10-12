using System.Collections.Generic;
using BattleTech;
using BattleTech.Framework;

namespace DropCostsEnhanced.Features
{
    public class BaseCostManager
    {
        public void Initialize()
        {
            return;
        }
        public int Cost
        {
            get;
            protected set;
        }

        protected string ObjectiveText
        {
            get;
            set;
        }

        public string FormattedCosts
        {
            get
            {
                return string.Format("{0:n0}", Cost);
            }
        }

        protected string uuid;

        public MissionObjectiveResult GetObjectiveResult()
        {
            string missionObjectiveResultString = $"{ObjectiveText}: ¢{FormattedCosts}";
            MissionObjectiveResult missionObjectiveResult = new MissionObjectiveResult(missionObjectiveResultString, uuid, false, true, ObjectiveStatus.Succeeded, false);
            return missionObjectiveResult;
        }

        public int CalculateFinalCosts(List<AbstractActor> actors)
        {
            return 0;
        }
    }
}