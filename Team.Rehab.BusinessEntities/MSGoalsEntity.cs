using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
    public class MSGoalsEntity
    {
       
        public string Description { get; set; }
        public Int16? Goalrowid { get; set; }
        public string GoalNMet { get; set; }
        public string GoalMet { get; set; }
        public string GoalType { get; set; }
        public string GoalPMet { get; set; }
        public string GoalMetM { get; set; }
        public string GoalMetDb { get; set; }
        public int PatGoalrowid { get; set; }

    }
}
