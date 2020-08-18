using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.Rehab.BusinessEntities
{
   public class MSProgExerciseEntity
    {
        public int ProgExerrowid { get; set; }
        public string Exercise { get; set; }
        public string Sets { get; set; }
        public string Reps { get; set; }
        public string Qty { get; set; }
        public string Weight { get; set; }
        public bool Hold { get; set; }
    
        public int docrowid { get; set; }

}
}
